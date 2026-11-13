using System.ComponentModel.DataAnnotations;

namespace EduKita.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(16)]
    public string Icon { get; set; } = "📚";

    [StringLength(200)]
    public string? Description { get; set; }

    public List<Course> Courses { get; set; } = new();
}
