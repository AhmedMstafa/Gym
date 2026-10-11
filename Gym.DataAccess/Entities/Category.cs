using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;

         public ICollection<Session> Sessions { get; set; } = [];
    }
}
