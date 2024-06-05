using System.Collections.Generic;
using WebForum.Models;

namespace WebForum.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string AvatarUrl { get; set; }
        public Role Role { get; set; }
        public string Description { get; set; }
        public List<Topic> Topics { get; set; }
    }
}
