using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributr.CustomerSupport.Code.CCCommandProcessing
{
    public class CCCommandProcessingSummaryViewModel
    {
        public CCCommandProcessingSummaryViewModel()
        {
            Items = new List<CCCommandProcessingSummary>();
        }

        public List<CCCommandProcessingSummary> Items { get; set; }

        public List<CostCentreSummary> GetSummary()
        {
            var groupByCC = Items.GroupBy(n => n.CCId)
                         .Select(n => new CostCentreSummary
                             {
                                 CCName = n.First().CCName,
                                 CCId = n.First().CCId,
                                 CCType = n.First().CCType,
                                 TotalCommands = n.Sum(x => x.TotalCommands),
                                 CommandsOnQueue = n.Sum(x => x.CommandsOnQueue),
                                 SubscriberProcessBegin = n.Sum(x => x.SubscriberProcessBegin),
                                 MarkedForRetry = n.Sum(x => x.MarkedForRetry),
                                 Complete = n.Sum(x => x.Complete),
                                 Failed = n.Sum(x => x.Failed),
                                 Items = n.ToList()
                             })
                             .OrderBy(x => x.CCName)
                         .ToList();
            return groupByCC;

        }



        public class CCCommandProcessingSummary : CommandSummaryBase
        {
            public Guid CCAppId { get; set; }
            public bool CCAppActive { get; set; }
        }

        public class CostCentreSummary : CommandSummaryBase
        {
            public List<CCCommandProcessingSummary> Items { get; set; } 
        }

        public abstract class CommandSummaryBase
        {
            public string CCName { get; set; }
            public Guid CCId { get; set; }
            public string CCType { get; set; }
            public long TotalCommands { get; set; }
            public long CommandsOnQueue { get; set; }
            public long SubscriberProcessBegin { get; set; }
            public long MarkedForRetry { get; set; }
            public long Complete { get; set; }
            public long Failed { get; set; }
        }
    }
}