using Gym.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gym.Presentation.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


     
    }
}
