using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{

    public interface IReportRepository
    {
        IEnumerable<Report> GetAllReports();
        IEnumerable<Report> GetUserReports(IdentityUser user);
        Report GetReportById(int reportId);
        void CreateReport(Report report);
        
        void EditReportById(int reportId, Report r);

    }
}
