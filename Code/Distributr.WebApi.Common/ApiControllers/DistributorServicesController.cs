using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Transactions;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.PG;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow.Impl.InventoryWorkFlow;
using log4net;
using ShipToAddress = Distributr.Core.Domain.Master.CostCentreEntities.ShipToAddress;

namespace Distributr.WebApi.ApiControllers
{
    public class DistributorServicesController : ApiController
    {
        ILog _log = LogManager.GetLogger("InventoryController");
        private IOutletRepository _outletRepository;
        private ICostCentreFactory _costCentreFactory;
        private ICostCentreRepository _costCentreRepository;
        private IRouteRepository _routeRepository;
        private IOutletCategoryRepository _outletCategoryRepository;
        private IOutletTypeRepository _outletTypeRepository;
        private IOutletVisitDayRepository _outletVisitDayRepository;
        private IOutletPriorityRepository _outletPriorityRepository;

        private IProductPricingTierRepository _productPricingTierRepository;
        private IVATClassRepository _vatClassRepository;
        private IDiscountGroupRepository _discountGroupRepository;
        private ITargetPeriodRepository _targetPeriodRepository;
        private ITargetRepository _targetRepository;
        private IUserRepository _userRepository;
        private IDistributorSalesmanRepository _distributorSalesmanRepository;
        private ISalesmanRouteRepository _salesmanRouteRepository;
        private IContactRepository _contactRepository;
        //private IMaritalStatusRepository _maritalStatusRepository;
        private IContactTypeRepository _contactTypeRepository;
        private IBankRepository _bankRepository;
        private IBankBranchRepository _bankBranchRepository;
        private IRegionRepository _regionRepository;
        public IPurchasingClerkRepository _purchasingClerkRepository;
        private IPurchasingClerkRouteRepository _purchasingClerkRouteRepository;
        private ICommoditySupplierRepository _commoditySupplierRepository;
        private ICommodityProducerRepository _commodityProducerRepository;
        private IMasterDataAllocationRepository _masterDataAllocationRepository;
        private ICommodityOwnerRepository _commodityOwnerRepository;
        private ICentreRepository _centreRepository;
        private IStoreRepository _storeRepository;
        private IVehicleRepository _vehicleRepository;
        private IEquipmentRepository _equipmentRepository;
        private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        private ITransactionsSummary _transactionsSummary;
        private IPgRepositoryHelper _pgRepository;

        public DistributorServicesController(IOutletRepository outletRepository, ICostCentreFactory costCentreFactory, ICostCentreRepository costCentreRepository, IRouteRepository routeRepository, IOutletCategoryRepository outletCategoryRepository, IOutletTypeRepository outletTypeRepository, IOutletVisitDayRepository outletVisitDayRepository, IOutletPriorityRepository outletPriorityRepository, IProductPricingTierRepository productPricingTierRepository, IVATClassRepository vatClassRepository, IDiscountGroupRepository discountGroupRepository, ITargetPeriodRepository targetPeriodRepository, ITargetRepository targetRepository, IUserRepository userRepository, IDistributorSalesmanRepository distributorSalesmanRepository, ISalesmanRouteRepository salesmanRouteRepository, IContactRepository contactRepository, IContactTypeRepository contactTypeRepository, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository, IRegionRepository regionRepository, IPurchasingClerkRepository purchasingClerkRepository, IPurchasingClerkRouteRepository purchasingClerkRouteRepository, ICommoditySupplierRepository commoditySupplierRepository, ICommodityProducerRepository commodityProducerRepository, IMasterDataAllocationRepository masterDataAllocationRepository, ICommodityOwnerRepository commodityOwnerRepository, ICentreRepository centreRepository, IStoreRepository storeRepository, IVehicleRepository vehicleRepository, IEquipmentRepository equipmentRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, ITransactionsSummary transactionsSummary, IPgRepositoryHelper pgRepository)
        {
            _outletRepository = outletRepository;
            _costCentreFactory = costCentreFactory;
            _costCentreRepository = costCentreRepository;
            _routeRepository = routeRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _outletTypeRepository = outletTypeRepository;
            _outletVisitDayRepository = outletVisitDayRepository;
            _outletPriorityRepository = outletPriorityRepository;
            _productPricingTierRepository = productPricingTierRepository;
            _vatClassRepository = vatClassRepository;
            _discountGroupRepository = discountGroupRepository;
            _targetPeriodRepository = targetPeriodRepository;
            _targetRepository = targetRepository;
            _userRepository = userRepository;
            _distributorSalesmanRepository = distributorSalesmanRepository;
            _salesmanRouteRepository = salesmanRouteRepository;
            _contactRepository = contactRepository;
            _contactTypeRepository = contactTypeRepository;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _regionRepository = regionRepository;
            _purchasingClerkRepository = purchasingClerkRepository;
            _purchasingClerkRouteRepository = purchasingClerkRouteRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _centreRepository = centreRepository;
            _storeRepository = storeRepository;
            _vehicleRepository = vehicleRepository;
            _equipmentRepository = equipmentRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _transactionsSummary = transactionsSummary;
            _pgRepository = pgRepository;
        }

        [HttpPost]
        public HttpResponseMessage Index()
        {
            return null;
        }

        [HttpGet]
        public HttpResponseMessage DistributorOutletList(Guid distributorId)
        {
            try
            {
                var items = _outletRepository.GetByDistributor(distributorId)
                    .Select(n => new OutletItem
                    {
                        MasterId = n.Id,
                        DateCreated = n._DateCreated,
                        DateLastUpdated = n._DateLastUpdated,
                        StatusId = (int)n._Status,
                        Name = n.Name,
                        OutletCategoryMasterId = n.OutletCategory == null ? Guid.Empty : n.OutletCategory.Id,
                        OutletTypeMasterId = n.OutletType == null ? Guid.Empty : n.OutletType.Id,
                        ParentCostCentreId = n.ParentCostCentre.Id,
                        RouteMasterId = n.Route == null ? Guid.Empty : n.Route.Id,
                        CostCentreTypeId = (int)n.CostCentreType,
                        OutletProductPricingTierMasterId = n.OutletProductPricingTier.Id,
                        OutletVATClassMasterId = n.VatClass == null ? Guid.Empty : n.VatClass.Id,
                        OutletDiscountGroupMasterId = n.DiscountGroup == null ? Guid.Empty : n.DiscountGroup.Id,
                        outLetCode = n.CostCentreCode,
                        Latitude = n.Latitude,
                        Longitude = n.Longitude,
                    })
                    .ToList();
                return Request.CreateResponse<List<OutletItem>>(HttpStatusCode.OK, items);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to retrieve outlet list", ex);
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "There was an error");
        }

