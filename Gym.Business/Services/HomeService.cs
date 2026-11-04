using Gym.Business.Common;
using Gym.Business.ViewModels.Home;
using Gym.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace Gym.Business.Services;

internal class HomeService(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<HomeService> logger) : IHomeService
{
    public async Task<HomeStatisticsViewModel> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving dashboard statistics from database.");

        var members = await unitOfWork.Members.GetAllAsync(cancellationToken);
        var trainers = await unitOfWork.Trainers.GetAllAsync(cancellationToken);
        var sessions = await unitOfWork.Sessions.GetAllAsync(cancellationToken);

        var today = dateTimeProvider.DateTimeToday;
        var now = dateTimeProvider.Now;

        // Active members: members who have at least one active membership (EndDate >= today)
        // Since Member.MemberShips is loaded via UnitOfWork or Repository, let's check members
        int activeMembersCount = 0;
        foreach (var member in members)
        {
            var memberWithMembership = await unitOfWork.Members.GetWithActiveMembershipAsync(member.Id, today, cancellationToken);
            if (memberWithMembership?.MemberShips != null && memberWithMembership.MemberShips.Any(ms => ms.EndDate >= today))
            {
                activeMembersCount++;
            }
        }

        var upcomingSessions = sessions.Count(s => now < s.StartDate);
        var ongoingSessions = sessions.Count(s => now >= s.StartDate && now < s.EndDate);
        var completedSessions = sessions.Count(s => now >= s.EndDate);

        var stats = new HomeStatisticsViewModel
        {
            TotalMembers = members.Count,
            ActiveMembers = activeMembersCount,
            TrainersCount = trainers.Count,
            UpcomingSessions = upcomingSessions,
            OngoingSessions = ongoingSessions,
            CompletedSessions = completedSessions
        };

        logger.LogInformation("Dashboard statistics retrieved successfully: {@Stats}", stats);
        return stats;
    }
}
