using System.ComponentModel.DataAnnotations;
using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Admin.Categories;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<Category> Categories { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _db.Categories.AsNoTracking()
            .Include(c => c.Courses)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var name = Input.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.Name == name))
        {
            TempData["Error"] = $"Kategori \"{name}\" sudah ada.";
            return RedirectToPage();
        }

        _db.Categories.Add(new Category
        {
            Name = name,
            Icon = string.IsNullOrWhiteSpace(Input.Icon) ? "book-open" : Input.Icon.Trim(),
            Description = Input.Description?.Trim(),
        });
        await _db.SaveChangesAsync();
        TempData["Status"] = $"Kategori \"{name}\" berhasil ditambahkan.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var category = await _db.Categories.Include(c => c.Courses).FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        if (category.Courses.Count > 0)
        {
            TempData["Error"] = $"Kategori \"{category.Name}\" masih dipakai oleh {category.Courses.Count} kursus dan tidak bisa dihapus.";
            return RedirectToPage();
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        TempData["Status"] = $"Kategori \"{category.Name}\" dihapus.";
        return RedirectToPage();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Nama kategori wajib diisi")]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [StringLength(16)]
        public string? Icon { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
