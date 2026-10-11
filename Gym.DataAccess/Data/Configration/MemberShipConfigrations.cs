using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.DataAccess.Data.Configration
{
    public class MemberShipConfigrations : IEntityTypeConfiguration<MemberShip>
    {
        public void Configure(EntityTypeBuilder<MemberShip> builder)
        {
       
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_MemberShip_StartDate_EndDate", "[StartDate] < [EndDate]");
            });

            //builder.HasIndex(m =>new
            //{
            //    m.MemberId,
            //    m.PlanId,
            //    m.StartDate,
            //}).IsUnique();

            builder.HasQueryFilter(m => m.DeletedAt == null);
        }
    }
}