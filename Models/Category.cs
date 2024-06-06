using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebForum.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}
