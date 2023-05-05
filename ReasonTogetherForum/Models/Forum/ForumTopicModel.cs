using ReasonTogetherForum.Models.Post;
using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Forum
{
    public class ForumTopicModel
    {
        public ForumListingModel Forum { get; set; }
        public IEnumerable<PostListingModel>? Posts { get; set; }
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}
