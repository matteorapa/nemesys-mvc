﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;


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


        [Authorize(Roles = "Investigator")]
        [HttpPut]
        public async void Promote(string id)
        {

            IdentityUser currentUser = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(currentUser);

            if (userRole.Contains("Reporter"))
            {
                //promote
                await _userManager.RemoveFromRoleAsync(currentUser, "Reporter");
                await _userManager.AddToRoleAsync(currentUser, "Investigator");
                RedirectToAction("Views/Account/ManageAccounts.cshtml");

            }
            else
            {
                //prevent promoting a investigator
                RedirectToAction("Views/Account/Error.cshtml");
            }


        
        }

        [Authorize(Roles = "Investigator")]
        [HttpPut]
        public async void Demote(string id)
        {
           
            IdentityUser currentUser = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(currentUser);

            if (userRole.Contains("Investigator"))
            {
                //demote
                await _userManager.RemoveFromRoleAsync(currentUser, "Investigator");
                await _userManager.AddToRoleAsync(currentUser, "Reporter");

                //check if demoting self

                
                //if (loggedUser.GetUserId() == id) {
                   // Redirect("Views/Home/Index");
                //}

            }
            else
            {
                //prevent demoting a reporter
                RedirectToAction("Views/Account/Error.cshtml");
            }
        }


        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> ManageAccounts()
        {
           
            ViewBag.Title = "Manage Accounts";
            var model = new ManageViewModel();

            model.Investigators = await _userManager.GetUsersInRoleAsync("Investigator");
            model.Reporters = await _userManager.GetUsersInRoleAsync("Reporter");
            model.TotalAccounts = _userManager.Users.Count();
        
            return View("Views/Account/ManageAccounts.cshtml", model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
         
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }


    }
}