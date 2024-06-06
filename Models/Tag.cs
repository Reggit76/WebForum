using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebForum.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; } = string.Empty;

        public ICollection<TopicTag> TopicTags { get; set; } = new List<TopicTag>();
    }
}
