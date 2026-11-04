using System;
using System.Collections.Generic;

namespace Gym.Business.ViewModels.Sessions
{
    public class SessionIndexViewModel
    {
        public int Id { get; init; }

        public string Speciality { get; init; } = default!;
        public string Description { get; init; } = default!;

        public string TrainerName { get; init; } = default!;

        public int Capacity { get; init; }

        public int BookedCount { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public int TrainerId { get; init; }

        public SessionStatus Status { get; init; }

        public TimeSpan Duration => EndDate - StartDate;

        public string HeaderClass => Status switch
        {
            SessionStatus.Completed => "bg-primary",
            SessionStatus.Upcoming => "bg-secondary",
            SessionStatus.Ongoing => "bg-success",
           _ => "bg-secondary",
        };

    }
}
