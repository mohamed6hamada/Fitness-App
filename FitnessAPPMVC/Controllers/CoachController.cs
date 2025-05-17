using Fitness_App.BL.Services;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace FitnessAPPMVC.Controllers
{
    [Authorize(Roles = "Coach")]
    public class CoachController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WorkoutPlanService _workoutPlanService;
        private readonly CoachService _coachService;
        private readonly ProgressLogServices _progressLogService;
        private readonly ClientCoachSubscriptionService _subscriptionService;

        // Static log storage for debugging
        private static readonly List<string> _debugLogs = new List<string>();
        private static readonly object _logsLock = new object();
        
        // Helper method to add a log entry
        private static void AddLog(string message)
        {
            lock (_logsLock)
            {
                // Add timestamp
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                
                // Add to in-memory log
                _debugLogs.Add(logEntry);
                
                // Keep only last 500 log entries
                if (_debugLogs.Count > 500)
                {
                    _debugLogs.RemoveAt(0);
                }
                
                // Also log to console
                Console.WriteLine(logEntry);
            }
        }

        public CoachController(
            UserManager<ApplicationUser> userManager,
            WorkoutPlanService workoutPlanService,
            CoachService coachService,
            ProgressLogServices progressLogService,
            ClientCoachSubscriptionService subscriptionService)
        {
            _userManager = userManager;
            _workoutPlanService = workoutPlanService;
            _coachService = coachService;
            _progressLogService = progressLogService;
            _subscriptionService = subscriptionService;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get current coach's email
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Find coach profile by email
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null)
                {
                    return View("Error", new ErrorViewModel { Message = "No coach profile found for your account." });
                }

                // Get workout plans created by this coach
                var allWorkoutPlans = await _workoutPlanService.GetAllWorkoutPlansAsync();
                var coachWorkoutPlans = allWorkoutPlans.Where(wp => wp.CoachId == coach.Id).ToList();

                // Get count of all active users (clients)
                var clientsCount = await _userManager.GetUsersInRoleAsync("User");
                var activeClientCount = clientsCount.Count(u => u.IsActive && !u.IsDeleted);

                ViewBag.CoachName = coach.Name;
                ViewBag.CoachId = coach.Id;
                ViewBag.WorkoutPlansCount = coachWorkoutPlans.Count;
                ViewBag.AssignedUsersCount = activeClientCount;

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { Message = $"Error loading dashboard: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyWorkoutPlans()
        {
            try
            {
                AddLog("MyWorkoutPlans action called");
                
                // Get current coach's email
                var user = await _userManager.GetUserAsync(User);
                AddLog($"Current user email: {user?.Email}");
                
                // Find coach profile by email
                var coaches = await _coachService.GetAllCoachesAsync();
                AddLog($"Found {coaches.Count} total coaches in system");
                
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);
                AddLog($"Found coach profile: {(coach != null ? "Yes, ID=" + coach.Id : "No")}");

                if (coach == null)
                {
                    AddLog("No coach profile found for user");
                    return View("Error", new ErrorViewModel { Message = "No coach profile found for your account." });
                }

                // Get workout plans created by this coach
                AddLog($"Getting all workout plans to filter by coach ID {coach.Id}");
                var allWorkoutPlans = await _workoutPlanService.GetAllWorkoutPlansAsync();
                AddLog($"Found {allWorkoutPlans.Count} total workout plans");
                
                // Debug each plan
                foreach (var plan in allWorkoutPlans)
                {
                    AddLog($"Plan: ID={plan.Id}, Title='{plan.Title}', CoachId={plan.CoachId}");
                }
                
                var coachWorkoutPlans = allWorkoutPlans.Where(wp => wp.CoachId == coach.Id).ToList();
                AddLog($"After filtering, found {coachWorkoutPlans.Count} plans for this coach");

                // Try to check workoutplans table directly
                try
                {
                    var dbContext = HttpContext.RequestServices.GetService<Fitness_App.DAL.DbContext.FitnessAppDbContext>();
                    if (dbContext != null)
                    {
                        var dbPlans = await dbContext.WorkoutPlans.ToListAsync();
                        AddLog($"Direct DB query shows {dbPlans.Count} total workout plans");
                        foreach (var plan in dbPlans)
                        {
                            AddLog($"DB Plan: ID={plan.Id}, Title='{plan.Title}', CoachId={plan.CoachId}");
                        }
                        
                        var dbCoachPlans = dbPlans.Where(p => p.CoachId == coach.Id).ToList();
                        AddLog($"Direct DB query shows {dbCoachPlans.Count} plans for coach ID {coach.Id}");
                    }
                    else
                    {
                        AddLog("Could not access database context directly");
                    }
                }
                catch (Exception dbEx)
                {
                    AddLog($"Error accessing database directly: {dbEx.Message}");
                }

                ViewBag.CoachId = coach.Id;
                return View(coachWorkoutPlans);
            }
            catch (Exception ex)
            {
                AddLog($"ERROR in MyWorkoutPlans: {ex.Message}");
                AddLog($"Stack trace: {ex.StackTrace}");
                return View("Error", new ErrorViewModel { Message = $"Error loading workout plans: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddWorkoutPlan()
        {
            try
            {
                AddLog("AddWorkoutPlan GET action called");
                
                // Get current coach's email
                var user = await _userManager.GetUserAsync(User);
                AddLog($"Current user email: {user?.Email}");
                
                // Find coach profile by email
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                AddLog($"Found coach: {(coach != null ? "Yes, ID=" + coach.Id : "No")}");

                if (coach == null)
                {
                    AddLog("No coach profile found for current user");
                    return View("Error", new ErrorViewModel { Message = "No coach profile found for your account." });
                }

                var model = new WorkoutPlanViewModel
                {
                    CoachId = coach.Id
                };

                AddLog($"Created new WorkoutPlanViewModel with CoachId={model.CoachId}");

                // Removed problematic view engine check
                AddLog("Ready to return view with model");

                return View(model);
            }
            catch (Exception ex)
            {
                AddLog($"ERROR in AddWorkoutPlan GET: {ex.Message}");
                AddLog($"Stack trace: {ex.StackTrace}");
                return View("Error", new ErrorViewModel { Message = $"Error preparing to add workout plan: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorkoutPlan(WorkoutPlanViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Get current coach's email to verify ownership
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null)
                {
                    ModelState.AddModelError("", "No coach profile found for your account.");
                    return View(model);
                }

                // Set the coach ID
                model.CoachId = coach.Id;
                model.CreatedDate = DateTime.Now;

                // Add the workout plan
                var result = await _workoutPlanService.AddWorkoutPlanAsync(model);
                
                // Add success message
                TempData["SuccessMessage"] = "Workout plan created successfully!";
                return RedirectToAction(nameof(MyWorkoutPlans));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating workout plan: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditWorkoutPlan(int id)
        {
            try
            {
                var workoutPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
                if (workoutPlan == null)
                {
                    return NotFound();
                }

                // Get current coach's email to verify ownership
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null || workoutPlan.CoachId != coach.Id)
                {
                    return View("Error", new ErrorViewModel { Message = "You can only edit your own workout plans." });
                }

                var users = await _userManager.Users
                    .Where(u => u.IsActive && !u.IsDeleted)
                    .ToListAsync();

                Console.WriteLine($"Loading workout plan for edit - ID: {workoutPlan.Id}, Title: {workoutPlan.Title}");
                Console.WriteLine($"AssignedUserIds: {(workoutPlan.AssignedUserIds != null ? string.Join(", ", workoutPlan.AssignedUserIds) : "null")}");
                
                ViewBag.Users = users.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = string.IsNullOrEmpty(u.FullName) ? u.Email : $"{u.FullName} ({u.Email})",
                    Selected = workoutPlan.AssignedUserIds != null && workoutPlan.AssignedUserIds.Contains(u.Id)
                });

                return View(workoutPlan);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { Message = $"Error loading workout plan: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutPlan(WorkoutPlanViewModel model)
        {
            try
            {
                // Debug information
                Console.WriteLine($"EditWorkoutPlan called with ID: {model.Id}, Title: {model.Title}");
                Console.WriteLine($"CoachId: {model.CoachId}");
                Console.WriteLine($"AssignedUserIds: {(model.AssignedUserIds != null ? string.Join(", ", model.AssignedUserIds) : "null")}");
                
                if (ModelState.IsValid)
                {
                    // Get current coach's email to verify ownership
                    var user = await _userManager.GetUserAsync(User);
                    var coaches = await _coachService.GetAllCoachesAsync();
                    var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                    if (coach == null || model.CoachId != coach.Id)
                    {
                        ModelState.AddModelError("", "You can only edit workout plans for your own coach profile.");
                        return View(model);
                    }

                    var result = await _workoutPlanService.UpdateWorkoutPlanAsync(model);
                    Console.WriteLine($"Workout plan update result: {result}");
                    
                    // Add success message
                    TempData["SuccessMessage"] = "Workout plan updated successfully!";
                    return RedirectToAction(nameof(MyWorkoutPlans));
                }
                else
                {
                    // Log model state errors
                    foreach (var state in ModelState)
                    {
                        Console.WriteLine($"Key: {state.Key}, Errors: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                // If we got this far, something failed, redisplay form
                var users = await _userManager.Users
                    .Where(u => u.IsActive && !u.IsDeleted)
                    .ToListAsync();

                ViewBag.Users = users.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = u.Id,
                    Text = string.IsNullOrEmpty(u.FullName) ? u.Email : $"{u.FullName} ({u.Email})"
                });

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating workout plan: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkoutPlan(int id)
        {
            try
            {
                var workoutPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(id);
                if (workoutPlan == null)
                {
                    return NotFound();
                }

                // Get current coach's email to verify ownership
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null || workoutPlan.CoachId != coach.Id)
                {
                    return View("Error", new ErrorViewModel { Message = "You can only delete your own workout plans." });
                }

                await _workoutPlanService.DeleteWorkoutPlanAsync(id);
                return RedirectToAction(nameof(MyWorkoutPlans));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting workout plan: {ex.Message}";
                return RedirectToAction(nameof(MyWorkoutPlans));
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyClients()
        {
            var coachId = await GetCurrentCoachId();
            if (!coachId.HasValue)
                return RedirectToAction("Login", "Account");

            var clients = await _subscriptionService.GetCoachClients(coachId.Value);
            return View(clients);
        }

        [HttpGet]
        public async Task<IActionResult> ClientProgress(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Client ID is required");
                }

                // Get client details
                var client = await _userManager.FindByIdAsync(id);
                if (client == null || !client.IsActive || client.IsDeleted)
                {
                    return NotFound("Client not found or inactive");
                }

                // Check if this client is assigned to this coach
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null)
                {
                    return View("Error", new ErrorViewModel { Message = "No coach profile found for your account." });
                }

                // Get workout plans created by this coach
                var allWorkoutPlans = await _workoutPlanService.GetAllWorkoutPlansAsync();
                var coachWorkoutPlans = allWorkoutPlans.Where(wp => wp.CoachId == coach.Id).ToList();

                // Check if the client is assigned to any of this coach's workout plans
                var isClientOfCoach = coachWorkoutPlans.Any(wp => 
                    wp.AssignedUserIds != null && wp.AssignedUserIds.Contains(id));
                    
                if (!isClientOfCoach)
                {
                    return View("Error", new ErrorViewModel { Message = "This client is not assigned to any of your workout plans." });
                }

                // Get progress logs for the client
                var progressLogs = _progressLogService.GetAllProgressLogs(id);

                ViewBag.ClientName = client.FullName;
                ViewBag.ClientEmail = client.Email;

                return View(progressLogs);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { Message = $"Error loading client progress: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DiagnoseWorkoutPlans()
        {
            try
            {
                Console.WriteLine("DEBUG: DiagnoseWorkoutPlans called");
                
                // Get all workout plans from database
                var allWorkoutPlans = await _workoutPlanService.GetAllWorkoutPlansAsync();
                Console.WriteLine($"DEBUG: Found {allWorkoutPlans.Count} total workout plans in database");
                
                // Check if current user is a coach
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);
                
                if (coach != null)
                {
                    Console.WriteLine($"DEBUG: Current user is coach with ID {coach.Id}");
                    var coachWorkoutPlans = allWorkoutPlans.Where(wp => wp.CoachId == coach.Id).ToList();
                    Console.WriteLine($"DEBUG: Found {coachWorkoutPlans.Count} plans for this coach");
                }
                
                // Create a simple response with the diagnostic information
                var diagInfo = new
                {
                    TotalWorkoutPlans = allWorkoutPlans.Count,
                    Plans = allWorkoutPlans.Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.CoachId,
                        p.CoachName,
                        p.CreatedDate
                    }).ToList()
                };
                
                // Return as JSON for easy diagnostics
                return Json(diagInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in DiagnoseWorkoutPlans: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestCreateWorkoutPlan()
        {
            try
            {
                AddLog("TestCreateWorkoutPlan called");
                
                // Get current coach's email to verify ownership
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null)
                {
                    AddLog("No coach profile found for user");
                    return Json(new { Error = "No coach profile found for your account." });
                }
                
                AddLog($"Creating test workout plan for coach ID {coach.Id}");
                
                // Create a test workout plan with hardcoded values
                var model = new Fitness_App.BL.ViewModels.WorkoutPlanViewModel
                {
                    Title = $"Test Plan {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                    Description = "This is a test workout plan created directly for debugging purposes.",
                    CoachId = coach.Id,
                    CreatedDate = DateTime.Now
                };
                
                AddLog("About to call AddWorkoutPlanAsync with test plan");
                var result = await _workoutPlanService.AddWorkoutPlanAsync(model);
                AddLog($"Test workout plan added with ID: {result}");
                
                // Now verify we can retrieve it
                AddLog("Verifying plan was saved by retrieving it from database");
                var retrievedPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(result);
                if (retrievedPlan != null)
                {
                    AddLog($"Successfully retrieved plan with ID {result} and title '{retrievedPlan.Title}'");
                }
                else
                {
                    AddLog($"ERROR: Could not retrieve plan with ID {result} from database!");
                }
                
                return Json(new { 
                    Success = true, 
                    Message = "Test plan created successfully", 
                    PlanId = result,
                    PlanDetails = model
                });
            }
            catch (Exception ex)
            {
                AddLog($"ERROR in TestCreateWorkoutPlan: {ex.Message}");
                AddLog($"Stack trace: {ex.StackTrace}");
                return Json(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [HttpGet]
        public IActionResult ViewLogs()
        {
            // Return the logs as a simple HTML page
            var html = "<html><head><title>Debug Logs</title>" +
                       "<meta http-equiv='refresh' content='5'>" + // Auto-refresh every 5 seconds
                       "<style>body{font-family:Consolas,monospace;font-size:14px;}</style></head>" +
                       "<body><h1>Debug Logs</h1>" +
                       "<p><a href='/Coach/ClearLogs'>Clear Logs</a> | <a href='/Coach/Dashboard'>Back to Dashboard</a></p>" +
                       "<pre>";
            
            lock (_logsLock)
            {
                foreach (var log in _debugLogs)
                {
                    html += log + "\n";
                }
            }
            
            html += "</pre></body></html>";
            
            return Content(html, "text/html");
        }
        
        [HttpGet]
        public IActionResult ClearLogs()
        {
            lock (_logsLock)
            {
                _debugLogs.Clear();
                AddLog("Logs cleared");
            }
            
            return RedirectToAction("ViewLogs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlanApi([FromBody] WorkoutPlanViewModel model)
        {
            try
            {
                AddLog("CreateWorkoutPlanApi called");
                AddLog($"Received JSON data - Title: '{model.Title}', Description: '{model.Description}', CoachId: {model.CoachId}");
                
                // Get current coach's email to verify ownership
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null)
                {
                    AddLog("Error: No coach profile found for current user");
                    return Json(new { success = false, error = "No coach profile found for your account." });
                }
                
                // Override the CoachId with the authenticated coach's ID
                model.CoachId = coach.Id;
                AddLog($"Set CoachId to {model.CoachId} based on authenticated coach");
                
                // Attempt to save the workout plan
                try
                {
                    var result = await _workoutPlanService.AddWorkoutPlanAsync(model);
                    AddLog($"Workout plan successfully added with ID: {result}");
                    
                    // Verify it was saved
                    var savedPlan = await _workoutPlanService.GetWorkoutPlanByIdAsync(result);
                    if (savedPlan != null)
                    {
                        AddLog($"Successfully verified plan with ID {result} was saved");
                        return Json(new { 
                            success = true, 
                            message = "Workout plan created successfully!",
                            planId = result
                        });
                    }
                    else
                    {
                        AddLog("ERROR: Plan was not found after saving");
                        return Json(new { success = false, error = "Plan was saved but could not be retrieved afterward." });
                    }
                }
                catch (Exception ex)
                {
                    AddLog($"ERROR saving workout plan: {ex.Message}");
                    return Json(new { success = false, error = $"Error saving workout plan: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                AddLog($"ERROR in CreateWorkoutPlanApi: {ex.Message}");
                return Json(new { success = false, error = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignWorkoutPlan(int subscriptionId)
        {
            var coachId = await GetCurrentCoachId();
            if (!coachId.HasValue)
                return RedirectToAction("Login", "Account");

            var workoutPlans = await _workoutPlanService.GetWorkoutPlansByCoachAsync(coachId.Value);
            ViewBag.SubscriptionId = subscriptionId;
            return View(workoutPlans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignWorkoutPlan(int subscriptionId, int workoutPlanId)
        {
            var coachId = await GetCurrentCoachId();
            if (!coachId.HasValue)
                return RedirectToAction("Login", "Account");

            var result = await _subscriptionService.AssignWorkoutPlan(subscriptionId, workoutPlanId);
            if (result)
            {
                TempData["SuccessMessage"] = "Workout plan assigned successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to assign workout plan. Please try again.";
            }

            return RedirectToAction("MyClients");
        }

        private async Task<int?> GetCurrentCoachId()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return null;

            var coaches = await _coachService.GetAllCoachesAsync();
            var coach = coaches.FirstOrDefault(c => c.Email == user.Email);
            return coach?.Id;
        }

        // GET: Coach/ClientProgressLogs/{clientId}
        public async Task<IActionResult> ClientProgressLogs(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest("Client ID is required");
            }

            // Get the current coach's ID
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var coaches = await _coachService.GetAllCoachesAsync();
            var coach = coaches.FirstOrDefault(c => c.Email == user.Email);
            
            if (coach == null)
            {
                TempData["ErrorMessage"] = "No coach profile found for your account.";
                return RedirectToAction("Dashboard");
            }

            // Verify that this client is actually assigned to this coach
            var clientSubscription = await _subscriptionService.GetClientSubscriptions(clientId);
            var activeSubscription = clientSubscription.FirstOrDefault(s => s.IsActive);
            
            if (activeSubscription == null || activeSubscription.CoachId != coach.Id)
            {
                TempData["ErrorMessage"] = "You are not authorized to view this client's progress logs.";
                return RedirectToAction("MyClients");
            }

            // Get the client's progress logs
            var progressLogs = _progressLogService.GetAllProgressLogs(clientId);

            // Get client details for the view
            var client = await _userManager.FindByIdAsync(clientId);
            ViewBag.ClientName = client?.UserName ?? "Client";

            // Create the view model
            var viewModel = new ClientProgressViewModel
            {
                ClientId = clientId,
                ClientName = client?.UserName ?? "Client",
                ProgressLogs = progressLogs,
                // Add summary statistics
                StartWeight = progressLogs.OrderBy(l => l.Date).FirstOrDefault()?.Weight ?? 0,
                CurrentWeight = progressLogs.OrderByDescending(l => l.Date).FirstOrDefault()?.Weight ?? 0,
                TotalWorkouts = progressLogs.Count,
                LastUpdateDate = progressLogs.OrderByDescending(l => l.Date).FirstOrDefault()?.Date ?? DateTime.MinValue
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DownloadClientCsv(string clientId)
        {
            // Verify coach's access to this client
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var coaches = await _coachService.GetAllCoachesAsync();
            var coach = coaches.FirstOrDefault(c => c.Email == user.Email);
            
            if (coach == null)
            {
                TempData["ErrorMessage"] = "No coach profile found for your account.";
                return RedirectToAction("Dashboard");
            }

            // Verify client subscription
            var clientSubscription = await _subscriptionService.GetClientSubscriptions(clientId);
            var activeSubscription = clientSubscription.FirstOrDefault(s => s.IsActive);
            
            if (activeSubscription == null || activeSubscription.CoachId != coach.Id)
            {
                TempData["ErrorMessage"] = "You are not authorized to download this client's progress logs.";
                return RedirectToAction("MyClients");
            }

            // Get client details and progress logs
            var client = await _userManager.FindByIdAsync(clientId);
            var progressLogs = _progressLogService.GetAllProgressLogs(clientId);
            
            // Generate CSV content
            var csvContent = "Date,Weight (kg),Goal Weight (kg),Chest (cm),Waist (cm),Arms (cm),Thighs (cm),Notes\n";
            foreach (var log in progressLogs.OrderByDescending(l => l.Date))
            {
                csvContent += $"{log.Date:MM/dd/yyyy},{log.Weight},{log.GoalWeight},{log.Chest},{log.Waist},{log.Arms},{log.Thighs},\"{log.Notes?.Replace("\"", "\"\"")}\"\n";
            }

            var fileName = $"{client?.UserName ?? "client"}_progress_logs_{DateTime.Now:yyyyMMdd}.csv";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            
            return File(bytes, "text/csv", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsubscribeClient(int subscriptionId)
        {
            try
            {
                // Get current coach's information
                var user = await _userManager.GetUserAsync(User);
                var coaches = await _coachService.GetAllCoachesAsync();
                var coach = coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach == null)
                {
                    TempData["ErrorMessage"] = "Coach profile not found.";
                    return RedirectToAction(nameof(MyClients));
                }

                // Get the subscription and verify it belongs to this coach
                var subscription = await _subscriptionService.GetSubscriptionById(subscriptionId);
                if (subscription == null || subscription.CoachId != coach.Id)
                {
                    TempData["ErrorMessage"] = "Invalid subscription or unauthorized access.";
                    return RedirectToAction(nameof(MyClients));
                }

                // Perform the unsubscription
                var result = await _subscriptionService.UnsubscribeClient(subscriptionId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Client has been successfully unsubscribed.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to unsubscribe client. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction(nameof(MyClients));
        }
    }

    // View model for clients listing
    public class ClientViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int WorkoutPlans { get; set; }
    }

    public class ErrorViewModel
    {
        public string Message { get; set; }
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
} 