using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Instructor.Courses;

public class ManageModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManageModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public Course Course { get; set; } = null!;
    public int StudentCount { get; set; }

    [BindProperty]
    public string NewModuleTitle { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Course = await LoadCourseAsync(id);
        if (Course is null)
        {
            return NotFound();
        }

        StudentCount = await _db.Enrollments.CountAsync(e => e.CourseId == id);
        return Page();
    }

    public async Task<IActionResult> OnPostAddModuleAsync(int id)
    {
        var course = await LoadCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(NewModuleTitle))
        {
            TempData["Error"] = "Judul modul tidak boleh kosong.";
            return RedirectToPage(new { id });
        }

        var nextPosition = await _db.CourseModules.Where(m => m.CourseId == id).MaxAsync(m => (int?)m.Position) ?? 0;
        _db.CourseModules.Add(new CourseModule
        {
            CourseId = id,
            Title = NewModuleTitle.Trim(),
            Position = nextPosition + 1,
        });
        await _db.SaveChangesAsync();
        TempData["Status"] = "Modul baru berhasil ditambahkan.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteModuleAsync(int id, int moduleId)
    {
        var course = await LoadCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        var module = await _db.CourseModules.FirstOrDefaultAsync(m => m.Id == moduleId && m.CourseId == id);
        if (module is not null)
        {
            _db.CourseModules.Remove(module);
            await _db.SaveChangesAsync();
            TempData["Status"] = $"Modul \"{module.Title}\" dihapus.";
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteLessonAsync(int id, int lessonId)
    {
        var course = await LoadCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        var lesson = await _db.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId && l.CourseModule.CourseId == id);
        if (lesson is not null)
        {
            _db.Lessons.Remove(lesson);
            await _db.SaveChangesAsync();
            TempData["Status"] = $"Pelajaran \"{lesson.Title}\" dihapus.";
        }
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostTogglePublishAsync(int id)
    {
        var course = await LoadCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        course.IsPublished = !course.IsPublished;
        await _db.SaveChangesAsync();
        TempData["Status"] = course.IsPublished ? "Kursus dipublikasikan." : "Kursus disembunyikan dari katalog.";
        return RedirectToPage(new { id });
    }

    private async Task<Course?> LoadCourseAsync(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");
        return await _db.Courses
            .Include(c => c.Category)
            .Include(c => c.Modules.OrderBy(m => m.Position)).ThenInclude(m => m.Lessons.OrderBy(l => l.Position))
            .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.InstructorId == userId));
    }
}
