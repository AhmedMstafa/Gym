using System;
using System.Collections.Generic;

namespace Gym.Business.ViewModels
{
    public class CategoryViewModel : BaseEntityViewModel
    {
        public string Name { get; set; } = default!;

        public List<SessionViewModel> Sessions { get; set; } = new List<SessionViewModel>();
    }
}
