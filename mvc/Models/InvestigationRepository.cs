using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models
{
    public class InvestigationRepository : IInvestigationRepository
    {
        private readonly AppDbContext _appDbContext;

        public InvestigationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Investigation GetInvestigationById(int investigationId)
        {

            return _appDbContext.Investigations.Include(i => i.Report).FirstOrDefault(i => i.InvestigationId == investigationId);
        }

        public Investigation GetInvestigationByReportId(int reportId)
        {

            return _appDbContext.Investigations.Include(i => i.Report).FirstOrDefault(i => i.ReportId == reportId);
        }


        public IEnumerable<Investigation> GetAllInvestigations()
        {
            return _appDbContext.Investigations.Include(i => i.Report);
        }


        public void CreateInvestigation(Investigation i)
        {
            _appDbContext.Investigations.Add(i);
            _appDbContext.SaveChanges();

        }


        public void EditInvestigation(int reportId, Investigation inv)
        {
            var rec =  _appDbContext.Investigations.FirstOrDefault(i => i.ReportId == reportId);
            if (rec != null)
            {
                // Make changes on entity
                rec.DateOfAction = inv.DateOfAction;
                rec.InvDescription = inv.InvDescription;


                _appDbContext.Investigations.Update(rec);

                _appDbContext.SaveChanges();
            }
        }
    }
}
