using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.Business.ViewModels.Members
{
    public class MemberDetailsViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Photo { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;
        public string Gender { get; set; } = default!;

        public string Address { get; set; } = default!;
        public string DateOfBirth { get; set; } = default!;
        public string PlanName { get; set; } = default!;

        public string MemberShipStartDate { get; set; } = default!;

        public string MemberShipEndDate { get; set; } = default!;


    }
}
