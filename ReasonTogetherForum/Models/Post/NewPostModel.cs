using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Post
{
    public class NewPostModel
    {
        public string ForumName { get; set; }
        public int ForumId { get; set; }
        public string AuthorName { get; set; }
        public string ForumImageUrl { get; set; }

		[Required(ErrorMessage = "Title is required")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Content is required")]
		public string Content { get; set; }
		public DateTime Created { get; set; }
		public virtual ReasonTogetherForum.Data.ApplicationUser User { get; set; }
	}
}