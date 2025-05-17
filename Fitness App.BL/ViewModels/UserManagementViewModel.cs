using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fitness_App.BL.ViewModels
{
    public class UserManagementViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }
        
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; }
        
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }
        
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; set; }
        
        // Progress statistics
        [Display(Name = "Progress Logs")]
        public int ProgressLogsCount { get; set; }
        
        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }
    }
    
    public class UserListViewModel
    {
        public List<UserManagementViewModel> Users { get; set; }
        public int TotalUsers { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalUsers / PageSize);
        public string SearchTerm { get; set; }
        public string RoleFilter { get; set; }
        public string StatusFilter { get; set; }
    }
} 