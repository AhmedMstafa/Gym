using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Members;
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
            .Map(dest => dest.Address, src => src.Address != null 
                ? $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}" 
                : "-")
            .Map(dest => dest.PlanName, src => src.MemberShips.FirstOrDefault() != null 
                ? src.MemberShips.FirstOrDefault()!.Plan.Name 
                : "No Active Plan")
            .Map(dest => dest.MemberShipStartDate, src => src.MemberShips.FirstOrDefault() != null 
                ? src.MemberShips.FirstOrDefault()!.StartDate.ToShortDateString() 
                : "-")
            .Map(dest => dest.MemberShipEndDate, src => src.MemberShips.FirstOrDefault() != null 
                ? src.MemberShips.FirstOrDefault()!.EndDate.ToShortDateString() 
                : "-");

        config.NewConfig<HealthRecord, HealthRecordViewModel>()
            .Map(dest => dest.BloodType, src => src.BloodType.ToString());

        config.NewConfig<Member, EditMemberViewModel>()
            .Map(dest => dest.BuildingNumber, src => src.Address != null ? src.Address.BuildingNumber : 0)
            .Map(dest => dest.City, src => src.Address != null ? src.Address.City : string.Empty)
            .Map(dest => dest.Street, src => src.Address != null ? src.Address.Street : string.Empty);

        config.NewConfig<CreateMemberViewModel, Member>()
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
    }
}
