using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.DAL.Models
{
    public class SubscriptionPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        public int DurationInDays { get; set; }
    }
}
