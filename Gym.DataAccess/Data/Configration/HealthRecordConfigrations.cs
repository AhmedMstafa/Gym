using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Data.Configration
{
    public class HealthRecordConfigrations : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
           builder.Property(hr => hr.Hight)
                  .HasPrecision(5, 2);

            builder.Property(hr => hr.Weight)
                   .HasPrecision(5, 2);

            builder.Property(hr => hr.BloodType)
                   .HasConversion<string>();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_HealthRecord_Hight", "[Hight] >= 0 AND [Hight] <= 300");
                t.HasCheckConstraint("CK_HealthRecord_Weight", "[Weight] >= 0 AND [Weight] <= 500");

            });

            builder.HasQueryFilter(hr => hr.IsDeleted == false);
        }
    }
}
