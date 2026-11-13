using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Learn;

public class CourseModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CourseModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public Course Course { get; set; } = null!;
    public Enrollment? Enrollment { get; set; }
    public Lesson? CurrentLesson { get; set; }
    public HashSet<int> CompletedLessonIds { get; set; } = new();
    public bool PreviewOnly { get; set; }
    public bool CanManage { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int courseId, int? lessonId)
    {
        Course = await _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Modules.OrderBy(m => m.Position)).ThenInclude(m => m.Lessons.OrderBy(l => l.Position))
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (Course is null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        CanManage = userId is not null && (User.IsInRole("Admin") || Course.InstructorId == userId);

        if (userId is not null)
        {
            Enrollment = await _db.Enrollments.AsNoTracking()
                .Include(e => e.LessonProgresses)
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
            CompletedLessonIds = Enrollment?.LessonProgresses.Select(p => p.LessonId).ToHashSet() ?? new();
        }

        var lessons = CourseStats.OrderedLessons(Course);
        TotalLessons = lessons.Count;
        CompletedCount = lessons.Count(l => CompletedLessonIds.Contains(l.Id));

        if (Enrollment is null && !CanManage)
        {
            // Pengunjung tanpa pendaftaran hanya boleh melihat pelajaran pratinjau
            CurrentLesson = lessons.FirstOrDefault(l => l.Id == lessonId && l.IsPreview)
                ?? lessons.FirstOrDefault(l => l.IsPreview);
            if (CurrentLesson is null)
            {
                return RedirectToPage("/Courses/Details", new { id = courseId });
            }
            PreviewOnly = true;
        }
        else
        {
            CurrentLesson = lessons.FirstOrDefault(l => l.Id == lessonId) ?? lessons.FirstOrDefault();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int courseId, int lessonId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            return Challenge();
        }

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
        if (enrollment is null)
        {
            return Forbid();
        }

        if (!await _db.LessonProgresses.AnyAsync(p => p.EnrollmentId == enrollment.Id && p.LessonId == lessonId))
        {
            _db.LessonProgresses.Add(new LessonProgress
            {
                EnrollmentId = enrollment.Id,
                LessonId = lessonId,
                CompletedAt = DateTime.UtcNow,
            });
            await _db.SaveChangesAsync();
        }

        // Hitung ulang status penyelesaian kursus
        var completedIds = await _db.LessonProgresses
            .Where(p => p.EnrollmentId == enrollment.Id)
            .Select(p => p.LessonId)
            .ToListAsync();
        var completedSet = completedIds.ToHashSet();
        var totalLessons = await _db.CourseModules.Where(m => m.CourseId == courseId)
            .SelectMany(m => m.Lessons)
            .CountAsync();
        enrollment.CompletedAt = totalLessons > 0 && completedSet.Count >= totalLessons
            ? DateTime.UtcNow
            : null;
        await _db.SaveChangesAsync();

        // Arahkan ke pelajaran berikutnya yang belum selesai
        var orderedLessons = await _db.CourseModules.Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Position)
            .SelectMany(m => m.Lessons.OrderBy(l => l.Position))
            .Select(l => l.Id)
            .ToListAsync();
        var startIndex = orderedLessons.IndexOf(lessonId);
        var nextId = orderedLessons.Skip(startIndex + 1).FirstOrDefault(id => !completedSet.Contains(id));
        nextId = nextId == 0 ? orderedLessons.FirstOrDefault(id => !completedSet.Contains(id)) : nextId;
        nextId = nextId == 0 ? lessonId : nextId;

        return RedirectToPage(new { courseId, lessonId = nextId });
    }
}
