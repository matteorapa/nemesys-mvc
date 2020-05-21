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
    public class EditInvestigationViewModel
    {
        [Required(ErrorMessage = "Please enter date of action.")]
        public DateTime DateOfAction { get; set; }

        [Required(ErrorMessage = "Please enter report status")]
        public string ReportStatus { get; set; }

        [Required(ErrorMessage = "Please enter a brief investigation description")]
        [StringLength(2048, MinimumLength = 1)]
        public string InvDescription { get; set; }
        
        public IList<ApplicationUser> Investigators { get; set; }

        [Required]
        public ApplicationUser SelectedInvestigator { get; set; }
    }

    public class CreateInvestigationViewModel
    {
        [Required(ErrorMessage = "Please enter date of action.")]
        public DateTime DateOfAction { get; set; }

        [Required(ErrorMessage = "Please enter report status")]
        public string ReportStatus { get; set; }

        [Required(ErrorMessage = "Please enter a brief investigation description")]
        [StringLength(100, MinimumLength = 1)]
        public string InvDescription { get; set; }

        public string SelectedInvestigator { get; set; } //set to logged investigator
    }

    public class NewInvestigationViewModel
    {
        public int ReportId { get; set; }
    }

    public class InvestigationListViewModel
    {
        public int TotalInvestigations { get; set; }
        public IEnumerable<Investigation> Investigations { get; set; }
    }

    
}
