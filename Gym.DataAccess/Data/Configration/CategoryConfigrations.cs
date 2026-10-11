using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Data.Configration
{
    public class CategoryConfigrations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                   .HasMaxLength(30);

            builder.HasQueryFilter(c => !c.IsDeleted);

        }
    }
}
