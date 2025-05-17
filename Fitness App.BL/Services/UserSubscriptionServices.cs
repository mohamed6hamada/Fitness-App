using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Fitness_App.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.BL.Servecies
{
    public class UserSubscriptionServices
    {
        private readonly ClientRepository<UserSubscriptionViewModel> clientRepository;

        public UserSubscriptionServices(ClientRepository<UserSubscriptionViewModel> clientRepository) 
        {
            this.clientRepository = clientRepository;
        }
        public void AddUserSubscription(UserSubscriptionViewModel userSubscription)
        {
            if (userSubscription == null)
            {
                throw new ArgumentNullException(nameof(userSubscription));
            }
            var subscription = new UserSubscription
            {
                UserId = userSubscription.UserId,
                SubscriptionPlanId = userSubscription.SubscriptionPlanId,
                StartDate = userSubscription.StartDate,
                EndDate = userSubscription.EndDate,
                IsActive = true
            };
            clientRepository.AddUserSubscription(subscription);
        }
        public bool IsUserSubscribed(string Id)
        {
            var userSubscriptions = clientRepository.CheckUserSubscriptions(Id);
            
            if(userSubscriptions == true)
            {
                return true;
            }
            else
                return false;
        }
    }
}
