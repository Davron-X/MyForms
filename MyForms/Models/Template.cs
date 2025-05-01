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

        [Display(Name = "Title", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public string Description { get; set; } = string.Empty;

        public int TopicId { get; set; }

        [ForeignKey("TopicId")]
        [Display(Name = "Topic", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public Topic? Topic { get; set; }

        [NotMapped]
        [Display(Name ="UploadImage",ResourceType = typeof(MyForms.Resources.SharedResource))]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }

        [Display(Name = "IsPrivate", ResourceType = typeof(MyForms.Resources.SharedResource))]
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
