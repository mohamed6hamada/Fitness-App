using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness_App.DAL.Models
{
    public class ClientCoachSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public int CoachId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual ApplicationUser Client { get; set; }

        [ForeignKey("CoachId")]
        public virtual Coach Coach { get; set; }

        // Optional: Link to the workout plan assigned to this subscription
        public int? WorkoutPlanId { get; set; }

        [ForeignKey("WorkoutPlanId")]
        public virtual WorkoutPlan AssignedWorkoutPlan { get; set; }
    }
} 