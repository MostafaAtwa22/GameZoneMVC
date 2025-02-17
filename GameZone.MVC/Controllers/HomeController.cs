using System.Diagnostics;
using GameZone.MVC.Models;
using GameZone.Services.Services.GamesServices;
using Microsoft.AspNetCore.Mvc;

namespace GameZone.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGamesServices _gamesServices;

        public HomeController(ILogger<HomeController> logger, 
            IGamesServices gamesServices)
        {
            _logger = logger;
            _gamesServices = gamesServices;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _gamesServices.GetAll();
            return View(games);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
