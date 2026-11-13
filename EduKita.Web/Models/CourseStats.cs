namespace EduKita.Web.Models;

public static class CourseStats
{
    public static List<Lesson> OrderedLessons(Course course) =>
        course.Modules.OrderBy(m => m.Position).SelectMany(m => m.Lessons).ToList();

    public static int LessonCount(Course course) => OrderedLessons(course).Count;

    public static int TotalMinutes(Course course) => OrderedLessons(course).Sum(l => l.DurationMinutes);

    public static string DurationLabel(int minutes) =>
        minutes >= 60 ? $"{minutes / 60} jam {minutes % 60} mnt" : $"{minutes} mnt";

    public static string DurationLabel(Course course) => DurationLabel(TotalMinutes(course));

    public static int Percent(Enrollment enrollment)
    {
        var total = LessonCount(enrollment.Course);
        return total == 0 ? 0 : (int)Math.Round(100.0 * enrollment.LessonProgresses.Count / total);
    }
}
