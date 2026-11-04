using Gym.Business.ViewModels.Sessions;

namespace Gym.Business.ViewModels
{
    public class  CategoryViewModel : BaseEntityViewModel
    {
        public string Name { get; set; } = default!;

        public List<SessionIndexViewModel> Sessions { get; set; } = new List<SessionIndexViewModel>();
    }
}
