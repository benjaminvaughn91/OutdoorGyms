using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace OutdoorGyms.Models
{
    public class DBInitializer
    {
        public static void EnsurePopulated(IServiceProvider services)
        {

            var context = services.GetRequiredService<ApplicationDbContext>();
            if (!context.Countys.Any())
            {
                context.Countys.AddRange(
                  new County { CountyId = "D00", CountyName = "OutdoorGyms" },
                  new County { CountyId = "D01", CountyName = "Stockholms län" },
                  new County { CountyId = "D02", CountyName = "Västerbottens län" },
                  new County { CountyId = "D03", CountyName = "Norrbottens län" },
                  new County { CountyId = "D04", CountyName = "Uppsala län" },
                  new County { CountyId = "D05", CountyName = "Södermanlands län" },
                  new County { CountyId = "D06", CountyName = "Östergötlands län" },
                  new County { CountyId = "D07", CountyName = "Jönköpings län" },
                  new County { CountyId = "D08", CountyName = "Kronobergs län" },
                  new County { CountyId = "D09", CountyName = "Kalmar län" },
                  new County { CountyId = "D10", CountyName = "Gotlands län" },
                  new County { CountyId = "D11", CountyName = "Blekinge län" },
                  new County { CountyId = "D12", CountyName = "Skåne län" },
                  new County { CountyId = "D13", CountyName = "Hallands län" },
                  new County { CountyId = "D14", CountyName = "Västra Götalands län" },
                  new County { CountyId = "D15", CountyName = "Värmlands län" },
                  new County { CountyId = "D16", CountyName = "Örebro län" },
                  new County { CountyId = "D17", CountyName = "Västmanlands län" },
                  new County { CountyId = "D18", CountyName = "Dalarna län" },
                  new County { CountyId = "D19", CountyName = "Gävleborgs län" },
                  new County { CountyId = "D20", CountyName = "Västernorrlands län" },
                  new County { CountyId = "D21", CountyName = "Jämtlands län" }
                );
                context.SaveChanges(); 
            }

            if (!context.GymStatuses.Any())
            {
                context.GymStatuses.AddRange(
                  new GymStatus { StatusId = "S_A", StatusName = "Pending" },
                  new GymStatus { StatusId = "S_B", StatusName = "Approved" },
                  new GymStatus { StatusId = "S_C", StatusName = "Rejected" }
                );
                context.SaveChanges();
            }

            if (!context.Sequences.Any())
            {
                context.Sequences.Add(
                  new Sequence { CurrentValue = 200 }
                  );
                context.SaveChanges();
            }

            if (!context.Gyms.Any())
            {
                context.Gyms.AddRange(
                  new Gym { RefNumber = "2020-45-195", Place = "Fyrisån", Town = "Uppsala", DateOfContribution = new DateTime(2020, 04, 24), Description = "Litet gym motsatt stadsparken på andra sidan ån.",  ContributorName = "Anna Åkerman", StatusId = "S_A", CountyId = "D04", EmployeeId = "E100" });
                context.SaveChanges();
            }
        }
    }
}
