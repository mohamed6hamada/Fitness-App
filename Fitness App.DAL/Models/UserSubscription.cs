using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.DAL.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public virtual SubscriptionPlan Plan { get; set; }
    }

}
