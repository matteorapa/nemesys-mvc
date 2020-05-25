
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System;

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

        [HttpGet]
        public IActionResult Search(string search)
        {
            var model = new InvestigationListViewModel();

            //clean search input ...
            ViewBag.Title = "Results for " + search;


            model.Investigations = _investigationRepository.GetAllInvestigations().Where(x => x.InvDescription.Contains(search, System.StringComparison.OrdinalIgnoreCase));        
            model.TotalInvestigations = model.Investigations.Count();

       

            return View("Views/Investigation/Index.cshtml", model);
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> UserIndex()
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser idenUser = await _userManager.FindByIdAsync(currentUser);

            ViewBag.Title = "My Investigations";

            var model = new InvestigationListViewModel();
            model.Investigations = _investigationRepository.GetUserInvestigations(idenUser).OrderByDescending(i => i.DateOfAction);
            model.TotalInvestigations = model.Investigations.Count();
            return View("Views/Investigation/Index.cshtml", model);
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Investigator")]
        [HttpPost]
        public async Task <IActionResult> Create([Bind("DateOfAction", "ReportStatus", "InvDescription")] CreateInvestigationViewModel investigation, int id)
        { 
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await _userManager.FindByIdAsync(currentUser);
            //todo get selected user from dropdown and set it as user

            if (ModelState.IsValid)
            {
                Investigation i = new Investigation()
                {
                    DateOfAction = investigation.DateOfAction,
                    InvestigatorEmail = await _userManager.GetEmailAsync(user),
                    InvestigatorPhone = await _userManager.GetPhoneNumberAsync(user),
                    User = user,
                    InvDescription = investigation.InvDescription,
                    ReportId = id                    
                };

                var report = _reportRepository.GetReportById(id);
                report.ReportStatus = investigation.ReportStatus;

                _reportRepository.EditReportById(id, report);

                _investigationRepository.CreateInvestigation(i);

                return RedirectToAction("Index");
            }

            else
            {
                return this.View(investigation);
            }

        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var Investigation = _investigationRepository.GetInvestigationById(id);

            if (Investigation == null)
            {
                var model = new NewInvestigationViewModel();
                model.ReportId = id;
                return View("Views/Investigation/NoInvestigation.cshtml", model);
            }
            else
            {
                return View(Investigation);
            }
                
        }

        [Authorize(Roles = "Investigator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inv = _investigationRepository.GetInvestigationByReportId(id);
            var rep = _reportRepository.GetReportById(id);

            var Investigation = new EditInvestigationViewModel();
            Investigation.DateOfAction = inv.DateOfAction;
            Investigation.InvDescription = inv.InvDescription;
            Investigation.ReportStatus = rep.ReportStatus;
            Investigation.SelectedInvestigator = inv.User;
            Investigation.Investigators = await _userManager.GetUsersInRoleAsync("Investigator"); ;

            

            return View(Investigation);
        }


        [Authorize(Roles = "Investigator")]
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("DateOfAction", "InvDescription", "ReportStatus", "SelectedInvestigator")] EditInvestigationViewModel investigation, int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(investigation.SelectedInvestigator.Id);


            if (ModelState.IsValid)
            {
                Investigation i = new Investigation()
                {
                    DateOfAction = investigation.DateOfAction,
                    InvestigatorEmail = investigation.SelectedInvestigator.Email,
                    InvestigatorPhone = investigation.SelectedInvestigator.PhoneNumber,
                    InvDescription = investigation.InvDescription,
                    ReportId = id,
                    User = investigation.SelectedInvestigator
                };

                Console.WriteLine("Changed investigator to " + investigation.SelectedInvestigator.UserName);

                var report = _reportRepository.GetReportById(id);
                report.ReportStatus = investigation.ReportStatus;

                _reportRepository.EditReportById(id, report);
                _investigationRepository.EditInvestigation(id, i);

                //send email to new investigator
                var loggedUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                ApplicationUser loggedAccount = await _userManager.FindByIdAsync(loggedUser);

                //do not send email if changed user is currently logged user
                if (!loggedAccount.Id.Equals(user.Id))
                {
                    {

                        MailAddress to = new MailAddress(user.Email);
                        MailAddress from = new MailAddress("nemesys.mailsystem@gmail.com"); //system email

                        MailMessage message = new MailMessage(from, to);
                        message.IsBodyHtml = true;
                        string html = "<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'><head> <title></title> <meta http-equiv='X-UA-Compatible' content='IE=edge'> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'> <meta name='viewport' content='width=device-width, initial-scale=1'> <style type='text/css'> #outlook a{padding:0;}.ReadMsgBody{width:100%;}.ExternalClass{width:100%;}.ExternalClass *{line-height:100%;}body{margin:0;padding:0;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;}table, td{border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt;}img{border:0;height:auto;line-height:100%; outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;}p{display:block;margin:13px 0;}</style> <style type='text/css'> @media only screen and (max-width:480px){@-ms-viewport{width:320px;}@viewport{width:320px;}}</style><!--[if mso]> <xml> <o:OfficeDocumentSettings> <o:AllowPNG/> <o:PixelsPerInch>96</o:PixelsPerInch> </o:OfficeDocumentSettings> </xml><![endif]--><!--[if lte mso 11]> <style type='text/css'> .outlook-group-fix{width:100% !important;}</style><![endif]--> <link href='https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700' rel='stylesheet' type='text/css'><link href='https://fonts.googleapis.com/css?family=Helvetica' rel='stylesheet' type='text/css'> <style type='text/css'> @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);@import url(https://fonts.googleapis.com/css?family=Helvetica); </style> <style type='text/css'> @media only screen and (min-width:480px){.mj-column-per-66{width:66.66666666666666% !important; max-width: 66.66666666666666%;}.mj-column-per-33{width:33.33333333333333% !important; max-width: 33.33333333333333%;}.mj-column-per-100{width:100% !important; max-width: 100%;}}</style> <style type='text/css'> </style> <style type='text/css'>.hide_on_mobile{display: none !important;}@media only screen and (min-width: 480px){.hide_on_mobile{display: block !important;}}.hide_section_on_mobile{display: none !important;}@media only screen and (min-width: 480px){.hide_section_on_mobile{display: table !important;}}.hide_on_desktop{display: block !important;}@media only screen and (min-width: 480px){.hide_on_desktop{display: none !important;}}.hide_section_on_desktop{display: table !important;}@media only screen and (min-width: 480px){.hide_section_on_desktop{display: none !important;}}[owa] .mj-column-per-100{width: 100%!important;}[owa] .mj-column-per-50{width: 50%!important;}[owa] .mj-column-per-33{width: 33.333333333333336%!important;}p{margin: 0px;}@media only print and (min-width:480px){.mj-column-per-100{width:100%!important;}.mj-column-per-40{width:40%!important;}.mj-column-per-60{width:60%!important;}.mj-column-per-50{width: 50%!important;}mj-column-per-33{width: 33.333333333333336%!important;}}</style> </head> <body style='background-color:#FFFFFF;'> <div style='background-color:#FFFFFF;'><!--[if mso | IE]> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#FFFFFF;background-color:#FFFFFF;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#FFFFFF;background-color:#FFFFFF;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:5px 0px 5px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:middle;width:399.99999999999994px;' ><![endif]--> <div class='mj-column-per-66 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:middle;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:middle;' width='100%'> </table> </div><!--[if mso | IE]> </td><td class='' style='vertical-align:middle;width:199.99999999999997px;' ><![endif]--> <div class='mj-column-per-33 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:middle;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:middle;' width='100%'> <tbody><tr> <td align='right' style='font-size:0px;padding:0px 9px 0px 0px;word-break:break-word;'> <div style='font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:11px;line-height:1.5;text-align:right;color:#000000;'> <p><span style='color: #000000;'><a href='*|WEBVERSION|*' style='color: #000000;'>Webversion</a></span></p></div></td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#0E0D0D;background-color:#0E0D0D;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#0E0D0D;background-color:#0E0D0D;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:top;width:600px;' ><![endif]--> <div class='mj-column-per-100 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'> <tbody><tr> <td style='background:#1E1E1F;font-size:0px;word-break:break-word;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'><tr><td height='50' style='vertical-align:top;height:50px;'><![endif]--> <div style='height:50px;'> &#xA0; </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr><tr> <td align='center' style='background:#1E1E1F;font-size:0px;padding:0px 0px 0px 0px;word-break:break-word;'> <div style='font-family:Helvetica, sans-serif;font-size:11px;line-height:1.5;text-align:center;color:#000000;'> <p><span style='font-family: tahoma, arial, helvetica, sans-serif; font-size: 48px;'><span style='color: #ffffff;'>Nemesys</span></span></p></div></td></tr><tr> <td align='center' style='background:#1E1E1F;font-size:0px;padding:15px 15px 15px 15px;word-break:break-word;'> <div style='font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:11px;line-height:1.5;text-align:center;color:#E0E0E0;'> <p><span style='font-size: 16px;'>The Near Miss Exposure and Reporting System.</span></p></div></td></tr><tr> <td style='background:#1E1E1F;font-size:0px;padding:10px 25px;padding-top:10px;padding-right:10px;padding-bottom:10px;padding-left:10px;word-break:break-word;'> <p style='border-top:solid 1px #333333;font-size:1;margin:0px auto;width:100%;'> </p><!--[if mso | IE]> <table align='center' border='0' cellpadding='0' cellspacing='0' style='border-top:solid 1px #333333;font-size:1;margin:0px auto;width:580px;' role='presentation' width='580px' > <tr> <td style='height:0;line-height:0;'> &nbsp; </td></tr></table><![endif]--> </td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#0E0D0D;background-color:#0E0D0D;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#0E0D0D;background-color:#0E0D0D;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:top;width:600px;' ><![endif]--> <div class='mj-column-per-100 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'> <tbody><tr> <td align='center' style='background:#1E1E1F;font-size:0px;padding:15px 15px 15px 15px;word-break:break-word;'> <div style='font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:11px;line-height:1.5;text-align:center;color:#FFFFFF;'> <p><span style='font-size: 16px;'>If you no not wish to investigate, contact <strong>" + loggedAccount.UserName + "</strong> via<span style='text-decoration: underline;'> email " + loggedAccount.Email + " </span> or by calling on <em>" + loggedAccount.PhoneNumber + "</em></span></p></div></td></tr><tr> <td style='background:#1E1E1F;font-size:0px;word-break:break-word;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'><tr><td height='50' style='vertical-align:top;height:50px;'><![endif]--> <div style='height:50px;'> &#xA0; </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#0E0D0D;background-color:#0E0D0D;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#0E0D0D;background-color:#0E0D0D;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:top;width:600px;' ><![endif]--> <div class='mj-column-per-100 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'> <tbody><tr> <td align='center' vertical-align='middle' style='background:#1E1E1F;font-size:0px;padding:10px 10px 10px 10px;word-break:break-word;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='border-collapse:separate;line-height:100%;'> <tbody><tr> <td align='center' bgcolor='#514a9d' role='presentation' style='border:0px #1F1F1F solid;border-radius:2px;cursor:auto;font-style:normal;mso-padding-alt:16px 48px 16px 48px;background:#514a9d;' valign='middle'> <a href='https://localhost:5001/Report/Details/" + id + "' style='display:inline-block;background:#514a9d;color:#ffffff;font-family:Ubuntu, Helvetica, Arial, sans-serif, Helvetica, Arial, sans-serif;font-size:16px;font-style:normal;font-weight:normal;line-height:100%;Margin:0;text-decoration:none;text-transform:none;padding:16px 48px 16px 48px;mso-padding-alt:0px;border-radius:2px;' target='_blank'> <div><span style='font-family: helvetica, arial, sans-serif; font-size: 16px;'><strong>Investigate Report</strong></span></div></a> </td></tr></tbody></table> </td></tr><tr> <td style='background:#1E1E1F;font-size:0px;word-break:break-word;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'><tr><td height='50' style='vertical-align:top;height:50px;'><![endif]--> <div style='height:50px;'> &#xA0; </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#FFFFFF;background-color:#FFFFFF;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#FFFFFF;background-color:#FFFFFF;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:top;width:600px;' ><![endif]--> <div class='mj-column-per-100 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'> <tbody><tr> <td style='font-size:0px;word-break:break-word;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'><tr><td height='10' style='vertical-align:top;height:10px;'><![endif]--> <div style='height:10px;'> &#xA0; </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#FFFFFF;background-color:#FFFFFF;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#FFFFFF;background-color:#FFFFFF;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:top;width:600px;' ><![endif]--> <div class='mj-column-per-100 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'> <tbody><tr> <td style='font-size:0px;word-break:break-word;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'><tr><td height='10' style='vertical-align:top;height:10px;'><![endif]--> <div style='height:10px;'> &#xA0; </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table> <table align='center' border='0' cellpadding='0' cellspacing='0' class='' style='width:600px;' width='600' > <tr> <td style='line-height:0px;font-size:0px;mso-line-height-rule:exactly;'><![endif]--> <div style='background:#FAF5F5;background-color:#FAF5F5;Margin:0px auto;max-width:600px;'> <table align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background:#FAF5F5;background-color:#FAF5F5;width:100%;'> <tbody> <tr> <td style='direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;vertical-align:top;'><!--[if mso | IE]> <table role='presentation' border='0' cellpadding='0' cellspacing='0'> <tr> <td class='' style='vertical-align:top;width:600px;' ><![endif]--> <div class='mj-column-per-100 outlook-group-fix' style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;'> <table border='0' cellpadding='0' cellspacing='0' role='presentation' style='vertical-align:top;' width='100%'> <tbody><tr> <td align='center' style='font-size:0px;padding:0px 0px 0px 0px;word-break:break-word;'> <div style='font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:11px;line-height:1.5;text-align:center;color:#000000;'> <p style='font-size: 11px;'>Sent automatically using nemesys.</p></div></td></tr></tbody></table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </td></tr></tbody> </table> </div><!--[if mso | IE]> </td></tr></table><![endif]--> </div></body></html>";
                        message.Subject = "Nemesys - Added to investigation";
                        message.Body = html + "";
                        SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential("nemesys.mailsystem@gmail.com", "Qwerty123!"),
                            EnableSsl = true
                        };


                        try
                        {
                            client.Send(message);
                        }
                        catch (SmtpException ex)
                        {
                            //throw error
                        }

                    }



                    return RedirectToAction("Index");
                }

                else
                {
                    return this.View(investigation);
                }
            }
            return RedirectToAction("Index");
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