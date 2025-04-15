namespace MyForms.Models.VM.TemplateVMs
{
    public class TemplateUpdateVM
    {
        public TemplateSettingVM? TemplateSettingVM { get; set; }
        public Template Template { get; set; } = null!;
        public IEnumerable<string> TagNames { get; set; } = new List<string>();
        public IEnumerable<string> Emails { get; set; } = new List<string>();
    }
}
