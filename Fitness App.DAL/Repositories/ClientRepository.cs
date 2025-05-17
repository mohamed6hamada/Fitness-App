using System;
using System.Collections.Generic;
using System.Linq;
using Fitness_App.BL.Interfaces;
using Fitness_App.BL.Models;
using Fitness_App.DAL.DbContext;
using Fitness_App.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Fitness_App.DAL.Repositories
{
    public class ClientRepository<T> : IClientRepository<T> where T : class
    {
        private readonly FitnessAppDbContext _context;

        public ClientRepository(FitnessAppDbContext context)
        {
            _context = context;
        }
        public bool CheckUserSubscriptions(string userId)
        {
            try
            {
                var userSubscriptions = _context.userSubscriptions.FirstOrDefault(us => us.UserId == userId);

                if (userSubscriptions == null)
                {
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking user subscriptions: {ex.Message}", ex);
            }
        }
        public void AddUserSubscription(UserSubscription userSubscription)
        {
            try
            {
                _context.userSubscriptions.Add(userSubscription);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user subscription: {ex.Message}", ex);
            }
        }

        public void AddProgressLog(ProgressLog progressLog)
        {
            try
            {
                Console.WriteLine($"Adding progress log for user {progressLog.UserId} on date {progressLog.Date}");
                _context.ProgressLog.Add(progressLog);
                _context.SaveChanges();
                Console.WriteLine($"Successfully saved progress log with ID {progressLog.ProgressLogId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProgressLog: {ex}");
                throw new Exception($"Error adding progress log: {ex.Message}", ex);
            }
        }

        public List<ProgressLog> GetProgressLogs()
        {
            try
            {
                Console.WriteLine("Starting GetProgressLogs...");
                var logs = _context.ProgressLog
                    .Include(p => p.User)
                    .AsNoTracking()  // For better performance on read-only operations
                    .ToList();
                
                Console.WriteLine($"Retrieved {logs.Count} progress logs");
                foreach (var log in logs)
                {
                    Console.WriteLine($"Log ID: {log.ProgressLogId}, User ID: {log.UserId}, Date: {log.Date}, Weight: {log.Weight}");
                }
                return logs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProgressLogs: {ex}");
                throw new Exception($"Error retrieving progress logs: {ex.Message}", ex);
            }
        }

        public ProgressLog GetProgressLogById(int id)
        {
            try
            {
                var log = _context.ProgressLog.Include(p => p.User).FirstOrDefault(p => p.ProgressLogId == id);
                Console.WriteLine($"Retrieved progress log {id}: {(log != null ? "Found" : "Not Found")}");
                return log;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProgressLogById: {ex}");
                throw new Exception($"Error retrieving progress log: {ex.Message}", ex);
            }
        }

        public void DeleteProgressLog(int id)
        {
            try
            {
                var progressLog = _context.ProgressLog.Find(id);
                if (progressLog != null)
                {
                    _context.ProgressLog.Remove(progressLog);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting progress log: {ex.Message}", ex);
            }
        }

        public void UpdateProgressLog(ProgressLog progressLog)
        {
            try
            {
                _context.Entry(progressLog).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating progress log: {ex.Message}", ex);
            }
        }

        public T GetUserById(string userId)
        {
            try
            {
                // Since we're working with ApplicationUser, we need to cast
                if (typeof(T) == typeof(ApplicationUser))
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                    return user as T;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user: {ex.Message}", ex);
            }
        }

        // Other implemented methods from the interface
        public T CalculateCalories()
        {
            throw new NotImplementedException();
        }

        public T ChangePassword(T user)
        {
            throw new NotImplementedException();
        }

        public T GetMyFeedbacks(int ClientId)
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

        public T SubmitFeedback(Feedback feedback)
        {
            throw new NotImplementedException();
        }

        public T UpdateProfile(T user)
        {
            throw new NotImplementedException();
        }
    }
}