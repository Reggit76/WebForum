using System.Collections.Generic;
using System.Threading.Tasks;
using WebForum.Models;

namespace WebForum.Services
{
    public interface IForumService
    {
        Task<IEnumerable<Topic>> GetAllTopicsAsync();
        Task<Topic> GetTopicByIdAsync(int id);
        Task<IEnumerable<Post>> GetPostsByTopicIdAsync(int topicId);
        Task AddTopicAsync(Topic topic);
        Task AddPostAsync(Post post);
    }
}
