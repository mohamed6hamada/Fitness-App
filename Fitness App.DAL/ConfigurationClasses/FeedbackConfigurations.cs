using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness_App.BL.Models;
using Fitness_App.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitness_App.DAL.ConfigurationClasses
{
    public class FeedbackConfigurations : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> entity)
        {

            
                entity.HasOne(f => f.User)
                        .WithMany(u => u.Feedbacks)
                         .HasForeignKey(f => f.UserId)
                          .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
