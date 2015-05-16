using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.Core.Domain.Master.CostCentreEntities;

using Distributr.WSAPI.Lib.System.Utility.CCAudit;

namespace Distributr.CustomerSupport.Code.CCAudit
{
    public class CCAuditHitSummaryViewModel
    {
        public CCAuditHitSummaryViewModel(DateTime dt, CostCentre costCentre, List<CCAuditItem> items, string selectedaction)
        {
            ItemsByAction = new List<CCAuditItem>();
            Date = dt;
            CostCentreId = costCentre.Id;
            CostCentre = costCentre.Name;
            Setup(items, selectedaction);
            
            SelectedAction = selectedaction;
        }

        void Setup(List<CCAuditItem> items, string selectedAction )
        {
            TotalHits = items.Count();
            HitSummaryByAction = (from i in items
                                  group i by i.Action
                                  into g
                                  select new ActionSummary {ActionName = g.Key, NoHits = g.Count()})
                                  .ToList();

            if (!string.IsNullOrWhiteSpace(selectedAction))
            {
                if (selectedAction == "All")
                    ItemsByAction = items.OrderByDescending(n => n.DateInsert).ToList();
                else
                    ItemsByAction = items.Where(n => n.Action == selectedAction)
                        .OrderByDescending(n => n.DateInsert)
                        .ToList();
            }
        }

        public Guid CostCentreId { get; set; }
        public DateTime Date { get; set; }
        public string CostCentre{get;set;}
        public string CostCentreType { get; set; }
        public int TotalHits { get; set; }
        public string SelectedAction { get; set; }

        public List<CCAuditItem> ItemsByAction { get; set; }
        public List<ActionSummary> HitSummaryByAction { get;set; } 

        public class ActionSummary
        {
            public string ActionName { get; set; }
            public int NoHits { get; set; }
        }
    }
}