using Gym.Business.ViewModels.Members;
using Gym.DataAccess.Entities;

namespace Gym.Business.Mapping.Resolvers;

public class MemberMembershipEndDateResolver : IValueResolver<Member, MemberDetailsViewModel, string>
{
    public string Resolve(Member source, MemberDetailsViewModel? destination = default)
    {
        return Resolve(source);
    }

    public static string Resolve(Member source)
    {
        var membership = source.MemberShips.FirstOrDefault();
        return membership?.EndDate.ToShortDateString() ?? "-";
    }
}
