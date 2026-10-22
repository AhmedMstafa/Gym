using Gym.Business.ViewModels.Members;
using Gym.Business.ViewModels.Sessions;
using System;

namespace Gym.Business.ViewModels
{
    public class BookingViewModel : BaseEntityViewModel
    {
        public DateTime Date { get; set; }

        public bool IsAttended { get; set; }

        public int MemberId { get; set; }

        public MemberIndexViewModel? Member { get; set; }

        public int SessionId { get; set; }

        public SessionIndexViewModel? Session { get; set; }
    }
}
