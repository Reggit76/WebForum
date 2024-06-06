using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebForum.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum Role
    {
        RegularUser,
        Moderator,
        Administrator
    }

    public class User : IdentityUser<int>
    {

        [Required]
        public Gender Gender { get; set; }

        public string AvatarUrl { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        public string Description { get; set; } = string.Empty;
        public bool IsBanned { get; set; }

        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
