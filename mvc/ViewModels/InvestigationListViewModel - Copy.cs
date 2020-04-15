using mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.ViewModels
{
    public class InvestigationListViewModel
    {
        public int TotalInvestigations { get; set; }
        public IEnumerable<Investigation> Investigations { get; set; }
    }
}
