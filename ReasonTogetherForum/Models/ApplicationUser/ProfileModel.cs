using Microsoft.AspNetCore.Components.RenderTree;
using SmartBreadcrumbs.Nodes;

namespace ReasonTogetherForum.Models.ApplicationUser
{
    public class ProfileModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserRating { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsAdmin { get; set; }

        public DateTime MemberSince { get; set; }
        public IFormFile ImageUpload { get; set; }

        public static MvcBreadcrumbNode GetParentPage(string referer, IList<string> userRoles, string id, bool isAdmin)
        {
			MvcBreadcrumbNode parentPage = null;

			if (string.IsNullOrEmpty(referer))
			{
				return parentPage;
			}
						
			Uri baseUri = new Uri(referer);
			string seg4 = string.Empty;
			if (baseUri.Segments != null && baseUri.Segments.Count() > 3)
			{
				seg4 = baseUri.Segments[3];
			}

			if (!string.IsNullOrEmpty(referer)
				&& referer.ToLower().Contains("post")
				&& !string.IsNullOrEmpty(seg4))
			{
				parentPage = new MvcBreadcrumbNode("Index", "Post", "Topic");
				parentPage.RouteValues = new { id = seg4 };
			}
			else if (!string.IsNullOrEmpty(referer)
				&& referer.ToLower().Contains("forum")
				&& !string.IsNullOrEmpty(seg4))
			{
				parentPage = new MvcBreadcrumbNode("Topic", "Forum", "Topic");
				parentPage.RouteValues = new { id = seg4 };
			}
            else if (!string.IsNullOrEmpty(referer) && !referer.ToLower().Contains("forum")
                 && !referer.ToLower().Contains("post")
                 && !referer.ToLower().Contains("profile"))
            {
				parentPage = null;
            }
            else if (!string.IsNullOrEmpty(referer)
                && referer.ToLower().Contains("profile"))
            {
                parentPage = new MvcBreadcrumbNode("Index", "Profile", "View Users");
                parentPage.RouteValues = new { id = id };
            }            

			return parentPage;
		}

		public static MvcBreadcrumbNode GetForumBreadcrumbNode(RouteValueDictionary rv, ReasonTogetherForum.Data.Forum forum, int id)
		{
			MvcBreadcrumbNode breadcrumbNode = new MvcBreadcrumbNode("Index", "Profile", "View Users");
			breadcrumbNode.RouteValues = new { id = id };

			if (rv == null)
			{
				return breadcrumbNode;
			}

			var seg1 = rv.FirstOrDefault();
			var seg2 = rv.LastOrDefault();

			if (seg1.Value != null && seg2.Value != null && seg1.Value.ToString() == "Forum")
			{
				var parentPage = new MvcBreadcrumbNode("Index", "Forum", "Topics");
				breadcrumbNode = new MvcBreadcrumbNode("Topic", "Forum", forum?.Title) { Parent = parentPage };
				breadcrumbNode.RouteValues = new { id = seg2.Value.ToString() };					
			}			

			return breadcrumbNode;
		}
	}
}
