using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Course> Courses { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Level { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

        var query = _db.Courses.AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Modules).ThenInclude(m => m.Lessons)
            .Where(c => c.IsPublished);

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var q = Q.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(q) || c.Summary.ToLower().Contains(q) || c.Instructor.FullName.ToLower().Contains(q));
        }
        if (CategoryId is int categoryId)
        {
            query = query.Where(c => c.CategoryId == categoryId);
        }
        if (!string.IsNullOrEmpty(Level) && Level != "semua")
        {
            query = query.Where(c => c.Level == Level);
        }

        Courses = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }
}
