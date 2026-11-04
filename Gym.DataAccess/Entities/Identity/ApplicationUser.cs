using Microsoft.AspNetCore.Identity;

namespace Gym.DataAccess.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
    }
}
