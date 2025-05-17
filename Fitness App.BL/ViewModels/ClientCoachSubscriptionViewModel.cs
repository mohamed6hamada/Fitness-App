using System;
using System.ComponentModel.DataAnnotations;

namespace Fitness_App.BL.ViewModels
{
    public class ClientCoachSubscriptionViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Client ID is required")]
        public string ClientId { get; set; }
        
        public string? ClientName { get; set; }
        public string? ClientEmail { get; set; }
        
        [Required(ErrorMessage = "Coach selection is required")]
        public int CoachId { get; set; }
        
        public string? CoachName { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        
        public int? WorkoutPlanId { get; set; }
        public string? WorkoutPlanTitle { get; set; }
    }
} 