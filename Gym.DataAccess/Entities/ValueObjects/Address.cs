using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Entities.ValueObjects
{
    public class Address
    {
        public string City { get; set; } = default!;

        public string Street { get; set; } = default!;

        public int BuildingNumber { get; set; }


    }
}
