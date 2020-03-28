using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserEmail { get; set; }
        public int Phone { get; set; }
        public bool IsInvestigator{ get; set; }
        public string Password { get; set; }
    }
}
