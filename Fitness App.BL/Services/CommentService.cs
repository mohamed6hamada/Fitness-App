using System;
using System.Collections.Generic;
using System.Linq;
using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;

namespace Fitness_App.BL.Services
{
    public class CommentService
    {
        private readonly IAdminRepository<Comment> _commentRepository;
        private readonly IClientRepository<ApplicationUser> _userRepository;

        public CommentService(IAdminRepository<Comment> commentRepository, 
                              IClientRepository<ApplicationUser> userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public void AddComment(CommentViewModel commentViewModel)
        {
            var comment = new Comment
            {
                Content = commentViewModel.Content,
                BlogId = commentViewModel.BlogId,
                UserId = commentViewModel.UserId,
                CreatedAt = DateTime.Now
            };

            _commentRepository.AddComment(comment);
        }

        public List<CommentViewModel> GetCommentsByBlogId(int blogId)
        {
            var comments = _commentRepository.GetCommentsByBlogId(blogId);
            var commentViewModels = new List<CommentViewModel>();
            
            foreach (var comment in comments)
            {
                var user = _userRepository.GetUserById(comment.UserId);
                var viewModel = new CommentViewModel
                {
                    CommentId = comment.CommentId,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    BlogId = comment.BlogId,
                    UserId = comment.UserId,
                    UserName = user != null ? ((ApplicationUser)user).FullName : "Unknown User",
                    UserEmail = user != null ? ((ApplicationUser)user).Email : "unknown@example.com"
                };
                
                commentViewModels.Add(viewModel);
            }

            return commentViewModels;
        }

        public void DeleteComment(int commentId)
        {
            _commentRepository.DeleteComment(commentId);
        }
    }
} 