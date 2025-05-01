using System.ComponentModel.DataAnnotations;

namespace MyForms.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public string Password { get; set; } = string.Empty;      

        [Display(Name ="RememberMe", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public bool IsRemeber { get; set; }
    }
}
