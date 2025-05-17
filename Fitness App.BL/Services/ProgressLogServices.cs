using System;
using System.Collections.Generic;
using System.Linq;
using Fitness_App.BL.Interfaces;
using Fitness_App.DAL.Models;
using Fitness_App.Models.ViewModels;
using Newtonsoft.Json;
using System.Text;

namespace Fitness_App.BL.Services
{
    public class ProgressLogServices
    {
        private readonly IClientRepository<ApplicationUser> _clientRepository;

        public ProgressLogServices(IClientRepository<ApplicationUser> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public void AddProgressLog(ProgressLogViewModel viewModel)
        {
            Console.WriteLine($"Service: Adding progress log for user {viewModel.UserId}");
            
            // Ensure UserEmail is set
            if (string.IsNullOrEmpty(viewModel.UserEmail) && !string.IsNullOrEmpty(viewModel.UserId))
            {
                var user = _clientRepository.GetUserById(viewModel.UserId);
                if (user != null)
                {
                    viewModel.UserEmail = ((ApplicationUser)user).Email;
                    Console.WriteLine($"Service: Set email to {viewModel.UserEmail}");
                }
            }
            
            // Calculate daily calorie needs
            CalculateDailyCalorieNeeds(viewModel);
            
            var progressLog = MapViewModelToModel(viewModel);
            _clientRepository.AddProgressLog(progressLog);
            Console.WriteLine("Service: Progress log added successfully");
        }

        public List<ProgressLogViewModel> GetAllProgressLogs(string userId)
        {
            Console.WriteLine($"Service: Getting all progress logs for user {userId}");
            var allLogs = _clientRepository.GetProgressLogs();
            Console.WriteLine($"Service: Retrieved {allLogs.Count} total logs from repository");
            
            var userLogs = allLogs
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Date)
                .ToList();

            Console.WriteLine($"Service: Filtered to {userLogs.Count} logs for user {userId}");
            
            var viewModels = userLogs.Select(log => MapModelToViewModel(log)).ToList();
            Console.WriteLine($"Service: Mapped {viewModels.Count} logs to view models");
            
            // Ensure user email is populated for each log
            if (viewModels.Any() && !string.IsNullOrEmpty(userId))
            {
                var user = _clientRepository.GetUserById(userId);
                if (user != null)
                {
                    var email = ((ApplicationUser)user).Email;
                    foreach (var vm in viewModels)
                    {
                        vm.UserEmail = email;
                        // Calculate calories for each log
                        CalculateDailyCalorieNeeds(vm);
                    }
                    Console.WriteLine($"Service: Added email {email} to all logs");
                }
                else
                {
                    Console.WriteLine($"Service: Warning - User {userId} not found when trying to add email");
                }
            }

            return viewModels;
        }

        public ProgressLogViewModel GetProgressLogById(int id)
        {
            var progressLog = _clientRepository.GetProgressLogById(id);
            var viewModel = MapModelToViewModel(progressLog);
            
            // Calculate calories
            if (viewModel != null)
            {
                CalculateDailyCalorieNeeds(viewModel);
            }
            
            return viewModel;
        }

        public void UpdateProgressLog(ProgressLogViewModel viewModel)
        {
            // Calculate calories based on updated measurements
            CalculateDailyCalorieNeeds(viewModel);
            
            var progressLog = MapViewModelToModel(viewModel);
            var existingLog = _clientRepository.GetProgressLogById(progressLog.ProgressLogId);
            if (existingLog != null)
            {
                existingLog.Date = progressLog.Date;
                existingLog.Weight = progressLog.Weight;
                existingLog.GoalWeight = progressLog.GoalWeight;
                existingLog.Chest = progressLog.Chest;
                existingLog.Waist = progressLog.Waist;
                existingLog.Arms = progressLog.Arms;
                existingLog.Thighs = progressLog.Thighs;
                existingLog.Notes = progressLog.Notes;
                existingLog.UserId = progressLog.UserId;

                _clientRepository.UpdateProgressLog(existingLog);
            }
        }

        public void DeleteProgressLog(int id)
        {
            _clientRepository.DeleteProgressLog(id);
        }

        public ProgressLogViewModel GetProgressSummary(string userId)
        {
            var logs = _clientRepository.GetProgressLogs()
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Date)
                .ToList();

            if (!logs.Any())
                return new ProgressLogViewModel();

            var firstLog = logs.First();
            var latestLog = logs.Last();
            var viewModel = MapModelToViewModel(latestLog);

            // Calculate progress
            viewModel.WeightChange = latestLog.Weight - firstLog.Weight;
            viewModel.WeightToGoal = latestLog.GoalWeight - latestLog.Weight;

            // Prepare chart data
            var dates = logs.Select(l => l.Date.ToString("MM/dd/yyyy")).ToList();
            var weights = logs.Select(l => l.Weight).ToList();

            viewModel.ChartLabels = JsonConvert.SerializeObject(dates);
            viewModel.ChartData = JsonConvert.SerializeObject(weights);

            // Calculate calories
            CalculateDailyCalorieNeeds(viewModel);

            return viewModel;
        }

