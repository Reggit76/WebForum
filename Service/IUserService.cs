using System.Collections.Generic;
using System.Threading.Tasks;
using WebForum.Models;

namespace WebForum.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> SignInUserAsync(string email, string password);
        Task SignOutUserAsync();
        Task UpdateUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
