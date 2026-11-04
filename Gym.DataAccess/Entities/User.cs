using Gym.DataAccess.Entities.ValueObjects;
using Gym.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Gym.DataAccess.Entities
{
    public abstract class User : BaseEntity
    {
       public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public Address Address { get; set; } = default!;
    }
}
