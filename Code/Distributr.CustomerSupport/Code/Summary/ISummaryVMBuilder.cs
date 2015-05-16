using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.CustomerSupport.Code.Summary
{
    public interface ISummaryVMBuilder
    {
        OverallHitSummary HitSummary();
    }

    public class OverallHitSummary
    {
        public OverallHitSummary(List<HitSummary> hitSummary)
        {
            SetupHitSummary(hitSummary);
        }

        void SetupHitSummary(List<HitSummary> hitSummary)
        {
            var sb = new StringBuilder();
            sb.Append("[['Date', 'Hits']");

            string f = ",['{0}',{1}]";
            var dateRange =
                Enumerable.Range(-14, 15)
                          .Select(e =>
                              {
                                  var d = DateTime.Now.AddDays(e);
                                  int hits = 0;
                                  if (hitSummary.Any(n => n.DayOfMonth == d.Day && n.Month+1 == d.Month && n.Year+1900 == d.Year))
                                  {
                                      hits =
                                          hitSummary.First(
                                              n => n.DayOfMonth == d.Day && n.Month+1 == d.Month && n.Year+1900 == d.Year)
                                                    .Count;
                                  }
                                  return new { d, Hits=hits};
                              });

            foreach (var item in dateRange)
            {
                string dt = item.d.ToShortDateString().Substring(0, 5);
                sb.AppendFormat(f, dt, item.Hits);
            }

            sb.Append("]");
            HitSummaryForGoogleGraphs = sb.ToString();
        }

        public string HitSummaryForGoogleGraphs { get; set; }
    }
}