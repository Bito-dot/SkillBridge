using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkillBridge.Backend.Darren.Lessons;

namespace SkillBridge.Backend.Pages.Darren.Lessons
{
    [Authorize]
    public class ViewModel : PageModel
    {
        private readonly ILessonReader _lessonReader;

        public ViewModel(ILessonReader lessonReader)
        {
            _lessonReader = lessonReader;
        }

        public LessonItem Lesson { get; set; } = default!;
        public List<ResourceItem> Resources { get; set; } = new();
        public bool IsCompleted { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var lesson = await _lessonReader.GetLessonByIdAsync(id);
            if (lesson == null)
                return NotFound();

            Lesson = lesson;
            Resources = await _lessonReader.GetResourcesByLessonIdAsync(id);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdClaim, out var userId))
            {
                IsCompleted = await _lessonReader.IsLessonCompletedAsync(userId, id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostToggleCompletionAsync(long id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdClaim, out var userId))
            {
                await _lessonReader.ToggleLessonCompletionAsync(userId, id);
            }

            return RedirectToPage(new { id });
        }
    }
}