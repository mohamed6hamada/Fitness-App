using Fitness_App.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fitness_App.BL.Services
{
    public class SubscriptionExpirationService : BackgroundService
    {
        private readonly ILogger<SubscriptionExpirationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionExpirationService(
            ILogger<SubscriptionExpirationService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpiredSubscriptions();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking expired subscriptions");
                }

                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckExpiredSubscriptions()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FitnessAppDbContext>();

            var now = DateTime.UtcNow;

            // Check UserSubscriptions
            var expiredUserSubs = await dbContext.userSubscriptions
                .Where(s => s.IsActive && s.EndDate <= now)
                .ToListAsync();

            foreach (var sub in expiredUserSubs)
            {
                sub.IsActive = false;
                _logger.LogInformation($"Deactivated expired user subscription ID: {sub.Id}");
            }

            // Check ClientCoachSubscriptions
            var expiredCoachSubs = await dbContext.ClientCoachSubscriptions
                .Where(s => s.IsActive && s.EndDate.HasValue && s.EndDate <= now)
                .ToListAsync();

            foreach (var sub in expiredCoachSubs)
            {
                sub.IsActive = false;
                _logger.LogInformation($"Deactivated expired coach subscription ID: {sub.Id}");
            }

            await dbContext.SaveChangesAsync();
        }
    }
} 