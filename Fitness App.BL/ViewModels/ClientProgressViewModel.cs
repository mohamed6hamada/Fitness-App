using System;
using System.Collections.Generic;
using Fitness_App.Models.ViewModels;

namespace Fitness_App.BL.ViewModels
{
    public class ClientProgressViewModel
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public List<ProgressLogViewModel> ProgressLogs { get; set; }
        public double StartWeight { get; set; }
        public double CurrentWeight { get; set; }
        public int TotalWorkouts { get; set; }
        public DateTime LastUpdateDate { get; set; }

        // Calculate weight change
        public double WeightChange => CurrentWeight - StartWeight;
        public bool HasImproved => WeightChange < 0; // Assuming weight loss is the goal
    }
} 