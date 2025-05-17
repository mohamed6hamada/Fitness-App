using System;
using System.ComponentModel.DataAnnotations;

namespace Fitness_App.BL.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Comment content is required")]
        [Display(Name = "Your Comment")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public int BlogId { get; set; }

        public string UserId { get; set; }
        
        public string UserName { get; set; }
        
        public string UserEmail { get; set; }
    }
} 