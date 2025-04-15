using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyForms.Models.VM.TemplateVMs
{
    public class TemplateSettingVM
    {
        public Template Template { get; set; } = null!;
        public IEnumerable<SelectListItem> Topics { get; set; } = new List<SelectListItem>();
    }
}
