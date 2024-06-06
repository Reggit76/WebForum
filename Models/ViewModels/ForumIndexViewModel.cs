using System.Collections.Generic;

namespace WebForum.Models.ViewModels
{
    public class ForumIndexViewModel
    {
        public IEnumerable<Topic> RecentTopics { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
