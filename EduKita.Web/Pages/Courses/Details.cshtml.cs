using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Courses;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DetailsModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public Course Course { get; set; } = null!;
    public bool IsEnrolled { get; set; }
    public bool IsOwner { get; set; }
    public Enrollment? Enrollment { get; set; }
    public int StudentCount { get; set; }
    public int ProgressPercent { get; set; }
    public int? NextLessonId { get; set; }
    public List<Course> RelatedCourses { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Course = await _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Modules.OrderBy(m => m.Position)).ThenInclude(m => m.Lessons.OrderBy(l => l.Position))
            .FirstOrDefaultAsync(c => c.Id == id && c.IsPublished);

        if (Course is null)
        {
            return NotFound();
        }

        StudentCount = await _db.Enrollments.CountAsync(e => e.CourseId == id);

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(User)!;
            IsOwner = Course.InstructorId == userId;
            Enrollment = await _db.Enrollments.AsNoTracking()
                .Include(e => e.LessonProgresses)
                .FirstOrDefaultAsync(e => e.CourseId == id && e.UserId == userId);
            IsEnrolled = Enrollment is not null;

            if (Enrollment is not null)
            {
                ProgressPercent = CourseStats.Percent(Enrollment);
                var completedIds = Enrollment.LessonProgresses.Select(p => p.LessonId).ToHashSet();
                NextLessonId = CourseStats.OrderedLessons(Course).FirstOrDefault(l => !completedIds.Contains(l.Id))?.Id;
            }
        }

        RelatedCourses = await _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Modules).ThenInclude(m => m.Lessons)
            .Where(c => c.IsPublished && c.CategoryId == Course.CategoryId && c.Id != Course.Id)
            .OrderByDescending(c => c.CreatedAt)
            .Take(3)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostEnrollAsync(int id)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        var userId = _userManager.GetUserId(User)!;
        var course = await _db.Courses
            .Include(c => c.Modules).ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsPublished);

        if (course is null)
        {
            return NotFound();
        }

        if (course.InstructorId == userId)
        {
            TempData["Status"] = "Anda adalah instruktur kursus ini, tidak perlu mendaftar.";
            return RedirectToPage(new { id });
        }

        if (!await _db.Enrollments.AnyAsync(e => e.CourseId == id && e.UserId == userId))
        {
            _db.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = id,
                EnrolledAt = DateTime.UtcNow,
            });
            await _db.SaveChangesAsync();
            TempData["Status"] = "Selamat! Anda terdaftar di kursus ini. Selamat belajar.";
        }

        return RedirectToPage("/Learn/Course", new { courseId = id });
    }
}
