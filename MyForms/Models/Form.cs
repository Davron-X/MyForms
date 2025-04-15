using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class Form
    {
        [Key]
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public Template? Template { get; set; }

        public string FilledBy { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
        public List<FormAnswer> FormAnswers { get; set; } = new List<FormAnswer>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
