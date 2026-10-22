using Gym.Business.Common;
using Gym.Business.ViewModels.Sessions;
using Gym.DataAccess.Queries;

namespace Gym.Business.Services;

public sealed class SessionService(
    ISessionQueryService sessionQueryService,
    IDateTimeProvider dateTimeProvider) : ISessionService
{
    public async Task<IReadOnlyList<SessionIndexViewModel>> GetIndexItemsAsync(
        CancellationToken cancellationToken = default)
    {
        var sessions = await sessionQueryService.GetIndexItemsAsync(cancellationToken);
        var now = dateTimeProvider.Now;

        return sessions.Select(session => new SessionIndexViewModel
        {
            Id = session.Id,
            Speciality = session.Speciality,
            Description = session.Description,
            TrainerName = session.TrainerName,
            Capacity = session.Capacity,
            BookedCount = session.BookedCount,
            StartDate = session.StartDate,
            EndDate = session.EndDate,
            TrainerId = session.TrainerId,
            Status = GetStatus(session.StartDate, session.EndDate, now)
        }).ToList();
    }

    private static SessionStatus GetStatus(DateTime startDate, DateTime endDate, DateTime now)
    {
        if (now < startDate)
            return SessionStatus.Upcoming;

        return now < endDate
            ? SessionStatus.Ongoing
            : SessionStatus.Completed;
    }
}