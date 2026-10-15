using Gym.Business.Common;
using Gym.Business.ViewModels.Members;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gym.Business.Services
{
    public interface IMemberServices
    {
        Task<IEnumerable<MemberIndexViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result> CreateAsync(CreateMemberViewModel model, CancellationToken cancellationToken = default);
    }
}
