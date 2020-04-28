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
        private readonly UserManager<IdentityUser> _userManager;

        public ReportController(IReportRepository reportRepository, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _reportRepository = reportRepository;
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
            IdentityUser idenUser = await _userManager.FindByIdAsync(currentUser);

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

        [Authorize(Roles ="Reporter")]
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
                IdentityUser idenUser = await _userManager.FindByIdAsync(currentUser);
                
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
                    Upvotes = 0,
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
            IdentityUser idenUser = await _userManager.FindByIdAsync(currentUser);

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
        public IActionResult Edit([Bind("HazardLocation", "HazardDate", "HazardType", "HazardDescription", "Image")] EditReport thisReport, int id)
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
                }

                Report report = new Report()
                {
                    HazardLocation = thisReport.HazardLocation,
                    HazardDescription = thisReport.HazardDescription,
                    HazardDate = thisReport.HazardDate,
                    DateOfReport = DateTime.Now,
                    HazardType = thisReport.HazardType,
                    ImageUrl = "/images/reports/" + fileName,
                    Upvotes = 0,
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




    }
}