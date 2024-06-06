using System.ComponentModel.DataAnnotations.Schema;

namespace WebForum.Models
{
    public class TopicTag
    {
        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public Topic? Topic { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
