using Gym.Business.Common;
using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Members;

namespace Gym.Business.Services
{
    public interface IMemberServices
    {
        Task<IEnumerable<MemberIndexViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(CreateMemberViewModel model, CancellationToken cancellationToken = default);

        Task<MemberDetailsViewModel?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<HealthRecordViewModel?> GetHealthRecordAsync(int id, CancellationToken cancellationToken = default);

        Task<EditMemberViewModel?> GetForUpdateAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(int id,EditMemberViewModel model, CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);


    }
}
