using System.ComponentModel.DataAnnotations;

namespace MyForms.Models.DTOs
{
    public class RegistrationDto
    {
        [Required]
        [StringLength(254, MinimumLength = 5)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 100)]
        [Display(Name = "Password", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(MyForms.Resources.SharedResource))]

        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(maximumLength:60,MinimumLength =2)]
        [Display(Name = "FullName", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "RememberMe", ResourceType = typeof(MyForms.Resources.SharedResource))]
        public bool IsRemeber { get; set; }
    }
}
