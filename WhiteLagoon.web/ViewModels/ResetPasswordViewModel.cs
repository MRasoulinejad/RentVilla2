using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "Password must be between {2} and {1} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
