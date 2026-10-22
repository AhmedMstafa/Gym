using System;
using System.Collections.Generic;
using Gym.Business.ViewModels.Sessions;
using Gym.DataAccess.Enums;

namespace Gym.Business.ViewModels
{
    public class TrainerViewModel : UserViewModel
    {
        public Speciality Speciality { get; set; }

        public DateTime HierDate { get; set; }

        public List<SessionIndexViewModel> Sessions { get; set; } = new List<SessionIndexViewModel>();
    }
}
