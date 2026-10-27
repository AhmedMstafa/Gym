

using Gym.DataAccess.Enums;

namespace Gym.DataAccess.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;

        public Speciality RequiredSpeciality { get; set; }

         public ICollection<Session> Sessions { get; set; } = [];
    }
}
