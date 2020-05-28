using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.ViewModels;
using mvc.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

     

        [Authorize(Roles = "Investigator")]
        //[HttpPost]
        public async Task<IActionResult> Promote(string id)
        {
            try
            {
                ApplicationUser myUser = await _userManager.GetUserAsync(User);
                ApplicationUser currentUser = await _userManager.FindByIdAsync(id);

                var userRole = await _userManager.GetRolesAsync(currentUser);

                if (userRole.Contains("Reporter"))
                {
                    try
                    {
                        //promote
                        await _userManager.RemoveFromRoleAsync(currentUser, "Reporter");
                        await _userManager.AddToRoleAsync(currentUser, "Investigator");
                        _logger.LogInformation("User promoted successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,"Error when promoting user.");
                    }
                    if (myUser == currentUser)
                        return View("Views/Home/Index.cshtml");
                    return RedirectToAction("ManageAccounts");

                }
                else
                {
                    _logger.LogWarning("Error promoting user due to user's role not being Reporter");
                    //prevent promoting an investigator
                    return View("Views/Shared/Error.cshtml");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when obtaining user candidate details.");
                return View("Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Roles = "Investigator")]
        //[HttpPost]
        public async Task<IActionResult> Demote(string id)
        {
            try
            {
                ApplicationUser myUser = await _userManager.GetUserAsync(User);

                ApplicationUser currentUser = await _userManager.FindByIdAsync(id);
                var userRole = await _userManager.GetRolesAsync(currentUser);

                if (userRole.Contains("Investigator"))
                {
                    try
                    {
                        //demote
                        await _userManager.RemoveFromRoleAsync(currentUser, "Investigator");
                        await _userManager.AddToRoleAsync(currentUser, "Reporter");
                        _logger.LogInformation("User demoted successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error when demoting user.");
                    }
                    
                    if (myUser == currentUser)
                        return View("Views/Home/Index.cshtml");
                    return RedirectToAction("ManageAccounts");
                }
                else
                {
                    _logger.LogWarning("Error demoting user due to user's role not being Investigator");
                    //prevent demoting a reporter
                    return View("Views/Shared/Error.cshtml");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when obtaining user candidate details.");
                return View("Views/Shared/Error.cshtml");
            }
        }


        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> ManageAccounts()
        {
           
            ViewBag.Title = "Manage Accounts";
            var model = new ManageViewModel();

            try
            {
                model.Investigators = await _userManager.GetUsersInRoleAsync("Investigator");
                model.Reporters = await _userManager.GetUsersInRoleAsync("Reporter");
                model.TotalAccounts = _userManager.Users.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue when getting users for Managing Accounts");
            }
        
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
