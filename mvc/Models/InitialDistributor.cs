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


            //var user = new IdentityUser
            //{
            //    UserName = "Administrator",
            //    Email = "admin@nemesys.com",
            //    PhoneNumber = "21000000",
            //    EmailConfirmed = true,
            //    PhoneNumberConfirmed = true,
            //};

            //if (!context.Users.Any(u => u.UserName == user.UserName))
            //{
            //    var password = new PasswordHasher<IdentityUser>();
            //    var hashed = password.HashPassword(user, "secret");
            //    user.PasswordHash = hashed;

            //    var userStore = new UserStore<IdentityUser>(context);
            //    var result = userStore.CreateAsync(user);

            //}

            //AssignRoles(serviceProvider, user.Email, roles);
                       
            if (!context.Reports.Any())
            {
                context.AddRange(
                    new Report()
                    {
                        HazardLocation = "Ryanair Flight FR7798",
                        HazardDate = DateTime.Today,
                        DateOfReport = DateTime.Today,
                        HazardDescription = "19 year old went aboard with Corona Cerveza.",
                        ReporterEmail = "covid19@gov.mt",
                        ReporterPhone = "111",
                        Upvotes = 0,
                        HazardType = "Infected Patient",
                        ImageUrl = "/images/corona.png"

                    }
                );

                context.SaveChangesAsync();
            }
        }
    }
}
