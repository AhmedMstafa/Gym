using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Data.Configration
{
    public class TrainerConfigration : IEntityTypeConfiguration<Trainer>
    {
        public  void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(t => t.Speciality)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            //builder.HasQueryFilter(u => !u.IsDeleted);
        }

       
    }
}