using System;

namespace Gym.Business.ViewModels.Sessions;

public class SessionDetailsViewModel
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string TrainerName { get; set; } = default!;

    public int TrainerId { get; set; }

    public int Capacity { get; set; }

    public int BookedCount { get; set; }

    public int AvailableSlots => Math.Max(0, Capacity - BookedCount);

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public SessionStatus Status { get; set; }

    public TimeSpan Duration => EndDate - StartDate;

    public string HeaderClass => Status switch
    {
        SessionStatus.Completed => "bg-primary",
        SessionStatus.Upcoming => "bg-secondary",
        SessionStatus.Ongoing => "bg-success",
        _ => "bg-secondary",
    };
}
