using GameZone.Core.Models;
using GameZone.Services.Services.EmailServices;
using GameZone.Services.ViewModels.AccountVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GameZone.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSettings _emailSettings;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IEmailSettings emailSettings,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSettings = emailSettings;
            _roleManager = roleManager;
        }

        private async Task PopulateRoles(RegisterUserMV viewModel)
        {
            viewModel.RolesList = await _roleManager.Roles
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new RegisterUserMV
            {
                RolesList = await _roleManager.Roles
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    })
                    .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserMV viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRoles(viewModel);
                return View(viewModel);
            }

            if (await _userManager.FindByNameAsync(viewModel.UserName) != null)
                ModelState.AddModelError(string.Empty, "This User Name Already Exists !!");

            if (await _userManager.FindByEmailAsync(viewModel.Email) != null)
                ModelState.AddModelError(string.Empty, "This Email Already Exists !!");

            if (!ModelState.IsValid)
            {
                await PopulateRoles(viewModel);
                return View(viewModel);
            }

            var user = new ApplicationUser
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber,
                Country = viewModel.Country,
                City = viewModel.City
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                await PopulateRoles(viewModel);
                return View(viewModel);
            }

            var role = await _roleManager.FindByIdAsync(viewModel.RoleId!) ?? new IdentityRole("User");
            await _userManager.AddToRoleAsync(user, role.Name!);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var user = await _userManager.FindByEmailAsync(viewModel.UserNameOrEmail)
                       ?? await _userManager.FindByNameAsync(viewModel.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password!");
                return View(viewModel);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Your account is locked. Try again later.");
                return View(viewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, 
                viewModel.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                List<Claim> claims = new List<Claim>
                {
                    new Claim("Cover", user.Cover!),
                    new Claim("Country", user.Country),
                    new Claim("City", user.City)
                };

                await _signInManager.SignInWithClaimsAsync(user, viewModel.RememberMe, claims);
                return RedirectToAction(nameof(Index), "Home");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Your account has been locked due to multiple failed attempts. Try again later.");
            else
                ModelState.AddModelError(string.Empty, "Invalid username or password!");

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(ForgetPasswordVM viewModel)
        {
            if (ModelState.IsValid)
            {
                // check if the email is on the database or not
                var user = await _userManager.FindByEmailAsync(viewModel.Email);

                if (user is not null)
                {
                    // create token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // http://localhost:port/Account/ResetPassword?email
                    var passResetPasswordLink = Url.Action(nameof(ResetPassword), 
                        "Account",
                        new {email = viewModel.Email, token = token},
                        _configuration["ResetURL:Protocol"],
                        _configuration["ResetURL:LocalHost"]);

                    var email = new Email
                    {
                        Subject = "Reset Password",
                        To = user.Email!,
                        Body = passResetPasswordLink!
                    };

                    // call service send Email
                    _emailSettings.SendEmail(email);

                    return RedirectToAction(nameof(CheckYourInbox));
                }
                ModelState.AddModelError(string.Empty, "Email is not Exists !!");
            }

            return View(nameof(ForgetPassword), viewModel);
        }

        [HttpGet]
        public IActionResult CheckYourInbox()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["email"]!.ToString();
                var token = TempData["token"]!.ToString();

                var user = await _userManager.FindByEmailAsync(email!);

                var result = await _userManager.ResetPasswordAsync(user!, token!, model.Password);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Login));

                ModelState.AddModelError(string.Empty, "We couldn't reset your Password !!"); 
            }
            return View(model);
        }
    }
}
