using System.ComponentModel.DataAnnotations;
using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Instructor.Lessons;

public abstract class LessonFormPageModel : PageModel
{
    protected readonly ApplicationDbContext Db;
    protected readonly UserManager<ApplicationUser> UserManager;

    protected LessonFormPageModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        Db = db;
        UserManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public CourseModule Module { get; set; } = null!;
    public Course Course => Module.Course;

    public async Task<CourseModule?> GetOwnedModuleAsync(int moduleId)
    {
        var userId = UserManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");
        return await Db.CourseModules
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == moduleId && (isAdmin || m.Course.InstructorId == userId));
    }

    public async Task<Lesson?> GetOwnedLessonAsync(int lessonId)
    {
        var userId = UserManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");
        return await Db.Lessons
            .Include(l => l.CourseModule).ThenInclude(m => m.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId && (isAdmin || l.CourseModule.Course.InstructorId == userId));
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Judul wajib diisi")]
        [StringLength(200)]
        [Display(Name = "Judul Pelajaran")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "URL Video (opsional)")]
        [StringLength(500)]
        [Url(ErrorMessage = "Masukkan URL yang valid, cth: https://www.youtube.com/embed/xxxx")]
        public string? VideoUrl { get; set; }

        [Range(1, 600, ErrorMessage = "Durasi antara 1 sampai 600 menit")]
        [Display(Name = "Durasi (menit)")]
        public int DurationMinutes { get; set; } = 10;

        [Required(ErrorMessage = "Konten wajib diisi")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Jadikan pratinjau gratis")]
        public bool IsPreview { get; set; }
    }
}
