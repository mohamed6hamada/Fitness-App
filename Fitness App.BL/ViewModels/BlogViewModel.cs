using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fitness_App.BL.ViewModels
{
    public class BlogViewModel
    {
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Blog title is required")]
        [Display(Name = "Title")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string BlogTitle { get; set; }

        [Required(ErrorMessage = "Blog content is required")]
        [Display(Name = "Content")]
        public string BlogContent { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Please select an image for the blog")]
        [Display(Name = "Blog Image")]
        public IFormFile ImageFile { get; set; }
    }
}
