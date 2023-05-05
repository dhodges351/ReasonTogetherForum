using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
