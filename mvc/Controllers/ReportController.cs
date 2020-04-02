using System;
using System.Collections.Generic;
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

        public IActionResult Index()
        {
            ViewBag.Title = "Report Register";

            var model = new ReportRegisterViewModel();
            model.Reports = _reportRepository.GetAllReports().OrderByDescending(r => r.DateOfReport);
            model.TotalReports = model.Reports.Count();

            return View(model);
        }
    }
}