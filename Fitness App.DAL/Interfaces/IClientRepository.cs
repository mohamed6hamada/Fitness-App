using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness_App.BL.Models;
using Fitness_App.DAL.Models;

namespace Fitness_App.BL.Interfaces
{
    public interface IClientRepository<T> : IUserRepository<T> where T : class

    {
        void AddUserSubscription(UserSubscription userSubscription);
        void AddProgressLog(ProgressLog progressLog);
        List<ProgressLog> GetProgressLogs();
        ProgressLog GetProgressLogById(int id);
        void DeleteProgressLog(int id);
        void UpdateProgressLog(ProgressLog progressLog);
        T GetUserById(string userId);
        T SubmitFeedback(Feedback feedback);
        T GetMyFeedbacks(int ClientId);
        //T Subscribe(Subscription subscription);
        //T Unsubscribe(Subscription subscription);

        T CalculateCalories();
    }
}
