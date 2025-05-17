using System.Security.Claims;
using Fitness_App.BL.Servecies;
using Fitness_App.BL.Services;
using Fitness_App.BL.ViewModels;
using Fitness_App.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Fitness_App.DAL.Models;
using Fitness_App.DAL.DbContext;

namespace FitnessAPPMVC.Controllers
{
    [Authorize(Roles = "User")]
    public class ClientController : Controller
    {
        private readonly DietPlanService _dietPlanService;
        private readonly ExerciseServices _exerciseServices;
        private readonly SubscriptionServices _subscriptionServices;
        private readonly BlogServices _blogService;
        private readonly ProgressLogServices _progressLogService;
        private readonly UserSubscriptionServices userSubscriptionServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CoachService _coachService;
        private readonly WorkoutPlanService _workoutPlanService;
        private readonly ClientCoachSubscriptionService _subscriptionService;
        private readonly FitnessAppDbContext _context;

        public ClientController(
            DietPlanService dietPlanService,
            ExerciseServices exerciseServices,
            SubscriptionServices subscriptionServices,
            BlogServices blogService,
            ProgressLogServices progressLogService,
            UserSubscriptionServices userSubscriptionServices,
            UserManager<ApplicationUser> userManager,
            CoachService coachService,
            WorkoutPlanService workoutPlanService,
            ClientCoachSubscriptionService subscriptionService,
            FitnessAppDbContext context)
        {
            _dietPlanService = dietPlanService;
            _exerciseServices = exerciseServices;
            this._subscriptionServices = subscriptionServices;
            _blogService = blogService;
            _progressLogService = progressLogService;
            this.userSubscriptionServices = userSubscriptionServices;
            _userManager = userManager;
            _coachService = coachService;
            _workoutPlanService = workoutPlanService;
            _subscriptionService = subscriptionService;
            _context = context;
        }
        public IActionResult GetSubscriptionPlansForClient()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User not authenticated.");
            }

            // Check if user has an active subscription
            var hasActiveSubscription = userSubscriptionServices.IsUserSubscribed(userIdString);
            if (hasActiveSubscription)
            {
                // Check if user already has a coach subscription
                var hasCoachSubscription = _subscriptionService.GetClientSubscriptions(userIdString)
                    .Result
                    .Any(s => s.IsActive);

                if (hasCoachSubscription)
                {
                    return RedirectToAction("MyWorkoutPlan");
                }
                
                return RedirectToAction("SelectCoach");
            }

            var plans = _subscriptionServices.GetAllSubscriptionPlans();
            var viewModel = plans.Select(p => new SubscriptionPlanViewModel
            {
                PlanId = p.PlanId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                DurationInDays = p.DurationInDays
            }).ToList();

