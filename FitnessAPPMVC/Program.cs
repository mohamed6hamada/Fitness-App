using Fitness_App.BL.Interfaces;
using Fitness_App.BL.Servecies;
using Fitness_App.BL.Services;
using Fitness_App.BL.ViewModels;
using Fitness_App.DAL.DbContext;
using Fitness_App.DAL.Interfaces;
using Fitness_App.DAL.Models;
using Fitness_App.DAL.Repositories;
using Fitness_App.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserRoles.Services;

namespace FitnessAPPMVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<FitnessAppDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<FitnessAppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IAdminRepository<Blogs>, AdminRepository<Blogs>>();
            builder.Services.AddScoped<IAdminRepository<BlogViewModel>, AdminRepository<BlogViewModel>>();
            builder.Services.AddScoped(typeof(IAdminRepository<DietPlans>), typeof(AdminRepository<DietPlans>));
            builder.Services.AddScoped<IAdminRepository<ExerciseViewModel>, AdminRepository<ExerciseViewModel>>();
            builder.Services.AddScoped<IAdminRepository<SubscriptionPlanViewModel>, AdminRepository<SubscriptionPlanViewModel>>();
            builder.Services.AddScoped<IAdminRepository<Comment>, AdminRepository<Comment>>();

            builder.Services.AddScoped<IClientRepository<ProgressLogViewModel>, ClientRepository<ProgressLogViewModel>>();
            builder.Services.AddScoped<IClientRepository<ApplicationUser>, ClientRepository<ApplicationUser>>();
            builder.Services.AddScoped<IClientRepository<UserSubscriptionViewModel>, ClientRepository<UserSubscriptionViewModel>>();
            builder.Services.AddScoped(typeof(IClientRepository<>), typeof(ClientRepository<>));
            builder.Services.AddScoped<ClientRepository<UserSubscriptionViewModel>>();

            // Register coach & workout plan repositories
            builder.Services.AddScoped<IAdminRepository<CoachViewModel>, AdminRepository<CoachViewModel>>();
            builder.Services.AddScoped<IAdminRepository<WorkoutPlanViewModel>, AdminRepository<WorkoutPlanViewModel>>();
            builder.Services.AddScoped<Fitness_App.DAL.Interfaces.ICoachRepository, Fitness_App.DAL.Repositories.CoachRepository>();

            builder.Services.AddScoped<BlogServices>();
            builder.Services.AddScoped<DietPlanService>();
            builder.Services.AddScoped<ExerciseServices>();
            builder.Services.AddScoped<SubscriptionServices>();
            builder.Services.AddScoped<ProgressLogServices>();
            builder.Services.AddScoped<ProfileService>();
            builder.Services.AddScoped<UserManagementService>();
            builder.Services.AddScoped<UserSubscriptionServices>();
            builder.Services.AddScoped<CommentService>();

            // Register coach & workout plan services
            builder.Services.AddScoped<WorkoutPlanService>();
            builder.Services.AddScoped<ClientCoachSubscriptionService>();
            builder.Services.AddScoped<CoachService>();
            Console.WriteLine("Registered CoachService with dependencies: FitnessAppDbContext & IWebHostEnvironment");

            // Add background service for subscription expiration checks
            builder.Services.AddHostedService<SubscriptionExpirationService>();

            //----------

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(options =>
            //{
            //        options.Cookie.HttpOnly = true;
            //        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //        options.LoginPath = "/Account/Login";
            //        options.AccessDeniedPath = "/Account/AccessDenied";
            //        options.SlidingExpiration = true;
            //});

            var app = builder.Build();

            await SeedService.SeedDatabase(app.Services);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Add specific routes for coach-client functionality
            app.MapControllerRoute(
                name: "coach",
                pattern: "Coach/{action=Dashboard}/{id?}",
                defaults: new { controller = "Coach" });

            app.MapControllerRoute(
                name: "client",
                pattern: "Client/{action=Index}/{id?}",
                defaults: new { controller = "Client" });
            

            app.Run();
        }
    }
}
