using Fitness_App.BL.Interfaces;
using Fitness_App.DAL.DbContext;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Fitness_App.DAL.Repositories
{
    public class AdminRepository<T> : IAdminRepository<T> where T : class
    {
        private readonly FitnessAppDbContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminRepository(FitnessAppDbContext _context, IWebHostEnvironment webHostEnvironment)
        {
            context = _context;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<Blogs> GetAllBlogs()
        {
            return context.Blogs.ToList();
        }

        public Blogs GetBlogById(int id)
        {
            return context.Blogs.Find(id);
        }

        public void AddBlogs(Blogs blog)
        {
            context.Blogs.Add(blog);
            context.SaveChanges();
        }

        public void UpdateBlogs(Blogs blog)
        {
            context.Blogs.Update(blog);
            context.SaveChanges();
        }

        public void DeleteBlogs(int id)
        {
            var blog = context.Blogs.Find(id);
            if (blog != null)
            {
                context.Blogs.Remove(blog);
                context.SaveChanges();
            }
        }

        // Comment operations
        public void AddComment(Comment comment)
        {
            context.Comments.Add(comment);
            context.SaveChanges();
        }

        public List<Comment> GetAllComments()
        {
            return context.Comments.ToList();
        }

        public List<Comment> GetCommentsByBlogId(int blogId)
        {
            return context.Comments
                .Where(c => c.BlogId == blogId)
                .Include(c => c.User)
                .ToList();
        }

        public Comment GetCommentById(int id)
        {
            return context.Comments.Find(id);
        }

        public void DeleteComment(int id)
        {
            var comment = context.Comments.Find(id);
            if (comment != null)
            {
                context.Comments.Remove(comment);
                context.SaveChanges();
            }
        }

        public void UpdateComment(Comment comment)
        {
            context.Comments.Update(comment);
            context.SaveChanges();
        }

        ///////////////////////////////////////////////////////////
        public void AddDietPlan(DietPlans dietPlan)
        {
            if (dietPlan == null)
            {
                throw new ArgumentNullException(nameof(dietPlan));
            }

            context.DietPlans.Add(dietPlan);
            context.SaveChanges();
        }
        public void DeleteDietPlan(int id)
        {
            var dietPlan = context.DietPlans.Find(id);
            if (dietPlan != null)
            {
                // Delete the associated image file
                if (!string.IsNullOrEmpty(dietPlan.ImagePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, dietPlan.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                context.DietPlans.Remove(dietPlan);
                context.SaveChanges();
            }
        }
        public IEnumerable<DietPlans> GetAllDietPlans()
        {
            return context.DietPlans.ToList();
        }
        public T GetDietPlanById(int id)
        {
            return context.DietPlans.Find(id) as T;
        }
        /////////////////////////////////////////////////////////
       

        public T AdminsManagement()
        {
            throw new NotImplementedException();
        }

        public T ChangePassword(T user)
        {
            throw new NotImplementedException();
        }

        

        //public void DeleteCoach(Coach coach)
        //{
        //    throw new NotImplementedException();
        //}

        //////////////////////////////////////////////////////////
        public void AddExercise(Exercises exercises)
        {
            context.Exercises.Add(exercises);
            context.SaveChanges();
        }
        public void DeleteExercise(int id)
        {
            var exercise = context.Exercises.Find(id);

            context.Exercises.Remove(exercise);
            context.SaveChanges();
        }
      
        public List<Exercises> GetExerciseData()
        {
            var exercises = context.Exercises.ToList();
            return exercises;
        }
        public Exercises UpdateExercise(Exercises exercises)
        {
            var excitedExercise = context.Exercises.Find(exercises.ExerciseId);
            if (excitedExercise != null)
            {

                excitedExercise.Name = exercises.Name;
                excitedExercise.Description = exercises.Description;
                excitedExercise.GIF = exercises.GIF;
                excitedExercise.Type = exercises.Type;
                context.SaveChanges();
                return excitedExercise;
            }
            else
            {
                throw new ArgumentException("Exercise not found.");
            }
        }

        ///////////////////////////////////////////////////////////
        public SubscriptionPlan GetSubscriptionPlan(int PlanId)
        {
            return context.SubscriptionPlans.Find(PlanId);
        }
        public void AddSubscriptionPlan(SubscriptionPlan plan)
        {
            context.SubscriptionPlans.Add(plan);
            context.SaveChanges();
        }
        public List<SubscriptionPlan> GetSubscriptionPlans()
        {
            return context.SubscriptionPlans.ToList();
        }

        public void UpdateSubscriptionPlan(SubscriptionPlan plan)
        {
            context.SubscriptionPlans.Update(plan);
            context.SaveChanges();
        }
        public void DeleteSubscriptionPlan(int id)
        {
            var plan = context.SubscriptionPlans.Find(id);
            if (plan != null)
            {
                context.SubscriptionPlans.Remove(plan);
                context.SaveChanges();
            }
        }


        ////////////////////////////////////////////////////////////
        public T GetAllFeedback()
        {
            throw new NotImplementedException();
        }

        public T GetCoachById(int id)
        {
            throw new NotImplementedException();
        }

        public T GetCoachData()
        {
            throw new NotImplementedException();
        }

      

        public T GetUsersData()
        {
            throw new NotImplementedException();
        }

        public T Login(string Email, string Password)
        {
            throw new NotImplementedException();
        }

        public T Logout(string FName, string LName, string Password)
        {
            throw new NotImplementedException();
        }

        public T Register(T user)
        {
            throw new NotImplementedException();
        }

        public T ShowSystemAnalytics()
        {
            throw new NotImplementedException();
        }

      

        //public void UpdateCoach(Coach coach)
        //{
        //    throw new NotImplementedException();
        //}

      

        public T UpdateProfile(T user)
        {
            throw new NotImplementedException();
        }

        
    }
}
