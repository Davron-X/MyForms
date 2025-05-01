namespace MyForms.Models.VM.HomeVMs
{
    public class HomeVM
    {
        public IEnumerable<Template> RecentTemplates { get; set; } = new List<Template>();
        public IEnumerable<Template> PopularTemplates { get; set; } = new List<Template>();
        public IEnumerable<TagVM> Tags { get; set; }
    }
}
