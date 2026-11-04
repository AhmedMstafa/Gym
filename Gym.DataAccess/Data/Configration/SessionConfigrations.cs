using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.DataAccess.Data.Configration
{
    public class SessionConfigrations : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.ToTable("Sessions", t =>
            {
                t.HasCheckConstraint(
                    "CK_Session_Capacity",
                    "[Capacity] BETWEEN 1 AND 25"
                );

                t.HasCheckConstraint(
                    "CK_Session_DateRange",
                    "[EndDate] > [StartDate]"
                );
            });

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}