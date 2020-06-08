using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace mvc.Models
{
    public class InitialDistributor
    {
        private readonly ILogger<InitialDistributor> _logger;
        private readonly IConfiguration _config;

        public InitialDistributor(ILogger<InitialDistributor> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        public void InjectRoles(RoleManager<IdentityRole> roleManager)
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    string[] roles = new string[] { "Reporter", "Investigator", "Admin" };
                    foreach (string role in roles)
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).Wait();
                        _logger.LogInformation("Role {role} injected in NEMESYS", role);
                    }
                }
                else
                {
                    _logger.LogWarning("Roles not injected, since roles are already present in NEMESYS");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when injecting roles in NEMESYS.");
            }
        }

        public void InjectUsers(UserManager<ApplicationUser> userManager)
        {

            try
            {
                if (!userManager.Users.Any())
                {
                    var admin = new ApplicationUser()
                    {
                        Email = "admin@nomail.com",
                        UserName = "admin@nomail.com",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                    };

                    //Add to store
                    //getting password from appsettings

                    IdentityResult result = userManager.CreateAsync(admin, _config.GetSection("Credentials")["AdminPass"]).Result;
                    if (result.Succeeded)
                    {
                        //Add to role
                        userManager.AddToRoleAsync(admin, "Admin").Wait();
                        _logger.LogInformation("Added Investigator admin@nomail.com to users");
                    }


                    var inv = new ApplicationUser()
                    {
                        Email = "investigator@nomail.com",
                        UserName = "investigator@nomail.com",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D") //to track important profile updates (e.g. password change)
                    };

                    //Add to store
                    //getting password from appsettings
                    result = userManager.CreateAsync(inv, _config.GetSection("Credentials")["InvestigatorPass"]).Result;
                    if (result.Succeeded)
                    {
                        //Add to role
                        userManager.AddToRoleAsync(inv, "Investigator").Wait();
                        _logger.LogInformation("Added Investigator investigator@nomail.com to users");
                    }
                }
                else
                {
                    _logger.LogWarning("Users not injected, since users are already present in NEMESYS");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when injecting users in NEMESYS.");
            }
        }


        public void InjectData(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            try
            {
                if (!context.Reports.Any())
                {
                    var user = userManager.GetUsersInRoleAsync("Investigator").Result.FirstOrDefault();

                    context.AddRange(
                        new Report()
                        {
                            HazardLocation = "COVID-19 present in Malta",
                            HazardDate = DateTime.Today,
                            DateOfReport = DateTime.Today,
                            HazardDescription = "Corona Cerveza halted in supply due to COVID-19 pandemic",
                            ReporterEmail = user.Email,
                            ReporterPhone = user.PhoneNumber,
                            HazardType = "Unsafe Act",
                            ImageUrl = "/images/corona.jpg",
                            User = user

                        }
                    );

                    context.SaveChanges();
                    _logger.LogInformation("Report injected in NEMESYS");
                }
                else
                {
                    _logger.LogWarning("Reports not injected, since reports are already present in NEMESYS");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when injecting reports in NEMESYS.");
            }
        }
    }
}
