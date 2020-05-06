using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using mvc.ViewModels;
using mvc.Models;

namespace mvc.ViewModels
{
    public class EditInvestigation
    {
        [Required(ErrorMessage = "Please enter date of action.")]
        public DateTime DateOfAction { get; set; }

        [Required(ErrorMessage = "Please enter report status")]
        public string ReportStatus { get; set; }

        [Required(ErrorMessage = "Please enter a brief investigation description")]
        [StringLength(100, MinimumLength = 1)]
        public string InvDescription { get; set; }
        
        public IList<ApplicationUser> Investigators { get; set; }

        
        public string SelectedInvestigator { get; set; }
    }
}
