using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness_App.BL.Models;
using Fitness_App.DAL.Models;

namespace Fitness_App.BL.Interfaces
{
    public interface IAdminRepository<T> : IUserRepository<T> where T : class
    {

       // mohammed
        T GetUsersData();

        // Exercise management
        List<Exercises> GetExerciseData();
        void AddExercise(Exercises exercises);
        void DeleteExercise(int id);
        Exercises UpdateExercise(Exercises exercises);

        SubscriptionPlan GetSubscriptionPlan(int id);
        void DeleteSubscriptionPlan(int id);
        void UpdateSubscriptionPlan(SubscriptionPlan plan);
        List<SubscriptionPlan> GetSubscriptionPlans();
        void AddSubscriptionPlan(SubscriptionPlan plan);

        //mohammed

        //sara
        // Coach Management

        //ADD coach
        T GetCoachData();
        T GetCoachById(int id);
        //void UpdateCoach(Coach coach);
        //void DeleteCoach(Coach coach);
        //sara

        //Zeiado
        // Blogs Management
        void AddBlogs(Blogs blog);

        void UpdateBlogs(Blogs blog);
        void DeleteBlogs(int id);
        List<Blogs> GetAllBlogs();
        Blogs GetBlogById(int id);
        //Zeiado

        // Comments Management
        void AddComment(Comment comment);
        List<Comment> GetAllComments();
        List<Comment> GetCommentsByBlogId(int blogId);
        Comment GetCommentById(int id);
        void DeleteComment(int id);
        void UpdateComment(Comment comment);

        // tarek
        // Diet Plan management
        void AddDietPlan(DietPlans dietPlan);
        IEnumerable<DietPlans> GetAllDietPlans();
        T GetDietPlanById(int id);
        void DeleteDietPlan(int id);
        //tarek

        // Admin Management(GET method)
        T AdminsManagement();

        T ShowSystemAnalytics();
        // Feedback Management
        T GetAllFeedback();

    }
}
