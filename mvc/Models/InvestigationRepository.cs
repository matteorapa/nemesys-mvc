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
        public Investigation GetInvestigationById(int reportId)
        {
            return _appDbContext.Investigations.FirstOrDefault(i => i.ReportId == reportId);
        }
    }
}