        public List<ProgressLogViewModel> GetAllUsersProgressLogs()
        {
            var logs = _clientRepository.GetProgressLogs()
                .OrderByDescending(p => p.Date)
                .ToList();

            var viewModels = new List<ProgressLogViewModel>();
            foreach (var log in logs)
            {
                var vm = MapModelToViewModel(log);
                // Get user email from the repository
                var user = _clientRepository.GetUserById(log.UserId);
                if (user != null)
                {
                    vm.UserEmail = user.Email;
                }
                viewModels.Add(vm);
            }

            return viewModels;
        }

        // Generate CSV content from progress logs
        public string GenerateProgressLogCsv(List<ProgressLogViewModel> logs)
        {
            var csvBuilder = new StringBuilder();
            
            // Add header row
            csvBuilder.AppendLine("Date,Weight (kg),Goal Weight (kg),Chest (cm),Waist (cm),Arms (cm),Thighs (cm),Daily Calorie Needs,Activity Level,Notes,User Email");
            
            // Add data rows
            foreach (var log in logs)
            {
                // Format notes to avoid CSV injection and handle commas
                string safeNotes = log.Notes?.Replace("\"", "\"\"").Replace(",", ";") ?? "";
                
                csvBuilder.AppendLine(
                    $"{log.Date.ToString("yyyy-MM-dd")}," +
                    $"{log.Weight}," +
                    $"{log.GoalWeight}," +
                    $"{log.Chest}," +
                    $"{log.Waist}," +
                    $"{log.Arms}," +
                    $"{log.Thighs}," +
                    $"{log.DailyCalorieNeeds ?? 0}," +
                    $"{log.ActivityLevel ?? "Moderate"}," +
                    $"\"{safeNotes}\"," +
                    $"{log.UserEmail ?? ""}"
                );
            }
            
            return csvBuilder.ToString();
        }

        // Helper methods for mapping between models and viewmodels
        private ProgressLog MapViewModelToModel(ProgressLogViewModel viewModel)
        {
            return new ProgressLog
            {
                ProgressLogId = viewModel.ProgressLogId,
                Date = viewModel.Date,
                Weight = viewModel.Weight,
                GoalWeight = viewModel.GoalWeight,
                Chest = viewModel.Chest,
                Waist = viewModel.Waist,
                Arms = viewModel.Arms,
                Thighs = viewModel.Thighs,
                Notes = viewModel.Notes,
                UserId = viewModel.UserId,
                ActivityLevel = viewModel.ActivityLevel,
                DailyCalorieNeeds = viewModel.DailyCalorieNeeds
            };
        }

        private ProgressLogViewModel MapModelToViewModel(ProgressLog model)
        {
            if (model == null)
                return null;

            var viewModel = new ProgressLogViewModel
            {
                ProgressLogId = model.ProgressLogId,
                Date = model.Date,
                Weight = model.Weight,
                GoalWeight = model.GoalWeight,
                Chest = model.Chest,
                Waist = model.Waist,
                Arms = model.Arms,
                Thighs = model.Thighs,
                Notes = model.Notes,
                UserId = model.UserId,
                ActivityLevel = model.ActivityLevel ?? "Moderate",
                DailyCalorieNeeds = model.DailyCalorieNeeds
            };

            // Try to get user email
            if (!string.IsNullOrEmpty(model.UserId))
            {
                var user = _clientRepository.GetUserById(model.UserId);
                if (user != null)
                {
                    viewModel.UserEmail = ((ApplicationUser)user).Email;
                }
            }

            return viewModel;
        }

        private List<ProgressLogViewModel> MapModelListToViewModelList(List<ProgressLog> models)
        {
            return models.Select(m => MapModelToViewModel(m)).ToList();
        }
        
        // Calculate daily calorie needs based on user data
        private void CalculateDailyCalorieNeeds(ProgressLogViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.UserId))
                return;
            
            // Get user data
            var user = _clientRepository.GetUserById(viewModel.UserId) as ApplicationUser;
            if (user == null)
                return;
            
            // If we don't have height or age data, we can't calculate BMR
            if (!user.Height.HasValue || !user.DateOfBirth.HasValue)
                return;
                
            // Get age from date of birth
            int age = CalculateAge(user.DateOfBirth.Value);
            double heightCm = user.Height.Value;
            double weightKg = viewModel.Weight;
            string gender = user.Gender?.ToLower() ?? "male"; // Default to male if not specified
            
            // Calculate BMR (Basal Metabolic Rate) using Mifflin-St Jeor Equation
            double bmr;
            if (gender == "female")
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * age - 161;
            }
            else // male or other
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * age + 5;
            }
            
            // Apply activity factor
            double activityFactor = GetActivityFactor(viewModel.ActivityLevel);
            int calorieNeeds = (int)Math.Round(bmr * activityFactor);
            
            // Set the value
            viewModel.DailyCalorieNeeds = calorieNeeds;
        }

        // Helper method to calculate age from date of birth
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;
                
            return age;
        }

        // Get activity factor based on activity level
        private double GetActivityFactor(string activityLevel)
        {
            switch (activityLevel?.ToLower() ?? "moderate")
            {
                case "sedentary":
                    return 1.2; // Little or no exercise
                case "light":
                    return 1.375; // Light exercise 1-3 days/week
                case "moderate":
                    return 1.55; // Moderate exercise 3-5 days/week
                case "active":
                    return 1.725; // Active - Hard exercise 6-7 days/week
                case "very active":
                    return 1.9; // Very active - Hard daily exercise & physical job
                default:
                    return 1.55; // Default to moderate
            }
        }
    }
}