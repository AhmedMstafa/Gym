using Gym.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Data.Configuration
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                   .HasMaxLength(50);

            builder.Property(p => p.Description)
                   .HasMaxLength(200);

            builder.Property(p => p.Price)
                   .HasPrecision(10, 2);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Plan_DurationDays",
                    "DurationDays BETWEEN 1 AND 365"
                );
            });

            builder.HasIndex(p => p.Name)
                   .IsUnique();
        }
    }
}