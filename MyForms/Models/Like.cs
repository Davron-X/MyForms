using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
        public int TemplateId { get; set; }
        public Template? Template { get; set; }
    }
}
