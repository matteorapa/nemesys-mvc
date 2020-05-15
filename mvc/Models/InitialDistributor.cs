using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class InitialDistributor
    {

        //public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
        //{
        //    UserManager<IdentityUser> _userManager = services.GetService<UserManager<IdentityUser>>();
        //    IdentityUser user = await _userManager.FindByEmailAsync(email);
        //    var result = await _userManager.AddToRolesAsync(user, roles);

        //    return result;
        //}

        public static void InjectRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                string[] roles = new string[] { "Reporter", "Investigator", "Admin" };
                foreach (string role in roles)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }
        public static void Inject(AppDbContext context)
        {

            string[] roles = new string[] { "Reporter", "Investigator", "Admin" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    var newRole = new IdentityRole(role);
                    newRole.NormalizedName = role.ToUpper();
                    roleStore.CreateAsync(newRole);
                }
            }
        }

        public static void InjectUsers(UserManager<ApplicationUser> userManager)
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
                IdentityResult result = userManager.CreateAsync(admin, "3c4Z8e7^9W_?pfhQpK").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
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
                result = userManager.CreateAsync(inv, "VxGEwxGJ9x!qA#+3@c").Result;
                if (result.Succeeded)
                {
                    //Add to role
                    userManager.AddToRoleAsync(inv, "Investigator").Wait();
                }
            }
        }


        public static void InjectData(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            if (!context.Reports.Any())
            {
                var user = userManager.GetUsersInRoleAsync("Investigator").Result.FirstOrDefault();

                context.AddRange(
                    new Report()
                    {
                        HazardLocation = "Ryanair Flight FR7798",
                        HazardDate = DateTime.Today,
                        DateOfReport = DateTime.Today,
                        HazardDescription = "19 year old went aboard with Corona Cerveza.",
                        ReporterEmail = "covid19@gov.mt",
                        ReporterPhone = "111",
                        HazardType = "Infected Patient",
                        ImageUrl = "/images/corona.png",
                        User = user

                    }
                );

                context.SaveChanges();
            }
        }
    }
}
