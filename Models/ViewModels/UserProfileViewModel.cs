using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string AvatarUrl { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public Role Role { get; set; }
        public int Rating { get; set; }

        public ICollection<Topic> Topics { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
