using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.BL.ViewModels
{
    public class SubscriptionPlanViewModel
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int DurationInDays { get; set; }
    }
}
