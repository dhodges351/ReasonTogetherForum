using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Models.ApplicationUser;
using SmartBreadcrumbs.Nodes;

namespace ReasonTogetherForum.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUpload _uploadService;
		private readonly IOptions<AzureStorageSettings> _azureStorageSettings;
        private string _azureStorageAccount { get; set; }

		public ProfileController(UserManager<ApplicationUser> userManager, IApplicationUser userService, IUpload uploadService, IOptions<AzureStorageSettings> options)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
			_azureStorageSettings = options;
			_azureStorageAccount = _azureStorageSettings.Value.ConnectionStrings[0];
		}

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var profiles = _userService.GetAll()
                .OrderByDescending(user => user.Rating)
                .Select(u => new ProfileModel
                {
                    UserId = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    ProfileImageUrl = u.ProfileImageUrl,
                    UserRating = u.Rating.ToString(),
                    MemberSince = u.MemberSince
                });

            var model = new ProfileListModel
            {
                Profiles = profiles
            };

            if (User.IsInRole("Admin"))
            {
				var viewUsersPage = new MvcBreadcrumbNode("Index", "Profile", "View Users");
				ViewData["BreadcrumbNode"] = viewUsersPage;
				ViewData["Title"] = "View Users";
			}

            return View(model);
        }

        public IActionResult Detail(string id)
        {
            bool isAdmin = User.IsInRole("Admin");
            var user = _userService.GetById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var model = new ProfileModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRating = user.Rating.ToString(),
                Email= user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin")
			};

			string referer = HttpContext.Request.Headers["Referer"].ToString();
			MvcBreadcrumbNode parentPage = ProfileModel.GetParentPage(referer, userRoles, id, isAdmin);
            MvcBreadcrumbNode profilePage = new MvcBreadcrumbNode("Index", "Profile", "Profile") { Parent = parentPage };
            
			ViewData["BreadcrumbNode"] = profilePage;
			ViewData["Title"] = "Profile";

			return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var userId = _userManager.GetUserId(User);
            
            // connect to an Azure storage container
            string connectionString = _azureStorageSettings.Value.ConnectionStrings[0];

            // get blob container implemented in upload service
            var container = _uploadService.GetBlobContainer(connectionString, "profile-images");

            // get the file from the user and parse the content disposition response header
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

            // grab the file name
            var filename = contentDisposition.FileName.Trim();

			// get reference to a block blob
			var blockBlob = container.GetBlockBlobReference(filename.Value);

            // on the block blob, upload our file <---file uploaded to the cloud
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

            // set the User's profile image to the URI that comes back from our
            // block blobs.
            await _userService.SetProfileImage(userId, blockBlob.Uri);

			// redirect to the user's profile page.
			return RedirectToAction("Detail", "Profile", new { id = userId });
        }
    }
}
