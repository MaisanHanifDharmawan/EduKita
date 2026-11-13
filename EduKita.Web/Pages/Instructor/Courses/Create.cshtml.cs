using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduKita.Web.Pages.Instructor.Courses;

public class CreateModel : CourseFormPageModel
{
    public CreateModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        : base(db, userManager)
    {
    }

    public async Task OnGetAsync()
    {
        await LoadCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadCategoriesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var course = new Course
        {
            Title = Input.Title.Trim(),
            Summary = Input.Summary.Trim(),
            Description = Input.Description?.Trim() ?? string.Empty,
            Level = Input.Level,
            CategoryId = Input.CategoryId,
            InstructorId = UserManager.GetUserId(User)!,
            IsPublished = Input.IsPublished,
            CreatedAt = DateTime.UtcNow,
        };

        Db.Courses.Add(course);
        await Db.SaveChangesAsync();

        TempData["Status"] = "Kursus berhasil dibuat! Lanjutkan dengan menambahkan modul dan pelajaran.";
        return RedirectToPage("/Instructor/Courses/Manage", new { id = course.Id });
    }
}
