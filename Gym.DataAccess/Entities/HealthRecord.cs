using Gym.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Entities
{
    public class HealthRecord : BaseEntity
    {
        public decimal Hight { get; set; }

        public decimal Weight { get; set; } 

        public BloodType BloodType { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;
    }
}
