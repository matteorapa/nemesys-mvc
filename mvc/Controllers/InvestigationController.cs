using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels;

namespace mvc.Controllers
{
    public class InvestigationController : Controller
    {
        private readonly IInvestigationRepository _investigationRepository;

        public InvestigationController(IInvestigationRepository investigationRepository)
        {
            _investigationRepository = investigationRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int ReportId)
        {
            var inv = _investigationRepository.GetInvestigationById(ReportId);
            
            if (inv == null)
                return NotFound();
            else
                return View(inv);
        }
    }
}