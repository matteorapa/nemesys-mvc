using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels;

namespace mvc.Controllers
{
    public class InvestigationController : Controller
    {
        private readonly IInvestigationRepository _investigationRepository;
        private readonly IReportRepository _reportRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvestigationController(IInvestigationRepository investigationRepository, IReportRepository reportRepository, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _investigationRepository = investigationRepository;
            _reportRepository = reportRepository;
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Title = "All Investigations";
            var model = new InvestigationListViewModel();
            model.Investigations = _investigationRepository.GetAllInvestigations();
            model.TotalInvestigations = model.Investigations.Count();
            return View(model);
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Investigator")]
        [HttpPost]
        public async Task <IActionResult> Create([Bind("DateOfAction", "ReportStatus", "InvDescription")] EditInvestigation vInvestigation, int id)
        { 
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await _userManager.FindByIdAsync(currentUser);
            //todo get selected user from dropdown and set it as user

            if (ModelState.IsValid)
            {
                Investigation i = new Investigation()
                {
                    DateOfAction = vInvestigation.DateOfAction,
                    InvestigatorEmail = await _userManager.GetEmailAsync(user),
                    InvestigatorPhone = await _userManager.GetPhoneNumberAsync(user),
                    User = user,
                    InvDescription = vInvestigation.InvDescription,
                    ReportId = id                    
                };

                var report = _reportRepository.GetReportById(id);
                report.ReportStatus = vInvestigation.ReportStatus;

                _reportRepository.EditReportById(id, report);

                _investigationRepository.CreateInvestigation(i);

                return RedirectToAction("Index");
            }

            else
            {
                return this.View(vInvestigation);
            }

        }       

        [HttpGet]
        public IActionResult Details(int id)
        {
            var inv = _investigationRepository.GetInvestigationByReportId(id);

            if (inv == null)
            {
                var model = new NewInvestigationViewModel();
                model.ReportId = id;
                return View("Views/Investigation/NoInvestigation.cshtml", model);
            }
            else
                return View(inv);
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inv = _investigationRepository.GetInvestigationByReportId(id);
            var rep = _reportRepository.GetReportById(id);

            var EditInv = new EditInvestigation();
            EditInv.DateOfAction = inv.DateOfAction;
            EditInv.InvDescription = inv.InvDescription;
            EditInv.ReportStatus = rep.ReportStatus;
            EditInv.Investigators = await _userManager.GetUsersInRoleAsync("Investigator");
            return View(EditInv);
        }


        [Authorize(Roles = "Investigator")]
        [HttpPost]
        public async Task <IActionResult> Edit([Bind("DateOfAction", "InvDescription", "ReportStatus", "SelectedInvestigator")] EditInvestigation vInvestigation, int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(vInvestigation.SelectedInvestigator);
            if (ModelState.IsValid)
            {
                Investigation i = new Investigation()
                {
                    DateOfAction = vInvestigation.DateOfAction,
                    InvestigatorEmail = user.Email,
                    InvestigatorPhone = user.PhoneNumber,
                    InvDescription = vInvestigation.InvDescription,
                    ReportId = id,
                    User = user
                };

                var report = _reportRepository.GetReportById(id);
                report.ReportStatus = vInvestigation.ReportStatus;

                _reportRepository.EditReportById(id, report);

                _investigationRepository.EditInvestigation(id, i);

                //send email to new investigator

                return RedirectToAction("Index");
            }

            else
            {
                return this.View(vInvestigation);
            }
        }

       

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public IActionResult Users(ApplicationUser u)
        {
            ViewBag.Title = "All Investigations";
            var model = new InvestigationListViewModel();
            model.Investigations = _investigationRepository.GetAllInvestigations();
            model.TotalInvestigations = model.Investigations.Count();
            return View(model);
            //get list of users

        }


    }
}