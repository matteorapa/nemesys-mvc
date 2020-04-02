using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _appDbContext;

        public ReportRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Report> GetAllReports()
        {
            return _appDbContext.Reports;
        }

        public Report GetReportById(int reportId)
        {
            return _appDbContext.Reports.FirstOrDefault(r => r.ReportId == reportId);
        }
    }
}
