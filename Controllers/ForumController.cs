using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebForum.Models;
using WebForum.Models.ViewModels;
using WebForum.Services;

namespace WebForum.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForumService _forumService;
        private readonly IUserService _userService;

        public ForumController(IForumService forumService, IUserService userService)
        {
            _forumService = forumService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var topics = await _forumService.GetAllTopicsAsync();
            return View(topics);
        }

        public async Task<IActionResult> Details(int id)
        {
            var topic = await _forumService.GetTopicByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            var model = new TopicDetailsViewModel
            {
                Topic = topic,
                Posts = await _forumService.GetPostsByTopicIdAsync(id)
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateTopicViewModel model)
        {
            if (ModelState.IsValid)
            {
                var topic = new Topic
                {
                    Title = model.Title,
                    Content = model.Content,
                    Created = DateTime.UtcNow,
                    UserId = int.Parse(User.Identity.Name) // Assuming User.Identity.Name contains user id
                };

                await _forumService.AddTopicAsync(topic);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(int topicId, string content)
        {
            if (ModelState.IsValid)
            {
                var post = new Post
                {
                    Content = content,
                    Created = DateTime.UtcNow,
                    TopicId = topicId,
                    UserId = int.Parse(User.Identity.Name) // Assuming User.Identity.Name contains user id
                };

                await _forumService.AddPostAsync(post);
                return RedirectToAction(nameof(Details), new { id = topicId });
            }
            return RedirectToAction(nameof(Details), new { id = topicId });
        }
    }
}
