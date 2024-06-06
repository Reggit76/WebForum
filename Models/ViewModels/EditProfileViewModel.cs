using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public Gender Gender { get; set; }

        [Url]
        public string AvatarUrl { get; set; }

        public string Description { get; set; }
    }
}
