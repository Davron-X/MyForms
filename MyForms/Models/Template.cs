using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class Template
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TopicId { get; set; }
        [ForeignKey("TopicId")]
        public Topic? Topic { get; set; }
        [NotMapped]
        [Display(Name ="Upload Image")]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedById { get; set; } 
        public ApplicationUser? ApplicationUser { get; set; }
        public List<Question> Questions { get; set; } = new();
        public List<TemplateTag> TemplateTags { get; set; } = new();
        public List<TemplateUserAccess> TemplateUsers { get; set; } = new();
        public List<Like> Likes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Form> Forms { get; set; } = new();
    }
}
