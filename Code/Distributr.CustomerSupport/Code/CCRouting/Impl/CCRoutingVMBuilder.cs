using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.CustomerSupport.Code.CCRouting.Impl
{
    public class CCRoutingVMBuilder : ICCRoutingVMBuilder
    {
        private ICostCentreRepository _costCentreRepository;
        private ICostCentreApplicationRepository _centreApplicationRepository;
        private ICommandRoutingOnRequestRepository _commandRoutingOnRequestRepository;

        public CCRoutingVMBuilder( ICostCentreRepository costCentreRepository, ICostCentreApplicationRepository centreApplicationRepository, ICommandRoutingOnRequestRepository commandRoutingOnRequestRepository)
        {
            _costCentreRepository = costCentreRepository;
            _centreApplicationRepository = centreApplicationRepository;
            _commandRoutingOnRequestRepository = commandRoutingOnRequestRepository;
        }

        public CCRoutingSummaryViewModel GetRoutingSummary()
        {
            var ccRouteOnRequestSummary = _commandRoutingOnRequestRepository.GetCCRouteOnRequestSummary(null);
            var deliveredSummary = _commandRoutingOnRequestRepository.GetCCAppIdRouteOnRequestDeliveredSummary(null);
            
            //pull it all together 
            List<CostCentre> costCentres = _costCentreRepository.GetAll().ToList();
            List<CostCentreApplication> costCentreApplications = _centreApplicationRepository.GetAll().ToList();
            var subitems = new List<CCRoutingSummaryViewModel.CCRoutingStatusSummaryItem>();
            foreach (var ccapp in costCentreApplications)
            {
                Func<int> del = () =>
                    {
                        if (deliveredSummary.All(n => n.DestinationCostCentreApplicationId != ccapp.Id))
                            return 0;
                        var stsum = deliveredSummary.First(n => n.DestinationCostCentreApplicationId == ccapp.Id);
                        return stsum.DeliveredCount;
                    };
                subitems.Add(new CCRoutingSummaryViewModel.CCRoutingStatusSummaryItem
                    {
                        DestinationCostCentreApplicationId = ccapp.Id,
                        DestinationCostCentreId = ccapp.CostCentreId,
                        Delivered = del()

                    });
            }

            var items = new List<CCRoutingSummaryViewModel.CCSummaryItem>();
            foreach (var costCentre in costCentres)
            {
                Func<Tuple<int,int,int>> tc = () =>
                    {
                        if (ccRouteOnRequestSummary.All(n => n.CostCentreId != costCentre.Id))
                            return new Tuple<int, int, int>(0,0,0);
                        var lt = ccRouteOnRequestSummary.First(n => n.CostCentreId == costCentre.Id);
                        return new Tuple<int, int,int>(lt.Count, lt.ValidCount, lt.RetiredCount);
                    };
                
                items.Add(new CCRoutingSummaryViewModel.CCSummaryItem
                    {
                        CostCentre = costCentre.Name,
                        DestinationCostCentreId = costCentre.Id,
                        TotalCount = tc().Item1,
                        ValidCount = tc().Item2,
                        RetiredCount = tc().Item3,
                        CCAppStatusItems = subitems.Where(n => n.DestinationCostCentreId == costCentre.Id).ToList()
                    });
            }
            return new CCRoutingSummaryViewModel{Items = items};
        }

        public CCRoutingDetailViewModel RoutingDetail(Guid costCentreId, int dayOfYear, int year)
        {
            var items = _commandRoutingOnRequestRepository.GetCostCentreRouteOnRequestDetail(costCentreId, dayOfYear,
                                                                                             year);
            List<CostCentre> ccList = _costCentreRepository.GetAll(true).ToList();
            List<CostCentreApplication> ccApps = _centreApplicationRepository.GetAll(true).ToList();
            Func<Guid, CostCentre> getCCFromCCAppId = guid =>
                {
                    var ccapp = ccApps.FirstOrDefault(n => n.Id == guid);
                    if (ccapp == null)
                        return new Distributor(Guid.Empty){Name = "???"};
                    var cc = ccList.First(n => n.Id == ccapp.CostCentreId);
                    return cc;
                };
            var vmItems = items.Select(n => new CCRoutingDetailViewModel.CCRoutingDetailItem
                {
                    CommandRouteOnRequestId = n.CommandRouteOnRequestId,
                    IsRetired = n.IsRetired,
                    IsValid = n.IsValid,
                    DateAdded = n.DateAdded,
                    DateDelivered = n.DateDelivered,
                    DocumentId = n.DocumentId,
                    ParentDocumentId = n.ParentDocumentId,
                    JsonCommand = splitJson( n.JsonCommand),
                    Delivered = n.Delivered,
                    CommandType = n.CommandType,
                    CommandGeneratedByCostCentreApplicationId = n.CommandGeneratedByCostCentreApplicationId,
                    CommandGeneratedByCostCentre = getCCFromCCAppId(n.CommandGeneratedByCostCentreApplicationId).Name,
                    CommandGeneratedByCostCentreId = getCCFromCCAppId(n.CommandGeneratedByCostCentreApplicationId).Id
                })
                .OrderByDescending(n => n.DateAdded)
                .ToList();
            var result = new CCRoutingDetailViewModel
                {
                    CostCentreId = costCentreId.ToString(),
                    CostCentreName = ccList.First(n => n.Id == costCentreId).Name,
                    Items = vmItems
                };
            return result;
        }
        private string splitJson(string Json)
        {
            //return Json;
            return Json.Replace(",\"", ",<br/>\"");
        }

       
    }
}