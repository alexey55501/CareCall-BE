using System.ComponentModel.DataAnnotations;

namespace SupportPlatform.SharedModels.DTO.Auth
{
    public class LoginUserDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter correct email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
