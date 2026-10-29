using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace SkillBridge.Backend.Pages.Darren.Admin.Resources
{
    public class ResourceFormModel
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long LessonId { get; set; }
        public string Type { get; set; } = "PDF";
        public string Description { get; set; } = string.Empty;
        public string? Transcript { get; set; }
    }

    public class EditModel : PageModel
    {
        [BindProperty]
        public ResourceFormModel Resource { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        public bool IsEditMode => Resource.Id > 0;

        public void OnGet(long? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                // Populate existing resource details for Edit mode
                Resource = new ResourceFormModel
                {
                    Id = id.Value,
                    Title = "Linux command reference",
                    LessonId = 1,
                    Type = "PDF",
                    Description = "A one-page reminder of navigation, file and permission commands."
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Handle file upload and MySQL save logic here
            return RedirectToPage("Index");
        }
    }
}