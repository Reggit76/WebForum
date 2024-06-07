using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
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
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ForumController> _logger;

        public ForumController(IForumService forumService, ICategoryService categoryService, UserManager<User> userManager, ILogger<ForumController> logger)
        {
            _forumService = forumService;
            _categoryService = categoryService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entered Index method");
            var topics = await _forumService.GetAllTopicsAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();
            _logger.LogInformation("Fetched topics and categories for Index view");

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
            _logger.LogInformation("Entered Details method with id={id}", id);
            var topic = await _forumService.GetTopicByIdAsync(id);
            if (topic == null)
            {
                _logger.LogWarning("Topic with id={id} not found", id);
                return NotFound();
            }
            var posts = await _forumService.GetPostsByTopicIdAsync(id);
            var model = new TopicDetailsViewModel
            {
                Topic = topic,
                Posts = posts.ToList(),
                NewPost = new Post { TopicId = id }
            };
            _logger.LogInformation("Fetched topic and posts for Details view with id={id}", id);
            return View(model);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            _logger.LogInformation("Entered CreateCategory (GET) method");
            return View();
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            _logger.LogInformation("Entered CreateCategory (POST) method with model={model}", model);
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid");
                await _categoryService.AddCategoryAsync(model);
                _logger.LogInformation("Category created with id={model.Id}", model.Id);
                return RedirectToAction("Index");
            }
            _logger.LogWarning("Model state is invalid");
            return View(model);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            _logger.LogInformation("Entered EditCategory (GET) method with id={id}", id);
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with id={id} not found", id);
                return NotFound();
            }
            return View(category);
        }

        [Authorize(Roles = "Moderator,Administrator")]
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            _logger.LogInformation("Entered EditCategory (POST) method with id={model.Id}", model.Id);
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid");
                await _categoryService.UpdateCategoryAsync(model);
                _logger.LogInformation("Category updated with id={model.Id}", model.Id);
                return RedirectToAction("Index");
            }
            _logger.LogWarning("Model state is invalid");
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateTopic(int? categoryId)
        {
            _logger.LogInformation("Entered CreateTopic (GET) method with categoryId={categoryId}", categoryId);
            var categories = await _categoryService.GetAllCategoriesAsync();
            var model = new CreateTopicViewModel
            {
                Categories = categories.ToList(),
                CategoryId = categoryId ?? 0
            };
            _logger.LogInformation("Fetched categories for CreateTopic view");
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTopic(CreateTopicViewModel model)
        {
            _logger.LogInformation("Entered CreateTopic (POST) method with model={model}", model);
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid");
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found");
                    return Unauthorized();
                }

                var topic = new Topic
                {
                    Title = model.Title,
                    Content = model.Content,
                    CategoryId = model.CategoryId,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _forumService.AddTopicAsync(topic);
                _logger.LogInformation("Topic created with id={topic.Id}", topic.Id);
                return RedirectToAction("Index");
            }

            model.Categories = await _categoryService.GetAllCategoriesAsync();
            _logger.LogWarning("Model state is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditTopic(int id)
        {
            _logger.LogInformation("Entered EditTopic (GET) method with id={id}", id);
            var topic = await _forumService.GetTopicByIdAsync(id);
            if (topic == null)
            {
                _logger.LogWarning("Topic with id={id} not found", id);
                return NotFound();
            }

            var model = new CreateTopicViewModel
            {
                Id = topic.Id,
                Title = topic.Title,
                Content = topic.Content,
                CategoryId = topic.CategoryId,
                Categories = await _categoryService.GetAllCategoriesAsync()
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditTopic(CreateTopicViewModel model)
        {
            _logger.LogInformation("Entered EditTopic (POST) method with id={model.Id}", model.Id);
            if (ModelState.IsValid)
            {
                var topic = await _forumService.GetTopicByIdAsync(model.Id);
                if (topic == null)
                {
                    _logger.LogWarning("Topic with id={model.Id} not found", model.Id);
                    return NotFound();
                }

                topic.Title = model.Title;
                topic.Content = model.Content;
                topic.CategoryId = model.CategoryId;

                await _forumService.UpdateTopicAsync(topic);
                _logger.LogInformation("Topic with id={model.Id} updated", model.Id);
                return RedirectToAction("Details", new { id = model.Id });
            }

            model.Categories = await _categoryService.GetAllCategoriesAsync();
            _logger.LogWarning("Model state is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            _logger.LogInformation("Entered DeleteTopic method with id={id}", id);
            var topic = await _forumService.GetTopicByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (topic == null || user == null || (user.Id != topic.UserId && !User.IsInRole("Moderator") && !User.IsInRole("Administrator")))
            {
                _logger.LogWarning("Unauthorized delete attempt or topic not found with id={id}", id);
                return Unauthorized();
            }

            await _forumService.DeleteTopicAsync(id);
            _logger.LogInformation("Topic with id={id} deleted", id);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreatePost(int topicId)
        {
            _logger.LogInformation("Entered CreatePost (GET) method with topicId={topicId}", topicId);
            var model = new Post { TopicId = topicId };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(Post model)
        {
            _logger.LogInformation("Entered CreatePost (POST) method with model={model}", model);
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found");
                    return Unauthorized();
                }

                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                await _forumService.AddPostAsync(model);
                _logger.LogInformation("Post created with id={model.Id}", model.Id);
                return RedirectToAction("Details", new { id = model.TopicId });
            }
            _logger.LogWarning("Model state is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            _logger.LogInformation("Entered EditPost (GET) method with id={id}", id);
            var post = await _forumService.GetPostByIdAsync(id);
            if (post == null)
            {
                _logger.LogWarning("Post with id={id} not found", id);
                return NotFound();
            }

            var model = new PostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                Rating = post.Rating
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditPost(PostViewModel model)
        {
            _logger.LogInformation("Entered EditPost (POST) method with model={model}", model);
            if (ModelState.IsValid)
            {
                var post = await _forumService.GetPostByIdAsync(model.Id);
                if (post == null)
                {
                    _logger.LogWarning("Post with id={model.Id} not found", model.Id);
                    return NotFound();
                }

                post.Content = model.Content;
                post.Rating = model.Rating;

                await _forumService.UpdatePostAsync(post);
                _logger.LogInformation("Post with id={model.Id} updated", model.Id);
                return RedirectToAction("Details", new { id = post.TopicId });
            }
            _logger.LogWarning("Model state is invalid");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            _logger.LogInformation("Entered DeletePost method with id={id}", id);
            var post = await _forumService.GetPostByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (post == null || user == null || (user.Id != post.UserId && !User.IsInRole("Moderator") && !User.IsInRole("Administrator")))
            {
                _logger.LogWarning("Unauthorized delete attempt or post not found with id={id}", id);
                return Unauthorized();
            }

            await _forumService.DeletePostAsync(id);
            _logger.LogInformation("Post with id={id} deleted", id);
            return RedirectToAction("Details", new { id = post.TopicId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VotePost(int postId, bool upvote)
        {
            _logger.LogInformation("Entered VotePost method with postId={postId} and upvote={upvote}", postId, upvote);
            var post = await _forumService.GetPostByIdAsync(postId);
            if (post == null)
            {
                _logger.LogWarning("Post with id={postId} not found", postId);
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return Unauthorized();
            }

            if (upvote)
            {
                post.Rating++;
            }
            else
            {
                post.Rating--;
            }

            await _forumService.UpdatePostAsync(post);
            _logger.LogInformation("Post with id={postId} updated with new rating={rating}", postId, post.Rating);

            return RedirectToAction("Details", new { id = post.TopicId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search(string searchString)
        {
            _logger.LogInformation("Entered Search method with searchString={searchString}", searchString);
            var topics = await _forumService.SearchTopicsAsync(searchString);
            var categories = await _categoryService.GetAllCategoriesAsync();
            var model = new ForumIndexViewModel
            {
                RecentTopics = topics,
                Categories = categories
            };
            _logger.LogInformation("Fetched search results for Search view");
            return View("Index", model);
        }
    }
}
