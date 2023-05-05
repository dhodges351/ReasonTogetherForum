using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Account
{
    public class UserLoginModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; }

        public string returnUrl { get; set; } = "/";    

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
