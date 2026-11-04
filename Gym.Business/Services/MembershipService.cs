using Gym.Business.Common;
using Gym.Business.ViewModels.MembershipViewModels;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Repositories;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Gym.Business.Services
{
    internal sealed class MembershipService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        ILogger<MembershipService> logger) : IMembershipService
    {
        public async Task<IEnumerable<MemberShipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
            => (await unitOfWork.Memberships.GetAllWithDetailsAsync(ct))
                .Select(x => new MemberShipViewModel
                {
                    Id = x.Id,
                    MemberId = x.MemberId,
                    PlanId = x.PlanId,
                    MemberName = x.Member.Name,
                    PlanName = x.Plan.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToList();

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default)
            => (await unitOfWork.Plans.GetAllAsync(ct))
                .OrderBy(x => x.Name)
                .Select(x => new PlanSelectListViewModel { Id = x.Id, Name = x.Name })
                .ToList();

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default)
            => (await unitOfWork.Members.GetAllAsync(ct)).OrderBy(x => x.Name)
                .Select(x => new MemberSelectListViewModel { Id = x.Id, Name = x.Name })
                .ToList();

        public async Task<Result> CreateMembershipAsync(CreateMemberShipViewModel model, CancellationToken ct = default)
        {
            var plan = await unitOfWork.Plans.GetByIdAsync(model.PlanId, ct, false);
            var member = await unitOfWork.Members.GetByIdAsync(model.MemberId, ct, false);
            if (plan is null || member is null)
                return Result.Failure("InvalidMembership", "The selected member or plan was not found.");

            var startDate = model.StartDate ?? dateTimeProvider.DateTimeToday;
            await unitOfWork.Memberships.AddAsync(new MemberShip
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                StartDate = startDate,
                EndDate = startDate.AddDays(plan.DurationDays)
            });

            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

    public async Task<Result> DeleteMembershipAsync(int membershipId, CancellationToken ct = default)
        {
            var membership = await unitOfWork.Memberships.GetByIdIncludingDeletedAsync(membershipId, ct);

            if (membership is null)
                return Result.Failure("MembershipNotFound", "Membership was not found.");

            await unitOfWork.Memberships.SoftDeleteAsync(membership, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
