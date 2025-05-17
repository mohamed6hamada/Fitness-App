using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Fitness_App.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Fitness_App.BL.Models;
using Fitness_App.DAL.ConfigurationClasses;

namespace Fitness_App.DAL.DbContext
{
    public class FitnessAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public FitnessAppDbContext(DbContextOptions<FitnessAppDbContext> opton) : base(opton)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Ignore<Date>();

            modelBuilder.ApplyConfiguration(new ProgressLogConfigurations());

            // Ensure the table name and relationships are correct
            modelBuilder.Entity<ProgressLog>()
                .ToTable("ProgressLog") // Ensure the table name matches your database schema
                .HasKey(pl => pl.ProgressLogId);

            // Configure the many-to-many relationship between Users and WorkoutPlans
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.AssignedWorkoutPlans)
                .WithMany(w => w.AssignedUsers);

            // Configure ClientCoachSubscription relationships
            modelBuilder.Entity<ClientCoachSubscription>()
                .HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClientCoachSubscription>()
                .HasOne(s => s.Coach)
                .WithMany()
                .HasForeignKey(s => s.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClientCoachSubscription>()
                .HasOne(s => s.AssignedWorkoutPlan)
                .WithMany()
                .HasForeignKey(s => s.WorkoutPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            // If there are other entities, ensure their configurations are applied as well
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessAppDbContext).Assembly);
        }
        //public DbSet<Admin> Admins { get; set; }
        //public DbSet<Client> Clients { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<DietPlans> DietPlans { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<ProgressLog> ProgressLog { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<UserSubscription> userSubscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<ClientCoachSubscription> ClientCoachSubscriptions { get; set; }
        //public DbSet<Subscription> Subscriptions { get; set; }
        //public DbSet<Message> Messages { get; set; }
    }
}
