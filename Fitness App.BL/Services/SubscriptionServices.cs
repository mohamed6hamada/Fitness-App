using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.BL.Servecies
{
    public class SubscriptionServices
    {
        private readonly IAdminRepository<SubscriptionPlanViewModel> _adminRepository;
        public SubscriptionServices(IAdminRepository<SubscriptionPlanViewModel> adminRepository)
        {
            _adminRepository = adminRepository;
        }
        //public SubscriptionPlanViewModel GetSubscriptionPlan(int id)
        //{
        //   var sub= _adminRepository.GetSubscriptionPlan(id);
        //    var viewmodel = new SubscriptionPlanViewModel
        //    {
        //        PlanId = sub.PlanId,
        //        Name = sub.Name,
        //        Description = sub.Description,
        //        Price = sub.Price
        //    };return viewmodel;
        //}
        
        public SubscriptionPlan GetSubscriptionPlan(int id)
        {
            var sub = _adminRepository.GetSubscriptionPlan(id);
           
            return sub;
        }
        public void AddSubscriptionPlan(SubscriptionPlanViewModel plan)
        {
            _adminRepository.AddSubscriptionPlan(new DAL.Models.SubscriptionPlan
            {
                Name = plan.Name,
                Price = plan.Price,
                Description = plan.Description,
                DurationInDays = plan.DurationInDays
            });
        }
        public void UpdateSubscriptionPlan(SubscriptionPlanViewModel plan)
        {
            _adminRepository.UpdateSubscriptionPlan(new DAL.Models.SubscriptionPlan
            {
                PlanId = plan.PlanId,
                Name = plan.Name,
                Price = plan.Price,
                Description = plan.Description,
                DurationInDays = plan.DurationInDays
            });
        }
        public List<SubscriptionPlanViewModel> GetAllSubscriptionPlans()
        {
            var plans = _adminRepository.GetSubscriptionPlans();
            return plans.Select(p => new SubscriptionPlanViewModel
            {
                PlanId = p.PlanId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                DurationInDays = p.DurationInDays
            }).ToList();
        }
        public void DeleteSubscriptionPlan(int id)
        {
            try
            {
                _adminRepository.DeleteSubscriptionPlan(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
