using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebForum.Data;
using WebForum.Models;

namespace WebForum.Services
{
    public class ForumService : IForumService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ForumService> _logger;

        public ForumService(ApplicationDbContext context, ILogger<ForumService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            _logger.LogInformation("Fetching all topics");
            var topics = await _context.Topics.ToListAsync();
            _logger.LogInformation("Fetched {count} topics", topics.Count);
            return topics;
        }

        public async Task<List<Topic>> GetRecentTopicsAsync()
        {
            _logger.LogInformation("Fetching recent topics");
            var topics = await _context.Topics
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();
            _logger.LogInformation("Fetched {count} recent topics", topics.Count);
            return topics;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories");
            var categories = await _context.Categories
                .Include(c => c.Topics)
                .ToListAsync();
            _logger.LogInformation("Fetched {count} categories", categories.Count);
            return categories;
        }

        public async Task<Topic> GetTopicByIdAsync(int id)
        {
            _logger.LogInformation("Fetching topic by id={id}", id);
            var topic = await _context.Topics
                        .Include(t => t.User)  
                        .Include(t => t.Posts)
                        .ThenInclude(p => p.User)  
                        .FirstOrDefaultAsync(t => t.Id == id);
            if (topic == null)
            {
                _logger.LogWarning("Topic with id={id} not found", id);
            }
            return topic;
        }

        public async Task<List<Post>> GetPostsByTopicIdAsync(int topicId)
        {
            _logger.LogInformation("Fetching posts by topicId={topicId}", topicId);
            var posts = await _context.Posts
                .Where(p => p.TopicId == topicId)
                .ToListAsync();
            _logger.LogInformation("Fetched {count} posts for topicId={topicId}", posts.Count, topicId);
            return posts;
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            _logger.LogInformation("Fetching post by id={id}", id);
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                _logger.LogWarning("Post with id={id} not found", id);
            }
            return post;
        }

        public async Task AddTopicAsync(Topic topic)
        {
            _logger.LogInformation("Adding topic with title={title}", topic.Title);
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Topic added with id={id}", topic.Id);
        }

        public async Task AddPostAsync(Post post)
        {
            _logger.LogInformation("Adding post with content={content}", post.Content);
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Post added with id={id}", post.Id);
        }

        public async Task DeleteTopicAsync(int id)
        {
            _logger.LogInformation("Deleting topic with id={id}", id);
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Topic with id={id} deleted", id);
            }
            else
            {
                _logger.LogWarning("Topic with id={id} not found", id);
            }
        }

        public async Task DeletePostAsync(int id)
        {
            _logger.LogInformation("Deleting post with id={id}", id);
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Post with id={id} deleted", id);
            }
            else
            {
                _logger.LogWarning("Post with id={id} not found", id);
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            _logger.LogInformation("Adding category with name={name}", category.Name);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category added with id={id}", category.Id);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching category by id={id}", id);
            var category = await _context.Categories
                .Include(c => c.Topics)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                _logger.LogWarning("Category with id={id} not found", id);
            }
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _logger.LogInformation("Updating category with id={id}", category.Id);
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category with id={id} updated", category.Id);
        }

        public async Task UpdateTopicAsync(Topic topic)
        {
            _logger.LogInformation("Updating topic with id={id}", topic.Id);
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Topic with id={id} updated", topic.Id);
        }

        public async Task UpdatePostAsync(Post post)
        {
            _logger.LogInformation("Updating post with id={id}", post.Id);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Post with id={id} updated", post.Id);
        }
    }
}
