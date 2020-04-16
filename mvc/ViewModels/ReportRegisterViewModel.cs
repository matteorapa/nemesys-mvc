using mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace mvc.ViewModels
{
    public class ReportRegisterViewModel
    {
        public int TotalReports { get; set; }
        public IEnumerable<Report> Reports { get; set; }
    }
}
