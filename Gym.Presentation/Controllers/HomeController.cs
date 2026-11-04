using Gym.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers
{
    public class HomeController(IHomeService homeService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var stats = await homeService.GetStatisticsAsync(ct);
            return View(stats);
        }
    }
}
