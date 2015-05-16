using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WSAPI.Lib.Services.CommandAudit;

namespace Distributr.CustomerSupport.Code.CCCommandProcessing
{
    public class CCCommandProcessingDetailViewModel
    {
        public CCCommandProcessingDetailViewModel(DateTime dt, CostCentre costCentre, List<CommandProcessingAudit> items)
        {
            Date = dt;
            ShortDate = dt.ToShortDateString();
            Items = new List<CommandProcessingAuditItem>();
            CostCentreName = costCentre.Name;
            CostCentreId = costCentre.Id.ToString();
            Setup(items);
        }

        public CCCommandProcessingDetailViewModel(CostCentre costCentre, List<CommandProcessingAudit> items)
        {
            Items = new List<CommandProcessingAuditItem>();
            CostCentreName = costCentre.Name;
            CostCentreId = costCentre.Id.ToString();
            Setup(items);
        }

        public CCCommandProcessingDetailViewModel(CostCentre costCentre)
        {
            CostCentreName = costCentre.Name;
            CostCentreId = costCentre.Id.ToString();
            GroupItems = new List<CommandProcessingAuditGroup>();
        }

        private void Setup(List<CommandProcessingAudit> items)
        {
            Items = items.Select(n => new CommandProcessingAuditItem
                {
                    Id = n.Id.ToString(),
                    DateInserted = n.DateInserted,
                    CostCentreApplicationId = n.CostCentreApplicationId.ToString(),
                    DocumentId = n.DocumentId.ToString(),
                    ParentDocumentId = n.DocumentId.ToString(),
                    JsonCommand = splitJson( n.JsonCommand),
                    CommandType = n.CommandType.ToString(),
                    Status = n.Status.ToString(),
                    RetryCounter = n.RetryCounter,
                })
                .OrderByDescending(n => n.DateInserted)
                .ToList();
            Total = items.Count();
            OnQueue = items.Count(n => n.Status == CommandProcessingStatus.OnQueue);
            Begin = items.Count(n => n.Status == CommandProcessingStatus.SubscriberProcessBegin);
            Retry = items.Count(n => n.Status == CommandProcessingStatus.MarkedForRetry);
            Complete = items.Count(n => n.Status == CommandProcessingStatus.Complete);
            Failed = items.Count(n => n.Status == CommandProcessingStatus.Failed);
        }

        public DateTime Date { get; set; }
        public string ShortDate { get; set; }
        public string CostCentreId { get; set; }
        public string CostCentreName { get; set; }
        public int Total { get; set; }
        public int OnQueue { get; set; }
        public int Begin { get; set; }
        public int Retry { get; set; }
        public int Complete { get; set; }
        public int Failed { get; set; }
        public List<CommandProcessingAuditItem> Items { get; set; }
        public List<CommandProcessingAuditGroup> GroupItems { get; set; }

        private string splitJson(string Json)
        {
            //return Json;
            return Json.Replace(",\"", ",<br/>\"");
        }

        public class CommandProcessingAuditItem
        {
            public string Id { get; set; }
            public int CostCentreCommandSequence { get; set; }
            public string CostCentreApplicationId { get; set; }
            public string DocumentId { get; set; }
            public string ParentDocumentId { get; set; }
            public string JsonCommand { get; set; }
            public string CommandType { get; set; }
            public string Status { get; set; }
            public int RetryCounter { get; set; }
            public DateTime DateInserted { get; set; }
            public string Time { get; set; }
        }

        public class CommandProcessingGrouping
        {
            public string Id { get; set; }
            public int CostCentreCommandSequence { get; set; }
            public string CostCentreApplicationId { get; set; }
            public string DocumentId { get; set; }
            public string ParentDocumentId { get; set; }
            public string JsonCommand { get; set; }
            public string CommandType { get; set; }
            public string Status { get; set; }
            public int RetryCounter { get; set; }
            public DateTime DateInserted { get; set; }
            public string Time { get; set; }
        }
    }
}