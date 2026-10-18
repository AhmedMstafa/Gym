using Gym.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {
        public Task<bool> IsEmailTakenAsync(string normalizedEmail,int ? excludeId = null, CancellationToken cancellationToken = default);
        public Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null, CancellationToken cancellationToken = default);

        public Task<Member?> GetWithMembershipAsync(int id, CancellationToken cancellationToken = default);

        public Task<bool> HasUpcommingActiveSessionsAsync(int id, CancellationToken cancellationToken = default); 

    }
}
