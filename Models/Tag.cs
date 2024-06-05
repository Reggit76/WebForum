using System.Collections.Generic;

namespace WebForum.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<TopicTag> TopicTags { get; set; } = new List<TopicTag>();
        
        public Tag() { }
    }
}
