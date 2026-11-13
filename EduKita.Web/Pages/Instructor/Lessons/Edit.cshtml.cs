using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduKita.Web.Pages.Instructor.Lessons;

public class EditModel : LessonFormPageModel
{
    public EditModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        : base(db, userManager)
    {
    }

    public Lesson Lesson { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Lesson = await GetOwnedLessonAsync(id)
            ?? throw new InvalidOperationException("Pelajaran tidak ditemukan atau bukan milik Anda.");
        Module = Lesson.CourseModule;

        Input = new InputModel
        {
            Title = Lesson.Title,
            VideoUrl = Lesson.VideoUrl,
            DurationMinutes = Lesson.DurationMinutes,
            Content = Lesson.Content,
            IsPreview = Lesson.IsPreview,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Lesson = await GetOwnedLessonAsync(id)
            ?? throw new InvalidOperationException("Pelajaran tidak ditemukan atau bukan milik Anda.");
        Module = Lesson.CourseModule;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        Lesson.Title = Input.Title.Trim();
        Lesson.VideoUrl = string.IsNullOrWhiteSpace(Input.VideoUrl) ? null : Input.VideoUrl.Trim();
        Lesson.Content = Input.Content.Trim();
        Lesson.DurationMinutes = Input.DurationMinutes;
        Lesson.IsPreview = Input.IsPreview;

        await Db.SaveChangesAsync();

        TempData["Status"] = "Pelajaran berhasil diperbarui.";
        return RedirectToPage("/Instructor/Courses/Manage", new { id = Module.CourseId });
    }
}
