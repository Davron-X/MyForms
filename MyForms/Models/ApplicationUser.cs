using Microsoft.AspNetCore.Identity;

namespace MyForms.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public List<TemplateUserAccess> TemplateUsers { get; set; } = new();
        public List<Like> Likes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Template> Templates  { get; set; } = new();
        public List<Form> Forms { get; set; } = new();

    }
}
