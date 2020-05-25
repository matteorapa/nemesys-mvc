using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels;
using mvc.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signinManager = signInManager;
            _userManager = userManager;
        }

     

        [Authorize(Roles = "Investigator")]
        //[HttpPost]
        public async Task<IActionResult> Promote(string id)
        {
            ApplicationUser myUser = await _userManager.GetUserAsync(User);

            ApplicationUser currentUser = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(currentUser);

            if (userRole.Contains("Reporter"))
            {
                //promote
                await _userManager.RemoveFromRoleAsync(currentUser, "Reporter");
                await _userManager.AddToRoleAsync(currentUser, "Investigator");

                if (myUser == currentUser)
                    return View("Views/Home/Index.cshtml");
                return RedirectToAction("ManageAccounts");

            }
            else
            {
                //prevent promoting a investigator
                return View("Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Roles = "Investigator")]
        //[HttpPost]
        public async Task<IActionResult> Demote(string id)
        {
            ApplicationUser myUser = await _userManager.GetUserAsync(User);

            ApplicationUser currentUser = await _userManager.FindByIdAsync(id);
            var userRole = await _userManager.GetRolesAsync(currentUser);

            if (userRole.Contains("Investigator"))
            {
                //demote
                await _userManager.RemoveFromRoleAsync(currentUser, "Investigator");
                await _userManager.AddToRoleAsync(currentUser, "Reporter");

                if (myUser == currentUser)
                    return View("Views/Home/Index.cshtml");
                return RedirectToAction("ManageAccounts");
            }
            else
            {
                //prevent demoting a reporter
                return View("Views/Shared/Error.cshtml");
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

        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            var model = new ManageViewModel();

            //clean search input ...
            ViewBag.Title = "Results for " + search;

            var Investigators = await _userManager.GetUsersInRoleAsync("Investigator");
            var Reporters = await _userManager.GetUsersInRoleAsync("Reporter");
            var user = await _userManager.FindByNameAsync(search);

            List<ApplicationUser> userList = new List<ApplicationUser>();

            if (Investigators.Contains(user)){
                userList.Add(user);
                model.Investigators = userList;
            }
            else if (Reporters.Contains(user)){
                userList.Add(user);
                model.Reporters = userList;
            } 


            return View("Views/Account/ManageAccounts.cshtml", model);
        }




    }
}
