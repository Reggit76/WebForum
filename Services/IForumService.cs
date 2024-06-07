using System.Collections.Generic;
using System.Threading.Tasks;
using WebForum.Models;

namespace WebForum.Services
{
    public interface IForumService
    {
        Task<List<Topic>> GetAllTopicsAsync();
        Task<List<Topic>> GetRecentTopicsAsync();
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Topic> GetTopicByIdAsync(int id);
        Task<List<Post>> GetPostsByTopicIdAsync(int topicId);
        Task<Post> GetPostByIdAsync(int id); 
        Task AddTopicAsync(Topic topic);
        Task AddPostAsync(Post post);
        Task DeleteTopicAsync(int id);
        Task DeletePostAsync(int id);
        Task AddCategoryAsync(Category category);
        Task<Category> GetCategoryByIdAsync(int id);
        Task UpdateCategoryAsync(Category category);
        Task UpdateTopicAsync(Topic topic);
        Task UpdatePostAsync(Post post);
        Task<List<Topic>> SearchTopicsAsync(string searchString);
    }
}
