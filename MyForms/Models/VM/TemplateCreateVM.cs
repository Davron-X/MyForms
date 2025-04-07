using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyForms.Models.VM
{
    public class TemplateCreateVM
    {
        public IEnumerable<SelectListItem> Topics { get; set; } = new List<SelectListItem>();
        public IEnumerable<string> Tags { get; set; } = new List<string>();
        public IEnumerable<string> Emails { get; set; } = new List<string>();
        public Template Template { get; set; } = null!;
    }
}
