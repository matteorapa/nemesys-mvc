using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace mvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        //One-to-Many (User to Upvotes) relationship with Upvote - EF Core Convention 4   
        public ICollection<Upvote> Upvotes { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
