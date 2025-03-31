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
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(maximumLength:60,MinimumLength =2)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name ="Remeber me")]
        public bool IsRemeber { get; set; }
    }
}
