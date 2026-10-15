using Gym.DataAccess.Enums;
using System;

namespace Gym.Business.ViewModels
{
    public class UserViewModel : BaseEntityViewModel
    {
        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public AddressViewModel Address { get; set; } = new AddressViewModel();
    }
}
