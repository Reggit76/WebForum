using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebForum.Data;
using WebForum.Models;

namespace WebForum.Services
{
    public class ForumService : IForumService
    {
        private readonly ApplicationDbContext _context;

        public ForumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            return await _context.Topics.Include(t => t.User).ToListAsync();
        }

        public async Task<Topic> GetTopicByIdAsync(int id)
        {
            return await _context.Topics.Include(t => t.User).SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Post>> GetPostsByTopicIdAsync(int topicId)
        {
            return await _context.Posts.Include(p => p.User).Where(p => p.TopicId == topicId).ToListAsync();
        }

        public async Task AddTopicAsync(Topic topic)
        {
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
        }

        public async Task AddPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }
    }
}
