using Microsoft.AspNetCore.Identity;
using mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.ViewModels
{
    

    public class ManageViewModel {
        public int TotalAccounts { get; set; }
        public IEnumerable<ApplicationUser> Investigators { get; set; }
        public IEnumerable<ApplicationUser> Reporters { get; set; }
    }
}
