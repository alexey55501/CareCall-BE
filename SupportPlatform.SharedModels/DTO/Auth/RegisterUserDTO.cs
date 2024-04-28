using SupportPlatform.SharedModels.Base;
using System.ComponentModel.DataAnnotations;

namespace SupportPlatform.SharedModels.DTO.Auth
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter correct email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password should be at least 8 symbols length.")]
        [MaxLength(255, ErrorMessage = "Password should have 255 symbols length max.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation are not same.")]
        public string PasswordConfirmation { get; set; }
        [Required(ErrorMessage = "Terms & privacy policy agreement is required.")]
        [Range(1, 1, ErrorMessage = "You should accept our terms & privacy policy to continue.")]
        public bool IsAgreedWithTerms { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public PlatformRole PlatformRole { get; set; }
    }
}