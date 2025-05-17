using Fitness_App.BL.Services;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessAPPMVC.Controllers
{
    [Authorize] // This controller is accessible to all authenticated users
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProgressLogServices _progressLogServices;

        public ProfileController(
            ProfileService profileService,
            UserManager<ApplicationUser> userManager,
            ProgressLogServices progressLogServices)
        {
            _profileService = profileService;
            _userManager = userManager;
            _progressLogServices = progressLogServices;
        }

        // GET: Profile - Works for both Admin and User roles
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _profileService.GetUserProfileAsync(userId);
            
            if (profile == null)
                return NotFound();
            
            // Set ViewBag.IsAdmin to show/hide admin-specific options
            ViewBag.IsAdmin = User.IsInRole("Admin");
            
            return View(profile);
        }

        // GET: Profile/Edit - Works for both Admin and User roles
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _profileService.GetUserProfileAsync(userId);
            
            if (profile == null)
                return NotFound();
            
            return View(profile);
        }

        // POST: Profile/Edit - Works for both Admin and User roles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            // Only validate required fields
            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError("FullName", "Full name is required");
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (model.Id != userId)
                return Forbid();

            var success = await _profileService.UpdateProfileAsync(model);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Your profile has been updated successfully.";
                // Ensure we're redirecting to the correct action
                return RedirectToAction("Index", "Profile");
            }
            
            ModelState.AddModelError("", "Failed to update profile. Please try again.");
            return View(model);
        }

        // GET: Profile/View/{id} - Admin only to view other users' profiles
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> View(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
                
            var profile = await _profileService.GetUserProfileAsync(id);
            
            if (profile == null)
                return NotFound();
                
            return View(profile);
        }
    }
} 