using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Course> FeaturedCourses { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public int TotalCourses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalLessons { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _db.Categories.AsNoTracking().Include(c => c.Courses).OrderBy(c => c.Name).ToListAsync();

        FeaturedCourses = await _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Modules).ThenInclude(m => m.Lessons)
            .Where(c => c.IsPublished)
            .OrderByDescending(c => c.CreatedAt)
            .Take(6)
            .ToListAsync();

        TotalCourses = await _db.Courses.CountAsync(c => c.IsPublished);
        TotalStudents = await _db.Users.CountAsync();
        TotalLessons = await _db.Lessons.CountAsync();
    }
}
