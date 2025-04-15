using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyForms.Models.VM.TemplateVMs
{
    public class TemplateCreateVM
    {
        public TemplateSettingVM? TemplateSettingVM { get; set; }
        public IEnumerable<string> TagNames { get; set; } = new List<string>();
        public IEnumerable<string> Emails { get; set; } = new List<string>();
        public IList<Question> Questions { get; set; } = new List<Question>();
        public Template Template { get; set; } = null!;
    }
}
