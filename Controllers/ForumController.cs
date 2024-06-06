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
        private readonly ICategoryService _categoryService;

        public ForumController(IForumService forumService, ICategoryService categoryService)
        {
            _forumService = forumService;
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var topics = await _forumService.GetAllTopicsAsync();
            var categories = await _forumService.GetAllCategoriesAsync();
            var model = new ForumIndexViewModel
            {
                RecentTopics = topics,
                Categories = categories
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var topic = await _forumService.GetTopicByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            var posts = await _forumService.GetPostsByTopicIdAsync(id);
            var model = new TopicDetailsViewModel
            {
                Topic = topic,
                Posts = posts.ToList(),
                NewPost = new Post { TopicId = id }
            };
            return View(model);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategoryAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateTopic()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTopic(CreateTopicViewModel model)
        {
            if (ModelState.IsValid)
            {
                var topic = new Topic
                {
                    Title = model.Title,
                    Content = model.Content,
                    CategoryId = model.CategoryId,
                    UserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value),
                    CreatedAt = DateTime.UtcNow
                };
                await _forumService.AddTopicAsync(topic);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            await _forumService.DeleteTopicAsync(id);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreatePost(int topicId)
        {
            var model = new Post { TopicId = topicId };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(Post model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                model.CreatedAt = DateTime.UtcNow;
                await _forumService.AddPostAsync(model);
                return RedirectToAction("Details", new { id = model.TopicId });
            }
            return View(model);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _forumService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            await _forumService.DeletePostAsync(id);
            return RedirectToAction("Details", new { id = post.TopicId });
        }
    }
}
