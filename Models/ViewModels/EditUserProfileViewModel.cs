using System.ComponentModel.DataAnnotations;
using WebForum.Models;

namespace WebForum.Models.ViewModels
{
    public class EditUserProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        public Gender Gender { get; set; }

        [StringLength(256)]
        public string AvatarUrl { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }
    }
}
