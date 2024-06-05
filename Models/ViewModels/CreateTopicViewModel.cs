using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class CreateTopicViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
