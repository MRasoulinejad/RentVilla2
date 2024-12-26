using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.web.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
