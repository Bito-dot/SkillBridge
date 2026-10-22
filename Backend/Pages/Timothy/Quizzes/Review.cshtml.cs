using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkillBridge.Web.Features.Quizzes;

namespace SkillBridge.Web.Pages.Quizzes;

public sealed class ReviewModel(IQuizReader quizzes, QuizAttemptStore attempts) : PageModel
{
    public SavedAttempt Attempt { get; private set; } = null!;
    public QuizOverview Quiz { get; private set; } = null!;
    public List<QuizQuestion> Questions { get; private set; } = [];
    public Dictionary<long, long> Answers { get; private set; } = [];
    public bool IsPassed => Attempt.Score * 100m >= Attempt.MaximumMarks * Quiz.PassPercentage;

    public async Task<IActionResult> OnGetAsync(long attemptId, CancellationToken cancellationToken)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Challenge();
        var attempt = await attempts.FindAsync(attemptId, userId, cancellationToken);
        if (attempt is null || !attempt.IsSubmitted) return NotFound();
        var quiz = await quizzes.GetOverviewAsync(attempt.QuizId, userId, User.IsInRole("Admin"), cancellationToken);
        if (quiz is null) return NotFound();
        Attempt = attempt;
        Quiz = quiz;
        Questions = await attempts.QuestionsAsync(quiz.Id, cancellationToken);
        Answers = await attempts.AnswersAsync(attemptId, cancellationToken);
        return Page();
    }
}
