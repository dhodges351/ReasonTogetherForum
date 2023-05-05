using ReasonTogetherForum.Models.Forum;

namespace ReasonTogetherForum.Models.Post
{
	public class PostListingModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string AuthorName{ get; set; }
		public int AuthorRating { get; set; }
		public string AuthorId { get; set; }
		public string ForumImageUrlHtml { get; set; }
		public DateTime DatePosted { get; set; }
		public int ForumId { get; set; }
		public string ForumImageUrl { get; set; }
		public string ForumName { get; set; }
		public ForumListingModel Forum { get; set; }
		public int RepliesCount { get; set; }
		public string AuthorNameRatingLink => BuildAuthorNameLink();

        private string BuildAuthorNameLink()
        {
			string lnkStr = string.Empty;

            lnkStr = $"<a href='/Profile/Detail?id={AuthorId}'>{AuthorName}</a> ({AuthorRating})";

			return lnkStr;
        }
    }
}
