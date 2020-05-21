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
        IEnumerable<Investigation> GetUserInvestigations(ApplicationUser user);

        void CreateInvestigation(Investigation investigation); 
        void EditInvestigation(int reportId, Investigation investigation);

    }
}
