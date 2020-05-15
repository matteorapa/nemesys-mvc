using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUpvoteRepository _upvoteRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(IReportRepository reportRepository, IUpvoteRepository upvoteRepository, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _reportRepository = reportRepository;
            _upvoteRepository = upvoteRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "Report Register";

            var model = new ReportRegisterViewModel();
            model.Reports = _reportRepository.GetAllReports().OrderByDescending(r => r.DateOfReport);
            model.TotalReports = model.Reports.Count();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserIndex()
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

            ViewBag.Title = "Report Register";

            var model = new ReportRegisterViewModel();
            model.Reports = _reportRepository.GetUserReports(idenUser).OrderByDescending(r => r.DateOfReport);
            model.TotalReports = model.Reports.Count();

            return View("Views/Report/Index.cshtml", model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var rep = _reportRepository.GetReportById(id);
            if (rep == null)
                return NotFound();
            else
                return View(rep);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("HazardLocation", "HazardDate", "HazardType", "HazardDescription", "Image")] EditReport newReport){
            
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (newReport.Image != null)
                {
                    var img = newReport.Image;
                    var extension = "." + img.FileName.Split('.')[img.FileName.Split('.').Length - 1];                    

                    fileName = Guid.NewGuid().ToString() + extension;
                    var path = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\reports\\" + fileName;
                    using (var bits = new FileStream(path, FileMode.Create))
                    {
                        newReport.Image.CopyTo(bits);
                    }
                }

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);
                
                Report report = new Report()
                {
                    User = idenUser,
                    HazardLocation = newReport.HazardLocation,
                    HazardDescription = newReport.HazardDescription,
                    HazardDate = newReport.HazardDate,
                    DateOfReport = DateTime.Now,
                    HazardType = newReport.HazardType,
                    ImageUrl = "/images/reports/" + fileName,
                    ReporterEmail = idenUser.Email,
                    ReporterPhone = idenUser.PhoneNumber,
                    ReportStatus = "Open"
                };

                _reportRepository.CreateReport(report);
                return RedirectToAction("Index");
            }
            else
            {
                return this.View(newReport);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rep = _reportRepository.GetReportById(id);

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

            if (rep.User == idenUser)
            {
                EditReport editRep = new EditReport();
                editRep.HazardLocation = rep.HazardLocation;
                editRep.HazardDate = rep.HazardDate;
                editRep.HazardType = rep.HazardType;
                editRep.HazardDescription = rep.HazardDescription;
                editRep.ImageUrl = rep.ImageUrl;

                return View(editRep);
            }
            else
            {
                return View("Views/Report/Error.cshtml");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit([Bind("HazardLocation", "HazardDate", "HazardType", "HazardDescription", "ImageUrl", "Image")] EditReport thisReport, int id)
        {

            if (ModelState.IsValid)
            {
                string fileName = "";
                if (thisReport.Image != null)
                {
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
                    HazardDescription = thisReport.HazardDescription,
                    HazardDate = thisReport.HazardDate,
                    DateOfReport = DateTime.Now,
                    HazardType = thisReport.HazardType,
                    ImageUrl = thisReport.ImageUrl
                };

                _reportRepository.EditReportById(id, report);
                return RedirectToAction("Index");
            }
            else
            {
                return this.View(thisReport);
            }
        }

        [HttpGet]
        public IActionResult Search(string search)
        {   
            var model = new ReportRegisterViewModel();

            //clean search input ...
            ViewBag.Title = "Results for " + search;
            
            
            model.Reports = _reportRepository.GetAllReports().Where(x => x.HazardLocation.Contains(search, StringComparison.OrdinalIgnoreCase));
            model.TotalReports = model.Reports.Count();

            return View(model);
        }

        public async Task<IActionResult> Upvote(int id)
        {
            var r = _reportRepository.GetReportById(id);

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

            var existingU = _upvoteRepository.GetUserUpvote(idenUser, r);

            if (existingU == null)
            {
                Upvote u = new Upvote()
                {
                    Report = r,
                    User = idenUser
                };

                _upvoteRepository.CreateUpvote(u);

                return RedirectToAction("Index");
            }

            else
            {
                return View("Views/Report/Error.cshtml");
            }
        }

        public async Task<IActionResult> Downvote(int id)
        {
            var r = _reportRepository.GetReportById(id);

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

            var existingU = _upvoteRepository.GetUserUpvote(idenUser, r);

            if (existingU != null)
            {

                _upvoteRepository.DeleteUpvote(existingU);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Views/Report/Error.cshtml");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult HallOfFame()
        {
            ViewBag.Title = "Hall of Fame";


            return View();
        }




    }
}