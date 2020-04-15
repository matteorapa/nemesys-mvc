using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using mvc.ViewModels;

namespace mvc.ViewModels
{
    public class EditInvestigation
    {
        [Required(ErrorMessage = "Please enter date of action.")]
        public DateTime DateOfAction { get; set; }

        [Required(ErrorMessage = "Please enter investigator's email")]
        [StringLength(30, MinimumLength = 5)]
        public string InvestigatorEmail { get; set; }

        [Required(ErrorMessage = "Please enter investigator's phone number")]
        public int InvestigatorPhone { get; set; }

        [Required(ErrorMessage = "Please enter a brief investigation description")]
        [StringLength(100, MinimumLength = 1)]
        public string InvDescription { get; set; }
    }
}
