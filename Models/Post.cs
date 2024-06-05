using System;

namespace WebForum.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int TopicId { get; set; }
        public Topic? Topic { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
