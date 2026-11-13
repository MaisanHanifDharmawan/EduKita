using Microsoft.AspNetCore.Identity;

namespace EduKita.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
