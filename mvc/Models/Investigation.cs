using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class Investigation
    {
        public int Id { get; set; }
        public DateTime DateOfReport { get; set; }
        public string Location { get; set; }
        public DateTime HazardDate { get; set; }
        public string ImageUrl { get; set; }
        public int Upvotes { get; set; }
    }
}
