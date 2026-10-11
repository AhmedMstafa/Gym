using Gym.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Data.Configration
{
    public class TrainerConfigration : UserConfigrations<Trainer>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Trainer> builder)
        {
            base.Configure(builder);
            builder.Property(t => t.Speciality)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            
        }
    }
}