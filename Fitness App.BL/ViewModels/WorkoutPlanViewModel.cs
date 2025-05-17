using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.BL.ViewModels
{
    public class WorkoutPlanViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(5000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Coach is required")]
        public int CoachId { get; set; }
        
        public string? CoachName { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(500)]
        public string? Goals { get; set; }

        [StringLength(100)]
        public string? Duration { get; set; }

        [StringLength(500)]
        public string? Equipment { get; set; }

        public string? Schedule { get; set; }
        public string? Notes { get; set; }

        // Navigation properties for assigned users
        public List<string>? AssignedUserIds { get; set; }
        public List<AssignedUserViewModel>? AssignedUsers { get; set; }

        public WorkoutPlanViewModel()
        {
            AssignedUserIds = new List<string>();
            AssignedUsers = new List<AssignedUserViewModel>();
            CreatedDate = DateTime.Now;
        }
    }

    public class AssignedUserViewModel
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
} 