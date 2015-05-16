using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.CustomerSupport.Code.CCCommandProcessing;
using Distributr.CustomerSupport.Paging;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.CustomerSupport.Code.CCAudit
{
    public class CCCommandAuditViewModelBuilder :ICCCommandAuditViewModelBuilder
    {
        private readonly ICostCentreRepository _costCentreRepository;
        private readonly ICostCentreApplicationRepository _costCentreApplicationRepository;
        private readonly ICommandProcessingAuditRepository _commandProcessingAuditRepository;
        private readonly ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;

        public CCCommandAuditViewModelBuilder(ICostCentreRepository costCentreRepository, ICostCentreApplicationRepository costCentreApplicationRepository, ICommandProcessingAuditRepository commandProcessingAuditRepository, ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository)
        {
            _costCentreRepository = costCentreRepository;
            _costCentreApplicationRepository = costCentreApplicationRepository;
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
        }

        public IEnumerable<CostCentreRef> GetCostCentres()
        {
            var costCentreAliases = _costCentreRepository.GetAll()
                .Select(n => new CostCentreRef {Id = n.Id, Name = n.Name, CostCentreType = n.CostCentreType.ToString()});
            return costCentreAliases;
        }

        public CCCommandProcessingDetailViewModel GetCommandAuditDetails(Guid costCentreId, Guid costCentreAppId, int pageIndex, int pageSize, out int count)
        {
            var costCentre = _costCentreRepository.GetById(costCentreId);
            var items = _commandProcessingAuditRepository.GetByApplicationId(costCentreAppId, pageIndex, pageSize, 0, out count);
            var xs = items.GroupBy(n => n.ParentDocumentId);
            var groupItems = (from x in xs
                              let item = x.First()
                              let createCommand = x.FirstOrDefault(n => n.CommandType.Contains("Create"))
                              select new CommandProcessingAuditGroup
                                         {
                                             DocumentId = x.Key, 
                                             NoOfCommands = x.Count(), 
                                             DateInserted = item.DateInserted, 
                                             DateDelivered = item.DateInserted, 
                                             DocumentType = createCommand.CommandType.Remove(0, 6)
                                         }).ToList();
            count = groupItems.Count();
            var vm = new CCCommandProcessingDetailViewModel(costCentre);
            vm.GroupItems.AddRange(groupItems);
            return vm;
        }

        public CCCommandProcessingDetailViewModel GetCommandProcessingDetails(Guid costCentreId, Guid applicationId,
            int currentPageIndex, int pageSize, CommandProcessingStatus status, out int count)
        {
            var costCentre = _costCentreRepository.GetById(costCentreId);
            var items = _commandProcessingAuditRepository.GetByApplicationId(applicationId, currentPageIndex, pageSize, status, out count);
            var vm = new CCCommandProcessingDetailViewModel(costCentre, items);
            vm.Items = vm.Items.ToList();
            return vm;
        }

        public IEnumerable<CCComandRoutingItem> GetCommandRoutingItems(Guid costCentreId, DateTime date, 
            int currentPageIndex, int itemsPerPage, out int count)
        {
            List<long> costCentreCommands = _commandRoutingOnRequestRepository.GetCommandsAsIntegers(
                costCentreId, date, currentPageIndex, itemsPerPage, out count);
            List<Guid> costCentreApps = GetCostCentreApplications(costCentreId).Select(n => n.Id).ToList();
            List<CCComandRoutingItem> routedItems = _commandRoutingOnRequestRepository.GetCommandRoutingItems(
                costCentreCommands, costCentreApps);
            List<long> routedCommands = routedItems.Select(n => n.CommandAsInteger).ToList();
            List<CommandRef> costCentreCommandRefs = _commandRoutingOnRequestRepository.GetCommandRefs(routedCommands);
            IEnumerable<CCComandRoutingItem> routedItemsFull = routedItems.Join(
                costCentreCommandRefs, 
                x => x.CommandAsInteger, y => y.CommandIdAsInteger,
                (x, y) => new CCComandRoutingItem
                              {
                                  CommandAsInteger = x.CommandAsInteger,
                                  DateDelivered = x.DateDelivered,
                                  DateProcessed = x.DateProcessed,
                                  Delivered = x.Delivered,
                                  CommandId = y.CommandId,
                                  CommandType = y.CommandType,
                                  DocumentId = y.DocumentId,
                              });
            return routedItemsFull;
        }

        public IEnumerable<CostCentreApplication> GetCostCentreApplications(Guid costCentreId)
        {
            var costCentreApps = _costCentreApplicationRepository.GetAll()
                .Where(n => n.CostCentreId == costCentreId)
                .OrderBy(n => n._DateCreated);
            return costCentreApps;
        }
    }
}