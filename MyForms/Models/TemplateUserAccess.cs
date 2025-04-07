using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyForms.Models
{
    public class TemplateUserAccess
    {
        [Key]
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public Template? Template { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
