using System.ComponentModel.DataAnnotations;
using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Instructor.Courses;

public abstract class CourseFormPageModel : PageModel
{
    protected readonly ApplicationDbContext Db;
    protected readonly UserManager<ApplicationUser> UserManager;

    protected CourseFormPageModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        Db = db;
        UserManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList CategoryOptions { get; set; } = null!;

    public async Task LoadCategoriesAsync()
    {
        var categories = await Db.Categories.OrderBy(c => c.Name).ToListAsync();
        CategoryOptions = new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
    }

    protected async Task<Course?> GetOwnedCourseAsync(int id)
    {
        var userId = UserManager.GetUserId(User)!;
        var isAdmin = User.IsInRole("Admin");
        return await Db.Courses.FirstOrDefaultAsync(c => c.Id == id && (isAdmin || c.InstructorId == userId));
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Judul wajib diisi")]
        [StringLength(160)]
        [Display(Name = "Judul Kursus")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ringkasan wajib diisi")]
        [StringLength(300)]
        [Display(Name = "Ringkasan Singkat")]
        public string Summary { get; set; } = string.Empty;

        [Display(Name = "Deskripsi Lengkap")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Pilih level")]
        public string Level { get; set; } = "Pemula";

        [Required(ErrorMessage = "Pilih kategori")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Terbitkan sekarang")]
        public bool IsPublished { get; set; }
    }
}
