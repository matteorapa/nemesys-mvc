using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public interface IInvestigationRepository
    {
        Investigation GetInvestigationById(int reportId);
        IEnumerable<Investigation> GetAllInvestigations();

        void CreateInvestigation(Investigation investigation); 
        Investigation EditInvestigation(int reportId);

    }
}
