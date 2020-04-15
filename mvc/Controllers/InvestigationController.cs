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
            ViewBag.Title = "All Investigations";
            var model = new InvestigationListViewModel();
            model.Investigations = _investigationRepository.GetAllInvestigations();
            model.TotalInvestigations = model.Investigations.Count();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create([Bind("DateOfAction", "InvestigatorEmail", "InvestigatorPhone", "InvDescription")] ValidateInvestigation vInvestigation, int id)
        {

            Investigation i = new Investigation()
            {
                DateOfAction = vInvestigation.DateOfAction,
                InvestigatorEmail = vInvestigation.InvestigatorEmail,
                InvestigatorPhone = vInvestigation.InvestigatorPhone,
                InvDescription = vInvestigation.InvDescription,
                ReportId = id,
            };

            _investigationRepository.CreateInvestigation(i);
            return RedirectToAction("Index");
            
        }

       

        [HttpGet]
        public IActionResult Details(int id)
        {
            var inv = _investigationRepository.GetInvestigationById(id);

            if (inv == null)
            {
                var model = new NewInvestigationViewModel();
                model.ReportId = id;
                return View("Views/Investigation/Details.cshtml", model);
            }
            else
                return View(inv);
        }

        [HttpPost]
        public IActionResult Edit([Bind("DateOfAction", "InvestigatorEmail", "InvestigatorPhone", "InvDescription")] EditInvestigation vInvestigation, int id)
        {
            //todo
            return RedirectToAction("Index");
        }

    }
}