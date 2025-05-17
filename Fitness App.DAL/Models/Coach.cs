using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_App.DAL.Models
{
    public class Coach
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(255)]
        public string? Specialization { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public int Experience { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<ClientCoachSubscription> ClientSubscriptions { get; set; }
        public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; }

        public Coach()
        {
            ClientSubscriptions = new HashSet<ClientCoachSubscription>();
            WorkoutPlans = new HashSet<WorkoutPlan>();
        }
    }
} 