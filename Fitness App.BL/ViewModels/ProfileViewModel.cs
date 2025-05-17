using Fitness_App.BL.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Fitness_App.BL.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Upload New Picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Bio")]
        [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? Bio { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Facebook URL")]
        [OptionalUrl(ErrorMessage = "Please enter a valid URL")]
        public string? FacebookUrl { get; set; }

        [Display(Name = "Instagram URL")]
        [OptionalUrl(ErrorMessage = "Please enter a valid URL")]
        public string? InstagramUrl { get; set; }

        [Display(Name = "Twitter URL")]
        [OptionalUrl(ErrorMessage = "Please enter a valid URL")]
        public string? TwitterUrl { get; set; }

        [Display(Name = "LinkedIn URL")]
        [OptionalUrl(ErrorMessage = "Please enter a valid URL")]
        public string? LinkedInUrl { get; set; }

        [Display(Name = "Fitness Goals")]
        [MaxLength(500, ErrorMessage = "Fitness goals cannot exceed 500 characters")]
        public string? FitnessGoals { get; set; }
        
        [Display(Name = "Preferred Workout Times")]
        public string? PreferredWorkoutTimes { get; set; }
        
        [Display(Name = "Height (cm)")]
        [Range(0, 300, ErrorMessage = "Please enter a valid height")]
        public double? Height { get; set; }

        // Summary statistics
        public int TotalProgressLogs { get; set; }
        public double? WeightChange { get; set; }
        public DateTime? MemberSince { get; set; }
    }
} 