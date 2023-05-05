using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Models.Forum;
using ReasonTogetherForum.Models.Post;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using ReasonTogetherForum.Models.ApplicationUser;
using Microsoft.AspNetCore.Authorization;

namespace ReasonTogetherForum.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForum _forumService;
        private readonly IPost _postService;
        private readonly IUpload _uploadService;
        private readonly IOptions<AzureStorageSettings> _azureStorageSettings;
        private string _azureStorageAccount { get; set; }
        private IWebHostEnvironment _hostingEnvironment;        

        public ForumController(IForum forumService, 
                IPost postService, 
                UserManager<ApplicationUser> userManager, 
                IApplicationUser userService, 
                IUpload uploadService, 
                IOptions<AzureStorageSettings> options,
                IWebHostEnvironment environment)
        {
            _forumService = forumService;
            _postService = postService;
            _uploadService = uploadService;
            _azureStorageSettings = options;
            _azureStorageAccount = _azureStorageSettings.Value.ConnectionStrings[0];
            _hostingEnvironment = environment;
        }

        [Breadcrumb("Topics")]
        public IActionResult Index()
        {
            var forums = _forumService.GetAll()
                .Select(forum => new ForumListingModel { 
                    Id = forum.Id,
                    Name = forum.Title,
                    Description = forum.Description,                    
                    NumberOfPosts = forum.Posts?.Count() ?? 0,
                    NumberOfUsers = _forumService.GetActiveUsers(forum.Id).Count(),
                    ImageUrl = forum.ImageUrl,
                    HasRecentPost = _forumService.HasRecentPost(forum.Id)
                });

            var model = new ForumIndexModel
            {
                ForumList = forums?.OrderBy(f => f.Name)
            };

            return View(model);
        }
      
        public IActionResult Topic(int id, string searchQuery)
		{
            var forum = _forumService.GetById(id);
            var posts = _postService.GetFilteredPosts(forum, searchQuery).ToList();

            var postListings = posts?.Select(post => new PostListingModel
            {
                Id = post.Id,
                AuthorId = post.User.Id,
                AuthorName = post.User.UserName,
                AuthorRating = post.User.Rating,
                Title = post.Title,
                DatePosted = post.Created,
                RepliesCount = post.Replies.Count(),
                Forum = BuildForumListing(post)
            });

            var model = new ForumTopicModel
            {
                Posts = postListings,
                Forum = BuildForumListing(forum)
            };

            if (model.Posts == null || model.Posts.Count() == 0)
            {
                model.EmptySearchResults = true;
                model.SearchQuery = searchQuery;
            }

			var rv = HttpContext.Request.RouteValues;
            MvcBreadcrumbNode breadcrumbNode = ProfileModel.GetForumBreadcrumbNode(rv, forum, id);
			ViewData["BreadcrumbNode"] = breadcrumbNode;
			ViewData["Title"] = breadcrumbNode.Title;

			return View(model);
		}

        [HttpPost]
        public IActionResult Search(int id, string searchQuery)
        {
            return RedirectToAction("Topic", new { id, searchQuery });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new AddForumModel();
			return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddForum(AddForumModel model)
        {
            var imageUri = "/images/users/default.png";

            if (model.ImageUpload != null)
            {
                var blockBlob = UploadForumImage(model.ImageUpload);
                imageUri = blockBlob.Uri.AbsoluteUri;
            }

            var forum = new Forum
            {
                Title = model.Title,
                Description = model.Description,
                Created = DateTime.Now,
                ImageUrl = imageUri,
            };

            await _forumService.Create(forum);
            return RedirectToAction("Index", "Forum");
        }

        private CloudBlockBlob UploadForumImage(IFormFile file)
        {
            string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");
            if (file.Length > 0)
            {
                string filePath = Path.Combine(uploads, file.FileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(fileStream);
                }
            }
            string connectionString = _azureStorageSettings.Value.ConnectionStrings[0];
            var container = _uploadService.GetBlobContainer(connectionString, "forum-images");
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var filename = contentDisposition.FileName?.Trim();
            var blockBlob = container.GetBlockBlobReference(filename);
            blockBlob.UploadFromStreamAsync(file.OpenReadStream()).Wait();
            return blockBlob;
        }

        private ForumListingModel BuildForumListing(Post post)
		{
            var forum = post.Forum; 
            return BuildForumListing(forum);
		}

		private ForumListingModel BuildForumListing(Forum forum)
		{			
			ForumListingModel? forumListingModel = null;
			if (forum != null)
			{
				forumListingModel = new ForumListingModel
				{
					Id = forum.Id,
					Name = forum.Title,
					Description = forum.Description,
					ImageUrl = forum.ImageUrl
				};
			}
			else
			{
				forumListingModel = new ForumListingModel { Id = 0 };
			}

			return forumListingModel;
		}
	}
}
