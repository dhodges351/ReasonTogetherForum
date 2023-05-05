using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Account
{
    public class UserRegistrationModel
    {       
		[Required]
		[StringLength(12, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
		[Display(Name = "User Name")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
