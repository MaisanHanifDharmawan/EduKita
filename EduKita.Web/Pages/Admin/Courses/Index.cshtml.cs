using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Admin.Courses;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Course> Courses { get; set; } = new();

    public async Task OnGetAsync()
    {
        Courses = await _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Enrollments)
            .Include(c => c.Modules).ThenInclude(m => m.Lessons)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostTogglePublishAsync(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        course.IsPublished = !course.IsPublished;
        await _db.SaveChangesAsync();
        TempData["Status"] = course.IsPublished
            ? $"Kursus \"{course.Title}\" dipublikasikan."
            : $"Kursus \"{course.Title}\" disembunyikan.";
        return RedirectToPage();
    }
}
