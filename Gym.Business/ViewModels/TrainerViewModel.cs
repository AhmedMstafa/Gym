using Gym.DataAccess.Enums;

namespace Gym.Business.ViewModels;

public class TrainerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Speciality Speciality { get; set; }
}
