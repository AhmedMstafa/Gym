using Gym.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Presentation.Controllers
{
   
    public class SessionsController(ISessionService sessionService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await sessionService.GetIndexItemsAsync(ct));
        }
    }
}
