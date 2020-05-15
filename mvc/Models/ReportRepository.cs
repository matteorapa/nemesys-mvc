using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            return _appDbContext.Reports.Include(r => r.Upvotes).ToList();
        }

        public IEnumerable<Report> GetUserReports(ApplicationUser user)
        {
            return _appDbContext.Reports.Include(r => r.Upvotes).Where(r => r.User == user).ToList();
        }

        public Report GetReportById(int reportId)
        {
            return _appDbContext.Reports.Include(r => r.Upvotes).FirstOrDefault(r => r.ReportId == reportId);
        }

        public void EditReportById(int reportId, Report rep)
        {
            var rec = _appDbContext.Reports.Include(r => r.Upvotes).FirstOrDefault(r => r.ReportId == reportId);
           
            if (rec != null)
            {
                // Make changes on entity
                rec.HazardDescription = rep.HazardDescription;
                rec.HazardDate = rep.HazardDate;
                rec.HazardType = rep.HazardType;
                rec.HazardDescription = rep.HazardDescription;
                rec.ImageUrl = rep.ImageUrl;
                rec.ReportStatus = rep.ReportStatus;

                _appDbContext.Reports.Update(rec);

                _appDbContext.SaveChanges();
            }
        }

        public void CreateReport(Report rep)
        {
            _appDbContext.Reports.Add(rep);
            _appDbContext.SaveChanges();
        }
    }
}
