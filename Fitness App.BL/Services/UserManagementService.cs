using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness_App.BL.Services
{
    public class UserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClientRepository<ApplicationUser> _clientRepository;
        private readonly ProgressLogServices _progressLogServices;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IClientRepository<ApplicationUser> clientRepository,
            ProgressLogServices progressLogServices)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _clientRepository = clientRepository;
            _progressLogServices = progressLogServices;
        }

        public async Task<UserListViewModel> GetUsersAsync(
            string searchTerm = "", 
            string roleFilter = "", 
            string statusFilter = "",
            int page = 1, 
            int pageSize = 10)
        {
            // Start with all users
            IQueryable<ApplicationUser> usersQuery = _userManager.Users
                .Where(u => !u.IsDeleted); // By default, don't show deleted users
            
            // Apply search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u => 
                    u.FullName.Contains(searchTerm) || 
                    u.Email.Contains(searchTerm) ||
                    u.UserName.Contains(searchTerm) ||
                    u.PhoneNumber.Contains(searchTerm));
            }
            
            // Apply status filter if provided
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isActive = statusFilter.ToLower() == "active";
                usersQuery = usersQuery.Where(u => u.IsActive == isActive);
            }
            
            // Get total count before pagination
            var totalUsers = await usersQuery.CountAsync();
            
            // Apply pagination
            var pagedUsers = await usersQuery
                .OrderByDescending(u => u.MemberSince)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            // Create view models for each user
            var userViewModels = new List<UserManagementViewModel>();
            
            foreach (var user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                // Check role filter if provided
                if (!string.IsNullOrEmpty(roleFilter) && !roles.Contains(roleFilter))
                {
                    continue; // Skip users who don't have the filtered role
                }
                
                var progressLogs = _progressLogServices.GetAllProgressLogs(user.Id);
                
                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfilePicture,
                    MemberSince = user.MemberSince,
                    Roles = roles.ToList(),
                    IsActive = user.IsActive,
                    IsDeleted = user.IsDeleted,
                    ProgressLogsCount = progressLogs?.Count ?? 0,
                    LastLogin = user.LastLoginDate
                });
            }
            
            // If role filter was applied after database query
            if (!string.IsNullOrEmpty(roleFilter))
            {
                totalUsers = userViewModels.Count; // Recalculate total after filtering
            }
            
            return new UserListViewModel
            {
                Users = userViewModels,
                TotalUsers = totalUsers,
                CurrentPage = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                StatusFilter = statusFilter
            };
        }
        
        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        
        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            user.IsDeleted = true;
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        
        public async Task<bool> PermanentDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        
        public async Task<UserManagementViewModel> GetUserDetailsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            
            var roles = await _userManager.GetRolesAsync(user);
            var progressLogs = _progressLogServices.GetAllProgressLogs(user.Id);
            
            return new UserManagementViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                MemberSince = user.MemberSince,
                Roles = roles.ToList(),
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted,
                ProgressLogsCount = progressLogs?.Count ?? 0,
                LastLogin = user.LastLoginDate
            };
        }
        
        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }
    }
} 