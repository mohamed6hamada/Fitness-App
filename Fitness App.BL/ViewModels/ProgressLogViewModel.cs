using System;
using System.ComponentModel.DataAnnotations;

namespace Fitness_App.Models.ViewModels
{
    public class ProgressLogViewModel
    {
        public int ProgressLogId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Log Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Current weight is required")]
        [Range(0, 500, ErrorMessage = "Please enter a valid weight between 0-500")]
        [Display(Name = "Current Weight (kg)")]
        public double Weight { get; set; }

        [Display(Name = "Goal Weight (kg)")]
        [Range(0, 500, ErrorMessage = "Please enter a valid goal weight between 0-500")]
        public double GoalWeight { get; set; }

        [Display(Name = "Chest (cm)")]
        [Range(0, 300, ErrorMessage = "Please enter a valid measurement")]
        public double Chest { get; set; }

        [Display(Name = "Waist (cm)")]
        [Range(0, 300, ErrorMessage = "Please enter a valid measurement")]
        public double Waist { get; set; }

        [Display(Name = "Arms (cm)")]
        [Range(0, 300, ErrorMessage = "Please enter a valid measurement")]
        public double Arms { get; set; }

        [Display(Name = "Thighs (cm)")]
        [Range(0, 300, ErrorMessage = "Please enter a valid measurement")]
        public double Thighs { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(3000, ErrorMessage = "Notes cannot exceed 3000 characters")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        // Weight progress calculations
        [Display(Name = "Weight Change")]
        public double? WeightChange { get; set; }

        [Display(Name = "Weight to Goal")]
        public double? WeightToGoal { get; set; }

        // Calorie calculation
        [Display(Name = "Daily Calorie Needs")]
        public int? DailyCalorieNeeds { get; set; }
        
        [Display(Name = "Activity Level")]
        public string ActivityLevel { get; set; } = "Moderate";

        // User ID will be set from the current user in the controller
        public string UserId { get; set; }
        
        // User Email for display purposes
        [Display(Name = "User Email")]
        public string UserEmail { get; set; }

        // For chart/graph data
        public string ChartLabels { get; set; } // JSON string of dates
        public string ChartData { get; set; }   // JSON string of weights
    }
}