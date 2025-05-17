using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.BL.ViewModels
{
    public class CoachViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(255)]
        public string? Specialization { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
        public int Experience { get; set; }

        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // For uploading coach image - explicitly nullable
        public IFormFile? ImageFile { get; set; }

        // Count of workout plans created by this coach
        public int WorkoutPlansCount { get; set; }
    }
} 