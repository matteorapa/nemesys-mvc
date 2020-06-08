using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;

namespace mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //initialising Serilog logger to write logs to file nemesys_logs.txt
            Log.Logger = new LoggerConfiguration()
                //The package includes middleware for smarter HTTP request logging. 
                //The default request logging implemented by ASP.NET Core is noisy, with multiple events emitted per request.
                //The included middleware condenses these into a single event that carries method, path, status code, and timing information.

                //http requests logged have to be at least of log event level Warning
                //else, our logs would be too noisy
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) 
                .WriteTo.Console()
                //sinking logs to a file
                .WriteTo.File("logs\\nemesys_logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();
                Microsoft.Extensions.Logging.ILogger <InitialDistributor> logger = host.Services.GetService<ILogger<InitialDistributor>>();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        //retrieving services necessary for initial seeding
                        var context = services.GetRequiredService<AppDbContext>();
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                        IConfiguration configuration = services.GetRequiredService<IConfiguration>();


                        //injecting the initials roles, users and reports in the system, if none are present
                        InitialDistributor iDist = new InitialDistributor(logger, configuration);
                        iDist.InjectRoles(roleManager);
                        iDist.InjectUsers(userManager);
                        iDist.InjectData(userManager, context);

                    }
                    catch(InvalidOperationException ioex) //services not succeffully retrieved
                    {
                        Log.Error(ioex, "Service Type/s not found");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occured at the seeding stage");
                    }

                    host.Run();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.Information("End of logs for session");
                Log.CloseAndFlush();
            }         

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog(); //Generic host to use Serilog for logging
    }
}
