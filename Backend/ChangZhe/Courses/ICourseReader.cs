namespace SkillBridge.Web.Features.Courses;

public interface ICourseReader
{
    Task<IReadOnlyList<CourseSummary>> GetPublishedAsync(string? search, CancellationToken cancellationToken, string? category = null, string sort = "title");
    Task<CourseDetail?> GetByIdAsync(long id, bool includeDraft, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseSummary>> GetEnrolledAsync(long userId, CancellationToken cancellationToken);
}
