using Fitness_App.BL.Servecies;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fitness_App.BL.Services;
using Fitness_App.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Fitness_App.BL.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fitness_App.DAL.DbContext;

namespace FitnessAPPMVC.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
       
        /////////////////////////tarek/////////////////////////
        private readonly DietPlanService _dietPlanService;
        private readonly ExerciseServices _exerciseServices;
        private readonly SubscriptionServices _subscriptioServices;
        private readonly BlogServices _blogService;
        private readonly IWebHostEnvironment _env;
        private readonly ProgressLogServices _progressLogService;
        private readonly IClientRepository<ApplicationUser> _clientRepository;
        private readonly UserManagementService _userManagementService;
        private readonly CoachService _coachService;
        private readonly WorkoutPlanService _workoutPlanService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            DietPlanService dietPlanService, 
            ExerciseServices exerciseServices, 
            SubscriptionServices services,
            BlogServices blogService,
            IWebHostEnvironment env,
            ProgressLogServices progressLogService,
            IClientRepository<ApplicationUser> clientRepository,
            UserManagementService userManagementService,
            CoachService coachService,
            WorkoutPlanService workoutPlanService,
            UserManager<ApplicationUser> userManager)
        {
            _dietPlanService = dietPlanService;
            _exerciseServices = exerciseServices;
            this._subscriptioServices = services;
            _blogService = blogService;
            _env = env;
            _progressLogService = progressLogService;
            _clientRepository = clientRepository;
            _userManagementService = userManagementService;
            _coachService = coachService;
            _workoutPlanService = workoutPlanService;
            _userManager = userManager;
        }

        public IActionResult ManageSubscriptionPlans()
        {
            var plans = _subscriptioServices.GetAllSubscriptionPlans();
            var viewModel = plans.Select(p => new SubscriptionPlanViewModel
            {
                PlanId = p.PlanId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                DurationInDays = p.DurationInDays
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult UpdateSubscriptionPlan(int id)
        {
            var plan = _subscriptioServices.GetAllSubscriptionPlans().FirstOrDefault(p => p.PlanId == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View("UpdateSubscriptionPlan", plan);
        }
        [HttpPost]
        public IActionResult UpdateSubscriptionPlan(SubscriptionPlanViewModel model)
        {
            if (model.Name == null || model.Price == 0 || model.Description == null)
            {
                ModelState.AddModelError("", "Please fill all the fields");
                return View("UpdateSubscriptionPlan", model);
            }

            _subscriptioServices.UpdateSubscriptionPlan(model);
            return RedirectToAction("ManageSubscriptionPlans");



        }
        [HttpGet]
        public IActionResult AddSubscriptionPlan()
        {
            return View("AddSubscriptionPlan");
        }
        [HttpPost]
        public IActionResult AddSubscriptionPlan(SubscriptionPlanViewModel model)
        {
            if (model.Name == null || model.Price == 0 || model.Description == null || model.DurationInDays == 0)
            {
                ModelState.AddModelError("", "Please fill all the fields");
                return View("AddSubscriptionPlan", model);
            }

            _subscriptioServices.AddSubscriptionPlan(model);
            return RedirectToAction("ManageSubscriptionPlans");

        }
        public IActionResult DeleteSubscriptionPlan(int id)
        {
            try
            {
                _subscriptioServices.DeleteSubscriptionPlan(id);
            }
            catch (Exception ex)
            {
                // Log error
                ModelState.AddModelError("", "An error occurred while deleting the subscription plan.");
            }
            return RedirectToAction("ManageSubscriptionPlans");
        }

        public IActionResult DietPlanManagement()
        {
            var dietPlans = _dietPlanService.GetAllDietPlans();
            return View(dietPlans);
            
        }

        // Step 2: Show the Add Diet Plan Form
        public IActionResult AddDietPlan()
        {
            
            return View();
        }

        // Step 3: Handle Form Submission
        [HttpPost]
        public async Task<IActionResult> AddDietPlan(DietPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _dietPlanService.AddDietPlanAsync(model);
                return RedirectToAction("DietPlanManagement");
            }

            return View(model); // If invalid, stay on form
        }

        public IActionResult DeleteDietPlan(int id)
        {
            _dietPlanService.DeleteDietPlan(id);
            return RedirectToAction("DietPlanManagement");
        }



        /////////////////////////tarek/////////////////////////


        public IActionResult GetExerciseData()
        {
            var data = _exerciseServices.GetAllExercises();
            return View("GetExerciseData", data);
        }
        
        [HttpGet]
        public IActionResult AddExercises()
        {
            return View("AddExercises");
        }

        [HttpPost]
        public async Task<IActionResult> AddExerciseToDb(ExerciseViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("AddExercises", model);
            //}

            try
            {
                if (model.GIFFile != null && model.GIFFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await model.GIFFile.CopyToAsync(memoryStream);
                    model.GIF = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("GIFFile", "Please upload a GIF file.");
                    return View("AddExercises", model);
                }

                // You could map the view model to a domain model here if needed
                _exerciseServices.AddExercise(model);

                return RedirectToAction("GetExerciseData");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding exercise: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while adding the exercise.");
                return View("AddExercises", model);
            }
        }
        //public IActionResult ExerciseMangement()
        //{
        //    var data = _exerciseServices.GetAllExercises();
        //    return View(data);
        //}
        public IActionResult Delete(int id)
        {
            try
            {
                _exerciseServices.DeleteExercise(id);
            }
            catch (Exception ex)
            {
                // Log error
                ModelState.AddModelError("", "An error occurred while deleting the exercise.");
            }
            return RedirectToAction("GetExerciseData");
        }
        public IActionResult Update(int id)
        {
            var exercise = _exerciseServices.GetAllExercises().FirstOrDefault(e => e.ExerciseId == id);
            return View("Update", exercise);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateExercise(ExerciseViewModel exercise)
        {
            try
            {
                if (exercise.GIFFile != null && exercise.GIFFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await exercise.GIFFile.CopyToAsync(memoryStream);
                    exercise.GIF = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("GIFFile", "Please upload a GIF file.");
                    return View("Update", exercise.ExerciseId);
                }

                // You could map the view model to a domain model here if needed
                _exerciseServices.UpdateExercise(exercise);
                return RedirectToAction("GetExerciseData");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding exercise: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while adding the exercise.");
                return View("AddExercises", exercise);
            }

        }



       
        
       
        
        
       

        ////////////////// Blogs //////////////////

        // List all blogs
        public IActionResult BlogManagement()
        {
            var blogs = _blogService.GetAllBlogs();
            return View(blogs);
        }

        // Show the form
        public async Task<IActionResult> AddBlog() => View(new BlogViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBlog(BlogViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.BlogTitle))
                ModelState.AddModelError("BlogTitle", "Blog title is required");
            
            if (string.IsNullOrWhiteSpace(model.BlogContent))
                ModelState.AddModelError("BlogContent", "Blog content is required");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Map ViewModel to Entity
                var blog = new Blogs
                {
                    BlogTitle = model.BlogTitle.Trim(),
                    BlogContent = model.BlogContent.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                // Handle image upload
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, .png and .gif files are allowed");
                        return View(model);
                    }

                    if (model.ImageFile.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        ModelState.AddModelError("ImageFile", "File size cannot exceed 5MB");
                        return View(model);
                    }

                    var uploadDir = Path.Combine(_env.WebRootPath, "images", "blogs");
                    Directory.CreateDirectory(uploadDir);

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Store the path with forward slashes and starting with /
                    blog.ImagePath = $"/images/blogs/{fileName}";
                }

                _blogService.AddBlog(blog);
                TempData["SuccessMessage"] = "Blog created successfully!";
                return RedirectToAction("BlogManagement");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the blog: {ex.Message}");
                return View(model);
            }
        }

        // Show edit form
        public IActionResult EditBlog(int id)
        {
            var blog = _blogService.GetBlogById(id);
            if (blog == null) return NotFound();

            var vm = new BlogViewModel
            {
                BlogId = blog.BlogId,
                BlogTitle = blog.BlogTitle,
                BlogContent = blog.BlogContent,
                ImagePath = blog.ImagePath,
                CreatedAt = blog.CreatedAt
            };
            return View(vm);
        }

        // Handle edit submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(BlogViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.BlogTitle))
                ModelState.AddModelError("BlogTitle", "Blog title is required");
            
            if (string.IsNullOrWhiteSpace(model.BlogContent))
                ModelState.AddModelError("BlogContent", "Blog content is required");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var blog = _blogService.GetBlogById(model.BlogId);
                if (blog == null) return NotFound();

                blog.BlogTitle = model.BlogTitle.Trim();
                blog.BlogContent = model.BlogContent.Trim();

                // Handle new image upload
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, .png and .gif files are allowed");
                        return View(model);
                    }

                    if (model.ImageFile.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        ModelState.AddModelError("ImageFile", "File size cannot exceed 5MB");
                        return View(model);
                    }

                    var uploadDir = Path.Combine(_env.WebRootPath, "images", "blogs");
                    Directory.CreateDirectory(uploadDir);

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(blog.ImagePath))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, blog.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldPath);
                            }
                            catch (Exception ex)
                            {
                                // Log the error but continue with the update
                                Console.WriteLine($"Error deleting old image: {ex.Message}");
                            }
                        }
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Store the path with forward slashes and starting with /
                    blog.ImagePath = $"/images/blogs/{fileName}";
                }

                _blogService.UpdateBlog(blog);
                TempData["SuccessMessage"] = "Blog updated successfully!";
                return RedirectToAction("BlogManagement");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating the blog: {ex.Message}");
                return View(model);
            }
        }

        // Delete a blog
        public IActionResult DeleteBlog(int id)
        {
            _blogService.DeleteBlog(id);
            return RedirectToAction("BlogManagement");
        }

        public IActionResult ViewAllUsersProgress()
        {
            var allProgressLogs = _progressLogService.GetAllUsersProgressLogs();
            return View(allProgressLogs);
        }

        // Download all users' progress logs as CSV
        public IActionResult DownloadAllProgressCsv()
        {
            var logs = _progressLogService.GetAllUsersProgressLogs();
            
            if (!logs.Any())
            {
                TempData["ErrorMessage"] = "No progress logs found to download.";
                return RedirectToAction(nameof(ViewAllUsersProgress));
            }
            
            var csvContent = _progressLogService.GenerateProgressLogCsv(logs);
            var fileName = $"all_users_progress_logs_{DateTime.Now:yyyyMMdd}.csv";
            
            return File(new System.Text.UTF8Encoding().GetBytes(csvContent), 
                "text/csv", 
                fileName);
        }

        // Download a specific user's progress logs as CSV
        public IActionResult DownloadUserProgressCsv(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(ViewAllUsersProgress));
            }
            
            var user = _clientRepository.GetUserById(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(ViewAllUsersProgress));
            }
            
            var logs = _progressLogService.GetAllProgressLogs(userId);
            
            if (!logs.Any())
            {
                TempData["ErrorMessage"] = "No progress logs found for this user.";
                return RedirectToAction(nameof(ViewAllUsersProgress));
            }
            
            var userEmail = ((ApplicationUser)user).Email;
            var csvContent = _progressLogService.GenerateProgressLogCsv(logs);
            var fileName = $"user_{userEmail.Replace("@", "_at_")}_progress_logs_{DateTime.Now:yyyyMMdd}.csv";
            
            return File(new System.Text.UTF8Encoding().GetBytes(csvContent), 
                "text/csv", 
                fileName);
        }

        public IActionResult ViewUserProgress(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(ViewAllUsersProgress));
            }
            
            var logs = _progressLogService.GetAllProgressLogs(userId);
            
            if (!logs.Any())
            {
                TempData["ErrorMessage"] = "No progress logs found for this user.";
                return RedirectToAction(nameof(ViewAllUsersProgress));
            }
            
            // Get user details for display
            var user = _clientRepository.GetUserById(userId);
            if (user != null)
            {
                ViewBag.UserName = ((ApplicationUser)user).FullName;
                ViewBag.UserEmail = ((ApplicationUser)user).Email;
                ViewBag.UserId = userId;
            }
            
            return View(logs);
        }

        // User Management Section

        public async Task<IActionResult> UserManagement(
            string searchTerm = "",
            string roleFilter = "",
            string statusFilter = "",
            int page = 1,
            int pageSize = 10)
        {
            var model = await _userManagementService.GetUsersAsync(
                searchTerm, roleFilter, statusFilter, page, pageSize);
            
            ViewBag.Roles = await _userManagementService.GetAllRolesAsync();
            
            return View(model);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(UserManagement));
            }
            
            var user = await _userManagementService.GetUserDetailsAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(UserManagement));
            }
            
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(UserManagement));
            }
            
            var result = await _userManagementService.ToggleUserStatusAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "User status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user status. Please try again.";
            }
            
            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(UserManagement));
            }
            
            var result = await _userManagementService.SoftDeleteUserAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "User marked as deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user. Please try again.";
            }
            
            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(UserManagement));
            }
            
            var result = await _userManagementService.PermanentDeleteUserAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "User permanently deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to permanently delete user. Please try again.";
            }
            
            return RedirectToAction(nameof(UserManagement));
        }

        // Admin Dashboard - Coaches Management
        public IActionResult Dashboard()
        {
            // You can add logic here to retrieve coach data or other dashboard information
            return View();
        }

        #region Coach Management
        [HttpGet]
        public async Task<IActionResult> Coaches()
        {
            try
            {
                Console.WriteLine("Fetching coaches from database");
                var coaches = await _coachService.GetAllCoachesAsync();
                Console.WriteLine($"Found {coaches.Count} coaches in database");
                
                // List all coaches for debugging
                foreach (var coach in coaches)
                {
                    Console.WriteLine($"Coach: ID={coach.Id}, Name={coach.Name}, Email={coach.Email}");
                }
                
                return View(coaches);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Coaches action: {ex.Message}");
                TempData["ErrorMessage"] = $"Error loading coaches: {ex.Message}";
                return View(new List<CoachViewModel>());
            }
        }

        [HttpGet]
        public IActionResult AddCoach()
        {
            return View(new CoachViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCoach(CoachViewModel model)
        {
            try
            {
                Console.WriteLine($"AddCoach POST action called with Name: {model.Name}, Email: {model.Email}");
                
                if (ModelState.IsValid)
                {
                    var coachId = await _coachService.AddCoachAsync(model, true);
                    Console.WriteLine($"Coach added with ID: {coachId}");
                    
                    // Direct database check
                    var dbContext = HttpContext.RequestServices.GetRequiredService<FitnessAppDbContext>();
                    var coachFromDb = await dbContext.Coaches.FindAsync(coachId);
                    
                    if (coachFromDb != null)
                    {
                        Console.WriteLine($"Verified coach in database: ID={coachFromDb.Id}, Name={coachFromDb.Name}");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Coach was not found in database after adding!");
                    }
                    
                    TempData["SuccessMessage"] = "Coach added successfully! A user account has been created with the email address and default password 'Coach@123'.";
                    return RedirectToAction(nameof(Coaches));
                }
                else
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            // Log the error or add it to TempData
                            Console.WriteLine($"Validation error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding coach: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                TempData["ErrorMessage"] = $"Error adding coach: {ex.Message}";
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCoach(int id)
        {
            var coach = await _coachService.GetCoachByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            return View(coach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCoach(CoachViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _coachService.UpdateCoachAsync(model);
                return RedirectToAction(nameof(Coaches));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCoach(int id)
        {
            await _coachService.DeleteCoachAsync(id);
            return RedirectToAction(nameof(Coaches));
        }
        #endregion

        #region Workout Plan Management
        [HttpGet]
        public async Task<IActionResult> WorkoutPlans()
        {
            var workoutPlans = await _workoutPlanService.GetAllWorkoutPlansAsync();
            return View(workoutPlans);
        }

        [HttpGet]
        public async Task<IActionResult> AddWorkoutPlan()
        {
            var model = new WorkoutPlanViewModel();
            ViewBag.Coaches = await GetCoachesSelectList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorkoutPlan(WorkoutPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _workoutPlanService.AddWorkoutPlanAsync(model);
                return RedirectToAction(nameof(WorkoutPlans));
            }
            ViewBag.Coaches = await GetCoachesSelectList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditWorkoutPlan(int id)
        {
            var workoutPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
            if (workoutPlan == null)
            {
                return NotFound();
            }
            ViewBag.Coaches = await GetCoachesSelectList();
            return View(workoutPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutPlan(WorkoutPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _workoutPlanService.UpdateWorkoutPlanAsync(model);
                return RedirectToAction(nameof(WorkoutPlans));
            }
            ViewBag.Coaches = await GetCoachesSelectList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkoutPlan(int id)
        {
            await _workoutPlanService.DeleteWorkoutPlanAsync(id);
            return RedirectToAction(nameof(WorkoutPlans));
        }

        // Helper methods
        private async Task<IEnumerable<SelectListItem>> GetCoachesSelectList()
        {
            var coaches = await _coachService.GetAllCoachesAsync();
            return coaches.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> TestCreateWorkoutPlan(int coachId, string title, string description)
        {
            try
            {
                Console.WriteLine($"AdminController: TestCreateWorkoutPlan called with coachId={coachId}, title={title}");
                
                // Create a workout plan with passed values or defaults
                var model = new WorkoutPlanViewModel
                {
                    Title = !string.IsNullOrEmpty(title) ? title : $"Test Plan {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Description = !string.IsNullOrEmpty(description) ? description : "This is a test workout plan created by admin.",
                    CoachId = coachId,
                    CreatedDate = DateTime.Now
                };
                
                // Validate mandatory fields
                if (coachId <= 0)
                {
                    return Json(new { success = false, error = "A valid coach must be selected." });
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    return Json(new { success = false, error = "Title is required." });
                }

                // Save the workout plan
                Console.WriteLine("AdminController: About to call AddWorkoutPlanAsync");
                var result = await _workoutPlanService.AddWorkoutPlanAsync(model);
                Console.WriteLine($"AdminController: Workout plan added with ID: {result}");
                
                // Verify it was saved
                var savedPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(result);
                if (savedPlan != null)
                {
                    Console.WriteLine($"AdminController: Successfully verified plan with ID {result}");
                    return Json(new { 
                        success = true, 
                        message = "Workout plan created successfully!",
                        planId = result,
                        planDetails = model
                    });
                }
                else
                {
                    Console.WriteLine("AdminController: ERROR - Plan not found after saving");
                    return Json(new { success = false, error = "Plan could not be retrieved after saving." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdminController: ERROR in TestCreateWorkoutPlan: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestEditWorkoutPlan(int id, int coachId, string title, string description)
        {
            try
            {
                Console.WriteLine($"AdminController: TestEditWorkoutPlan called with id={id}, coachId={coachId}, title={title}");
                
                // Get the existing workout plan
                var existingPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
                if (existingPlan == null)
                {
                    return Json(new { success = false, error = "Workout plan not found." });
                }
                
                // Update the model
                existingPlan.Title = !string.IsNullOrEmpty(title) ? title : existingPlan.Title;
                existingPlan.Description = description; // Can be null/empty
                existingPlan.CoachId = coachId > 0 ? coachId : existingPlan.CoachId;
                
                // Validate mandatory fields
                if (existingPlan.CoachId <= 0)
                {
                    return Json(new { success = false, error = "A valid coach must be selected." });
                }

                if (string.IsNullOrEmpty(existingPlan.Title))
                {
                    return Json(new { success = false, error = "Title is required." });
                }

                // Save the updated workout plan
                Console.WriteLine("AdminController: About to call UpdateWorkoutPlanAsync");
                var result = await _workoutPlanService.UpdateWorkoutPlanAsync(existingPlan);
                Console.WriteLine($"AdminController: Workout plan update result: {result}");
                
                if (result)
                {
                    return Json(new { 
                        success = true, 
                        message = "Workout plan updated successfully!"
                    });
                }
                else
                {
                    return Json(new { success = false, error = "Failed to update workout plan." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdminController: ERROR in TestEditWorkoutPlan: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
