using ReasonTogetherForum.Models.Post;

namespace ReasonTogetherForum.Models.Search
{
    public class SearchResultModel
    {
        public IEnumerable<PostListingModel> Posts { get; set;}
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}