        [HttpPost]
        public HttpResponseMessage OutletAdd(OutletItem outletItem)
        {
            var response = new ResponseBool {Success = false};
            try
            {
                Outlet outlet =
                    _costCentreFactory.CreateCostCentre(outletItem.MasterId, CostCentreType.Outlet,
                                                        _costCentreRepository.GetById(outletItem.ParentCostCentreId)) as
                    Outlet;
                outlet.Route = _routeRepository.GetById(outletItem.RouteMasterId);
                outlet.OutletCategory = _outletCategoryRepository.GetById(outletItem.OutletCategoryMasterId);
                outlet.OutletType = _outletTypeRepository.GetById(outletItem.OutletTypeMasterId);
                outlet.Name = outletItem.Name;
                outlet.OutletProductPricingTier =
                    _productPricingTierRepository.GetById(outletItem.OutletProductPricingTierMasterId);
                outlet.CostCentreCode = outletItem.outLetCode;
                outlet.CostCentreCode = outletItem.outLetCode;
                outlet.Latitude = outletItem.Latitude;
                outlet.Longitude = outletItem.Longitude;
                outlet._SetStatus(EntityStatus.Active);
                foreach (var address in outletItem.ShippingAddresses)
                {

                    var shipToAddress = new ShipToAddress(address.MasterId)
                        {
                            Name = address.Name,
                            PhysicalAddress = address.PhysicalAddress,
                            PostalAddress = address.PostalAddress,
                            Longitude = address.Longitude ?? 0,
                            Latitude = address.Latitude ?? 0,
                            Description = address.Description,
                        };
                    if (address.Latitude != null) shipToAddress.Latitude = (decimal) address.Latitude;
                    if (address.Longitude != null) shipToAddress.Longitude = (decimal) address.Longitude;
                    outlet.AddShipToAddress(shipToAddress);
                }
                try
                {
                    outlet.VatClass = _vatClassRepository.GetById(outletItem.OutletVATClassMasterId);
                }
                catch //cn: VATClass is removed or some other error
                {
                    outlet.VatClass = null;
                }
                try
                {
                    outlet.DiscountGroup = _discountGroupRepository.GetById(outletItem.OutletDiscountGroupMasterId);
                }
                catch //cn: VATClass is removed or some other error
                {
                    outlet.DiscountGroup = null;
                }
                var o = _costCentreRepository.Save(outlet);
                response.Success = true;
                response.ErrorInfo = "Successfully saved outlet " + outlet.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid outlet fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
                //return errorMsg;
            }
            catch (Exception ex) //any other

            {
                response.ErrorInfo = "Error: An error occurred when saving the outlet.";
                _log.Error("Error: An error occurred when saving the outlet.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage OutletUpdate(OutletItem outletItem)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                Outlet outlet = _outletRepository.GetById(outletItem.MasterId) as Outlet;
                outlet.Route = _routeRepository.GetById(outletItem.RouteMasterId);
                outlet.Name = outletItem.Name;
                outlet.OutletCategory = _outletCategoryRepository.GetById(outletItem.OutletCategoryMasterId);
                outlet.OutletType = _outletTypeRepository.GetById(outletItem.OutletTypeMasterId);
                outlet.OutletProductPricingTier =
                    _productPricingTierRepository.GetById(outletItem.OutletProductPricingTierMasterId);
                outlet.CostCentreCode = outletItem.outLetCode;
                outlet.CostCentreCode = outletItem.outLetCode;
                outlet.Latitude = outletItem.Latitude;
                outlet.Longitude = outletItem.Longitude;
                outlet._Status = (EntityStatus)outletItem.StatusId;
                outlet.ShipToAddresses.Clear();
                foreach (var address in outletItem.ShippingAddresses)
                {
                    var shipToAddress = new ShipToAddress(address.MasterId)
                    {
                        Name = address.Name,
                        PhysicalAddress = address.PhysicalAddress,
                        PostalAddress = address.PostalAddress,
                        Longitude = address.Longitude ?? 0,
                        Latitude = address.Latitude ?? 0,
                        Description = address.Description,
                        _Status = (EntityStatus)address.EntityStatus,

                    };
                    outlet.AddShipToAddress(shipToAddress);
                }
                try
                {
                    outlet.VatClass = _vatClassRepository.GetById(outletItem.OutletVATClassMasterId);
                }
                catch //cn: VATClass is removed or some other error
                {
                    outlet.VatClass = null;
                }
                try
                {
                    outlet.DiscountGroup = _discountGroupRepository.GetById(outletItem.OutletDiscountGroupMasterId);
                }
                catch //cn: VATClass is removed or some other error
                {
                    outlet.DiscountGroup = null;
                }

                _costCentreRepository.Save(outlet);
                response.Success = true;
                response.ErrorInfo = "Successfully updated outlet " + outlet.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid outlet fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + ".\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Error: An error occurred when updating the outlet.";
                _log.Error("Error: An error occurred when updating the outlet.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpPost]
        public HttpResponseMessage OutletsApprove(List<Guid> outletIds)
        {
            var response = new ResponseBool { Success = false };

            Outlet outlet = null;
            try
            {
                if (outletIds != null && outletIds.Count > 0)
                {
                    foreach (Guid outletId in outletIds)
                    {
                        outlet = _costCentreRepository.GetById(outletId) as Outlet;
                        if (outlet != null)
                        {
                            outlet._Status = EntityStatus.Active;
                            _costCentreRepository.Save(outlet);
                        }
                    }
                }
                response.Success = true;
                response.ErrorInfo = "Successfully approved outlet " + outlet.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid outlet fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the outlet.";
                _log.Error("Error: An error occurred when saving the outlet.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage OutletDeactivate(Guid outletId)
        {
            var response = new ResponseBool { Success = false };
            Outlet outlet = null;
            try
            {
                outlet = _outletRepository.GetById(outletId) as Outlet;
                _costCentreRepository.SetInactive(outlet);

                response.Success = true;
                response.ErrorInfo = "Successfully deactivated outlet " + outlet.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid outlet fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deactivating the outlet.";
                _log.Error("Error: An error occurred when deactivating the outlet.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage OutletActivate(Guid outletId)
        {
            var response = new ResponseBool { Success = false };
            Outlet outlet = null;
            try
            {
                outlet = _outletRepository.GetById(outletId) as Outlet;
                _costCentreRepository.SetActive(outlet);

                response.Success = true;
                response.ErrorInfo = "Successfully activated outlet " + outlet.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid outlet fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when activating the outlet.";
                _log.Error("Error: An error occurred when activating the outlet.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage OutletDelete(Guid outletId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var outlet = _outletRepository.GetById(outletId) as Outlet;
                if(outlet !=null)
                {
                    _outletRepository.SetAsDeleted(outlet);
                    response.Success = true;
                    response.ErrorInfo = "Successfully deleted outlet: " + outlet.Name;
                }
                else
                {
                    response.ErrorInfo = "Outlet not found";
                }
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In outlet fields. \n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Error: An error occurred when deleting the outlet.\nCause:"+ex.Message;
                _log.Error("Error: An error occurred when deleting the outlet.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
           
        }

        #region outlet priority
        [HttpPost]
        public HttpResponseMessage OutletPriorityAdd(List<OutletPriorityItem> outletPriorityItems)
        {
            var response = new ResponseBool {Success = false};
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    foreach (var outletPriorityItem in outletPriorityItems)
                    {
                        Guid id = outletPriorityItem.MasterId;
                        OutletPriority existing = _outletPriorityRepository.GetById(id, true);
                        if (existing != null)
                            id = existing.Id;

                        var item = new OutletPriority(id)
                                       {
                                           Priority = outletPriorityItem.Priority,
                                           Route = _routeRepository.GetById(outletPriorityItem.RouteId),
                                           Outlet = new CostCentreRef() {Id = outletPriorityItem.OutletId},
                                           EffectiveDate = outletPriorityItem.EffectiveDate,
                                           _Status = (EntityStatus) outletPriorityItem.StatusId,
                                           _DateCreated = outletPriorityItem.DateCreated,
                                           _DateLastUpdated = outletPriorityItem.DateLastUpdated,
                                       };

                        _outletPriorityRepository.Save(item);
                    }
                    response.Success = true;
                    response.ErrorInfo = "Successfully saved outlet priority";
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg = dve.ValidationResults.Results.Aggregate(
                        "Error: Invalid outlet priority fields.\n",
                        (current, msg) =>
                        current + ("\t- " + msg.ErrorMessage + ".\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when saving the outlet priority.";
                    _log.Error("Error: An error occurred when saving the outlet priority.", ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        public HttpResponseMessage OutletPriorityDeactivate(Guid outletId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var outlet = _outletPriorityRepository.GetById(outletId);
                if (outlet != null)
                {
                    _outletPriorityRepository.SetInactive(outlet);

                    response.Success = true;
                    response.ErrorInfo = "Successfully deactivated "+outlet.Route +"priority";
                    
                }
                else
                {
                    response.Success = true;
                    response.ErrorInfo = "Outlet not found ";
                    
                }
                   
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In outlet  prority fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deactivating the route.\nCause: "+ex.Message;
                _log.Error("Error: An error occurred when deactivating the route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        #endregion
        [HttpPost]
        public HttpResponseMessage OutletVisitDayAdd(List<OutletVisitDayItem> outletVisitDayItems)
        {
            var response = new ResponseBool {Success = false};
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {

                try
                {
                    foreach (OutletVisitDayItem outletVisitDayItem in outletVisitDayItems)
                    {
                        var item = new OutletVisitDay(outletVisitDayItem.MasterId)
                                       {
                                           Outlet = new CostCentreRef() {Id = outletVisitDayItem.OutletId},
                                           EffectiveDate = outletVisitDayItem.EffectiveDate,
                                           _DateLastUpdated = outletVisitDayItem.DateLastUpdated,
                                           _Status = (EntityStatus) outletVisitDayItem.StatusId,
                                           _DateCreated = outletVisitDayItem.DateCreated,
                                           Day = outletVisitDayItem.Day
                                       };

                        _outletVisitDayRepository.Save(item);
                    }
                    response.Success = true;
                    response.ErrorInfo = "Successfully saved outlet visit days";
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg =
                        dve.ValidationResults.Results.Aggregate("Error: Invalid outlet visit day fields.\n",
                                                                (current, msg) =>
                                                                current + ("\t- " + msg.ErrorMessage + ".\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                    //return errorMsg;
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when saving the outlet visit day.";
                    _log.Error("Error: An error occurred when saving the outlet visit day.", ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage TargetAdd(CostCentreTargetItem targetItem)
        {
            var response = new ResponseBool { Success = false };
            Target target = null;
            TargetPeriod targetPeriod = null;
            CostCentre costCentre = null;
            try
            {
                targetPeriod = _targetPeriodRepository.GetById(targetItem.TargetPeriodMasterId);
                costCentre = _costCentreRepository.GetById(targetItem.CostCentreMasterId);
                target = new Target(targetItem.MasterId)
                {
                    CostCentre = costCentre,
                    TargetPeriod = targetPeriod,
                    TargetValue = targetItem.TargetValue,
                    IsQuantityTarget = targetItem.IsQuantityTarget,
                };

                _targetRepository.Save(target);
                response.Success = true;
                response.ErrorInfo = "Successfully added target for " + target.CostCentre.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid target fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception e)
            {
                response.ErrorInfo = "Error: An error occured when saving the target.";
                _log.Error("Error: An error occured when saving the target.\n", e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage TargetUpdate(CostCentreTargetItem targetItem)
        {
            var response = new ResponseBool { Success = false };
            Target target = null;
            try
            {
                target = _targetRepository.GetById(targetItem.MasterId, true);
                if (target._Status != EntityStatus.Inactive)
                {
                    target.IsQuantityTarget = targetItem.IsQuantityTarget;
                    target.TargetPeriod = _targetPeriodRepository.GetById(targetItem.TargetPeriodMasterId);
                    target.TargetValue = targetItem.TargetValue;
                    target.CostCentre = _costCentreRepository.GetById(targetItem.CostCentreMasterId);

                    _targetRepository.Save(target);

                    response.Success = true;
                    response.ErrorInfo = "Successfully updated target for " + target.CostCentre.Name; ;
                }
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid target fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception e)
            {
                response.ErrorInfo = "Error: An error occured when saving the target.";
                _log.Error("Error: An error occured when saving the target.\n", e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage TargetDeactivate(Guid targetId)
        {
            var response = new ResponseBool { Success = false };
                Target target;
            try
            {
                target = _targetRepository.GetById(targetId);
                _targetRepository.SetInactive(target);
                response.Success = true;
                response.ErrorInfo = "Successfully deactivated target for " + target.CostCentre.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid target fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception e)
            {
                response.ErrorInfo = "Error: An error occured when deactivating the target";
                _log.Error("Error: An error occured when deactivating the target.\n", e);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage TargetActivate(Guid targetId)
        {
            var response = new ResponseBool { Success = false };
                Target target;
            try
            {
                target = _targetRepository.GetById(targetId, true);
                _targetRepository.SetActive(target);
                response.Success = true;
                response.ErrorInfo = "Successfully activated target for " + target.CostCentre.Name;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid target fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception e)
            {
                response.ErrorInfo = "Error: An error occured when activating the target.";
                _log.Error("Error: An error occured when activating the target.\n", e);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage TargetDelete(Guid targetId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                Target target = _targetRepository.GetById(targetId, true);
                _targetRepository.SetAsDeleted(target);
                response.Success = true;
                response.ErrorInfo = "Successfully deleted target";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid target fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception e)
            {
                response.ErrorInfo = "Error: An error occured when deleting the target.";
                _log.Error("Error: An error occured when deleting the target.\n", e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage RouteList(Guid distributorId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public HttpResponseMessage RouteAdd(RouteItem routeItem)
        {
            var response = new ResponseBool { Success = false };
            Route route = null;
            try
            {
                route = new Route(Guid.NewGuid());
                route.Code = routeItem.Code;
                route.Name = routeItem.Name;
                route.Region = _regionRepository.GetById(routeItem.RegionId);
                // route.Distributor = new CostCentreRef { Id = routeItem.MasterId };
                _routeRepository.Save(route);

                response.Success = true;
                response.ErrorInfo = "Route successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid route fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the route.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage RouteUpdate(RouteItem routeItem)
        {
            var response = new ResponseBool { Success = false };
            Route route = null;
            try
            {
                route = _routeRepository.GetById(routeItem.MasterId, true);
                if (route._Status != EntityStatus.Inactive)
                {
                    route.Code = routeItem.Code;
                    route.Name = routeItem.Name;
                    _routeRepository.Save(route);
                    response.Success = true;
                    response.ErrorInfo = "Successfully updated route " + route.Name;
                }
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid route fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the route.";
                _log.Error("Error: An error occurred when saving the route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage RouteDeactivate(Guid routeId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _routeRepository.SetInactive(_routeRepository.GetById(routeId));

                response.Success = true;
                response.ErrorInfo = "Successfully deactivated  route";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error:Route cannot be deactivated,Could you have assigned this route to a sales person?.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deactivating the route.";
                _log.Error("Error: An error occurred when deactivating the route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage RouteDelete(Guid routeId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _routeRepository.SetAsDeleted(_routeRepository.GetById(routeId));
                response.Success = true;
                response.ErrorInfo = "Successfully deleted route";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error:Route cannot be deleted,Could you have assigned this route to a sales person? \n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Error: An error occurred when deleting the route.";
                _log.Error("Error: An error occurred when deleting the route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage RouteActivate(Guid routeId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _routeRepository.SetActive(_routeRepository.GetById(routeId));
                response.Success = true;
                response.ErrorInfo = "Successfully activated route";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Iroute fields. \n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Error: An error occured when saving the route.";
                _log.Error("Error: An error occured when saving the route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage UserList(Guid distributorId)
        {
            //get all user belonging to costcenters whose parent cost center is this
            var ccs = _costCentreRepository.GetAll().Where(n => (n.ParentCostCentre != null && n.ParentCostCentre.Id == distributorId) || n.Id == distributorId).ToList();

            var users = new List<UserItem>();
            var sdas = ccs.Select(c => _userRepository.GetByDistributor(c.Id, true));
            ccs.ForEach(c => _userRepository.GetByDistributor(c.Id).ForEach(n =>
            {
                if (n != null)
                {
                    users.Add(new UserItem
                    {
                        MasterId = n.Id,
                        Username = n.Username,
                        Password = n.Password,
                        DateCreated = n._DateCreated,
                        DateLastUpdated = n._DateLastUpdated,
                        StatusId = (int)n._Status,
                        Mobile = n.Mobile,
                        PIN = n.PIN,
                        UserRoles = n.UserRoles,
                        UserType = (int)n.UserType,
                        CostCenterID = n.CostCentre,
                        TillNumber = n.TillNumber
                    });
                }
            }));

            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpPost]
        public HttpResponseMessage UserAdd(UserItem userItem)
        {
            var response = new ResponseBool {Success = false};
            User user = null;
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    user = new User(userItem.MasterId == Guid.Empty ? Guid.NewGuid() : userItem.MasterId);
                    user.Username = userItem.Username;
                    user.Password = userItem.Password;
                    user.PIN = userItem.PIN;
                    user.Mobile = userItem.Mobile;
                    user.UserType = (UserType) userItem.UserType;
                    user.CostCentre = userItem.CostCenterID;
                    user.TillNumber = userItem.TillNumber;

                    //For distributor salesman
                    if (userItem.UserType == (int) UserType.DistributorSalesman)
                    {
                        DistributorSalesman ds = null;
                        ds = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman,
                                                                 _costCentreRepository.GetById(userItem.CostCenterID))
                             as DistributorSalesman;
                        ds.Name = userItem.Username;
                        ds.CostCentreCode = userItem.CostCentreCode;
                        ds.Type = (DistributorSalesmanType) userItem.SalesmanType;
                        ds._SetStatus(EntityStatus.Active);

                        user.CostCentre = _costCentreRepository.Save(ds);
                    }

                    _userRepository.Save(user).ToString();
                    response.Success = true;
                    response.ErrorInfo = "Successfully created user " + user.Username;
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid user fields.\n",
                                                                              (current, msg) =>
                                                                              current +
                                                                              ("\t- " + msg.ErrorMessage + "\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when saving the user.\nCause: "+ex.Message;
                    _log.Error("Error: An error occurred when saving the user.", ex);
                }
            }
                return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage UserUpdate(UserItem userItem)
        {
            var response = new ResponseBool { Success = false };
            User user = null;
            DistributorSalesman ds = null;
            try
            {
                user = _userRepository.GetById(userItem.MasterId); 
                user.CostCentre = userItem.CostCenterID;
                //For salesman
                if (userItem.UserType == (int) UserType.DistributorSalesman)
                {
                    ds = _costCentreRepository.GetById(user.CostCentre) as DistributorSalesman;
                    if (ds == null)
                    {
                        ds = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman,
                                                                 _costCentreRepository.GetById(userItem.CostCenterID))
                             as DistributorSalesman;
                    }
                    ds.Type = (DistributorSalesmanType)userItem.SalesmanType;
                    ds.Name = userItem.Username;
                    ds.CostCentreCode = userItem.CostCentreCode;
                    ds._SetStatus(EntityStatus.Active);

                    user.CostCentre = _costCentreRepository.Save(ds);
                }
                user.Username = userItem.Username;
                user.Password = userItem.Password;
                user.PIN = userItem.PIN;
                user.Mobile = userItem.Mobile;
                user.UserType = (UserType)userItem.UserType;
                user.TillNumber = userItem.TillNumber;
                user.PassChange = userItem.PasswordChanged;
                _userRepository.Save(user);
                response.Success = true;
                response.ErrorInfo = "Successfully updated user " + user.Username;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid user fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when updating the user.\nCause:"+ex.Message;
                _log.Error("Error: An error occurred when updating the user.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage UserDeactivate(Guid userId)
        {
            var response = new ResponseBool { Success = false };

            User user = null;
            try
            {
                user = _userRepository.GetById(userId);
                _userRepository.SetInactive(user);
                if (user.UserType == UserType.DistributorSalesman)
                {
                    DistributorSalesmanDeactivate(user.CostCentre);
                }
                else if(user.UserType == UserType.PurchasingClerk)
                {
                    PurchasingClerkActivateOrDeactivate(user.CostCentre);
                }
                response.Success = true;
                response.ErrorInfo = "Successfully deactivated user " + user.Username;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid user fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deactivating the user.";
                _log.Error("Error: An error occurred when deactivating the user.", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage UserActivate(Guid userId)
        {
            var response = new ResponseBool { Success = false };
            User user = null;
            try
            {
                user = _userRepository.GetById(userId);
                _userRepository.SetActive(user);
                if (user.UserType == UserType.DistributorSalesman)
                {
                    var distSalesman = _costCentreRepository.GetById(user.CostCentre);
                    DistributorSalesmanActivate(distSalesman.Id);
                }
                else if (user.UserType == UserType.PurchasingClerk)
                {
                    PurchasingClerkActivateOrDeactivate(user.CostCentre);
                }
                response.Success = true;
                response.ErrorInfo = "Successfully activated user " + user.Username;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid user fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)//any other
            {
                response.ErrorInfo = "Error: An error occurred when activating the user.";
                _log.Error("Error: An error occurred when activating the user.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpGet]
        public HttpResponseMessage UserDelete(Guid userId)
        {
            var response = new ResponseBool { Success = false };
            User user = null;
            try
            {
                user = _userRepository.GetById(userId);
                _userRepository.SetAsDeleted(user);
                if (user.UserType == UserType.DistributorSalesman)
                {
                    var distSalesman = _costCentreRepository.GetById(user.CostCentre);
                    DistributorSalesmanDelete(distSalesman.Id);
                }
                response.Success = true;
                response.ErrorInfo = "Successfully deleted user";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid user fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleted the user.";
                _log.Error("Error: An error occurred when deleted the user.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SalesmanAdd(DistributorSalesmanItem dSItem)
        {
            var response = new ResponseBool { Success = false };
            DistributorSalesman ds = null;
            try
            {
                ds = _costCentreFactory.CreateCostCentre(dSItem.MasterId, CostCentreType.DistributorSalesman,
                                                        _costCentreRepository.GetById(dSItem.ParentCostCentreId)) as DistributorSalesman;
                ds.Name = dSItem.Name;
                ds._SetStatus(EntityStatus.Active);
                _costCentreRepository.Save(ds);

                response.Success = true;
                response.ErrorInfo = "Successfully added salesman.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg =
                    dve.ValidationResults.Results.Aggregate("Error: Invalid distributor salesman fields.\n",
                                                            (current, msg) =>
                                                            current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when updating the distributor salesman.";
                _log.Error("Error: An error occurred when updating the distributor salesman.", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpPost]
        public HttpResponseMessage SalesmanUserAdd(SalesmanAddDTO salesmanAddDto)
        {

            var response = new ResponseBool { Success = false };
            User user = null;
            try
            {
                var dsItem = salesmanAddDto.Salesman;
                var userItem = salesmanAddDto.User;
                CostCentre ds = _costCentreRepository.GetById(dsItem.MasterId);
                if (ds == null)
                {
                    ds = _costCentreFactory.CreateCostCentre(dsItem.MasterId, CostCentreType.DistributorSalesman,
                                                             _costCentreRepository.GetById(dsItem.ParentCostCentreId))
                         as
                         DistributorSalesman;
                }
                ds.Name = dsItem.Name;
                ds.CostCentreCode = dsItem.CostCentreCode;
                user = _userRepository.GetById(userItem.MasterId);
                if (user == null)
                    user = new User(Guid.NewGuid());

                user.Username = userItem.Username;
                user.Password = userItem.Password;
                user.PIN = userItem.PIN;
                user.Mobile = userItem.Mobile;
                user.UserType = (UserType)userItem.UserType;
                user.TillNumber = userItem.TillNumber;
                ValidationResultInfo vri = _userRepository.Validate(user);
                if (!vri.IsValid)
                    throw new DomainValidationException(vri, "Failed to save user");
                var ccId = _costCentreRepository.Save(ds);
                user.CostCentre = ccId;
                var userId = _userRepository.Save(user);
                response.Success = true;
                response.ErrorInfo = "Successfully saved.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid user fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the user.";
                _log.Error("Error: An error occurred when saving the user.", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SalesmanUpdate(DistributorSalesmanItem dSItem)
        {
            var response = new ResponseBool { Success = false };
            DistributorSalesman ds = null;
            try
            {
                ds = _distributorSalesmanRepository.GetById(dSItem.MasterId) as DistributorSalesman;
                ds.Name = dSItem.Name;
                ds.CostCentreCode = dSItem.CostCentreCode;
                _distributorSalesmanRepository.Save(ds);
                response.Success = true;
                response.ErrorInfo = "Successfully saved.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the distributor salesman.";
                _log.Error("Error: An error occurred when saving the distributor salesman.", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SalesmanDeactivate(Guid salesmanId)
        {
            var response = new ResponseBool { Success = false };
            DistributorSalesman ds = null;
            try
            {
                DistributorSalesmanDeactivate(salesmanId);

                response.Success = true;
                response.ErrorInfo = "Successfully deactivated.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the item.";
                _log.Error("Error: An error occurred when saving the item.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage SalesmanRoutesUpdate(List<DistributorSalesmanRouteItem> routes)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                foreach (DistributorSalesmanRouteItem item in routes)
                {

                    SalesmanRoute sr = new SalesmanRoute(item.MasterId)
                    {
                        DistributorSalesmanRef = new CostCentreRef { Id = item.CostCentreMasterId },
                        Route = _routeRepository.GetById(item.RouteMasterId),
                    };

                    _salesmanRouteRepository.Save(sr);
                    _log.Info(String.Format("Salesman {0} assigned to Route {1} ", item.CostCentreMasterId, item.RouteMasterId));
                }
                response.Success = true;
                response.ErrorInfo = "Successfully saved.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Failed to assign Route(s) to a salesman.\nCause: "+ex.Message;
                _log.Error("Failed to assign Route(s) to a salesman ", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SalesmanActivate(Guid salesmanId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                DistributorSalesmanActivate(salesmanId);
                response.Success = true;
                response.ErrorInfo = "Successfully saved.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Failed to activate salesman.\n"+ex.Message;
                _log.Error("Failed to activate salesman", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SalesmanDelete(Guid salesmanId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                DistributorSalesmanDelete(salesmanId);
                response.Success = true;
                response.ErrorInfo = "Successfully deleted.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = " Failed to delete salesman.\nCause: "+ex.Message;
                _log.Error("Failed to delete salesman", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SalesmanRouteDeactivate(Guid salesmanRouteId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var ds = _salesmanRouteRepository.GetById(salesmanRouteId) as SalesmanRoute;
                if (ds != null)
                    _salesmanRouteRepository.SetInactive(ds);
                response.Success = true;
                response.ErrorInfo = "Successfully deactivated.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Failed to deactivate salesman route.";
                _log.Error("Failed to deactivate salesman route.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SalesmanRouteActivate(Guid salesmanRouteId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var ds = _salesmanRouteRepository.GetById(salesmanRouteId) as SalesmanRoute;
                if (ds != null)
                    _salesmanRouteRepository.SetActive(ds);
                response.Success = true;
                response.ErrorInfo = "Successfully activated.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Failed to activate salesman route.";
                _log.Error("Failed to activate salesman route", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage SalesmanRouteDelete(Guid salesmanRouteId)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var ds = _salesmanRouteRepository.GetById(salesmanRouteId) as SalesmanRoute;
                if (ds != null)
                    _salesmanRouteRepository.SetAsDeleted(ds);
                response.Success = true; 
                response.ErrorInfo = "Successfully deleted.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Failed to delete salesman route.";
                _log.Error("Failed to delete salesman route.", ex);

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ContactList(Guid contactOwnerid)
        {

            List<Contact> contacts = _contactRepository.GetAll().Where(n => n.ContactOwnerMasterId == contactOwnerid).ToList();


            List<ContactItem> contactList = contacts.Select(n => new ContactItem
        {
            BusinessPhone = n.BusinessPhone,
            ChildrenNames = n.ChildrenNames,
            City = n.City,
            Company = n.Company,
            ContactClassification = (int)n.ContactClassification,
            ContactOwnerType = n.ContactOwnerType,
            ContactTypeMasterId = n.ContactType != null ? n.ContactType.Id : Guid.Empty,
            ContactOwnerMasterId = n.ContactOwnerMasterId,
            DateCreated = n._DateCreated,
            DateOfBirth = n.DateOfBirth,
            DateLastUpdated = n._DateLastUpdated,
            Email = n.Email,
            Fax = n.Fax,
            Firstname = n.Firstname,
            HomePhone = n.HomePhone,
            HomeTown = n.HomeTown,
            StatusId = (int)n._Status,
            JobTitle = n.JobTitle,
            Lastname = n.Lastname,
            //MaritalStatusMasterId = n.MStatus != null ? n.MStatus.Id : Guid.Empty,
            MasterId = n.Id,
            MobilePhone = n.MobilePhone,
            PhysicalAddress = n.PhysicalAddress,
            PostalAddress = n.PostalAddress,
            SpouseName = n.SpouseName,
            WorkExtPhone = n.WorkExtPhone
        }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, contactList);
        }

        [HttpPost]
        public HttpResponseMessage ContactsAdd(List<ContactItem> contacts)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                foreach (var contactItem in contacts)
                {
                    if (contactItem.IsNew)
                    {
                        Contact contact = new Contact(contactItem.MasterId)
                        {
                            BusinessPhone = contactItem.BusinessPhone,
                            ChildrenNames = contactItem.ChildrenNames,
                            City = contactItem.City,
                            Company = contactItem.Company,
                            ContactClassification = (ContactClassification)contactItem.ContactClassification,
                            ContactOwnerType = (ContactOwnerType)contactItem.ContactOwnerType,
                            ContactOwnerMasterId = contactItem.ContactOwnerMasterId,
                            DateOfBirth = contactItem.DateOfBirth,
                            Email = contactItem.Email,
                            Fax = contactItem.Fax,
                            Firstname = contactItem.Firstname,
                            HomePhone = contactItem.HomePhone,
                            HomeTown = contactItem.HomeTown,
                            JobTitle = contactItem.JobTitle,
                            Lastname = contactItem.Lastname,
                            MobilePhone = contactItem.MobilePhone,
                            PhysicalAddress = contactItem.PhysicalAddress,
                            PostalAddress = contactItem.PostalAddress,
                            SpouseName = contactItem.SpouseName,
                            WorkExtPhone = contactItem.WorkExtPhone, 
                            MStatus = (MaritalStatas) contactItem.MaritalStatusMasterId,
                        };
                        if (contactItem.ContactTypeMasterId != Guid.Empty)
                            contact.ContactType = _contactTypeRepository.GetById(contactItem.ContactTypeMasterId);

                        _contactRepository.Save(contact);
                    }
                    else
                    {
                        _ContactUpdate(contactItem);
                    }
                }
                response.Success = true;
                response.ErrorInfo = "Successfully saved contacts.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid contact fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the contact information.\nCause: "+ex.Message;
                _log.Error("Error: An error occurred when saving the contact information.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpPost]
        public HttpResponseMessage ContactUpdate(ContactItem contactItem)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _ContactUpdate(contactItem);
                response.ErrorInfo = "Successfully saved contacts";
                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid contact fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex)
            {
                response.ErrorInfo = "Error: An error occurred when saving the contact information.";
                _log.Error("Error: An error occurred when saving the contact information.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ContactDeactivate(Guid contactId)
        {
            var response = new ResponseBool { Success = false };
            Contact contact = null;
            try
            {
                contact = _contactRepository.GetById(contactId);
                _contactRepository.SetInactive(contact);

                response.Success = true;
                response.ErrorInfo = "Successfully saved";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid contact fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Info(errorMsg, dve);
            }
            catch(Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deactivating the contact.";
                _log.Info("Error: An error occurred when deactivating the contact.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ContactActivate(Guid contactId)
        {
            var response = new ResponseBool { Success = false };
            Contact contact = null;
            try
            {
                contact = _contactRepository.GetById(contactId);
                _contactRepository.SetActive(contact);
                response.Success = true;
                response.ErrorInfo = "Successfully saved";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid contact fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                _log.Info(errorMsg, dve);
                response.ErrorInfo = errorMsg;
            }
            catch(Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when activating contact.";
                _log.Info("Error: An error occurred when activating contact.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ContactDelete(Guid contactId)
        {
            var response = new ResponseBool { Success = false };
            Contact contact = null;
            try
            {
                contact = _contactRepository.GetById(contactId);
                _contactRepository.SetAsDeleted(contact);
                response.Success = true;
                response.ErrorInfo = "Successfully deleted contact.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid contact fields. \n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                _log.Error(errorMsg, dve);
                response.ErrorInfo = errorMsg;
            }
            catch(Exception ex)
            {
                response.ErrorInfo = "Error: An error occured when deleting the contact.";
                _log.Error("Error: An error occured when deleting the contact.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage BankList()
        {
            List<BankItem> result  = _bankRepository.GetAll()
                .Select(n => new BankItem
            {
                MasterId = n.Id,
                Code = n.Code,
                DateCreated = n._DateCreated,
                DateLastUpdated = n._DateLastUpdated,
                Description = n.Description,
                StatusId = (int)n._Status,
                Name = n.Name
            }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, new List<BankItem>(new[]{new BankItem(), new BankItem()}));

        }

        [HttpPost]
        public HttpResponseMessage BankAdd(BankItem bankItem)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var bank = new Bank(bankItem.MasterId)
                {
                    Name = bankItem.Name,
                    Code = bankItem.Code,
                    Description = bankItem.Code,
                };
                 _bankRepository.Save(bank).ToString();//return id
                response.Success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add bank", ex);//return error message
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpPost]
        public HttpResponseMessage BankUpdate(BankItem bankItem)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var existingBank = _bankRepository.GetById(bankItem.MasterId);
                existingBank.Code = bankItem.Code;
                existingBank.Name = bankItem.Name;
                existingBank.Description = bankItem.Description;
                _bankRepository.Save(existingBank);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update bank",ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage BankBranchList(Guid bankId)
        {
            List<BankBranchItem> items = _bankBranchRepository
                .GetAll()
                .Where(n => n.Bank.Id == bankId)
            .Select(n => new BankBranchItem
            {
                Code = n.Code,
                //Bank = n.Bank,
                DateCreated = n._DateCreated,
                DateLastUpdated = n._DateLastUpdated,
                Description = n.Description,
                StatusId = (int)n._Status,
                MasterId = n.Id,
                Name = n.Name
            }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        [HttpPost]
        public HttpResponseMessage BankBranchUpdate(BankBranchItem bankBranchItem)
        {
            var response = new ResponseBool { Success = false };

            var existing = _bankBranchRepository.GetById(bankBranchItem.MasterId);

            //existing.Bank = bankBranchItem.Bank;
            existing.Code = bankBranchItem.Code;
            existing.Description = bankBranchItem.Description;
            existing.Name = bankBranchItem.Name;

            try
            {
                _bankBranchRepository.Save(existing);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to update bank branch", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage BankBranchAdd(BankBranchItem bankBranchItem)
        {
            var response = new ResponseBool { Success = false };
            var bankBranch = new BankBranch(bankBranchItem.MasterId)
            {
                Name = bankBranchItem.Name,
                Code = bankBranchItem.Code,
                Description = bankBranchItem.Description,
                //Bank = bankBranchItem.Bank,
            };

            try
            {
                 _bankBranchRepository.Save(bankBranch);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add bank branch", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage PurchasingClerkAdd(PurchasingClerkItem purchasingClerkItem)
        {
            var response = new ResponseBool { Success = false };
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    Route route = null;
                    var hub = _costCentreRepository.GetById(purchasingClerkItem.ParentCostCentreId);
                    var purchasingClerk = _costCentreFactory.CreateCostCentre(purchasingClerkItem.MasterId, CostCentreType.PurchasingClerk, hub)
                        as PurchasingClerk;
                    if (purchasingClerk == null) throw new NullReferenceException();
                    purchasingClerk.Name = purchasingClerkItem.Name;
                    purchasingClerk.CostCentreCode = purchasingClerkItem.CostCentreCode;
                    purchasingClerk.User = new User(purchasingClerkItem.UserItem.MasterId)
                                               {
                                                   CostCentre = purchasingClerkItem.MasterId,
                                                   Username = purchasingClerkItem.UserItem.Username,
                                                   Password = purchasingClerkItem.UserItem.Password,
                                                   PIN = purchasingClerkItem.UserItem.PIN,
                                                   Mobile = purchasingClerkItem.UserItem.Mobile,
                                                   UserType = (UserType) purchasingClerkItem.UserItem.UserType,
                                                   TillNumber = purchasingClerkItem.UserItem.TillNumber,
                                               };
                    var query = new QueryPurchasingClerkRoute();
                    query.ShowInactive = true;
                    var existingAssignedRoutes = _purchasingClerkRouteRepository.Query(query)
                        .Where(n => n.PurchasingClerkRef.Id == purchasingClerkItem.MasterId);

                    //var existingAssignedRoutes = _purchasingClerkRouteRepository.GetAll(true)
                    //    .Where(n => n.PurchasingClerkRef.Id == purchasingClerkItem.MasterId);
                    var deletedRouteAssignments = existingAssignedRoutes
                        .Where(n => purchasingClerkItem.PurchasingClerkRouteItems.Select(x => x.RouteId).All(x => x != n.Route.Id));
                    foreach (var item in purchasingClerkItem.PurchasingClerkRouteItems)
                    {
                        route = _routeRepository.GetById(item.RouteId);
                        if(existingAssignedRoutes.Any(p=>p.Route==route))
                        {
                            continue;
                        }
                        var pcr = new PurchasingClerkRoute(item.MasterId)
                                                       {
                                                           Route = route,
                                                           PurchasingClerkRef = new CostCentreRef {Id = purchasingClerkItem.MasterId}
                                                       };
                        //_routeRepository.Save(route);
                        purchasingClerk.PurchasingClerkRoutes.Add(pcr);
                    }

                    foreach (var item in deletedRouteAssignments)
                    {
                        _purchasingClerkRouteRepository.SetAsDeleted(item);
                    }

                    purchasingClerk._SetStatus(EntityStatus.Active);
                    _purchasingClerkRepository.Save(purchasingClerk);

                    response.Success = true;
                    response.ErrorInfo = "Successfully added purchasing clerk.";
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid fields.\n",
                                                                              (current, msg) =>
                                                                              current +
                                                                              ("\t- " + msg.ErrorMessage + "\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when creating or updating purchasing clerk.";
                    _log.Error("Error: An error occurred when creating or updating the purchasing clerk.", ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [HttpPost]
        public HttpResponseMessage PurchasingClerkActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _purchasingClerkRepository.GetById(id);
                if (cs != null && cs._Status == EntityStatus.Inactive)
                {
                    _purchasingClerkRepository.SetActive(cs);
                    response.ErrorInfo = "Successfully activated purchasing clerk";
                }
                else if (cs != null && cs._Status == EntityStatus.Active)
                {
                    _commoditySupplierRepository.SetInactive(cs);
                    response.ErrorInfo = "Successfully deactivated purchasing  clerk";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage PurchasingClerkDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _purchasingClerkRepository.GetById(id);
                if (cs != null) _purchasingClerkRepository.SetAsDeleted(cs);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted purchasing clerk";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage PurchasingClerkRouteAdd(List<PurchasingClerkRouteItem> items)
        {
            var response = new ResponseBool { Success = false };
            PurchasingClerkRoute pcr = null;
            try
            {
                foreach (var item in items)
                {
                    var route = _routeRepository.GetById(item.RouteId);
                    pcr = new PurchasingClerkRoute(item.MasterId)
                    {
                        PurchasingClerkRef = new CostCentreRef { Id = item.PurchasingClerkCostCentreId },
                        Route = route
                    };

                    pcr._SetStatus(EntityStatus.Active);
                    _purchasingClerkRouteRepository.Save(pcr);
                    
                    _routeRepository.Save(route);
                }
                response.Success = true;
                response.ErrorInfo = "Successfully added purchasing clerk route.";
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when creating or updating purchasing clerk route.";
                _log.Error("Error: An error occurred when creating or updating the purchasing clerk route.", ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage PurchasingClerkRouteActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var csr = _purchasingClerkRouteRepository.GetById(id);
                if (csr != null && csr._Status == EntityStatus.Inactive)
                {
                    _purchasingClerkRouteRepository.SetActive(csr);
                    response.ErrorInfo = "Successfully activated purchasing clerk route";
                }
                else if (csr != null && csr._Status == EntityStatus.Active)
                {
                    _purchasingClerkRouteRepository.SetInactive(csr);
                    response.ErrorInfo = "Successfully deactivated purchasing  clerk route";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage PurchasingClerkRouteDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var csr = _purchasingClerkRouteRepository.GetById(id);
                if (csr != null) _purchasingClerkRouteRepository.SetAsDeleted(csr);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted purchasing clerk route";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage CommoditySupplierAdd(CommoditySupplier commoditySupplier)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _commoditySupplierRepository.Save(commoditySupplier);

                response.Success = true;
                response.ErrorInfo = "Commodity supplier successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid commodity supplier fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the commodity supplier.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the commodity supplier.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommoditySupplierActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _commoditySupplierRepository.GetById(id);
                if (cs != null && cs._Status == EntityStatus.Inactive)
                {
                    _commoditySupplierRepository.SetActive(cs);
                    response.ErrorInfo = "Successfully activated commodity supplier";
                }
                else if (cs != null && cs._Status == EntityStatus.Active)
                {
                    _commoditySupplierRepository.SetInactive(cs);
                    response.ErrorInfo = "Successfully deactivated commodity supplier";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommoditySupplierDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _commoditySupplierRepository.GetById(id);
                if (cs != null) _commoditySupplierRepository.SetAsDeleted(cs);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted commodity supplier";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage CommodityProducerAdd(CommodityProducer commodityProducer)
        {
            var response = new ResponseBool { Success = false };
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    _commodityProducerRepository.Save(commodityProducer);


                    var existingAllocationForThisProducer = _masterDataAllocationRepository.GetByAllocationType(
                        MasterDataAllocationType.CommodityProducerCentreAllocation)
                        .Where(n => n.EntityAId == commodityProducer.Id);

                    var unallocated =
                        existingAllocationForThisProducer.Where(
                            n =>
                            commodityProducer.CommodityProducerCentres.Select(c => c.Id).All(cId => n.EntityBId != cId));

                    foreach (var centre in commodityProducer.CommodityProducerCentres)
                    {
                        var allocation = new MasterDataAllocation(Guid.NewGuid())
                                             {
                                                 _Status = EntityStatus.Active,
                                                 AllocationType =
                                                     MasterDataAllocationType.CommodityProducerCentreAllocation,
                                                 EntityAId = commodityProducer.Id,
                                                 EntityBId = centre.Id
                                             };
                        _masterDataAllocationRepository.Save(allocation);
                    }

                    foreach (var allocation in unallocated)
                    {
                        _masterDataAllocationRepository.DeleteAllocation(allocation.Id);
                    }

                    response.Success = true;
                    response.ErrorInfo = "Commodity producer successfully added.";
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg =
                        dve.ValidationResults.Results.Aggregate("Error: Invalid commodity producer fields.\n",
                                                                (current, msg) =>
                                                                current + ("\t- " + msg.ErrorMessage + "\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when saving the commodity producer.\n" +
                                         ex.ToString();
                    _log.Error("Error: An error occurred when saving the commodity supplier.", ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage CommodityProducerListAdd(List<CommodityProducer> commodityProducersList)
        {
            var response = new ResponseBool { Success = false };
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    foreach (var commodityProducer in commodityProducersList)
                    {
                        _commodityProducerRepository.Save(commodityProducer);


                        var existingAllocationForThisProducer = _masterDataAllocationRepository.GetByAllocationType(
                            MasterDataAllocationType.CommodityProducerCentreAllocation)
                            .Where(n => n.EntityAId == commodityProducer.Id);

                        var unallocated =
                            existingAllocationForThisProducer.Where(
                                n =>
                                commodityProducer.CommodityProducerCentres.Select(c => c.Id).All(
                                    cId => n.EntityBId != cId));

                        foreach (var centre in commodityProducer.CommodityProducerCentres)
                        {
                            var allocation = new MasterDataAllocation(Guid.NewGuid())
                                                 {
                                                     _Status = EntityStatus.Active,
                                                     AllocationType =
                                                         MasterDataAllocationType.CommodityProducerCentreAllocation,
                                                     EntityAId = commodityProducer.Id,
                                                     EntityBId = centre.Id
                                                 };
                            _masterDataAllocationRepository.Save(allocation);
                        }

                        foreach (var allocation in unallocated)
                        {
                            _masterDataAllocationRepository.DeleteAllocation(allocation.Id);
                        }
                    }
                    response.Success = true;
                    response.ErrorInfo = "Commodity producer successfully added.";
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg =
                        dve.ValidationResults.Results.Aggregate("Error: Invalid commodity producer fields.\n",
                                                                (current, msg) =>
                                                                current + ("\t- " + msg.ErrorMessage + "\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when saving the commodity producer.\n" +
                                         ex.ToString();
                    _log.Error("Error: An error occurred when saving the commodity supplier.", ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommodityProducerActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _commodityProducerRepository.GetById(id);
                if (cs != null && cs._Status == EntityStatus.Inactive)
                {
                    _commodityProducerRepository.SetActive(cs);
                    response.ErrorInfo = "Successfully activated commodity producer";
                }
                else if (cs != null && cs._Status == EntityStatus.Active)
                {
                    _commodityProducerRepository.SetInactive(cs);
                    response.ErrorInfo = "Successfully deactivated commodity producer";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommodityProducerDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var commodityProducer = _commodityProducerRepository.GetById(id);
                if (commodityProducer != null) _commodityProducerRepository.SetAsDeleted(commodityProducer);

                var existingAllocationForThisProducer = _masterDataAllocationRepository.GetByAllocationType(
                   MasterDataAllocationType.CommodityProducerCentreAllocation, commodityProducer.Id);

                foreach (var allocation in existingAllocationForThisProducer)
                {
                    _masterDataAllocationRepository.DeleteAllocation(allocation.Id);
                }

                response.Success = true;
                response.ErrorInfo = "Successfully deleted commodity producer";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage CommodityOwnerAdd(CommodityOwnerItem commodityOwnerItem)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var commodityOwner = new CommodityOwner(commodityOwnerItem.MasterId)
                                         {
                                             Surname = commodityOwnerItem.Surname,
                                             _Status = (EntityStatus) commodityOwnerItem.StatusId,
                                             PostalAddress = commodityOwnerItem.PostalAddress,
                                             PinNo = commodityOwnerItem.PinNo,
                                             PhysicalAddress = commodityOwnerItem.PhysicalAddress,
                                             OfficeNumber = commodityOwnerItem.OfficeNumber,
                                             BusinessNumber = commodityOwnerItem.BusinessNumber,
                                             PhoneNumber = commodityOwnerItem.PhoneNumber,
                                             IdNo = commodityOwnerItem.IdNo,
                                             LastName = commodityOwnerItem.LastName,
                                             MaritalStatus = (MaritalStatas) commodityOwnerItem.MaritalStatasMasterId,
                                             Code = commodityOwnerItem.Code,
                                             CommodityOwnerType =
                                                 _commodityOwnerTypeRepository.GetById(
                                                     commodityOwnerItem.CommodityOwnerTypeMasterId),
                                             CommoditySupplier =
                                                 _commoditySupplierRepository.GetById(
                                                     commodityOwnerItem.CommoditySupplierMasterId) as CommoditySupplier,
                                             DateOfBirth = commodityOwnerItem.DateOfBirth,
                                             Description = commodityOwnerItem.Description,
                                             Email = commodityOwnerItem.Email,
                                             FaxNumber = commodityOwnerItem.FaxNumber,
                                             FirstName = commodityOwnerItem.FirstName,
                                             Gender = (Gender) commodityOwnerItem.GenderId,
                                         };
                _commodityOwnerRepository.Save(commodityOwner);
                var contact = _contactRepository.GetByContactsOwnerId(commodityOwnerItem.CommoditySupplierMasterId).FirstOrDefault();
                if(contact==null)
                {
                    contact = new Contact(Guid.NewGuid());
                }
                contact.BusinessPhone = commodityOwnerItem.BusinessNumber;
                    contact.ChildrenNames = "";
                    contact.City = "";
                    contact.Company = "";
                    contact.ContactClassification=ContactClassification.PrimaryContact;
                    contact.ContactOwnerMasterId = commodityOwnerItem.CommoditySupplierMasterId;
                    contact.ContactOwnerType=ContactOwnerType.CommoditySupplier;
                    contact.ContactType = _contactTypeRepository.GetById(
                        commodityOwnerItem.CommodityOwnerTypeMasterId);
                    contact.DateOfBirth = commodityOwnerItem.DateOfBirth;
                    contact.Email = commodityOwnerItem.Email;
                    contact.Fax = commodityOwnerItem.FaxNumber;
                    contact.Firstname = commodityOwnerItem.FirstName;
                    contact.HomePhone = commodityOwnerItem.BusinessNumber;
                    contact.HomeTown = "";
                    contact.JobTitle = "Farmer";
                    contact.Lastname = commodityOwnerItem.LastName;
                    contact.MobilePhone = commodityOwnerItem.PhoneNumber;
                    contact.BusinessPhone = commodityOwnerItem.BusinessNumber;
                    contact.PhysicalAddress = "";
                    contact.PostalAddress = commodityOwnerItem.PhysicalAddress;
                    contact.SpouseName = "";
                _contactRepository.Save(contact);

                response.Success = true;
                response.ErrorInfo = "Commodity producer successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid commodity owner fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage CommodityOwnerListAdd(List<CommodityOwnerItem> commodityOwnerItemsList)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                foreach (var commodityOwnerItem in commodityOwnerItemsList)
                {
                    var commodityOwner = new CommodityOwner(commodityOwnerItem.MasterId)
                                             {
                                                 Surname = commodityOwnerItem.Surname,
                                                 _Status = (EntityStatus) commodityOwnerItem.StatusId,
                                                 PostalAddress = commodityOwnerItem.PostalAddress,
                                                 PinNo = commodityOwnerItem.PinNo,
                                                 PhysicalAddress = commodityOwnerItem.PhysicalAddress,
                                                 OfficeNumber = commodityOwnerItem.OfficeNumber,
                                                 BusinessNumber = commodityOwnerItem.BusinessNumber,
                                                 PhoneNumber = commodityOwnerItem.PhoneNumber,
                                                 IdNo = commodityOwnerItem.IdNo,
                                                 LastName = commodityOwnerItem.LastName,
                                                 MaritalStatus =
                                                     (MaritalStatas) commodityOwnerItem.MaritalStatasMasterId,
                                                 Code = commodityOwnerItem.Code,
                                                 CommodityOwnerType =
                                                     _commodityOwnerTypeRepository.GetById(
                                                         commodityOwnerItem.CommodityOwnerTypeMasterId),
                                                 CommoditySupplier =
                                                     _commoditySupplierRepository.GetById(
                                                         commodityOwnerItem.CommoditySupplierMasterId) as
                                                     CommoditySupplier,
                                                 DateOfBirth = commodityOwnerItem.DateOfBirth,
                                                 Description = commodityOwnerItem.Description,
                                                 Email = commodityOwnerItem.Email,
                                                 FaxNumber = commodityOwnerItem.FaxNumber,
                                                 FirstName = commodityOwnerItem.FirstName,
                                                 Gender = (Gender) commodityOwnerItem.GenderId,
                                             };
                    _commodityOwnerRepository.Save(commodityOwner);
                }
                response.Success = true;
                response.ErrorInfo = "Commodity producer successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid commodity owner fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                response.Success = false;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                response.Success = false;
                _log.Error("Error: An error occurred when saving the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommodityOwnerActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _commodityOwnerRepository.GetById(id);
                if (cs != null && cs._Status == EntityStatus.Inactive)
                {
                    _commodityOwnerRepository.SetActive(cs);
                    response.ErrorInfo = "Successfully activated commodity owner";
                }
                else if (cs != null && cs._Status == EntityStatus.Active)
                {
                    _commodityOwnerRepository.SetInactive(cs);
                    response.ErrorInfo = "Successfully deactivated commodity owner";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CommodityOwnerDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _commodityOwnerRepository.GetById(id);
                if (cs != null) _commodityOwnerRepository.SetAsDeleted(cs);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted commodity owner";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage CentreAdd(Centre centre)
        {
            var response = new ResponseBool { Success = false };
            using (TransactionScope scope = TransactionUtils.CreateTransactionScope())
            {
                try
                {
                    _centreRepository.Save(centre);

                    response.Success = true;
                    response.ErrorInfo = "Centre successfully added.";
                    scope.Complete();
                }
                catch (DomainValidationException dve)
                {
                    string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid centre fields.\n",
                                                                              (current, msg) =>
                                                                              current +
                                                                              ("\t- " + msg.ErrorMessage + "\n"));
                    response.ErrorInfo = errorMsg;
                    _log.Error(errorMsg, dve);
                }
                catch (Exception ex) //any other
                {
                    response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                    _log.Error("Error: An error occurred when saving the entity.", ex);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CentreActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var centre = _centreRepository.GetById(id);
                if (centre != null && centre._Status == EntityStatus.Inactive)
                {
                    _centreRepository.SetActive(centre);
                    response.ErrorInfo = "Successfully activated centre";
                }
                else if (centre != null && centre._Status == EntityStatus.Active)
                {
                    _centreRepository.SetInactive(centre);
                    response.ErrorInfo = "Successfully deactivated centre";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage CentreDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var centre = _centreRepository.GetById(id);
                if (centre != null) _centreRepository.SetAsDeleted(centre);

                var existingAllocationForThisCentre = _masterDataAllocationRepository.GetByAllocationType(
                    MasterDataAllocationType.RouteCentreAllocation, Guid.Empty, centre.Id);

                foreach (var allocation in existingAllocationForThisCentre)
                {
                    _masterDataAllocationRepository.DeleteAllocation(allocation.Id);
                }

                response.Success = true;
                response.ErrorInfo = "Successfully deleted centre";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage StoreAdd(Store store)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _storeRepository.Save(store);

                response.Success = true;
                response.ErrorInfo = "Store successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid store fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage StoreActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var store = _storeRepository.GetById(id);
                if (store != null && store._Status == EntityStatus.Inactive)
                {
                    _storeRepository.SetActive(store);
                    response.ErrorInfo = "Successfully activated store.";
                }
                else if (store != null && store._Status == EntityStatus.Active)
                {
                    _storeRepository.SetInactive(store);
                    response.ErrorInfo = "Successfully deactivated store.";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage StoreDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var store = _storeRepository.GetById(id);
                if (store != null) _storeRepository.SetAsDeleted(store);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted store";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage ContainerAdd(SourcingContainer container)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _equipmentRepository.Save(container);

                response.Success = true;
                response.ErrorInfo = "Cotainer successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid containrt fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ContainerActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _equipmentRepository.GetById(id);
                if (cs != null && cs._Status == EntityStatus.Inactive)
                {
                    _equipmentRepository.SetActive(cs);
                    response.ErrorInfo = "Successfully activated container";
                }
                else if (cs != null && cs._Status == EntityStatus.Active)
                {
                    _equipmentRepository.SetInactive(cs);
                    response.ErrorInfo = "Successfully deactivated container";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage ContainerDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _equipmentRepository.GetById(id);
                if (cs != null) _equipmentRepository.SetAsDeleted(cs);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted container";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage EquipmentAdd(Equipment equipment)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _equipmentRepository.Save(equipment);

                response.Success = true;
                response.ErrorInfo = string.Format("Equipment of type {0} successfully added.", equipment.EquipmentType.ToString());
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid equipment fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage EquipmentActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _equipmentRepository.GetById(id);
                if (cs != null && cs._Status == EntityStatus.Inactive)
                {
                    _equipmentRepository.SetActive(cs);
                    response.ErrorInfo = "Successfully activated equipment";
                }
                else if (cs != null && cs._Status == EntityStatus.Active)
                {
                    _equipmentRepository.SetInactive(cs);
                    response.ErrorInfo = "Successfully deactivated equipment";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage EquipmentDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var cs = _equipmentRepository.GetById(id);
                if (cs != null) _equipmentRepository.SetAsDeleted(cs);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted equipment";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage VehicleAdd(Vehicle vehicle)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _vehicleRepository.Save(vehicle);

                response.Success = true;
                response.ErrorInfo = "Vehicle successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid vehicle fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the entity.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage VehicleActivateOrDeactivate(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var vehicle = _vehicleRepository.GetById(id);
                if (vehicle != null && vehicle._Status == EntityStatus.Inactive)
                {
                    _vehicleRepository.SetActive(vehicle);
                    response.ErrorInfo = "Successfully activated vehicle.";
                }
                else if (vehicle != null && vehicle._Status == EntityStatus.Active)
                {
                    _vehicleRepository.SetInactive(vehicle);
                    response.ErrorInfo = "Successfully deactivated vehicle.";
                }

                response.Success = true;
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when editing the entity.";
                _log.Error("Error: An error occurred when editing the entiti.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpGet]
        public HttpResponseMessage VehicleDelete(Guid id)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                var vehicle = _vehicleRepository.GetById(id);
                if (vehicle != null) _vehicleRepository.SetAsDeleted(vehicle);

                response.Success = true;
                response.ErrorInfo = "Successfully deleted vehicle";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        [HttpPost]
        public HttpResponseMessage SaveOutletFarmerMapping(CostCentreMapping centreMapping)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                _costCentreRepository.SaveMapping(centreMapping);

                response.Success = true;
                response.ErrorInfo = "Successfully Save Farmer Outlet Mapping";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage MasterDataAllocationAdd(List<MasterDataAllocation> allocations)
        {
            var response = new ResponseBool {Success = false};
            try
            {

                foreach (var allocation in allocations)
                {
                    _masterDataAllocationRepository.Save(allocation);
                }

                response.Success = true;
                response.ErrorInfo = "Allocation successfully added.";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: Invalid allocation fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when saving the commodity producer.\n" + ex.ToString();
                _log.Error("Error: An error occurred when saving the commodity supplier.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage MasterDataAllocationDelete(List<Guid> ids)
        {
            var response = new ResponseBool { Success = false };
            try
            {
                foreach (Guid id in ids)
                {
                    var cs = _masterDataAllocationRepository.GetById(id);
                    if (cs != null) _masterDataAllocationRepository.DeleteAllocation(id);
                }
                response.Success = true;
                response.ErrorInfo = "Successfully deleted allocation";
            }
            catch (DomainValidationException dve)
            {
                string errorMsg = dve.ValidationResults.Results.Aggregate("Error: In entity fields.\n",
                                                                          (current, msg) =>
                                                                          current + ("\t- " + msg.ErrorMessage + "\n"));
                response.ErrorInfo = errorMsg;
                _log.Error(errorMsg, dve);
            }
            catch (Exception ex) //any other
            {
                response.ErrorInfo = "Error: An error occurred when deleting the entity.";
                _log.Error("Error: An error occurred when deleting the entity.", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        private void _ContactUpdate(ContactItem contactItem)
        {
            Contact contact = _contactRepository.GetById(contactItem.MasterId);

            contact.BusinessPhone = contactItem.BusinessPhone;
            contact.ChildrenNames = contactItem.ChildrenNames;
            contact.City = contactItem.City;
            contact.Company = contactItem.Company;
            contact.ContactClassification = (ContactClassification)contactItem.ContactClassification;
            contact.ContactOwnerType = (ContactOwnerType)contactItem.ContactOwnerType;
            contact.ContactOwnerMasterId = contactItem.ContactOwnerMasterId;
            contact.DateOfBirth = contactItem.DateOfBirth;
            contact.Email = contactItem.Email;
            contact.Fax = contactItem.Fax;
            contact.Firstname = contactItem.Firstname;
            contact.HomePhone = contactItem.HomePhone;
            contact.HomeTown = contactItem.HomeTown;
            contact.JobTitle = contactItem.JobTitle;
            contact.Lastname = contactItem.Lastname;
            contact.MobilePhone = contactItem.MobilePhone;
            contact.PhysicalAddress = contactItem.PhysicalAddress;
            contact.PostalAddress = contactItem.PostalAddress;
            contact.SpouseName = contactItem.SpouseName;

            contact.WorkExtPhone = contactItem.WorkExtPhone;
            contact.MStatus = (MaritalStatas) contactItem.MaritalStatusMasterId;
            if (contactItem.ContactTypeMasterId != Guid.Empty)
                contact.ContactType = _contactTypeRepository.GetById(contactItem.ContactTypeMasterId);
            else
                contact.ContactType = null;

            _contactRepository.Save(contact);



        }

        private void DistributorSalesmanDeactivate(Guid id)
        {
            DistributorSalesman ds = _distributorSalesmanRepository.GetById(id) as DistributorSalesman;
            _costCentreRepository.SetInactive(ds);
        }

        private void DistributorSalesmanActivate(Guid id)
        {
            DistributorSalesman ds = _distributorSalesmanRepository.GetById(id) as DistributorSalesman;
            _costCentreRepository.SetActive(ds);
        }

        private void DistributorSalesmanDelete(Guid id)
        {
            DistributorSalesman ds = _distributorSalesmanRepository.GetById(id) as DistributorSalesman;
            _costCentreRepository.SetAsDeleted(ds);
        }

        public HttpResponseMessage GetFarmerTotalCummWeight(Guid farmerId)
        {
            decimal cummWeight = _transactionsSummary.GetFarmerCummulativeWeightDelivered(farmerId);

            return Request.CreateResponse(HttpStatusCode.OK, cummWeight);
        }

        [HttpPost]
        public HttpResponseMessage GetFarmerSummaryDetails(QueryFarmerDetails farmerQuery)
        {
            var serviceType = farmerQuery.ServiceType;
            var summary = new FarmerSummaryDetail();
            switch (serviceType)
            {
                case ServiceType.Cummulative:
                    summary= _transactionsSummary.GetFarmerSummary(farmerQuery);
                    break;
                case ServiceType.Other:
                    break;
            }
            

            return Request.CreateResponse(HttpStatusCode.OK, summary);
        }

        public HttpResponseMessage GetFarmerTotalCummWeightByCode(string farmerCode)
        {
            
            var farmerSummary = _transactionsSummary.GetFarmerCummulativeWeightDeliveredByCode(farmerCode);
            return Request.CreateResponse(HttpStatusCode.OK, farmerSummary);
        }

        public HttpResponseMessage GetFarmerSummary(Guid farmerId)
        {
            var farmer = _commodityOwnerRepository.GetById(farmerId);
            FarmerSummary fs = new FarmerSummary
            {
                Code = farmer.Code,
                FullName = farmer.FullName,
                Id = farmer.Id,
            };
            var factory = _commoditySupplierRepository.GetById(farmer.CommoditySupplier.ParentCostCentre.Id, true);
            if (factory != null)
            {
                fs.FactoryId = factory.Id;
                fs.FactoryCode = factory.CostCentreCode;
            }

            var summary = _transactionsSummary.GetFarmerSummary(farmerId);

            PropertyInfo totalLastMonthWeightFeld = summary.GetType().GetProperty("totalLastMonthWeight");
            var totalLastMonthWeight = totalLastMonthWeightFeld.GetValue(summary);
            PropertyInfo qtyLastDeliveredField = summary.GetType().GetProperty("qtyLastDelivered");
            var qtyLastDelivered = qtyLastDeliveredField.GetValue(summary);
            PropertyInfo lastTranDateField = summary.GetType().GetProperty("lastTranDate");
            var lastTranDate = lastTranDateField.GetValue(summary);

            fs.LastDeliverlyDate = (DateTime) lastTranDate;
            fs.MonthlyCummWeight = Convert.ToDecimal(totalLastMonthWeight);
            fs.QtyLastDelivered = Convert.ToDecimal(qtyLastDelivered);

            return Request.CreateResponse(HttpStatusCode.OK, fs);
        }

        public HttpResponseMessage GetAllFarmers()
        {
            var farmers = _commodityOwnerRepository.GetAll(false);
            List<FarmerSummary> retlist = new List<FarmerSummary>();

            foreach (var farmer in farmers)
            {
                FarmerSummary fs = new FarmerSummary
                                       {
                                           Code = farmer.Code,
                                           FullName = farmer.FullName,
                                           Id = farmer.Id,
                                       };
                var factory = _commoditySupplierRepository.GetById(farmer.CommoditySupplier.ParentCostCentre.Id, true);
                if(factory!= null)
                {
                    fs.FactoryId = factory.Id;
                    fs.FactoryCode = factory.CostCentreCode;
                }
                retlist.Add(fs);
            }
            return Request.CreateResponse(HttpStatusCode.OK, retlist);
        }
        public HttpResponseMessage GetAllClientMembers()
        {
            var members = _pgRepository.GetClientMembers();

            return Request.CreateResponse(HttpStatusCode.OK, members);
        }
    }
}
