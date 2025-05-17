using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Fitness_App.DAL.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Fitness_App.BL.Services
{
    public class ClientCoachSubscriptionService
    {
        private readonly FitnessAppDbContext _context;

        public ClientCoachSubscriptionService(FitnessAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SubscribeToCoach(string clientId, int coachId, DateTime endDate)
        {
            try
            {
                // Check if client already has an active subscription with any coach
                var existingSubscription = await _context.ClientCoachSubscriptions
                    .FirstOrDefaultAsync(s => s.ClientId == clientId && s.IsActive);

                if (existingSubscription != null)
                {
                    return false; // Client already has an active subscription
                }

                var subscription = new ClientCoachSubscription
                {
                    ClientId = clientId,
                    CoachId = coachId,
                    StartDate = DateTime.UtcNow,
                    EndDate = endDate,
                    IsActive = true
                };

                _context.ClientCoachSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CoachViewModel>> GetAvailableCoaches()
        {
            return await _context.Coaches
                .Where(c => c.IsActive)
                .Select(c => new CoachViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Bio = c.Bio,
                    Experience = c.Experience,
                    ImageUrl = c.ImageUrl,
                    Specialization = c.Specialization
                })
                .ToListAsync();
        }

        public async Task<List<ClientCoachSubscriptionViewModel>> GetCoachClients(int coachId)
        {
            return await _context.ClientCoachSubscriptions
                .Where(s => s.CoachId == coachId && s.IsActive)
                .Select(s => new ClientCoachSubscriptionViewModel
                {
                    Id = s.Id,
                    ClientId = s.ClientId,
                    ClientName = s.Client.FullName,
                    ClientEmail = s.Client.Email,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    WorkoutPlanId = s.WorkoutPlanId,
                    WorkoutPlanTitle = s.AssignedWorkoutPlan.Title
                })
                .ToListAsync();
        }

        public async Task<List<ClientCoachSubscriptionViewModel>> GetClientSubscriptions(string clientId)
        {
            return await _context.ClientCoachSubscriptions
                .Where(s => s.ClientId == clientId)
                .Select(s => new ClientCoachSubscriptionViewModel
                {
                    Id = s.Id,
                    CoachId = s.CoachId,
                    CoachName = s.Coach.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive,
                    WorkoutPlanId = s.WorkoutPlanId,
                    WorkoutPlanTitle = s.AssignedWorkoutPlan.Title
                })
                .ToListAsync();
        }

        public async Task<bool> AssignWorkoutPlan(int subscriptionId, int workoutPlanId)
        {
            try
            {
                var subscription = await _context.ClientCoachSubscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive);

                if (subscription == null)
                    return false;

                subscription.WorkoutPlanId = workoutPlanId;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EndSubscription(int subscriptionId)
        {
            try
            {
                var subscription = await _context.ClientCoachSubscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive);

                if (subscription == null)
                    return false;

                subscription.IsActive = false;
                subscription.EndDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ClientCoachSubscriptionViewModel> GetSubscriptionById(int subscriptionId)
        {
            return await _context.ClientCoachSubscriptions
                .Where(s => s.Id == subscriptionId)
                .Select(s => new ClientCoachSubscriptionViewModel
                {
                    Id = s.Id,
                    CoachId = s.CoachId,
                    ClientId = s.ClientId,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive,
                    WorkoutPlanId = s.WorkoutPlanId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UnsubscribeClient(int subscriptionId)
        {
            try
            {
                var subscription = await _context.ClientCoachSubscriptions
                    .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive);

                if (subscription == null)
                    return false;

                subscription.IsActive = false;
                subscription.EndDate = DateTime.UtcNow;
                subscription.WorkoutPlanId = null; // Remove workout plan assignment
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 