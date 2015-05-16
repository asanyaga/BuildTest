using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Distributr.CustomerSupport.Code.CCCommandProcessing.Impl
{
    public class CCCommandProcessingViewModelBuilder : ICCCommandProcessingViewModelBuilder
    {
        private ICostCentreRepository _costCentreRepository;
        private ICostCentreApplicationRepository _costCentreApplicationRepository;
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;

        public CCCommandProcessingViewModelBuilder( ICostCentreRepository costCentreRepository, 
            ICostCentreApplicationRepository costCentreApplicationRepository, ICommandProcessingAuditRepository commandProcessingAuditRepository)
        {
            _costCentreRepository = costCentreRepository;
            _costCentreApplicationRepository = costCentreApplicationRepository;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
        }

        public CCCommandProcessingSummaryViewModel GetCommandProcessingSummary()
        {
            var summaryList =_commandProcessingAuditRepository.GetCommandProcessedSummary(null);

            List<CostCentre> costCentres = _costCentreRepository.GetAll().ToList();
            var initialList = _costCentreApplicationRepository.GetAll(true);
            var resultItems = new List<CCCommandProcessingSummaryViewModel.CCCommandProcessingSummary>();
            foreach (var costCentre in costCentres)
            {
                int count = 0;
                foreach (var ccApp in initialList.Where(n=> n.CostCentreId == costCentre.Id))
                {
                    count += 1;
                    var item = new CCCommandProcessingSummaryViewModel.CCCommandProcessingSummary
                    {
                        CCAppActive = ccApp._Status == EntityStatus.Active,
                        CCAppId = ccApp.Id,
                        CCId = ccApp.CostCentreId,
                        CCName = costCentre.Name,
                        CCType = costCentre.CostCentreType.ToString()
                    };
                    if (summaryList.Any(x => x.CostCentreApplicationId == ccApp.Id))
                    {
                        var ltItem = summaryList.First(x => x.CostCentreApplicationId == ccApp.Id);
                        item.TotalCommands = ltItem.Count;
                        item.CommandsOnQueue = ltItem.OnQueue;
                        item.SubscriberProcessBegin =
                            ltItem.SubscriberProcessBegin;
                        item.MarkedForRetry = ltItem.MarkedForRetry;
                        item.Complete = ltItem.Complete;
                        item.Failed = ltItem.Failed;
                    }
                    resultItems.Add(item);
                }
                if (count == 0)
                {
                    var item = new CCCommandProcessingSummaryViewModel.CCCommandProcessingSummary
                    {
                        CCAppActive =  false,
                        CCAppId = Guid.Empty,
                        CCId = costCentre.Id,
                        CCName = costCentre.Name,
                        CCType = costCentre.CostCentreType.ToString()
                    };
                    resultItems.Add(item);
                }

            }

            var vm = new CCCommandProcessingSummaryViewModel{Items = resultItems};
            return vm;
        }

        public CCCommandProcessingDetailViewModel GetCommandDetail(int dayofYear, int year, Guid costCentreId)
        {
            CostCentre cc = _costCentreRepository.GetById(costCentreId);
            DateTime dt = new DateTime(year, 1, 1).AddDays(dayofYear - 1);
            //to do get cost centre id
            var ccApps =  _costCentreApplicationRepository.GetByCostCentreId(costCentreId).Select(n => n.Id);
            List<CommandProcessingAudit> audit = new List<CommandProcessingAudit>();
            foreach (var ccApp in ccApps)
            {
                var items = _commandProcessingAuditRepository.GetByCCAppId(ccApp, dayofYear, year);    
                audit.AddRange(items);
            }
            
            return new CCCommandProcessingDetailViewModel(dt, cc, audit);
        }

        public decimal GetUnQueuedCommands()
        {
            return _commandProcessingAuditRepository.GetUnQueuedCommands();
        }

        public void QueueCommands()
        {
           _commandProcessingAuditRepository.QueueCommands();
        }

        public void Test()
        {
            _commandProcessingAuditRepository.Test();
        }
    }
}