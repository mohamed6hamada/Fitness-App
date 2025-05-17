using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Fitness_App.BL.Services
{
    public class ProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClientRepository<ApplicationUser> _clientRepository;
        private readonly ProgressLogServices _progressLogServices;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IClientRepository<ApplicationUser> clientRepository,
            ProgressLogServices progressLogServices)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _clientRepository = clientRepository;
            _progressLogServices = progressLogServices;
        }

        public async Task<ProfileViewModel> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var progressLogs = _progressLogServices.GetAllProgressLogs(userId);
            
            var profile = new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                Bio = user.Bio,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                FacebookUrl = user.FacebookUrl,
                InstagramUrl = user.InstagramUrl,
                TwitterUrl = user.TwitterUrl,
                LinkedInUrl = user.LinkedInUrl,
                FitnessGoals = user.FitnessGoals,
                PreferredWorkoutTimes = user.PreferredWorkoutTimes,
                Height = user.Height,
                TotalProgressLogs = progressLogs.Count,
                MemberSince = user.MemberSince
            };

            if (progressLogs.Count > 0)
            {
                // Get weight change
                var summary = _progressLogServices.GetProgressSummary(userId);
                profile.WeightChange = summary.WeightChange;
            }

            return profile;
        }

        public async Task<bool> UpdateProfileAsync(ProfileViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return false;

            // Only update essential fields and those that are provided
            user.FullName = model.FullName;
            
            // Update optional fields - ensure they're never null
            user.PhoneNumber = model.PhoneNumber ?? string.Empty;
                
            // Update other fields - ensure they're never null
            user.Bio = model.Bio ?? string.Empty;
            user.Gender = model.Gender ?? string.Empty;
            user.FacebookUrl = model.FacebookUrl ?? string.Empty;
            user.InstagramUrl = model.InstagramUrl ?? string.Empty;
            user.TwitterUrl = model.TwitterUrl ?? string.Empty;
            user.LinkedInUrl = model.LinkedInUrl ?? string.Empty;
            user.FitnessGoals = model.FitnessGoals ?? string.Empty;
            user.PreferredWorkoutTimes = model.PreferredWorkoutTimes ?? string.Empty;
            user.DateOfBirth = model.DateOfBirth;
            user.Height = model.Height;

            // Handle profile picture upload
            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                // Create profile pictures directory if it doesn't exist
                var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(uploadDir);

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicture.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Save the new image
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ProfilePictureFile.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePictureFile.CopyToAsync(stream);
                }

                // Update the profile picture path
                user.ProfilePicture = $"/images/profiles/{fileName}";
            }
            else if (string.IsNullOrEmpty(user.ProfilePicture))
            {
                // If user doesn't have a profile picture and didn't upload one, set default
                user.ProfilePicture = "/images/profiles/default-profile.png";
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
} 