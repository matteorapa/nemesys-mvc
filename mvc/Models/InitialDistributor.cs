using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models
{
    public class InitialDistributor
    {

        public static void Inject(AppDbContext context)
        {
            if (!context.Reports.Any())
            {
                context.AddRange(
                    new Report()
                    {
                        HazardLocation = "Ryanair Flight FR7798",
                        HazardDate = DateTime.Today,
                        DateOfReport = DateTime.Today,
                        HazardDescription = "19 year old went aboard with Corona Cerveza.",
                        ReporterEmail = "covid19@gov.mt",
                        ReporterPhone = 111,
                        Upvotes = 0,
                        HazardType = "Infected Patient",
                        ImageUrl = "/images/corona.png"

                    }
                );

                context.SaveChanges();
            }
        }
    }
}
