using EduKita.Web.Data;
using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public List<ApplicationUser> Users { get; set; } = new();
    public Dictionary<string, string> UserRoles { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public async Task OnGetAsync()
    {
        var query = _db.Users.AsNoTracking().OrderBy(u => u.FullName);
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var q = Q.Trim().ToLower();
            query = (IOrderedQueryable<ApplicationUser>)_db.Users.AsNoTracking()
                .Where(u => u.FullName.ToLower().Contains(q) || u.Email!.ToLower().Contains(q))
                .OrderBy(u => u.FullName);
        }

        Users = await query.ToListAsync();
        var roleIds = await _db.Roles.ToDictionaryAsync(r => r.Id, r => r.Name!);
        var userRoles = await _db.UserRoles.ToListAsync();
        UserRoles = userRoles.ToDictionary(ur => ur.UserId, ur => roleIds.TryGetValue(ur.RoleId, out var name) ? name : "?");
    }

    public async Task<IActionResult> OnPostSetRoleAsync(string userId, string role)
    {
        if (role != "Admin" && role != "Instructor" && role != "Student")
        {
            TempData["Error"] = "Peran tidak valid.";
            return RedirectToPage();
        }

        var currentUserId = _userManager.GetUserId(User);
        if (userId == currentUserId && role != "Admin")
        {
            TempData["Error"] = "Anda tidak bisa mencabut peran Admin dari akun Anda sendiri.";
            return RedirectToPage();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["Error"] = "Pengguna tidak ditemukan.";
            return RedirectToPage();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }
        await _userManager.AddToRoleAsync(user, role);

        TempData["Status"] = $"Peran {user.FullName} kini: {role}.";
        return RedirectToPage(new { Q });
    }
}
