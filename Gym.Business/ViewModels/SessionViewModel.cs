using System;
using System.Collections.Generic;

namespace Gym.Business.ViewModels
{
    public class SessionViewModel : BaseEntityViewModel
    {
        public string Description { get; set; } = default!;

        public int Capacity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TrainerId { get; set; }

        public TrainerViewModel? Trainer { get; set; }

        public int CategoryId { get; set; }

        public CategoryViewModel? Category { get; set; }

        public List<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
    }
}
