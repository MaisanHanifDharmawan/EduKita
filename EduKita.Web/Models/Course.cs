using System.ComponentModel.DataAnnotations;

namespace EduKita.Web.Models;

public class Course
{
    public int Id { get; set; }

    [Required, StringLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Summary { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Level { get; set; } = "Pemula";

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string InstructorId { get; set; } = string.Empty;
    public ApplicationUser Instructor { get; set; } = null!;

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CourseModule> Modules { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();
}

public class CourseModule
{
    public int Id { get; set; }

    [Required, StringLength(160)]
    public string Title { get; set; } = string.Empty;

    public int Position { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public List<Lesson> Lessons { get; set; } = new();
}

public class Lesson
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500), Url]
    public string? VideoUrl { get; set; }

    public string Content { get; set; } = string.Empty;

    [Range(1, 600)]
    public int DurationMinutes { get; set; } = 10;

    public int Position { get; set; }

    public bool IsPreview { get; set; }

    public int CourseModuleId { get; set; }
    public CourseModule CourseModule { get; set; } = null!;
}
