using System.ComponentModel.DataAnnotations;

namespace SupportPlatform.SharedModels.DTO.User
{
    public class UserPasswordUpdateDTO
    {
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "New password confirmation is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords are not same.")]
        public string ConfirmNewPassword { get; set; }
    }
}
