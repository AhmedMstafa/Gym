using Gym.Business.ViewModels;
using Gym.DataAccess.Entities;

namespace Gym.Business.Mapping.Resolvers;

public class HealthRecordBloodTypeResolver : IValueResolver<HealthRecord, HealthRecordViewModel, string>
{
    public string Resolve(HealthRecord source, HealthRecordViewModel? destination = default)
    {
        return Resolve(source);
    }

    public static string Resolve(HealthRecord source)
    {
        return source.BloodType.ToString();
    }
}
