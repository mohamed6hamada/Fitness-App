using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.DbContext;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness_App.BL.Services
{
    public class CoachService
    {
        private readonly FitnessAppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CoachService(
            FitnessAppDbContext context, 
            IWebHostEnvironment webHostEnvironment, 
            UserManager<ApplicationUser> userManager = null, 
            RoleManager<IdentityRole> roleManager = null)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<CoachViewModel>> GetAllCoachesAsync()
        {
            try
            {
                return await _context.Coaches
                    .Where(c => c.IsActive)
                    .Select(c => new CoachViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email ?? string.Empty,
                        Bio = c.Bio,
                        Specialization = c.Specialization,
                        Experience = c.Experience,
                        ImageUrl = c.ImageUrl,
                        IsActive = c.IsActive
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllCoachesAsync: {ex.Message}");
                return new List<CoachViewModel>();
            }
        }

        public async Task<CoachViewModel?> GetCoachByIdAsync(int id)
        {
            try
            {
                return await _context.Coaches
                    .Where(c => c.Id == id)
                    .Select(c => new CoachViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email ?? string.Empty,
                        Bio = c.Bio,
                        Specialization = c.Specialization,
                        Experience = c.Experience,
                        ImageUrl = c.ImageUrl,
                        IsActive = c.IsActive
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCoachByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<CoachViewModel?> GetCoachByEmailAsync(string email)
        {
            try
            {
                return await _context.Coaches
                    .Where(c => c.Email == email && c.IsActive)
                    .Select(c => new CoachViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email ?? string.Empty,
                        Bio = c.Bio,
                        Specialization = c.Specialization,
                        Experience = c.Experience,
                        ImageUrl = c.ImageUrl,
                        IsActive = c.IsActive
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCoachByEmailAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<int> AddCoachAsync(CoachViewModel model, bool createUserAccount = false, string defaultPassword = "Coach@123")
        {
            try
            {
                Console.WriteLine($"Adding coach: {model.Name}");
                
                string? uniqueFileName = null;
                
                // Process image if provided
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    Console.WriteLine("Processing image file");
                    uniqueFileName = await ProcessUploadedFile(model);
                }
                else
                {
                    Console.WriteLine("No image file provided - continuing without image");
                }

                var coach = new Coach
                {
                    Name = model.Name,
                    Bio = model.Bio,
                    Specialization = model.Specialization,
                    Email = model.Email,
                    ImageUrl = uniqueFileName,
                    WorkoutPlans = new List<WorkoutPlan>()
                };

                _context.Coaches.Add(coach);
                await _context.SaveChangesAsync();
                
                // Create a user account for the coach if requested and services are available
                if (createUserAccount && !string.IsNullOrEmpty(model.Email) && _userManager != null && _roleManager != null)
                {
                    await CreateCoachUserAccountAsync(model.Email, model.Name, defaultPassword);
                }
                
                Console.WriteLine($"Coach added with ID: {coach.Id}");
                return coach.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding coach: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateCoachAsync(CoachViewModel model)
        {
            try
            {
                var coach = await _context.Coaches.FindAsync(model.Id);

                if (coach == null)
                    return false;

                // Process new image if provided
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string? uniqueFileName = await ProcessUploadedFile(model);
                    
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(coach.ImageUrl))
                    {
                        DeleteImage(coach.ImageUrl);
                    }
                    
                    coach.ImageUrl = uniqueFileName;
                }

                coach.Name = model.Name;
                coach.Bio = model.Bio;
                coach.Specialization = model.Specialization;
                coach.Email = model.Email;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating coach: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCoachAsync(int id)
        {
            try
            {
                var coach = await _context.Coaches
                    .Include(c => c.WorkoutPlans)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (coach == null)
                    return false;

                // Check if coach has workout plans
                if (coach.WorkoutPlans != null && coach.WorkoutPlans.Any())
                {
                    // Cannot delete coach with workout plans
                    return false;
                }

                // Delete coach image
                if (!string.IsNullOrEmpty(coach.ImageUrl))
                {
                    DeleteImage(coach.ImageUrl);
                }

                _context.Coaches.Remove(coach);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting coach: {ex.Message}");
                return false;
            }
        }

        private async Task<string?> ProcessUploadedFile(CoachViewModel model)
        {
            string? uniqueFileName = null;

            try
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "coaches");
                    
                    Console.WriteLine($"Image upload folder: {uploadsFolder}");
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Console.WriteLine("Creating directory for coach images");
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    Console.WriteLine($"Saving image to: {filePath}");
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    
                    // Store the relative path starting with /images
                    uniqueFileName = "/images/coaches/" + uniqueFileName;
                    Console.WriteLine($"Image saved, path: {uniqueFileName}");
                    return uniqueFileName;
                }
                else
                {
                    Console.WriteLine("Image file is null or empty, not processing");
                }
                
                return uniqueFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image: {ex.Message}");
                return null;
            }
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                // Remove the leading slash if present
                imageUrl = imageUrl.TrimStart('/');
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"Deleted image: {filePath}");
                }
                else
                {
                    Console.WriteLine($"Image not found for deletion: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }

        // New method to create a user account for a coach
        private async Task<bool> CreateCoachUserAccountAsync(string email, string name, string defaultPassword)
        {
            try
            {
                // Check if the user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    Console.WriteLine($"User with email {email} already exists - just assigning Coach role");
                    // User exists - just add them to Coach role if not already
                    if (!await _userManager.IsInRoleAsync(existingUser, "Coach"))
                    {
                        await _userManager.AddToRoleAsync(existingUser, "Coach");
                    }
                    return true;
                }
                
                // Create the Coach role if it doesn't exist
                if (!await _roleManager.RoleExistsAsync("Coach"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Coach"));
                    Console.WriteLine("Created new Coach role");
                }
                
                // Create new user
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    EmailConfirmed = true // Auto-confirm for simplicity
                };
                
                var result = await _userManager.CreateAsync(user, defaultPassword);
                if (result.Succeeded)
                {
                    // Add to Coach role
                    await _userManager.AddToRoleAsync(user, "Coach");
                    Console.WriteLine($"Created new user account for coach {email} with Coach role");
                    return true;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Failed to create user: {errors}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating coach user account: {ex.Message}");
                return false;
            }
        }
    }
} 