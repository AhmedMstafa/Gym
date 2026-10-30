using Microsoft.AspNetCore.Identity;

namespace Gym.DataAccess.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string DisplayName { get; set; } = default!;
    }
}
