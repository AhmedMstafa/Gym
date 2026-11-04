using Gym.Business.ViewModels.Members;
using Gym.DataAccess.Enums;
using System;

namespace Gym.Business.ViewModels
{
    public class HealthRecordViewModel : BaseEntityViewModel
    {
        public decimal Hight { get; set; }

        public decimal Weight { get; set; }

        public string? Notes { get; set; }

        public string BloodType { get; set; } = default!;

        public int MemberId { get; set; }

        public MemberIndexViewModel? Member { get; set; }
    }
}
