using GameZone.Core.Models;
using GameZone.Services.Services.ManageServices;
using GameZone.Services.ViewModels.ManagesVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NToastNotify;
using System.Security.Claims;

namespace GameZone.MVC.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly IManageServices _manageServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        [ActivatorUtilitiesConstructor]
        public ManageController(IManageServices manageServices,
            UserManager<ApplicationUser> userManager,
            IToastNotification toastNotification)
        {
            _manageServices = manageServices;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claminIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string? userId = claim?.Value;

            var user = await _manageServices.GetById(userId);

            if (user is null)
                return NotFound();

            var model = new UpdateProfileVM
            {
                Id = user.Id,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                CurrentCover = user.Cover
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProfileVM appUser)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), appUser);
            var user = await _manageServices.Update(appUser);

            if (user is null)
                return BadRequest("The User Name is already exists !!");

            _toastNotification.AddSuccessToastMessage("The Profile Updated Successfully !!");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Email()
        {
            var claminIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string? userId = claim?.Value;

            var user = await _manageServices.GetById(userId);

            if (user is null)
                return NotFound();

            var model = new EmailVM
            {
                Email = user.Email!
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Email(EmailVM model)
        {
            if (ModelState.IsValid)
            {
                var claminIdentity = (ClaimsIdentity)User.Identity!;
                var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

                string? userId = claim?.Value;
                var user = await _manageServices.GetById(userId);

                if (user is null)
                    return NotFound();

                var email = await _manageServices.Email(model, user.Email!);

                if (email is not null)
                {
                    _toastNotification.AddSuccessToastMessage("The Email Updated Successfully !!");
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "This email is already used !!");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var claminIdentity = (ClaimsIdentity)User.Identity!;
                var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

                string? userId = claim?.Value;

                var result = await _manageServices.ChangePassword(userId!, model);
                if (result)
                {
                    _toastNotification.AddSuccessToastMessage("The Password Updated Successfully !!");
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "The old password is incorrect !!");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PersonalData()
        {
            var claminIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string? userId = claim?.Value;

            var user = await _manageServices.GetById(userId);

            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'.");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _manageServices.DownloadPersonalData(userId);

            if (result is null)
                return NotFound("User not found.");

            _toastNotification.AddSuccessToastMessage("The Personal Data Downloaded Successfully !!");
            return File(result.Value.FileContent, result.Value.ContentType, result.Value.FileName);
        }

        [HttpGet]
        public IActionResult DeletePersonalData()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePersonalData(DeleteDataVM model)
        {
            if (ModelState.IsValid)
            {
                var claminIdentity = (ClaimsIdentity)User.Identity!;
                var claim = claminIdentity.FindFirst(ClaimTypes.NameIdentifier);

                string? userId = claim?.Value;

                model.UserId = userId!;
                var result = await _manageServices.DeletePersonData(model);

                if (result)
                {
                    _toastNotification.AddErrorToastMessage("Your Account has been Deleted Successfully !!");
                    return RedirectToAction(nameof(Index), "Home");
                }

                ModelState.AddModelError(string.Empty, "The Password is incorrect !!");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                _toastNotification.AddErrorToastMessage("You Already have a password !!");
                return RedirectToAction(nameof(Index), "Home");
            }

            return View(new SetPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return RedirectToAction(nameof(Login));

            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (result.Succeeded)
            {
                _toastNotification.AddInfoToastMessage("You Set new Password Successfully !!");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}
