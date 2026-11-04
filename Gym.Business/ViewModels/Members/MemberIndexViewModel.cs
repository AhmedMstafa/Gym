namespace Gym.Business.ViewModels.Members
{
    public class MemberIndexViewModel 
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public string? Photo { get; set; }

        public DateTime JoinDate { get; set; }

        public string Email { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public DateOnly DateOfBirth { get; set; }

        public string Gender { get; set; } = default!;

    }
}
