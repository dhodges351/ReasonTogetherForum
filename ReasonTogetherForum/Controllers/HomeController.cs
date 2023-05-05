using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Models;
using ReasonTogetherForum.Models.Forum;
using ReasonTogetherForum.Models.Home;
using ReasonTogetherForum.Models.Post;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System.Diagnostics;

namespace ReasonTogetherForum.Controllers
{
    [DefaultBreadcrumb]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPost _postService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IPost postService, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _postService = postService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = BuildHomeIndexModel();
            return View(model);
        }
       
        public IActionResult Privacy()
        {  
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private HomeIndexModel BuildHomeIndexModel()
        {
            var latestPosts = _postService.GetLatestPosts(6);

            var posts = latestPosts.Select(post => new PostListingModel { 
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.User.UserName,
                AuthorId= post.User.Id,
                AuthorRating= post.User.Rating,
                DatePosted = post.Created,
                ForumImageUrl = post.Forum.ImageUrl,
                ForumImageUrlHtml = $"<img src='{post.Forum?.ImageUrl}' alt='Forum Image' style='width:25px;height25px;' />",
                RepliesCount = post.Replies.Count(),
                Forum = GetForumListingForPost(post)
            });  

			return new HomeIndexModel
            {
                LatestPosts = posts,
                SearchQuery = ""
            };
        }

		private ForumListingModel GetForumListingForPost(Post post)
		{
			var forum = post.Forum;
			return new ForumListingModel
			{
				Id = forum.Id,
				Name = forum.Title,
				ImageUrl = forum.ImageUrl
			};
		}
	}
}