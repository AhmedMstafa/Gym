using Gym.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Entities
{
    public class Trainer  : User
    {
        public Speciality Speciality { get; set; }

        public DateTime HierDate { get; set; }


        public ICollection<Session> Sessions { get; set; } = [];


    }
}
