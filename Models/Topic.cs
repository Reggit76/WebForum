using System;
using System.Collections.Generic;

namespace WebForum.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<TopicTag> TopicTags { get; set; } = new List<TopicTag>();
    }
}
