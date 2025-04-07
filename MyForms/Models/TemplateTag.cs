using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class TemplateTag
    {
        [Key]
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public Template? Template { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
