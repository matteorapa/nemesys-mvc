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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }       

        [HttpGet]
        public IActionResult Details(int id)
        {
            var inv = _investigationRepository.GetInvestigationById(id);

            if (inv == null)
            {
                var model = new NewInvestigationViewModel();
                model.ReportId = id;
                return View("Views/Investigation/Index.cshtml", model);
            }
            else
                return View(inv);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View();
        }
    }
}