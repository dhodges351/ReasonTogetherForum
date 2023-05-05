using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Models.Post;
using ReasonTogetherForum.Models.Reply;
using SmartBreadcrumbs.Nodes;

namespace ReasonTogetherForum.Controllers
{    
    public class PostController : Controller
    {       
		private readonly IPost _postService;
		private readonly IForum _forumService;
        private readonly IApplicationUser _userService;
        private readonly UserManager<ApplicationUser> _userManager;
		public PostController(IPost postService, 
            IForum forumService, 
            UserManager<ApplicationUser> user, 
            IApplicationUser userService)
        {
			_postService = postService;
            _forumService = forumService;
            _userManager = user;
            _userService = userService;
        }

        public IActionResult Index(int id)
        {
            var post = _postService.GetById(id);
            var replies = BuildPostReplies(post.Replies);

            var model = new PostIndexModel
            {
                Id = post.Id, 
                Title = post.Title,   
                AuthorId = post.User.Id,
                AuthorName = post.User.UserName,
                AuthorImageUrl = post.User.ProfileImageUrl,
                AuthorRating = post.User.Rating,
                Created = post.Created,
                PostContent = post.Content,
                Replies = replies,
                ForumId = post.Forum.Id,
                ForumName = post.Forum.Title,
                IsAuthorAdmin = IsAuthorAdmin(post.User)
            };            

            var parentPage = new MvcBreadcrumbNode("Topic", "Forum", "Topic");
            parentPage.RouteValues = new { id = post.Forum.Id };
            var topicPage = new MvcBreadcrumbNode("Index", "Post", post?.Title) { Parent = parentPage };          
            ViewData["BreadcrumbNode"] = topicPage;
            ViewData["Title"] = post?.Title; 
            return View(model);
        }

		private bool IsAuthorAdmin(ApplicationUser user)
		{
            return _userManager.GetRolesAsync(user)
                .Result.Contains("Admin");
		}

        [Authorize]
        public IActionResult Create(int id)
		{
			var forum = _forumService.GetById(id);

            var model = new NewPostModel
            {
                ForumName = forum.Title,
                ForumId = forum.Id,
                ForumImageUrl = forum.ImageUrl,
                AuthorName = User.Identity.Name
            };

            var parentPage = new MvcBreadcrumbNode("Topic", "Forum", "Topic");
            parentPage.RouteValues = new { id = forum.Id };
            var topicPage = new MvcBreadcrumbNode("Topic", "Forum", forum?.Title) { Parent = parentPage };
       
            ViewData["BreadcrumbNode"] = topicPage;
            ViewData["Title"] = forum?.Title;

            return View(model);
		}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(NewPostModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var userId = _userManager.GetUserId(User);            
            var user = _userManager.FindByIdAsync(userId).Result;
            var post = BuildPost(model, user);
            int forumId = 0;
            if (post != null)
            {
                if (post.Forum != null)
                {
                    forumId = post.Forum.Id;
                }
            }

            await _postService.Add(post);
            await _userService.UpdateUserRating(userId, typeof(Post));

            return RedirectToAction("Index", "Post", new { id = post.Id });
        }

		private Post BuildPost(NewPostModel model, ApplicationUser user)
		{
            var forum = _forumService.GetById(model.ForumId);
            return new Post
            { 
                Title = model.Title,
                Content = model.Content,
                Created = DateTime.Now,
                User = user,
                Forum = forum
            };
		}

		private IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply> replies)
		{
            return replies.Select(reply => new PostReplyModel { 
                Id = reply.Id,
                AuthorName = reply.User.UserName,
                AuthorId = reply.User.Id,
                AuthorImageUrl = reply.User.ProfileImageUrl,
                AuthorRating = reply.User.Rating,
                Created = reply.Created,
                ReplyContent= reply.Content,
                IsAuthorAdmin = IsAuthorAdmin(reply.User)
            });
		}
	}
}
