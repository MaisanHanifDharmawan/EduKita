using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Instructor;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public List<Course> MyCourses { get; set; } = new();
    public string DisplayName { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _userManager.FindByIdAsync(userId);
        DisplayName = user?.FullName ?? "Instruktur";
        var isAdmin = User.IsInRole("Admin");

        MyCourses = await _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Enrollments)
            .Include(c => c.Modules).ThenInclude(m => m.Lessons)
            .Where(c => isAdmin || c.InstructorId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var course = await GetOwnedCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
        TempData["Status"] = $"Kursus \"{course.Title}\" beserta seluruh isinya telah dihapus.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTogglePublishAsync(int id)
    {
        var course = await GetOwnedCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        course.IsPublished = !course.IsPublished;
        await _db.SaveChangesAsync();
        TempData["Status"] = course.IsPublished
            ? $"Kursus \"{course.Title}\" sekarang dipublikasikan."
            : $"Kursus \"{course.Title}\" disembunyikan dari katalog.";
        return RedirectToPage();
    }

    private async Task<Course?> GetOwnedCourseAsync(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");
        return await _db.Courses.FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.InstructorId == userId));
    }
}
