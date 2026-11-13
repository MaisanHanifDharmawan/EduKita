using System.ComponentModel.DataAnnotations;
using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Admin.Categories;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public Category Category { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Category = await _db.Categories.FindAsync(id);
        if (Category is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Name = Category.Name,
            Icon = Category.Icon,
            Description = Category.Description,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Category = await _db.Categories.FindAsync(id);
        if (Category is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var name = Input.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.Name == name && c.Id != id))
        {
            ModelState.AddModelError(string.Empty, $"Kategori \"{name}\" sudah ada.");
            return Page();
        }

        Category.Name = name;
        Category.Icon = string.IsNullOrWhiteSpace(Input.Icon) ? "book-open" : Input.Icon.Trim();
        Category.Description = Input.Description?.Trim();

        await _db.SaveChangesAsync();
        TempData["Status"] = "Kategori berhasil diperbarui.";
        return RedirectToPage("Index");
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
