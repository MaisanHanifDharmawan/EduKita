using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Instructor.Courses;

public class StudentsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentsModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public Course Course { get; set; } = null!;
    public List<Enrollment> Enrollments { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = _userManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");

        Course = await _db.Courses.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.InstructorId == userId));
        if (Course is null)
        {
            return NotFound();
        }

        Enrollments = await _db.Enrollments.AsNoTracking()
            .Include(e => e.User)
            .Include(e => e.Course).ThenInclude(c => c.Modules).ThenInclude(m => m.Lessons)
            .Include(e => e.LessonProgresses)
            .Where(e => e.CourseId == id)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        return Page();
    }
}
