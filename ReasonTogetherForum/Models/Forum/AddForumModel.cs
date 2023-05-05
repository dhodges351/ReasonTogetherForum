using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ReasonTogetherForum.Models.Forum
{
    public class AddForumModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Title is required")]       
        public string Title { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Description is required")] 
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile ImageUpload { get; set; }
    }
}
