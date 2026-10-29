using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace SkillBridge.Backend.Pages.Darren.Admin.Resources
{
    public class ResourceItem
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public string Type { get; set; } = "PDF";
        public string SizeText { get; set; } = "0 KB";
    }

    public class IndexModel : PageModel
    {
        public List<ResourceItem> Resources { get; set; } = new();

        public void OnGet()
        {
            // Populate list from database
        }
    }
}