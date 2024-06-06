﻿using System.ComponentModel.DataAnnotations;

namespace WebForum.Models.ViewModels
{
    public class CreateTopicViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