            return View("GetSubscriptionPlansForClient", viewModel);
        }
        [HttpGet]
        public ActionResult Checkout(int PlanId)
        {
            var plan = _subscriptionServices.GetSubscriptionPlan(PlanId);

            if (plan == null)
            {
                return NotFound("Subscription plan not found.");
            }

            var service = new StripeService();

            string successUrl = Url.Action("Success", "Client", new { planId = PlanId }, Request.Scheme);
            string cancelUrl = Url.Action("Cancel", "Client", null, Request.Scheme);

            var sessionUrl = service.CreateCheckoutSession(plan, successUrl, cancelUrl);
            return Redirect(sessionUrl);
        }
        public ActionResult Success(int planId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User not authenticated.");
            }

            var plan = _subscriptionServices.GetSubscriptionPlan(planId);
            if (plan == null)
            {
                return NotFound("Subscription plan not found.");
            }

            var viewmodel = new UserSubscriptionViewModel
            {
                UserId = userIdString,
                SubscriptionPlanId = plan.PlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationInDays),
                IsActive = true
            };

            userSubscriptionServices.AddUserSubscription(viewmodel);

            //return View("Success", plan);
            //return View(nameof(GetSubscriptionPlansForClient));
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Cancel()
        {

            ViewBag.Message = "Your payment process was canceled. You can try again anytime.";
            return View("Cancel");
        }
        public IActionResult AllDietPlans()
        {
            var plans = _dietPlanService.GetAllDietPlans();

            var viewModels = plans.Select(plan => new DietPlanDisplayViewModel
            {
                Id = plan.PlanId,
                PlanTitle = plan.PlanTitle,
                ImagePath = plan.ImagePath
            }).ToList();

            return View(viewModels);
        }

        public IActionResult ViewDietPlan(int id)
        {
            var plan = _dietPlanService.GetDietPlanById(id);
            if (plan == null)
                return NotFound();

            return View(plan);
        }

        public IActionResult GetExercisesForClient()
        {
            var data = _exerciseServices.GetAllExercises();
            return View("GetExercisesForClient", data);
        }

        //public IActionResult GetSubscriptionPlansForClient()
        //{
        //    var plans = _subscriptionServices.GetAllSubscriptionPlans();
        //    var viewModel = plans.Select(p => new SubscriptionPlanViewModel
        //    {
        //        PlanId = p.PlanId,
        //        Name = p.Name,
        //        Price = p.Price,
        //        Description = p.Description
        //    }).ToList();

        //    return View(viewModel);
        //}

        ///////////////////////// Blogs ///////////////////////

        // List all blogs (newest first). 
        // Later you can add int? categoryId to filter by category.
        public IActionResult AllBlogs()
        {
            var blogViewModels = _blogService.GetAllBlogsWithComments()
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(blogViewModels);
        }

        // Show a single blog's details (including image & full content)
        public IActionResult ViewBlog(int id)
        {
            var blogViewModel = _blogService.GetBlogWithComments(id);
            if (blogViewModel == null)
                return NotFound();

            return View(blogViewModel);
        }

        // POST: Add a comment to a blog
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(CommentViewModel commentViewModel)
        {
            Console.WriteLine($"AddComment: Received comment for blog ID {commentViewModel.BlogId}");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("AddComment: Model state is invalid");
                foreach (var modelStateEntry in ModelState)
                {
                    foreach (var error in modelStateEntry.Value.Errors)
                    {
                        Console.WriteLine($"Error for {modelStateEntry.Key}: {error.ErrorMessage}");
                    }
                }
                
                TempData["CommentError"] = "Please enter a comment.";
                return RedirectToAction("ViewBlog", new { id = commentViewModel.BlogId });
            }
            
            // Set the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("AddComment: User ID is empty");
                TempData["CommentError"] = "You must be logged in to comment.";
                return RedirectToAction("ViewBlog", new { id = commentViewModel.BlogId });
            }
            
            commentViewModel.UserId = userId;
            commentViewModel.CreatedAt = DateTime.Now;
            
            try 
            {
                _blogService.AddComment(commentViewModel);
                Console.WriteLine("AddComment: Comment added successfully");
                TempData["CommentSuccess"] = "Your comment has been added successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddComment: Error adding comment - {ex.Message}");
                TempData["CommentError"] = $"Error adding comment: {ex.Message}";
            }
            
            return RedirectToAction("ViewBlog", new { id = commentViewModel.BlogId });
        }

        ///////////////////////////////////////////////////////////////////////////////

        // GET: ProgressLog
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Index: Getting logs for user {userId}");
            
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Index: No user ID found!");
                TempData["ErrorMessage"] = "Unable to retrieve your progress logs. Please log out and log in again.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var logs = _progressLogService.GetAllProgressLogs(userId);
                Console.WriteLine($"Index: Retrieved {logs?.Count ?? 0} logs");

                if (logs == null)
                {
                    Console.WriteLine("Index: Logs is null!");
                    logs = new List<ProgressLogViewModel>();
                }

                return View(logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving logs: {ex}");
                TempData["ErrorMessage"] = $"Error loading progress logs: {ex.Message}";
                return View(new List<ProgressLogViewModel>());
            }
        }

        // GET: ProgressLog/Dashboard
        public IActionResult Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var summary = _progressLogService.GetProgressSummary(userId);
            return View(summary);
        }

        // GET: ProgressLog/Details/5
        public IActionResult Details(int id)
        {
            var progressLog = _progressLogService.GetProgressLogById(id);
            if (progressLog == null)
            {
                return NotFound();
            }

            // Security check to ensure the user can only see their own logs
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (progressLog.UserId != userId)
            {
                return Forbid();
            }

            return View(progressLog);
        }

        // GET: ProgressLog/Create
        public IActionResult Create()
        {
            var viewModel = new ProgressLogViewModel
            {
                Date = DateTime.Today
            };
            return View(viewModel);
        }

        // POST: ProgressLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProgressLogViewModel viewModel)
        {
            // Debug authentication status
            if (!User.Identity.IsAuthenticated)
            {
                // User is not authenticated
                TempData["ErrorMessage"] = "You must be logged in to create a progress log.";
                ModelState.AddModelError("", "You must be logged in to create a progress log.");
                return View(viewModel);
            }

            // Try to get the user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user ID was found
            if (string.IsNullOrEmpty(userId))
            {
                // UserId claim not found
                TempData["ErrorMessage"] = "User ID could not be determined. Please log out and log in again.";
                ModelState.AddModelError("", "User ID could not be determined. Please log out and log in again.");
                return View(viewModel);
            }

            // Set the user ID
            viewModel.UserId = userId;
            
            // Set the user email from claims
            viewModel.UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            
            // Initialize chart data
            viewModel.ChartLabels = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { viewModel.Date.ToString("MM/dd/yyyy") });
            viewModel.ChartData = Newtonsoft.Json.JsonConvert.SerializeObject(new[] { viewModel.Weight });

            // Remove validation errors for these properties
            ModelState.Remove("UserId");
            ModelState.Remove("ChartLabels");
            ModelState.Remove("ChartData");
            ModelState.Remove("UserEmail");

            if (ModelState.IsValid)
            {
                try
                {
                    _progressLogService.AddProgressLog(viewModel);
                    TempData["SuccessMessage"] = "Progress log saved successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the actual error
                    Console.WriteLine($"Error saving progress log: {ex}");
                    TempData["ErrorMessage"] = $"Unable to save progress log. Error: {ex.Message}";
                    ModelState.AddModelError("", $"Unable to save progress log. Error: {ex.Message}");
                }
            }
            else
            {
                // Log model state errors
                string errors = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model validation error: {error.ErrorMessage}");
                        errors += error.ErrorMessage + " ";
                    }
                }
                TempData["ErrorMessage"] = $"Form validation failed: {errors}";
            }

            return View(viewModel);
        }

        // GET: ProgressLog/Edit/5
        public IActionResult Edit(int id)
        {
            var progressLog = _progressLogService.GetProgressLogById(id);
            if (progressLog == null)
            {
                return NotFound();
            }

            // Security check to ensure the user can only edit their own logs
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (progressLog.UserId != userId)
            {
                return Forbid();
            }

            return View(progressLog);
        }

        // POST: ProgressLog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProgressLogViewModel viewModel)
        {
            if (id != viewModel.ProgressLogId)
            {
                return NotFound();
            }

            // Make sure we're not clearing these fields
            ModelState.Remove("UserId");
            ModelState.Remove("ChartLabels");
            ModelState.Remove("ChartData");
            ModelState.Remove("UserEmail");

            if (ModelState.IsValid)
            {
                try
                {
                    // Set the current user's ID
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    viewModel.UserId = userId;

                    // Verify ownership of the log
                    var existingLog = _progressLogService.GetProgressLogById(id);
                    if (existingLog == null || existingLog.UserId != userId)
                    {
                        return Forbid();
                    }

                    // Update the progress log
                    _progressLogService.UpdateProgressLog(viewModel);
                    
                    // Redirect to the index page to see the updated log
                    TempData["SuccessMessage"] = "Progress log updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to update progress log: {ex.Message}");
                }
            }
            return View(viewModel);
        }

        // GET: ProgressLog/Delete/5
        public IActionResult Delete(int id)
        {
            var progressLog = _progressLogService.GetProgressLogById(id);
            if (progressLog == null)
            {
                return NotFound();
            }

            // Security check to ensure the user can only delete their own logs
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (progressLog.UserId != userId)
            {
                return Forbid();
            }

            return View(progressLog);
        }

        // POST: ProgressLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Verify ownership of the log
            var progressLog = _progressLogService.GetProgressLogById(id);
            if (progressLog == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (progressLog.UserId != userId)
            {
                return Forbid();
            }

            _progressLogService.DeleteProgressLog(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: ProgressLog/DownloadCsv
        public IActionResult DownloadProgressCsv()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var logs = _progressLogService.GetAllProgressLogs(userId);
            
            if (!logs.Any())
            {
                TempData["ErrorMessage"] = "No progress logs found to download.";
                return RedirectToAction(nameof(Index));
            }
            
            var csvContent = _progressLogService.GenerateProgressLogCsv(logs);
            var fileName = $"my_progress_logs_{DateTime.Now:yyyyMMdd}.csv";
            
            return File(new System.Text.UTF8Encoding().GetBytes(csvContent), 
                "text/csv", 
                fileName);
        }

        #region Coach Content
        [HttpGet]
        public async Task<IActionResult> Coaches()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user has an active subscription
            var hasSubscription = userSubscriptionServices.IsUserSubscribed(userId);
            if (!hasSubscription)
            {
                TempData["ErrorMessage"] = "You need to subscribe to a plan first to access coaches.";
                return RedirectToAction("GetSubscriptionPlansForClient");
            }

            var coaches = await _coachService.GetAllCoachesAsync();
            return View(coaches);
        }

        [HttpGet]
        public async Task<IActionResult> CoachDetails(int id)
        {
            var coach = await _coachService.GetCoachByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }

            // Get all workout plans created by this coach
            var workoutPlans = await _workoutPlanService.GetWorkoutPlansByCoachIdAsync(id);
            ViewBag.WorkoutPlans = workoutPlans;

            return View(coach);
        }

        [HttpGet]
        public async Task<IActionResult> MyWorkoutPlans()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user has an active subscription
            var hasSubscription = userSubscriptionServices.IsUserSubscribed(userId);
            if (!hasSubscription)
            {
                TempData["ErrorMessage"] = "You need to subscribe to a plan first to access workout plans.";
                return RedirectToAction("GetSubscriptionPlansForClient");
            }

            // Check if user has an active coach subscription
            var clientSubscriptions = await _subscriptionService.GetClientSubscriptions(userId);
            var activeSubscription = clientSubscriptions.FirstOrDefault(s => s.IsActive);
            
            if (activeSubscription == null)
            {
                TempData["InfoMessage"] = "You need to select a coach first.";
                return RedirectToAction("SelectCoach");
            }

            try
            {
                var workoutPlans = await _workoutPlanService.GetAllWorkoutPlansAsync();
                workoutPlans = workoutPlans.OrderByDescending(wp => wp.CreatedDate).ToList();
                return View(workoutPlans);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading workout plans: {ex.Message}";
                return View(new List<WorkoutPlanViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> WorkoutPlanDetails(int id)
        {
            var workoutPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
            if (workoutPlan == null)
            {
                return NotFound();
            }

            return View(workoutPlan);
        }
        #endregion

        // Show available coaches after subscription
        public async Task<IActionResult> SelectCoach()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user has an active subscription
            var hasSubscription = userSubscriptionServices.IsUserSubscribed(userId);
            if (!hasSubscription)
            {
                TempData["ErrorMessage"] = "You need to subscribe to a plan first to select a coach.";
                return RedirectToAction("GetSubscriptionPlansForClient");
            }

            // Check if user already has an active coach subscription
            var clientSubscriptions = await _subscriptionService.GetClientSubscriptions(userId);
            var activeSubscription = clientSubscriptions.FirstOrDefault(s => s.IsActive);
            
            if (activeSubscription != null)
            {
                return RedirectToAction("MyWorkoutPlan");
            }

            var coaches = await _subscriptionService.GetAvailableCoaches();
            return View(coaches);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscribeToCoach(int coachId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the user's active subscription to determine the end date
            var userSubscription = _context.userSubscriptions
                .FirstOrDefault(s => s.UserId == userId && s.IsActive);

            if (userSubscription == null)
            {
                TempData["ErrorMessage"] = "You need an active subscription to subscribe to a coach.";
                return RedirectToAction("GetSubscriptionPlansForClient");
            }

            // Calculate the end date based on the subscription start date and duration
            var subscriptionPlan = _subscriptionServices.GetSubscriptionPlan(userSubscription.SubscriptionPlanId);
            var endDate = userSubscription.StartDate.AddDays(subscriptionPlan.DurationInDays);

            var result = await _subscriptionService.SubscribeToCoach(userId, coachId, endDate);
            if (result)
            {
                TempData["SuccessMessage"] = "Successfully subscribed to the coach!";
                return RedirectToAction("MyWorkoutPlan");
            }

            TempData["ErrorMessage"] = "Failed to subscribe to the coach. You might already have an active subscription.";
            return RedirectToAction("SelectCoach");
        }

        // View assigned workout plan
        public async Task<IActionResult> MyWorkoutPlan()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user has an active subscription
            var hasSubscription = userSubscriptionServices.IsUserSubscribed(userId);
            if (!hasSubscription)
            {
                TempData["ErrorMessage"] = "You need to subscribe to a plan first to access your workout plan.";
                return RedirectToAction("GetSubscriptionPlansForClient");
            }

            var subscriptions = await _subscriptionService.GetClientSubscriptions(userId);
            var activeSubscription = subscriptions.FirstOrDefault(s => s.IsActive);
            
            if (activeSubscription == null)
            {
                TempData["InfoMessage"] = "You need to select a coach first.";
                return RedirectToAction("SelectCoach");
            }

            if (!activeSubscription.WorkoutPlanId.HasValue)
            {
                TempData["InfoMessage"] = "Your coach hasn't assigned a workout plan yet.";
                return View(null);
            }

            var workoutPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(activeSubscription.WorkoutPlanId.Value);
            return View(workoutPlan);
        }
    }
}