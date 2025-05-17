using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_App.DAL.Models
{
    public class WorkoutPlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("Coach")]
        public int CoachId { get; set; }
        public virtual Coach Coach { get; set; }

        // Collection of users who have been assigned this workout plan
        public virtual ICollection<ApplicationUser> AssignedUsers { get; set; }

        public WorkoutPlan()
        {
            CreatedDate = DateTime.Now;
            AssignedUsers = new HashSet<ApplicationUser>();
        }
    }
} 