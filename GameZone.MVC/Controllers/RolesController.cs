using GameZone.Core.Models;
using GameZone.Services.ViewModels.RolesVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace GameZone.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        [ActivatorUtilitiesConstructor]
        public RolesController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IToastNotification toastNotification)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

            var rolesVM = new List<RoleDetails>();

            foreach (var role in roles)
            {
                var totalUsers = (await _userManager.GetUsersInRoleAsync(role.Name!)).Count;
                rolesVM.Add(new RoleDetails
                {
                    Id = role.Id,
                    Name = role.Name!,
                    TotalUsers = totalUsers
                });
            }

            return View(rolesVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = model.Name
                };

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _toastNotification.AddSuccessToastMessage("A new Role Created Successfully !!");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var item in result.Errors)
                    ModelState.AddModelError(string.Empty, item.Description);
            }
            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
                return NotFound();

            var res = await _roleManager.DeleteAsync(role);

            if (res.Succeeded)
            {
                _toastNotification.AddErrorToastMessage("You Delete the Role Successfully !!");
                return Ok(role);
            }

            return BadRequest("Can't delete this role !!");
        }
    }
}
