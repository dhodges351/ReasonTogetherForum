﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Models.Reply;
using SmartBreadcrumbs.Nodes;

namespace ReasonTogetherForum.Controllers
{  
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;

        public ReplyController(IPost postService, 
                UserManager<ApplicationUser> userManager,
                IApplicationUser userService)
        {
            _postService = postService;
            _userManager = userManager;
            _userService = userService;
        }            

        public async Task<IActionResult> Create(int id)
        {
            var post = _postService.GetById(id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var model = new PostReplyModel
            {
                PostContent = post.Content,
                PostTitle = post.Title,
                PostId = id,

                AuthorId = user.Id,
                AuthorName = User.Identity.Name,
                AuthorImageUrl = user.ProfileImageUrl,
                AuthorRating = user.Rating,
                IsAuthorAdmin = User.IsInRole("Admin"),

                ForumId = post.Forum.Id,
                ForumName = post.Forum.Title,
                ForumImageUrl = post.Forum.ImageUrl,

                Created = DateTime.Now,
            };

			var parentPage = new MvcBreadcrumbNode("Index", "Post", "Topic");
			parentPage.RouteValues = new { id = id };
			var replyPage = new MvcBreadcrumbNode("Index", "Post", "Reply") { Parent = parentPage };
			ViewData["BreadcrumbNode"] = replyPage;
			ViewData["Title"] = "Reply";

			return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReply(PostReplyModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            var reply = BuildReply(model, user);

            await _postService.AddReply(reply);
            await _userService.UpdateUserRating(userId, typeof(PostReply));

            return RedirectToAction("Index", "Post", new { id = model.PostId });
        }

        private PostReply BuildReply(PostReplyModel model, ApplicationUser user)
        {
            var post = _postService.GetById(model.PostId);
            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user
            };
        }
    }
}
