using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Instructor.Lessons;

public class CreateModel : LessonFormPageModel
{
    public CreateModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        : base(db, userManager)
    {
    }

    public async Task<IActionResult> OnGetAsync(int moduleId)
    {
        Module = await GetOwnedModuleAsync(moduleId)
            ?? throw new InvalidOperationException("Modul tidak ditemukan atau bukan milik Anda.");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int moduleId)
    {
        Module = await GetOwnedModuleAsync(moduleId)
            ?? throw new InvalidOperationException("Modul tidak ditemukan atau bukan milik Anda.");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var nextPosition = await Db.Lessons.Where(l => l.CourseModuleId == moduleId).MaxAsync(l => (int?)l.Position) ?? 0;

        Db.Lessons.Add(new Lesson
        {
            CourseModuleId = moduleId,
            Title = Input.Title.Trim(),
            VideoUrl = string.IsNullOrWhiteSpace(Input.VideoUrl) ? null : Input.VideoUrl.Trim(),
            Content = Input.Content.Trim(),
            DurationMinutes = Input.DurationMinutes,
            IsPreview = Input.IsPreview,
            Position = nextPosition + 1,
        });
        await Db.SaveChangesAsync();

        TempData["Status"] = "Pelajaran baru berhasil ditambahkan.";
        return RedirectToPage("/Instructor/Courses/Manage", new { id = Module.CourseId });
    }
}
