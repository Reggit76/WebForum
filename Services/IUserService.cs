using System.Collections.Generic;
using System.Threading.Tasks;
using WebForum.Models;
using WebForum.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace WebForum.Services
{
    public interface IUserService
    {
        Task<SignInResult> AuthenticateUserAsync(string email, string password);
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<UserProfileViewModel> GetUserProfileByIdAsync(int id);
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task SignOutUserAsync();
        Task ChangeUserRoleAsync(int id, string role);
        Task BanUserAsync(int id);
        Task UnbanUserAsync(int id);
        Task<List<User>> GetAllUsersAsync();
    }
}
