using System.ComponentModel.DataAnnotations;

namespace SpletnaTrgovinaDiploma.Data.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
