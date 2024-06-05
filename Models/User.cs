using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebForum.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = "/images/default-avatar.png";
        public Gender Gender { get; set; }
        public Role Role { get; set; } = Role.RegularUser;
        public string Description { get; set; } = String.Empty;
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public User() { }
    }

    public enum Role
    {
        RegularUser,
        Moderator,
        Administrator
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
