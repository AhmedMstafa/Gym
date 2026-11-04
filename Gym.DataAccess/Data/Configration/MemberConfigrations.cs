using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Data.Configration
{
    public class MemberConfigrations : IEntityTypeConfiguration<Member>
    {
        public  void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(m => m.Photo)
                .HasMaxLength(1000);
            builder.Property(m => m.JoinDate)
                .IsRequired();

            //builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}