using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class EditTopicViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}