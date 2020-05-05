using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class Upvote
    {
        
        public int UpvoteId { get; set; }

        //One-to-One relationship with User - EF Core Conventions
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        //One-to-Many (Report to Upvotes) relationship with Report - EF Core Convention 4
        public int ReportId { get; set; }
        public Report Report { get; set; }

    }
}
