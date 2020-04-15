using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
 
            return _appDbContext.Investigations.FirstOrDefault(i => i.InvestigationId == investigationId);
        }


        public IEnumerable<Investigation> GetAllInvestigations()
        {
            return _appDbContext.Investigations;
        }


        public void CreateInvestigation(Investigation i)
        {
            _appDbContext.Investigations.Add(i);
            _appDbContext.SaveChanges();

        }


        public Investigation EditInvestigation(int reportId)
        {
            //todo

            return _appDbContext.Investigations.FirstOrDefault(r => r.ReportId == reportId);
        }
    }
}
