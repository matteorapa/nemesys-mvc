using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels;
using Microsoft.Extensions.Logging;
using mvc.Models;
using System.Security.Claims;

namespace mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signinManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null)
            {
                var result = await _signinManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Username or password not correct");
            return View(loginViewModel);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = registerViewModel.Username,
                    PhoneNumber = registerViewModel.PhoneNumber,
                    Email = registerViewModel.Email
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Reporter");
                    return RedirectToAction("Login", "Account");
                }

                //If registration errors exist, show the first error one on the list
                ModelState.AddModelError("", result.Errors.First().Description);
            }

            return View(registerViewModel);
        }

        [HttpPut]
        public async Task<IActionResult> Promote()
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityUser idenUser = await _userManager.FindByIdAsync(currentUser);

            var userRole = await _userManager.GetRolesAsync(idenUser);

            if (userRole.Contains("Reporter"))
            {
                await _userManager.RemoveFromRoleAsync(idenUser, "Reporter");
                await _userManager.AddToRoleAsync(idenUser, "Investigator");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Views/Report/Error.cshtml"); //TO CHANGE
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }


    }
}
