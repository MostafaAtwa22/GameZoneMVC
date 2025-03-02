using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;

namespace GameZone.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        [ActivatorUtilitiesConstructor]
        public UsersController(ApplicationDbContext context,
            IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var claminIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string? userId = claim?.Value;

            return View(_context.ApplicationUsers
                .Where(u => u.Id != userId) // get all the users except the admin
                .ToList());
        }

        [HttpGet]
        public IActionResult LockUnLock(string? id)
        {
            var user = _context.ApplicationUsers
                .FirstOrDefault(u => u.Id == id);

            if (user is null)
                return NotFound();

            if (user.LockoutEnd is null || user.LockoutEnd < DateTime.Now)
            {
                _toastNotification.AddErrorToastMessage("The Account is Locked !!");
                user.LockoutEnd = DateTime.Now.AddMonths(1);
            }
            else
            {
                _toastNotification.AddSuccessToastMessage("You Release the Lock Successfully !!");
                user.LockoutEnd = DateTime.Now;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            _toastNotification.AddErrorToastMessage("The user account Deleted Successfully !!");
            _context.ApplicationUsers.Remove(user);
            _context.SaveChanges();

            return Json(new { success = true, message = "User deleted successfully." });
        }
    }
}
