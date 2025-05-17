using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.DbContext;
using Fitness_App.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness_App.BL.Services
{
    public class WorkoutPlanService
    {
        private readonly FitnessAppDbContext _context;

        public WorkoutPlanService(FitnessAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkoutPlanViewModel>> GetAllWorkoutPlansAsync()
        {
            try
            {
                return await _context.WorkoutPlans
                    .Include(w => w.Coach)
                    .Select(w => new WorkoutPlanViewModel
                    {
                        Id = w.Id,
                        Title = w.Title,
                        Description = w.Description,
                        CoachId = w.CoachId,
                        CoachName = w.Coach.Name,
                        CreatedDate = w.CreatedDate
                        
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllWorkoutPlansAsync: {ex.Message}");
                return new List<WorkoutPlanViewModel>();
            }
        }

        public async Task<List<WorkoutPlanViewModel>> GetWorkoutPlansByCoachIdAsync(int coachId)
        {
            var workoutPlans = await _context.WorkoutPlans
                .Include(w => w.Coach)
                .Include(w => w.AssignedUsers)
                .Where(w => w.CoachId == coachId)
                .ToListAsync();

            return workoutPlans.Select(w => new WorkoutPlanViewModel
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                CreatedDate = w.CreatedDate,
                CoachId = w.CoachId,
                CoachName = w.Coach?.Name,
                AssignedUserIds = w.AssignedUsers?.Select(u => u.Id).ToList() ?? new List<string>(),
                AssignedUsers = w.AssignedUsers?.Select(u => new AssignedUserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email
                }).ToList() ?? new List<AssignedUserViewModel>()
            }).ToList();
        }

        public async Task<List<WorkoutPlanViewModel>> GetWorkoutPlansByUserIdAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.AssignedWorkoutPlans)
                    .ThenInclude(w => w.Coach)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new List<WorkoutPlanViewModel>();

            return user.AssignedWorkoutPlans?.Select(w => new WorkoutPlanViewModel
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                CreatedDate = w.CreatedDate,
                CoachId = w.CoachId,
                CoachName = w.Coach?.Name
            }).ToList() ?? new List<WorkoutPlanViewModel>();
        }

        public async Task<WorkoutPlanViewModel?> GetWorkoutPlanByIdAsync(int id)
        {
            try
            {
                return await _context.WorkoutPlans
                    .Include(w => w.Coach)
                    .Where(w => w.Id == id)
                    .Select(w => new WorkoutPlanViewModel
                    {
                        Id = w.Id,
                        Title = w.Title,
                        Description = w.Description,
                        CoachId = w.CoachId,
                        CoachName = w.Coach.Name,
                        CreatedDate = w.CreatedDate
                        
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWorkoutPlanByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddWorkoutPlanAsync(WorkoutPlanViewModel model)
        {
            Console.WriteLine($"DEBUG WorkoutPlanService: AddWorkoutPlanAsync called with Title='{model.Title}'");
            Console.WriteLine($"DEBUG WorkoutPlanService: CoachId={model.CoachId}");
            
            try
            {
                var workoutPlan = new WorkoutPlan
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    CoachId = model.CoachId
                };

                Console.WriteLine($"DEBUG WorkoutPlanService: Created new WorkoutPlan entity with CoachId={workoutPlan.CoachId}");

                _context.WorkoutPlans.Add(workoutPlan);
                Console.WriteLine("DEBUG WorkoutPlanService: Added WorkoutPlan to context");
                
                var saveResult = await _context.SaveChangesAsync();
                Console.WriteLine($"DEBUG WorkoutPlanService: SaveChangesAsync result={saveResult}");

                // Verify the plan was saved correctly
                var savedPlan = await _context.WorkoutPlans.FindAsync(workoutPlan.Id);
                if (savedPlan != null)
                {
                    Console.WriteLine($"DEBUG WorkoutPlanService: Verified plan in DB with ID={savedPlan.Id}, Title='{savedPlan.Title}'");
                }
                else
                {
                    Console.WriteLine("ERROR WorkoutPlanService: Plan not found in DB after saving!");
                }

                // Assign to users if any
                if (model.AssignedUserIds != null && model.AssignedUserIds.Any())
                {
                    Console.WriteLine($"DEBUG WorkoutPlanService: Plan has {model.AssignedUserIds.Count} assigned users");
                    await AssignWorkoutPlanToUsersAsync(workoutPlan.Id, model.AssignedUserIds);
                }
                else
                {
                    Console.WriteLine("DEBUG WorkoutPlanService: No users to assign");
                }

                return workoutPlan.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR WorkoutPlanService: Exception in AddWorkoutPlanAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let the controller handle it
            }
        }

        public async Task<bool> UpdateWorkoutPlanAsync(WorkoutPlanViewModel model)
        {
            var workoutPlan = await _context.WorkoutPlans
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == model.Id);

            if (workoutPlan == null)
                return false;

            workoutPlan.Title = model.Title;
            workoutPlan.Description = model.Description;
            workoutPlan.CoachId = model.CoachId;

            // Update assigned users
            if (model.AssignedUserIds != null)
            {
                // Remove users that are no longer assigned
                var currentUserIds = workoutPlan.AssignedUsers.Select(u => u.Id).ToList();
                var usersToRemove = currentUserIds.Except(model.AssignedUserIds).ToList();
                var usersToAdd = model.AssignedUserIds.Except(currentUserIds).ToList();

                foreach (var userId in usersToRemove)
                {
                    var user = workoutPlan.AssignedUsers.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        workoutPlan.AssignedUsers.Remove(user);
                    }
                }

                foreach (var userId in usersToAdd)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        workoutPlan.AssignedUsers.Add(user);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWorkoutPlanAsync(int id)
        {
            var workoutPlan = await _context.WorkoutPlans
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workoutPlan == null)
                return false;

            // Remove all user assignments
            workoutPlan.AssignedUsers.Clear();

            _context.WorkoutPlans.Remove(workoutPlan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignWorkoutPlanToUserAsync(int workoutPlanId, string userId)
        {
            var workoutPlan = await _context.WorkoutPlans
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == workoutPlanId);

            if (workoutPlan == null)
                return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Check if already assigned
            if (workoutPlan.AssignedUsers.Any(u => u.Id == userId))
                return true; // Already assigned

            workoutPlan.AssignedUsers.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnassignWorkoutPlanFromUserAsync(int workoutPlanId, string userId)
        {
            var workoutPlan = await _context.WorkoutPlans
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == workoutPlanId);

            if (workoutPlan == null)
                return false;

            var user = workoutPlan.AssignedUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false; // Not assigned

            workoutPlan.AssignedUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignWorkoutPlanToUsersAsync(int workoutPlanId, List<string> userIds)
        {
            var workoutPlan = await _context.WorkoutPlans
                .Include(w => w.AssignedUsers)
                .FirstOrDefaultAsync(w => w.Id == workoutPlanId);

            if (workoutPlan == null)
                return false;

            foreach (var userId in userIds)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null && !workoutPlan.AssignedUsers.Any(u => u.Id == userId))
                {
                    workoutPlan.AssignedUsers.Add(user);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<WorkoutPlanViewModel>> GetWorkoutPlansByCoachAsync(int coachId)
        {
            try
            {
                return await _context.WorkoutPlans
                    .Where(wp => wp.CoachId == coachId)
                    .Select(wp => new WorkoutPlanViewModel
                    {
                        Id = wp.Id,
                        Title = wp.Title,
                        Description = wp.Description,
                        CoachId = wp.CoachId,
                        CoachName = wp.Coach.Name,
                        CreatedDate = wp.CreatedDate
                       
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetWorkoutPlansByCoachAsync: {ex.Message}");
                return new List<WorkoutPlanViewModel>();
            }
        }
    }
} 