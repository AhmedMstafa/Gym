namespace Gym.DataAccess.Queries
{
    public class SessionIndexQueryService
    {
        public int Id { get; init; }

        public string Speciality { get; init; } = default!;
        public string Description { get; init; } = default!;

        public string TrainerName { get; init; } = default!;

        public int BookedCount { get; init; } = default!;

        public int Capacity { get; init; }

        public DateTime StartDate { get; init; }

        public DateTime EndDate { get; init; }

        public int TrainerId { get; init; }

        public string Status { get; init; } = default!;


     
    }
}
