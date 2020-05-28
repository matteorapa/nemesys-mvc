using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Microsoft.Extensions.Logging;

namespace mvc.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUpvoteRepository _upvoteRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReportController> _logger;


        //constructor for Report Controller
        public ReportController(IReportRepository reportRepository, IUpvoteRepository upvoteRepository, IApplicationUserRepository applicationUserRepository, UserManager<ApplicationUser> userManager, ILogger<ReportController> logger)
        {
            _userManager = userManager;
            //using repos to get relevant data from DB
            _reportRepository = reportRepository;
            _upvoteRepository = upvoteRepository;

            _applicationUserRepository = applicationUserRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Report Index Screen accessed");
            ViewBag.Title = "Report Register";

            var model = new ReportRegisterViewModel();
            try
            {
                model.Reports = _reportRepository.GetAllReports().OrderByDescending(r => r.DateOfReport);
                model.TotalReports = model.Reports.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue when retrieving reports");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserIndex()
        {
            _logger.LogInformation("Users' Reports Index Screen accessed");

            //obtaining current user
            //var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser idenUser = await _userManager.GetUserAsync(User); //FindByIdAsync(currentUser);

            ViewBag.Title = "My Reports";

            var model = new ReportRegisterViewModel();
            try
            {
                //passing Reports for user and count of reports to ReportRegisterViewModel in ReportModel.cs
                model.Reports = _reportRepository.GetUserReports(idenUser).OrderByDescending(r => r.DateOfReport);
                model.TotalReports = model.Reports.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue when retrieving reports for user {user}", idenUser.UserName);
            }

            return View("Views/Report/Index.cshtml", model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                var rep = _reportRepository.GetReportById(id);
                if (rep == null)
                {
                    _logger.LogWarning("No reports found");
                    return NotFound();
                    }
                else
                    return View(rep);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue when retrieving details for report {id}", id);
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("HazardLocation", "HazardDate", "HazardType", "HazardDescription", "Image", "LatitudeMarker", "LongitudeMarker")] EditReport newReport)
        {
            if (ModelState.IsValid){
                try
                {
                    string fileName = "";
                    //image handling
                    if (newReport.Image != null)
                    {
                        var img = newReport.Image;
                        var extension = "." + img.FileName.Split('.')[img.FileName.Split('.').Length - 1];

                        //assigning guid to image to not have duplicate names
                        fileName = Guid.NewGuid().ToString() + extension;
                        //path for storing image
                        var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                        using (var bits = new FileStream(path, FileMode.Create))
                        {
                            newReport.Image.CopyTo(bits);
                        }
                    }

                    //obtaining current user
                    var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

                    //creating new report
                    Report report = new Report()
                    {
                        User = idenUser,
                        HazardLocation = newReport.HazardLocation,
                        LatitudeMarker = newReport.LatitudeMarker,
                        LongitudeMarker = newReport.LongitudeMarker,
                        HazardDescription = newReport.HazardDescription,
                        HazardDate = newReport.HazardDate,
                        DateOfReport = DateTime.Now,
                        HazardType = newReport.HazardType,
                        ImageUrl = "/images/reports/" + fileName,
                        ReporterEmail = idenUser.Email,
                        ReporterPhone = idenUser.PhoneNumber,
                        ReportStatus = "Open"

                    };

                    _reportRepository.CreateReport(report); //using CreateReport method in ReportRepo to create the report above
                    _logger.LogInformation("Report {report} created", report.ReportId);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Issue experienced when creating report");
                    return this.View(newReport);
                }
            }
            else
            {
                _logger.LogWarning("Model State invalid for creating report.");
                return this.View(newReport);
            }
            return this.View(newReport);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var rep = _reportRepository.GetReportById(id);

                //obtaining current user
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

                //if report belongs to user logged in - return report
                if (rep.User == idenUser)
                {
                    EditReport editRep = new EditReport();
                    editRep.HazardLocation = rep.HazardLocation;
                    editRep.HazardDate = rep.HazardDate;
                    editRep.HazardType = rep.HazardType;
                    editRep.HazardDescription = rep.HazardDescription;
                    editRep.ImageUrl = rep.ImageUrl;
                    editRep.LatitudeMarker = rep.LatitudeMarker;
                    editRep.LongitudeMarker = rep.LongitudeMarker;

                    return View(editRep);
                }
                else //report does not belong to user
                {
                    _logger.LogWarning("Unauthorized to edit report");
                    return LocalRedirect("/Identity/Account/AccessDenied");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when trying to edit report");
            }
            return LocalRedirect("/Identity/Account/AccessDenied");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit([Bind("HazardLocation", "HazardDate", "HazardType", "HazardDescription", "ImageUrl", "Image", "LatitudeMarker", "LongitudeMarker")] EditReport thisReport, int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string fileName = "";
                    if (thisReport.Image != null)
                    {
                        //image handling
                        var img = thisReport.Image;
                        var extension = "." + img.FileName.Split('.')[img.FileName.Split('.').Length - 1];

                        fileName = Guid.NewGuid().ToString() + extension;
                        var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                        using (var bits = new FileStream(path, FileMode.Create))
                        {
                            thisReport.Image.CopyTo(bits);
                        }

                        thisReport.ImageUrl = "/images/reports/" + fileName;
                    }

                    Report report = new Report()
                    {
                        HazardLocation = thisReport.HazardLocation,
                        LatitudeMarker = thisReport.LatitudeMarker,
                        LongitudeMarker = thisReport.LongitudeMarker,
                        HazardDescription = thisReport.HazardDescription,
                        HazardDate = thisReport.HazardDate,
                        DateOfReport = DateTime.Now,
                        HazardType = thisReport.HazardType,
                        ImageUrl = thisReport.ImageUrl
                    };

                    _reportRepository.EditReportById(id, report); //editing report having the same id with details from the report above
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Issue experienced when trying to make changes to report");
                }
            }
            else
            {
                _logger.LogWarning("Model State invalid for editing report.");
                return this.View(thisReport);
            }
            return this.View(thisReport);
        }

        [HttpGet]
        public IActionResult Search(string search)
        {
            var model = new ReportRegisterViewModel();

            //clean search input ...
            ViewBag.Title = "Results for " + search;

            if (search != null)  
            {
                //getting reports which contain string being searched
                model.Reports = _reportRepository.GetAllReports().Where(x => x.HazardLocation.Contains(search, StringComparison.OrdinalIgnoreCase));
                model.TotalReports = model.Reports.Count();
                _logger.LogInformation("{count} reports found when searching.", model.Reports.Count());
            }
            else //blank search provided
            {
                model.TotalReports = 0;
                _logger.LogWarning("Blank Search inputted");
            }

            return View("Views/Report/Index.cshtml", model);
        }

        public async Task<IActionResult> Upvote(int id)
        {
            try
            {
                var r = _reportRepository.GetReportById(id);

                //obtaining current user
                ApplicationUser idenUser = await _userManager.GetUserAsync(User); 

                var existingU = _upvoteRepository.GetUserUpvote(idenUser, r);

                if (existingU == null) //no upvotes exist from logged in user for a particular report being upvoted
                {
                    Upvote u = new Upvote()
                    {
                        Report = r,
                        User = idenUser
                    };

                    _upvoteRepository.CreateUpvote(u);
                    _logger.LogInformation("Upvote created by {user} for report {report}", idenUser.UserName, id);
                    return RedirectToAction("Index");
                }

                else
                {
                    _logger.LogWarning("User trying to upvote when upvote already exists!");
                    return View("Views/Shared/Error.cshtml");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when trying to obtatin upvotes for report");
            }

            return RedirectToAction("Index"); 
        }

        public async Task<IActionResult> Downvote(int id)
        {
            try
            {
                var r = _reportRepository.GetReportById(id);

                //obtaining current user
                ApplicationUser idenUser = await _userManager.GetUserAsync(User);

                var existingU = _upvoteRepository.GetUserUpvote(idenUser, r);

                if (existingU != null) //if upvote exists for report
                {
                    _upvoteRepository.DeleteUpvote(existingU);

                    _logger.LogInformation("Upvote removed by {user} for report {report}", idenUser.UserName, id);
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.LogWarning("User trying to downvote when upvote doesn't exist!");
                    return View("Views/Report/Error.cshtml");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when trying to obtatin upvotes for report");
            }

            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> HallOfFame()
        {

            ViewBag.Title = "Hall of Fame";
            var model = new HallFameModel();

            try
            {
                //getting users in Nemesys for the Hall of Fame
                model.Reporters = _applicationUserRepository.GetAllUsers();
                model.TotalReporters = _applicationUserRepository.GetAllUsers().Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue experienced when trying to load data for Hall of Fame");
            }

            return View(model);

        }
    }
}