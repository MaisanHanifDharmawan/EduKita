using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EduKita.Web.Data;

namespace EduKita.Web.Pages.Instructor.Courses;

public class EditModel : CourseFormPageModel
{
    public EditModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        : base(db, userManager)
    {
    }

    public Course Course { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Course = await GetOwnedCourseAsync(id);
        if (Course is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Title = Course.Title,
            Summary = Course.Summary,
            Description = Course.Description,
            Level = Course.Level,
            CategoryId = Course.CategoryId,
            IsPublished = Course.IsPublished,
        };
        await LoadCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var course = await GetOwnedCourseAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        await LoadCategoriesAsync();

        if (!ModelState.IsValid)
        {
            Course = course;
            return Page();
        }

        course.Title = Input.Title.Trim();
        course.Summary = Input.Summary.Trim();
        course.Description = Input.Description?.Trim() ?? string.Empty;
        course.Level = Input.Level;
        course.CategoryId = Input.CategoryId;
        course.IsPublished = Input.IsPublished;

        await Db.SaveChangesAsync();

        TempData["Status"] = "Perubahan kursus berhasil disimpan.";
        return RedirectToPage("/Instructor/Courses/Manage", new { id });
    }
}
