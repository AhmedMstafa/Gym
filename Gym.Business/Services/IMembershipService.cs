using Gym.Business.Common;
using Gym.Business.ViewModels.MembershipViewModels;

namespace Gym.Business.Services
{
	public interface IMembershipService
	{
        Task<IEnumerable<MemberShipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default);
        Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default);
        Task<Result> CreateMembershipAsync(CreateMemberShipViewModel model, CancellationToken ct = default);
        Task<Result> DeleteMembershipAsync(int membershipId, CancellationToken ct = default);

    }
}
