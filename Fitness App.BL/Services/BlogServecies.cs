using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.Models;
using System.Linq;

namespace Fitness_App.BL.Services
{
    public class BlogServices
    {
        private readonly IAdminRepository<Blogs> _blogRepo;
        private readonly CommentService _commentService;

        public BlogServices(IAdminRepository<Blogs> blogRepo, CommentService commentService)
        {
            _blogRepo = blogRepo;
            _commentService = commentService;
        }

        public List<Blogs> GetAllBlogs() => _blogRepo.GetAllBlogs();

        public Blogs GetBlogById(int id) => _blogRepo.GetBlogById(id);

        public void AddBlog(Blogs blog) => _blogRepo.AddBlogs(blog);

        public void DeleteBlog(int id) => _blogRepo.DeleteBlogs(id);

        public void UpdateBlog(Blogs blog) => _blogRepo.UpdateBlogs(blog);

        // New method to get blog with comments
        public BlogViewModel GetBlogWithComments(int id)
        {
            var blog = GetBlogById(id);
            if (blog == null)
                return null;

            var comments = _commentService.GetCommentsByBlogId(id);

            return new BlogViewModel
            {
                BlogId = blog.BlogId,
                BlogTitle = blog.BlogTitle,
                BlogContent = blog.BlogContent,
                ImagePath = blog.ImagePath,
                CreatedAt = blog.CreatedAt,
                
            };
        }

        // Get all blogs with their comments (for better performance)
        public List<BlogViewModel> GetAllBlogsWithComments()
        {
            var blogs = GetAllBlogs();
            var blogViewModels = new List<BlogViewModel>();
            
            foreach (var blog in blogs)
            {
                var comments = _commentService.GetCommentsByBlogId(blog.BlogId);
                
                blogViewModels.Add(new BlogViewModel
                {
                    BlogId = blog.BlogId,
                    BlogTitle = blog.BlogTitle,
                    BlogContent = blog.BlogContent,
                    ImagePath = blog.ImagePath,
                    CreatedAt = blog.CreatedAt,
                    
                });
            }
            
            return blogViewModels;
        }

        // Add a comment to a blog
        public void AddComment(CommentViewModel commentViewModel)
        {
            _commentService.AddComment(commentViewModel);
        }
    }
}
