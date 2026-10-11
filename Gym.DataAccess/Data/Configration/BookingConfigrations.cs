using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.DataAccess.Data.Configration
{
    public class BookingConfigrations : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {


            builder.Property(b => b.Date)
                .IsRequired()
                   .IsRequired();
            builder.HasIndex(x => new
            {
                x.SessionId,
                x.MemberId
            }).IsUnique();

          
           builder.HasOne(b => b.Member)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MemberId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}