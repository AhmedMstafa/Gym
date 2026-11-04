using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Entities
{
    public class Booking : BaseEntity
    {
            public DateTime Date { get; set; }

        public bool IsAttended { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public int SessionId { get; set; }

        public Session Session { get; set; } = default!;
    }
}
