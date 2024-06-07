using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebForum.Models;
using WebForum.Models.ViewModels;
using WebForum.Services;

namespace WebForum.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var users = await _userService.GetAllUsersAsync();

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.UserName.Contains(search) || u.Email.Contains(search)).ToList();
            }

            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new EditProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                Description = user.Description
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Gender = model.Gender;
                user.AvatarUrl = model.AvatarUrl;
                user.Description = model.Description;

                await _userService.UpdateUserAsync(user);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ChangeRole(int id, string role)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null || user.Role == Role.Administrator)
            {
                return BadRequest("Invalid user or role.");
            }

            if ((role == "Administrator" && User.IsInRole("Administrator") && user.Role != Role.Administrator) ||
                (role == "Moderator" && User.IsInRole("Administrator") && user.Role != Role.Administrator) ||
                (role == "RegularUser" && User.IsInRole("Administrator") && user.Role != Role.Administrator))
            {
                await _userService.ChangeUserRoleAsync(id, role);
            }
            else
            {
                return Forbid();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> BanUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null || user.Role == Role.Administrator)
            {
                return BadRequest("Invalid user.");
            }

            if (User.IsInRole("Administrator") || (User.IsInRole("Moderator") && user.Role == Role.RegularUser))
            {
                await _userService.BanUserAsync(id);
            }
            else
            {
                return Forbid();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> UnbanUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            await _userService.UnbanUserAsync(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Gender = model.Gender,
                    AvatarUrl = string.IsNullOrEmpty(model.AvatarUrl) ? "/images/default-avatar.png" : model.AvatarUrl,
                    Description = model.Description,
                    Role = model.Role
                };
                
                var result = await _userService.CreateUserAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userService.ChangeUserRoleAsync(user.Id, model.Role.ToString());
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
