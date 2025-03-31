using Microsoft.AspNetCore.Identity;

namespace MyForms.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
