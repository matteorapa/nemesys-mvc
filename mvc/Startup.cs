using System;

using mvc.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Serilog;

namespace mvc
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public IConfiguration _configuration { get; }

        //Requesting injection of an IWebHostEnvironment object
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            //storing the environment and configuration in private variables in the constructor, to be used in Startup file
            _env = env;
            _configuration = configuration;
        }

        //Adding services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            //Adding a single instance per code request. With each request, ASP.NET will create a new instance of the service.
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddTransient<IInvestigationRepository, InvestigationRepository>();
            services.AddTransient<IUpvoteRepository, UpvoteRepository>();
            services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();

            //Configuring Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //Password policy
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                //Lockout settings
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                //User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-._@+";
                options.User.RequireUniqueEmail = true;


            }).AddEntityFrameworkStores<AppDbContext>();

            //Configuring cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                //Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true; //re issues a cookie (to extend expiry time) if new request is done before expiration
                options.LoginPath = "/Identity/Account/Login"; //default path when a login with correct credentials is required
                //default path when access to a page is denied for a user
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            //Google login configuration
            services.AddAuthentication().AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = _configuration.GetSection("Authentication:Google");

                options.ClientId = _configuration["Authentication:Google:ClientId"];
                options.ClientSecret = _configuration["Authentication:Google:ClientSecret"];
            });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            //collection of repos will work on db provided by connection string
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc();
            services.AddRazorPages();

        }

        // Configuring the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseStaticFiles();

            //enabling middleware for request logging (after the components above, such as static files, are loade
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                   //default route in endpoints in domain not provided: nemesys.com/Home/Index

                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
