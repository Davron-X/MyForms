using System.ComponentModel.DataAnnotations;

namespace MyForms.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TemplateTag> TemplateTags { get; set; } = new();
    }
}
