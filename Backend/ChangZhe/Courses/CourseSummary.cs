
namespace SkillBridge.Web.Features.Courses;

public sealed record CourseSummary(
    long Id,
    string Title,
    string Summary,
    string Category,
    string Level,
    int LessonCount,
    int DurationMinutes,
    string ArtworkKey
)
{
    public string? ThumbnailUrl { get; init; }
}

public sealed record CourseDetail(
    CourseSummary Course,
    string Description,
    bool IsPublished
);
