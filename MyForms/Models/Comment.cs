using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int TemplateId { get; set; }
        public Template? Template { get; set; }
        public string UserId { get; set; } = string.Empty; 
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
