using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class CreateTopicViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        [StringLength(100, ErrorMessage = "The Title field must be a string with a maximum length of 100.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Content field is required.")]
        [StringLength(5000, ErrorMessage = "The Content field must be a string with a maximum length of 5000.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "The CategoryId field is required.")]
        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
