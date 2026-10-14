using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using SkillBridge.Web.Infrastructure.Data;

namespace SkillBridge.Web.Pages.ChangZhe.Courses.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel(MySqlConnectionFactory database) : PageModel
{
    // =========================================
    // 1. SEARCH AND FILTER PROPERTIES
    // =========================================

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public List<string> Categories { get; private set; } = new();

    public List<CourseRow> Courses { get; private set; } = new();

    // =========================================
    // 2. COURSE ROW MODEL
    // =========================================

    public record CourseRow(
        long Id,
        string Title,
        string Category,
        int Lessons,
        int Enrolments,
        bool Published
    );

    // =========================================
    // 3. LOAD COURSE CATEGORIES
    // =========================================

    private async Task LoadCategoriesAsync(CancellationToken ct)
    {
        Categories.Clear();

        await using var connection = await database.OpenAsync(ct);
        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT name
            FROM categories
            ORDER BY name
            """;

        await using var reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            Categories.Add(reader.GetString(0));
        }
    }

    // =========================================
    // 4. GET - DISPLAY COURSES
    // =========================================

    public async Task OnGetAsync(CancellationToken ct)
    {
        await LoadCategoriesAsync(ct);

        Courses.Clear();

        await using var connection = await database.OpenAsync(ct);
        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                c.id,
                c.title,
                cat.name,

                (
                    SELECT COUNT(*)
                    FROM lessons l
                    WHERE l.course_id = c.id
                ) AS lesson_count,

                (
                    SELECT COUNT(*)
                    FROM enrolments e
                    WHERE e.course_id = c.id
                ) AS enrolment_count,

                c.is_published

            FROM courses c

            INNER JOIN categories cat
                ON cat.id = c.category_id

            WHERE
                (
                    @search = ''
                    OR c.title LIKE CONCAT('%', @search, '%')
                )

                AND
                (
                    @category = ''
                    OR cat.name = @category
                )

                AND
                (
                    @status = ''
                    OR (
                        @status = 'Published'
                        AND c.is_published = 1
                    )
                    OR (
                        @status = 'Draft'
                        AND c.is_published = 0
                    )
                )

            ORDER BY c.id DESC
            """;

        command.Parameters.AddWithValue(
            "@search",
            Search?.Trim() ?? ""
        );

        command.Parameters.AddWithValue(
            "@category",
            Category?.Trim() ?? ""
        );

        command.Parameters.AddWithValue(
            "@status",
            Status?.Trim() ?? ""
        );

        await using var reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            Courses.Add(new CourseRow(
                Id: reader.GetInt64(0),
                Title: reader.GetString(1),
                Category: reader.GetString(2),
                Lessons: Convert.ToInt32(reader.GetInt64(3)),
                Enrolments: Convert.ToInt32(reader.GetInt64(4)),
                Published: reader.GetBoolean(5)
            ));
        }
    }

    // =========================================
    // 5. POST - DELETE COURSE
    // =========================================

    public async Task<IActionResult> OnPostDeleteAsync(
        long id,
        CancellationToken ct)
    {
        // Validate course ID
        if (id <= 0)
        {
            return BadRequest();
        }

        await using var connection = await database.OpenAsync(ct);

        // =====================================
        // CHECK IF COURSE EXISTS
        // =====================================

        await using (var check = connection.CreateCommand())
        {
            check.CommandText = """
                SELECT COUNT(*)
                FROM courses
                WHERE id = @id
                """;

            check.Parameters.AddWithValue("@id", id);

            var result = await check.ExecuteScalarAsync(ct);

            if (Convert.ToInt64(result) == 0)
            {
                return NotFound();
            }
        }

        // =====================================
        // CHECK LESSONS AND ENROLMENTS
        // =====================================

        await using (var dependencies = connection.CreateCommand())
        {
            dependencies.CommandText = """
                SELECT
                    (
                        SELECT COUNT(*)
                        FROM lessons
                        WHERE course_id = @id
                    )
                    +
                    (
                        SELECT COUNT(*)
                        FROM enrolments
                        WHERE course_id = @id
                    )
                """;

            dependencies.Parameters.AddWithValue("@id", id);

            var result = await dependencies.ExecuteScalarAsync(ct);

            if (Convert.ToInt64(result) > 0)
            {
                TempData["CourseMessage"] =
                    "This course cannot be deleted because it has existing lessons or enrolments.";

                return RedirectToPage(
                    "/ChangZhe/Courses/Admin/Index"
                );
            }
        }

        // =====================================
        // DELETE COURSE
        // =====================================

        try
        {
            await using var command = connection.CreateCommand();

            command.CommandText = """
                DELETE FROM courses
                WHERE id = @id
                """;

            command.Parameters.AddWithValue("@id", id);

            int affected = await command.ExecuteNonQueryAsync(ct);

            if (affected == 0)
            {
                return NotFound();
            }

            TempData["CourseMessage"] =
                "Course deleted successfully.";
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            TempData["CourseMessage"] =
                "This course cannot be deleted because other records are linked to it.";
        }

        return RedirectToPage(
            "/ChangZhe/Courses/Admin/Index"
        );
    }
    public string? Notice =>
    TempData["CourseMessage"] as string;
}
