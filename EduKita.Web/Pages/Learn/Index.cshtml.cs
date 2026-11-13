using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Learn;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public string DisplayName { get; set; } = string.Empty;
    public List<Enrollment> Enrollments { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _userManager.FindByIdAsync(userId);
        DisplayName = user?.FullName ?? User.Identity?.Name ?? "Siswa";

        Enrollments = await _db.Enrollments.AsNoTracking()
            .Include(e => e.Course).ThenInclude(c => c.Category)
            .Include(e => e.Course).ThenInclude(c => c.Instructor)
            .Include(e => e.Course).ThenInclude(c => c.Modules).ThenInclude(m => m.Lessons)
            .Include(e => e.LessonProgresses)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }
}
