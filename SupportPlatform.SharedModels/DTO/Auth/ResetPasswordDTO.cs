using System.ComponentModel.DataAnnotations;

namespace SupportPlatform.SharedModels.DTO.Auth
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password should be at least 8 symbols length.")]
        [MaxLength(255, ErrorMessage = "Password should have 255 symbols length max.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation are not same.")]
        public string PasswordConfirmation { get; set; }

        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
