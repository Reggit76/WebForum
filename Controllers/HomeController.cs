using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebForum.Services;
using WebForum.Models.ViewModels;
using System.Diagnostics;

namespace WebForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly IForumService _forumService;

        public HomeController(IForumService forumService)
        {
            _forumService = forumService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var topics = await _forumService.GetRecentTopicsAsync();
            var model = new ForumIndexViewModel
            {
                RecentTopics = topics
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
