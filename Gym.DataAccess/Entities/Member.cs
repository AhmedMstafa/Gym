using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Entities
{
    public class Member : User
    {
        public string? Photo { get; set; }

        public DateTime JoinDate { get; set; }

        public HealthRecord HealthRecord { get; set; } = default!;

        public ICollection<MemberShip> MemberShips { get; } = [];

        public ICollection<Booking> Bookings { get; } = [];

    }
}
