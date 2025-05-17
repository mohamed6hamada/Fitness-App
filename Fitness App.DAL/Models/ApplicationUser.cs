using Fitness_App.BL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.DAL.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? FullName { get; set; }
        
        // Profile fields
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        
        // Social media links
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        
        // Fitness specific fields
        public string? FitnessGoals { get; set; }
        public string? PreferredWorkoutTimes { get; set; }
        public double? Height { get; set; }
        
        // Account status fields
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        
        // Creation date
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        // Navigation properties to related entities
        public virtual ICollection<ClientCoachSubscription> CoachSubscriptions { get; set; }
        public virtual ICollection<ProgressLog> ProgressLogs { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<WorkoutPlan> AssignedWorkoutPlans { get; set; }

        public ApplicationUser()
        {
            CoachSubscriptions = new HashSet<ClientCoachSubscription>();
            ProgressLogs = new HashSet<ProgressLog>();
            Feedbacks = new HashSet<Feedback>();
            AssignedWorkoutPlans = new HashSet<WorkoutPlan>();
        }
    }
}
