using System.Collections.Generic;
using WebForum.Models;

namespace WebForum.Models.ViewModels
{
    public class TopicDetailsViewModel
    {
        public Topic Topic { get; set; }
        public List<Post> Posts { get; set; }
        public Post NewPost { get; set; }
    }
}
