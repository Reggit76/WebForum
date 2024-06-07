using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Content field is required.")]
        [StringLength(5000, ErrorMessage = "The Content field must be a string with a maximum length of 5000.")]
        public string Content { get; set; }

        public int Rating { get; set; }
    }
}
