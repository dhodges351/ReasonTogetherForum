using Microsoft.EntityFrameworkCore;
using ReasonTogetherForum.Data;

namespace ReasonTogetherForum.Services
{
    public class ForumService : IForum
    {
        private readonly ApplicationDbContext _context;
        private readonly IPost _postService;

        public ForumService(ApplicationDbContext context, IPost postService)
        {
            _context = context;
            _postService = postService;
        }

        public async Task Create(Forum forum)
        {
            _context.Add(forum);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int forumId)
        {
            var forum = GetById(forumId);
            if (forum != null)
            {
                _context.Remove(forum);
                await _context.SaveChangesAsync();
            }            
        }

        public IEnumerable<ApplicationUser> GetActiveUsers(int id)
        {
            var posts = GetById(id).Posts;

            if (posts != null && posts.Count() > 0)
            {
                var postUsers = posts?.Select(p => p.User);
                var replyUsers = posts?.SelectMany(p => p.Replies).Select(r => r.User);
                return postUsers.Union(replyUsers).Distinct();
            }

            return new List<ApplicationUser>();           
        }

        public IEnumerable<Forum> GetAll()
        {
            return _context.Forums
                .Include(forum => forum.Posts);
        }        

        public Forum GetById(int id)
        {
            var forum = _context.Forums.Where(x => x.Id == id)
                .Include(f => f.Posts).ThenInclude(u => u.User)
                .Include(f => f.Posts).ThenInclude(p => p.Replies).ThenInclude(u => u.User)
                .FirstOrDefault();

            if (forum != null && forum.Posts == null)
            {
                forum.Posts = new List<Post>();
            }

            return forum;
        }

        public bool HasRecentPost(int id)
        {
            const int hoursAgo = 48;
            var window = DateTime.Now.AddHours(-hoursAgo);
            return GetById(id).Posts.Any(post => post.Created > window);
        }

        public Task UpdateForumDescription(int forumId, string newDescription)
        {
            throw new NotImplementedException();
        }

        public Task UpdateForumTitle(int forumId, string newTitle)
        {
            throw new NotImplementedException();
        }
    }
}