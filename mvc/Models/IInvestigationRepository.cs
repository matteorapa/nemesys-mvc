﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public interface IInvestigationRepository
    {
        Investigation GetInvestigationById(int reportId);
        
        //Investigation AddInvestigation(int reportId); 
        //Investigation EditInvestigation(int reportId);

    }
}