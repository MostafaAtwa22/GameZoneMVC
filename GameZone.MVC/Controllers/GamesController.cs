using GameZone.Services.Services.CategoriesServices;
using GameZone.Services.Services.DevicesServices;
using GameZone.Services.Services.GamesServices;
using GameZone.Services.ViewModels.GamesVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameZone.MVC.Controllers
{
   
    public class GamesController : Controller
    {
        private readonly IGamesServices _gamesServices;
        private readonly ICategoriesServices _categoriesServices;
        private readonly IDevicesServices _devicesServices;
        

        public GamesController(IGamesServices gamesServices, 
            ICategoriesServices categoriesServices,
            IDevicesServices devicesServices)
        {
            _gamesServices = gamesServices;
            _categoriesServices = categoriesServices;
            _devicesServices = devicesServices;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var games = await _gamesServices.GetAll();
            return View(games);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Details(int id)
        {
            var game = await _gamesServices.GetById(id);

            if (game is null)
                return NotFound();

            return View(game);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateGameVM
            {
                Categories = await _categoriesServices.CategoriesList(),
                Devices = await _devicesServices.DevicesSelectList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateGameVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoriesServices.CategoriesList();
                model.Devices = await _devicesServices.DevicesSelectList();

                return View(model);
            }
            await _gamesServices.Create(model);

            return Redirect(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var game = await _gamesServices.GetById(id);

            if (game is null)
                return NotFound();

            EditGameVM viewModel = new()
            {
                Id = id,
                Name = game.Name,
                Description = game.Description,
                CategoryId = game.CategoryId,
                SelectedDevices = game.Devices.Select(d => d.Id).ToList(),
                Categories = await _categoriesServices.CategoriesList(),
                Devices = await _devicesServices.DevicesSelectList(),
                CurrentCover = game.Cover
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(EditGameVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoriesServices.CategoriesList();
                model.Devices = await _devicesServices.DevicesSelectList();

                return View(model);
            }

            var game = await _gamesServices.Update(model);

            if (game is null)
                return BadRequest();

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await _gamesServices.Delete(id);

            if (game is null)
                return BadRequest();

            return Ok();
        }
    }
}
