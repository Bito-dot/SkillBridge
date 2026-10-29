using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkillBridge.Backend.Darren.Lessons;

namespace SkillBridge.Backend.Pages.Darren.Admin.Lessons
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ILessonReader _lessonReader;

        public EditModel(ILessonReader lessonReader)
        {
            _lessonReader = lessonReader;
        }

        [BindProperty(SupportsGet = true)]
        public long? Id { get; set; }

        public bool IsCreate => !Id.HasValue;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<CourseOption> Courses { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Please select a course.")]
            public long CourseId { get; set; }

            [Required(ErrorMessage = "Title is required.")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
            public string Title { get; set; } = string.Empty;

            public string? Content { get; set; }

            [Range(1, 999, ErrorMessage = "Order must be between 1 and 999.")]
            public int OrderIndex { get; set; } = 1;

            [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
            public int DurationMinutes { get; set; } = 10;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Courses = await _lessonReader.GetCourseOptionsAsync();

            if (Id.HasValue)
            {
                var lesson = await _lessonReader.GetLessonByIdAsync(Id.Value);
                if (lesson == null)
                    return NotFound();

                Input = new InputModel
                {
                    CourseId = lesson.CourseId,
                    Title = lesson.Title,
                    Content = lesson.Content,
                    OrderIndex = lesson.OrderIndex,
                    DurationMinutes = lesson.DurationMinutes
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Courses = await _lessonReader.GetCourseOptionsAsync();
                return Page();
            }

            var lessonInput = new LessonInput
            {
                CourseId = Input.CourseId,
                Title = Input.Title.Trim(),
                Content = Input.Content,
                OrderIndex = Input.OrderIndex,
                DurationMinutes = Input.DurationMinutes
            };

            if (IsCreate)
            {
                await _lessonReader.CreateLessonAsync(lessonInput);
                TempData["StatusMessage"] = "Lesson created successfully.";
            }
            else
            {
                await _lessonReader.UpdateLessonAsync(Id!.Value, lessonInput);
                TempData["StatusMessage"] = "Lesson updated successfully.";
            }

            return RedirectToPage("Index");
        }
    }
}