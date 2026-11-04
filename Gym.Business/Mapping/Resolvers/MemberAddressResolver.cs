using Gym.Business.ViewModels.Members;
using Gym.DataAccess.Entities;

namespace Gym.Business.Mapping.Resolvers;

public class MemberAddressResolver : IValueResolver<Member, MemberDetailsViewModel, string>
{
    public string Resolve(Member source, MemberDetailsViewModel? destination = default)
    {
        return Resolve(source);
    }

    public static string Resolve(Member source)
    {
        return source.Address != null
            ? $"{source.Address.BuildingNumber} - {source.Address.Street} - {source.Address.City}"
            : "-";
    }
}
