using System.Collections.Generic;
using WebForum.Models;

namespace WebForum.Models.ViewModels
{
    public class TopicDetailsViewModel
    {
        public Topic Topic { get; set; }
        public IEnumerable<Post> Posts { get; set; }
    }
}
