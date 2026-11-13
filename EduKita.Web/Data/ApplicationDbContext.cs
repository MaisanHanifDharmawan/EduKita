using EduKita.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduKita.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseModule> CourseModules => Set<CourseModule>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(80).IsRequired();
            e.Property(c => c.Icon).HasMaxLength(16);
            e.HasIndex(c => c.Name).IsUnique();
        });

        builder.Entity<Course>(e =>
        {
            e.Property(c => c.Title).HasMaxLength(160).IsRequired();
            e.Property(c => c.Summary).HasMaxLength(300).IsRequired();
            e.Property(c => c.Level).HasMaxLength(20).IsRequired();
            e.HasIndex(c => c.CategoryId);
            e.HasIndex(c => c.InstructorId);
        });

        builder.Entity<CourseModule>(e =>
        {
            e.Property(m => m.Title).HasMaxLength(160).IsRequired();
            e.HasIndex(m => m.CourseId);
        });

        builder.Entity<Lesson>(e =>
        {
            e.Property(l => l.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(l => l.CourseModuleId);
        });

        builder.Entity<Enrollment>(e =>
        {
            e.HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
            e.HasIndex(x => x.CourseId);
        });

        builder.Entity<LessonProgress>(e =>
        {
            e.HasIndex(x => new { x.EnrollmentId, x.LessonId }).IsUnique();
            e.HasIndex(x => x.LessonId);
        });
    }
}
