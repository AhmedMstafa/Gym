using Gym.Business.ViewModels.Members;
using System;

namespace Gym.Business.ViewModels
{
    public class MemberShipViewModel : BaseEntityViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MemberId { get; set; }

        public MemberIndexViewModel? Member { get; set; }

        public int PlanId { get; set; }

        public PlanViewModel? Plan { get; set; }
    }
}
