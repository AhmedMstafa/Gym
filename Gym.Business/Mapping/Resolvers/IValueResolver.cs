namespace Gym.Business.Mapping.Resolvers;

public interface IValueResolver<in TSource, in TDestination, out TDestMember>
{
    TDestMember Resolve(TSource source, TDestination? destination = default);
}
