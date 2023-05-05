using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Admin
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
