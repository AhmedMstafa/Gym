using System;

namespace Gym.Business.ViewModels
{
    public class AddressViewModel
    {
        public string City { get; set; } = default!;

        public string Street { get; set; } = default!;

        public int BuildingNumber { get; set; }
    }
}
