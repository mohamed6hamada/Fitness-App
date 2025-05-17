using FitnessAPPMVC.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Fitness_App.BL.Services;
using Fitness_App.BL.ViewModels;

namespace FitnessAPPMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogServices _blogService;

        public HomeController(ILogger<HomeController> logger, BlogServices blogService)
        {
            _logger = logger;
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            // Get latest 3 blogs for the home page
            var blogViewModels = _blogService.GetAllBlogsWithComments()
                .OrderByDescending(b => b.CreatedAt)
                .Take(3)
                .ToList();
                
            ViewBag.LatestBlogs = blogViewModels;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
