using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Fitness_App.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitness_App.DAL.ConfigurationClasses
{
    public class ProgressLogConfigurations : IEntityTypeConfiguration<ProgressLog>
    {

        public void Configure(EntityTypeBuilder<ProgressLog> entity)
        {

            
                      entity.HasOne(p => p.User)
                            .WithMany(u => u.ProgressLogs)
                            .HasForeignKey(p => p.UserId)
                            .OnDelete(DeleteBehavior.Cascade);
           

        }
    }
}
