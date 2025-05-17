using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.DAL.Models
{
    public class Blogs
    {
        [Key]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Blog title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string BlogTitle { get; set; }

        [Required(ErrorMessage = "Blog content is required")]
        public string BlogContent { get; set; }
        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property for Comments
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
