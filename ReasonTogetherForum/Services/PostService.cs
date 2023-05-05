using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ReasonTogetherForum.Data;

namespace ReasonTogetherForum.Services
{
    public class PostService : IPost
    {
        private readonly ApplicationDbContext _context;
        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

		public async Task Add(Post post)
		{
			_context.Add(post);
			await _context.SaveChangesAsync();
		}

		public async Task AddReply(PostReply reply)
		{
            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();
        }

		public Task Create(Forum forum)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int forumId)
        {
            throw new NotImplementedException();
        }

		public Task EditPostContent(int id, string newContent)
		{
			throw new NotImplementedException();
		}

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
			if (string.IsNullOrEmpty(searchQuery))
			{
				return new List<Post>();
			}
			return GetAll().Where(post
                    => post.Title.ToLower().Contains(searchQuery.ToLower())
                    || post.Content.ToLower().Contains(searchQuery.ToLower()));
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
		{
			return string.IsNullOrEmpty(searchQuery) 
				? forum.Posts 
				: forum.Posts.Where(post 
					=> (post.Title.ToLower().Contains(searchQuery.ToLower()) 
				    || post.Content.ToLower().Contains(searchQuery.ToLower())));
        }

		public IEnumerable<Post> GetPostsByForum(int id)
		{
			return _context.Forums.Where(forum => forum.Id == id).First().Posts;
		}

		public IEnumerable<Post> GetAll()
		{
			return _context.Posts
				.Include(post => post.User)
				.Include(post => post.Replies)
					.ThenInclude(reply => reply.User)
				.Include(post => post.Forum);
        }

		Post IPost.GetById(int id)
		{
			 return _context.Posts.Where(post => post.Id == id)
				.Include(post => post.User)
				.Include(post => post.Replies)
					.ThenInclude(reply => reply.User)
				.Include(post => post.Forum)
				.First();
		}

        public IEnumerable<Post> GetLatestPosts(int n)
        {
            return GetAll().OrderByDescending(post => post.Created).Take(n);
        }
    }
}