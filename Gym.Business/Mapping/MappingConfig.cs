using Gym.Business.Mapping.Resolvers;
using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Members;
using Gym.Business.ViewModels.Sessions;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Entities.ValueObjects;
using Mapster;

namespace Gym.Business.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Member, MemberIndexViewModel>()
            .Map(dest => dest.Gender, src => src.Gender.ToString());

        config.NewConfig<Member, MemberDetailsViewModel>()
            .Map(dest => dest.Gender, src => src.Gender.ToString())
            .Map(dest => dest.DateOfBirth, src => src.DateOfBirth.ToShortDateString())
            .Map(dest => dest.Address, src => MemberAddressResolver.Resolve(src))
            .Map(dest => dest.PlanName, src => MemberPlanNameResolver.Resolve(src))
            .Map(dest => dest.MemberShipStartDate, src => MemberMembershipStartDateResolver.Resolve(src))
            .Map(dest => dest.MemberShipEndDate, src => MemberMembershipEndDateResolver.Resolve(src));

        config.NewConfig<HealthRecord, HealthRecordViewModel>()
            .Map(dest => dest.BloodType, src => HealthRecordBloodTypeResolver.Resolve(src));

        config.NewConfig<Member, EditMemberViewModel>()
            .Map(dest => dest.BuildingNumber, src => src.Address != null ? src.Address.BuildingNumber : 0)
            .Map(dest => dest.City, src => src.Address != null ? src.Address.City : string.Empty)
            .Map(dest => dest.Street, src => src.Address != null ? src.Address.Street : string.Empty);

        config.NewConfig<CreateMemberViewModel, Member>()
            .Ignore(dest => dest.Photo)
            .Map(dest => dest.Name, src => src.Name.Trim())
            .Map(dest => dest.Email, src => src.Email.Trim().ToLower())
            .Map(dest => dest.Phone, src => src.Phone.Trim())
            .Map(dest => dest.DateOfBirth, src => DateOnly.FromDateTime(src.DateOfBirth))
            .Map(dest => dest.Address, src => new Address
            {
                BuildingNumber = src.BuildingNumber,
                City = src.City.Trim(),
                Street = src.Street.Trim()
            })
            .Map(dest => dest.HealthRecord, src => new HealthRecord
            {
                Hight = src.HealthRecord.Hight,
                Weight = src.HealthRecord.Weight,
                Notes = src.HealthRecord.Notes
            });

        config.NewConfig<Session, SessionDetailsViewModel>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
            .Map(dest => dest.TrainerName, src => src.Trainer != null ? src.Trainer.Name : string.Empty)
            .Map(dest => dest.BookedCount, src => src.Bookings != null ? src.Bookings.Count : 0);

        config.NewConfig<CreateSessionViewModel, Session>()
            .Map(dest => dest.Description, src => src.Description.Trim());

        config.NewConfig<Session, EditSessionViewModel>();
    }
}
