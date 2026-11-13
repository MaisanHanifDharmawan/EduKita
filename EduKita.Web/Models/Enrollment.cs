using System.ComponentModel.DataAnnotations;

namespace EduKita.Web.Models;

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public List<LessonProgress> LessonProgresses { get; set; } = new();
}

public class LessonProgress
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
