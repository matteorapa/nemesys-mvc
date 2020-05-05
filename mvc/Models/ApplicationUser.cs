using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace mvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Upvote Upvote { get; set; }
    }
}
