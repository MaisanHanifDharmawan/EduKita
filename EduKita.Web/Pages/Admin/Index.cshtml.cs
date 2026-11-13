using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int TotalUsers { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int TotalPublished { get; set; }
    public int TotalEnrollments { get; set; }
    public int TotalLessons { get; set; }
    public List<Enrollment> RecentEnrollments { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalUsers = await _db.Users.CountAsync();
        TotalCourses = await _db.Courses.CountAsync();
        TotalPublished = await _db.Courses.CountAsync(c => c.IsPublished);
        TotalEnrollments = await _db.Enrollments.CountAsync();
        TotalLessons = await _db.Lessons.CountAsync();

        var instructorRole = await _db.Roles.FirstAsync(r => r.Name == "Instructor");
        var studentRole = await _db.Roles.FirstAsync(r => r.Name == "Student");
        TotalInstructors = await _db.UserRoles.CountAsync(ur => ur.RoleId == instructorRole.Id);
        TotalStudents = await _db.UserRoles.CountAsync(ur => ur.RoleId == studentRole.Id);

        RecentEnrollments = await _db.Enrollments.AsNoTracking()
            .Include(e => e.User)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrolledAt)
            .Take(8)
            .ToListAsync();
    }
}
