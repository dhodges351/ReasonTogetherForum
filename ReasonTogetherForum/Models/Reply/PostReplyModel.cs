﻿using System.ComponentModel.DataAnnotations;

namespace ReasonTogetherForum.Models.Reply
{
	public class 
		PostReplyModel
	{		
		public int Id { get; set; }
		public string AuthorId { get; set; }
		public string AuthorName { get; set; }
		public int AuthorRating { get; set; }
		public string AuthorImageUrl { get; set; }		
		public DateTime Created { get; set; }
		[Required]
		[Display(Name = "Reply Content")]
		public string ReplyContent { get; set; }
		public int PostId { get; set; }
		public bool IsAuthorAdmin { get; set; }
		public string PostTitle { get; set; }
		public string PostContent { get; set; }
		public string ForumName { get; set; }
		public string ForumImageUrl { get; set; }
		public int ForumId { get; set; }
	}
}