using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public interface IInvestigationRepository
    {
        Investigation GetInvestigationById(int investgationId);
        Investigation GetInvestigationByReportId(int reportId);
        IEnumerable<Investigation> GetAllInvestigations();

        void CreateInvestigation(Investigation investigation); 
        void EditInvestigation(int reportId, Investigation investigation);

    }
}
