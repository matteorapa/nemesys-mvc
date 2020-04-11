using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels;

namespace mvc.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
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

        [HttpGet]
        public IActionResult Details(int id)
        {
            var rep = _reportRepository.GetReportById(id);
            if (rep == null)
                return NotFound();
            else
                return View(rep);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("HazardLocation", "HazardDate", "HazardType", "HazardDescription", "Image")] EditReport newReport){
            
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

                Report report = new Report()
                {
                    HazardLocation = newReport.HazardLocation,
                    HazardDescription = newReport.HazardDescription,
                    HazardDate = newReport.HazardDate,
                    DateOfReport = DateTime.Now,
                    HazardType = newReport.HazardType,
                    ImageUrl = "/images/reports/" + fileName,
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


    }
}