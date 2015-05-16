using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Repository.MasterData;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Data.EF;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository
{
    internal class ClientMasterDataTrackerRepository : RepositoryMasterBase<ClientMasterDataTracker>, IClientMasterDataTrackerRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IMasterDataEnvelopeBuilder _envelopeBuilder;
        private ICostCentreRepository _costCentreRepository;
        private ICostCentreApplicationRepository _costCentreApplicationRepository;
        private IInventoryRepository _inventoryRepository;
        private IPaymentTrackerRepository _paymentTrackerRepository;

        public ClientMasterDataTrackerRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IMasterDataEnvelopeBuilder envelopeBuilder, ICostCentreRepository costCentreRepository, ICostCentreApplicationRepository costCentreApplicationRepository, IInventoryRepository inventoryRepository, IPaymentTrackerRepository paymentTrackerRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _envelopeBuilder = envelopeBuilder;
            _costCentreRepository = costCentreRepository;
            _costCentreApplicationRepository = costCentreApplicationRepository;
            _inventoryRepository = inventoryRepository;
            _paymentTrackerRepository = paymentTrackerRepository;
        }

        public Guid Save(ClientMasterDataTracker entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating ClientMasterDataTracker");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid ClientMasterDataTracker");
                throw new DomainValidationException(vri, "Failed to save invalid ClientMasterDataTracker");
            }

            tblClientMasterDataTracker reg = _ctx.tblClientMasterDataTracker.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (reg == null)
            {
                reg = new tblClientMasterDataTracker
                {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    Id= entity.Id
                };
                _ctx.tblClientMasterDataTracker.AddObject(reg);
            }
            else
            {  
             
               
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (reg.IM_Status != (int)entityStatus)
                reg.IM_Status = (int)entity._Status;
            reg.DateTimePushConfirmed = entity.DateTimePushConfirmed;
            reg.DateTimePushed = entity.DateTimePushed;
            reg.MasterDataId = entity.MasterDataId;
            reg.CostCentreApplicationId = entity.CostCentreApplicationId;

            reg.IM_DateLastUpdated = date;

            _log.Debug("trackerlog:" + reg.IM_DateCreated + "," + reg.IM_DateLastUpdated + "," + reg.IM_Status + ", " + reg.DateTimePushConfirmed + "," + reg.DateTimePushed);
            _log.Debug("Saving ClientMasterDataTracker");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblClientMasterDataTracker.Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, reg.Id));
            _log.DebugFormat("Successfully saved item id:{0}", reg.Id);
            return reg.Id;
        }

        public void SetAsDeleted(ClientMasterDataTracker entity)
        {
            throw new NotImplementedException();
        }

        public ClientMasterDataTracker GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting ClientMasterDataTracker by ID: {0}", Id);
            ClientMasterDataTracker entity = (ClientMasterDataTracker)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblClientMasterDataTracker.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "ClientMasterDataTracker-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ClientMasterDataTrackerList"; }
        }

        public override IEnumerable<ClientMasterDataTracker> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All ClientMasterDataTrackers; include Deactivated: {0}", includeDeactivated);

            IList<ClientMasterDataTracker> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ClientMasterDataTracker>(ids.Count);
                foreach (Guid id in ids)
                {
                    ClientMasterDataTracker entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblClientMasterDataTracker.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ClientMasterDataTracker p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public void SetInactive(ClientMasterDataTracker entity)
        {
            _log.Debug("Inactivating ClientMasterDataTracker");
            bool dependenciesPresent = false; //no dependencies with entities currently done

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblClientMasterDataTracker reg = _ctx.tblClientMasterDataTracker.First(n => n.Id == entity.Id);
                if (reg == null || reg.IM_Status==(int)EntityStatus.Inactive)
                {//not existing or already deactivated.
                    return;
                }
                reg.IM_Status = (int)EntityStatus.Inactive;// false;
                reg.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblClientMasterDataTracker.Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, reg.Id));
                
            }

        }

        public void SetActive(ClientMasterDataTracker entity)
        {
            throw new NotImplementedException();
        }

        public ValidationResultInfo Validate(ClientMasterDataTracker itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }

       

        public bool DoesCostCentreApplicationNeedToSync(Guid costCentreApplicationId, VirtualCityApp vcAppId)
        {
            foreach (MasterDataCollective masterdata in SyncMasterDataCollective.GetMasterDataCollective(vcAppId))//change to linq l8r
            {
                if (!isUpdated(costCentreApplicationId, masterdata)) 
                    return true;
            }
            return false;
        }

        private MasterDataEnvelope getEnvelope(Guid costCentreApplicationId, MasterDataCollective masterdata, DateTime? dateChosen)
        {
            _log.Debug("ClientMasterDataTrackerRepository:getEnvelope");
            DateTime dateSince = DateTime.Now.AddYears(-40);
            if (dateChosen != null)
                dateSince = dateChosen.Value;

            //if no entry in clientmasterdatatracker then create a default entry
            if (_ctx.tblClientMasterDataTracker.Count(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId == (int)masterdata) == 0)
            {
                tblClientMasterDataTracker phitem = new tblClientMasterDataTracker
                {
                    Id =Guid.NewGuid(),
                    IM_DateCreated = DateTime.Now,
                    IM_DateLastUpdated = DateTime.Now,
                    IM_Status =(int)EntityStatus.Active,// true,
                    MasterDataId = (int)masterdata,
                    CostCentreApplicationId = costCentreApplicationId,
                    DateTimePushed = DateTime.Now.AddYears(-40)

                };
                _ctx.tblClientMasterDataTracker.AddObject(phitem);
                try//remove once application (id) logic is stabilized
                {
                    _ctx.SaveChanges();
                }
                catch { }
            }
            else
            {
                if (dateChosen == null)
                {
                    //get cutoff date based on datatimepushed confirmed
                    DateTime? dt = _ctx.tblClientMasterDataTracker.Where(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId == (int)masterdata)
                        .Max(n => n.DateTimePushConfirmed);
                    if (dt != null)
                        dateSince = dt.Value;
                }
            }



            MasterDataEnvelope envelope = null;
            envelope = BuildEnvelope(costCentreApplicationId, masterdata, dateSince, envelope);
            return envelope;
        }

        private MasterDataEnvelope BuildEnvelope(Guid costCentreApplicationId, MasterDataCollective masterdata, DateTime dateSince, MasterDataEnvelope envelope)
        {
            CostCentre cct = null;
            CostCentreApplication ccApp = _costCentreApplicationRepository.GetById(costCentreApplicationId);
            if (ccApp != null)
                cct = _costCentreRepository.GetById(ccApp.CostCentreId);
            //DateTime date = qry.Max(n => n.IM_DateLastUpdated);
            switch (masterdata)
            {
                case MasterDataCollective.DistributorPendingDispatchWarehouse:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildDistributorPendingDispatchWarehouse(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.PricingTier:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildPricingTier(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.SaleProduct:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSaleProduct(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ReturnableProduct:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildReturnableProduct(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ConsolidatedProduct:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildConsolidatedProduct(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Country:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCountry(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductBrand:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductBrand(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Area:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildArea(dateSince, masterdata, cct);
                    break;
               
                case MasterDataCollective.Contact:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildContact(dateSince, masterdata, cct);
                    
                    break;
                case MasterDataCollective.Distributor:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildDistributor(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Outlet:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildOutlet(dateSince,masterdata, cct);
                    break;

                case MasterDataCollective.OutletCategory:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildOutletCategory(dateSince,masterdata, cct);
                    break;

                case MasterDataCollective.OutletType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildOutletType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Pricing:
                    //throw new Exception("MasterDataCollective.Pricing");
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildPricing(dateSince, masterdata, cct);
                    break;
               
                case MasterDataCollective.Producer:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProducer(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductFlavour:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductFlavour(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductPackagingType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductPackagingType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Region:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildRegion(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Route:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildRoute(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.SocioEconomicStatus:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSocioEconomicStatus(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Territory:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildTerritory(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.User:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildUser(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.VatClass:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildVatClass(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductPackaging:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductPackaging(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.DistributorSalesman:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildDistributorSalesman(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ChannelPackaging:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildChannelPackaging(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.FreeOfChargeDiscount:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildFreeOfChargeDiscount(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Competitor:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCompetitor(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CompetitorProduct:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCompetitorProduct(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Asset:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildAsset(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.AssetType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildAssetType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.District:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildDistrict(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Province:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProvince(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ReorderLevel:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildReorderLevel(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.TargetPeriod:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildTargetPeriod(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Target:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildTarget(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductDiscount:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductDiscount(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.SaleValueDiscount:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSaleValueDiscount(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.DiscountGroup:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildDiscountGroup(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.PromotionDiscount:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildPromotionDiscount(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CertainValueCertainProductDiscount:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCvcpDiscount(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ProductGroupDiscount:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildProductGroupDiscount(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.SalesmanRoute:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSalesmanRoute(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.UserGroup:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildUserGroup(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.UserGroupRole:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildUserGroupRole(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Bank:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildBank(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.BankBranch:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildBankBranch(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Supplier:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSupplier(dateSince, masterdata, cct);
                    break;

                //case MasterDataCollective.MaritalStatus:
                //    _log.Debug("MasterDataCollective" + masterdata.ToString());
                //    envelope = _envelopeBuilder.BuildMaritalStatus(dateSince, masterdata, cct);
                //    break;

                case MasterDataCollective.ContactType:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildContactType(dateSince, masterdata, cct);
                    
                    break;
                case MasterDataCollective.AssetCategory:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildAssetCategory(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.AssetStatus:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildAssetStatus(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.OutletPriority:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildOutletPriority(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.OutletVisitDay:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildOutletVisitDay(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.TargetItem:
                    _log.Debug("MasterDataCollective" + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildTargetItem(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Setting:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSetting(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.RetireSetting:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildRetireSetting(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CommodityType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommodityType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Commodity:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommodity(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CommodityOwnerType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommodityOwnerType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CommodityProducer:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommodityProducer(dateSince, masterdata, cct);
                    
                    break;
                case MasterDataCollective.CommoditySupplier:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommoditySupplier(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CommodityOwner:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommodityOwner(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CentreType:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCentreType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Centre:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCentre(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Hub:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildHub(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Store:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildStore(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.FieldClerk:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildFieldClerk(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Printer:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildPrinter(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.WeighScale:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildWeighScale(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.SourcingContainer:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildSourcingContainer(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.Vehicle:
                    _log.Debug("MasterDataCollective." + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildVehicle(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.CommodityProducerCentreAllocation:
                    _log.Debug("MasterDataCollective. " + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildCommodityProducerCentreAllocation(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.ContainerType:
                    _log.Debug("MasterDataCollective. " + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildContainerType(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.RouteCentreAllocation:
                    _log.Debug("MasterDataCollective. " + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildRouteCentreAllocation(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.RouteCostCentreAllocation:
                    _log.Debug("MasterDataCollective. " + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildRouteCostCentreAllocation(dateSince, masterdata, cct);
                    break;

                case MasterDataCollective.RouteRegionAllocation:
                    _log.Debug("MasterDataCollective. " + masterdata.ToString());
                    envelope = _envelopeBuilder.BuildRouteRegionAllocation(dateSince, masterdata, cct);
                    break;
                default:
                    throw new Exception("Failed to map to DTO " + masterdata.ToString());
            }
            UpdateMasterDataTracker(costCentreApplicationId, masterdata);
            return envelope;
        }

        private void UpdateMasterDataTracker(Guid costCentreApplicationId, MasterDataCollective masterdata)
        {
            tblClientMasterDataTracker reg = _ctx.tblClientMasterDataTracker.FirstOrDefault(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId==(int)masterdata);
            if(reg!=null)
            {
                reg.DateTimePushConfirmed = DateTime.Now;
                _ctx.SaveChanges();
            }
        }

        private bool isUpdated(Guid costCentreApplicationId, MasterDataCollective masterdata)
        {
            //AJM Not working correctly
            //return true;

            DateTime date= DateTime.Now.AddYears(-10);
            if (_ctx.tblClientMasterDataTracker.Where(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId == (int)masterdata).Any())
            {
                DateTime? dt = _ctx.tblClientMasterDataTracker.Where(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId == (int)masterdata)
                       .Max(n => n.DateTimePushConfirmed);
                if (dt.HasValue)
                    date = dt.Value;// _ctx.tblClientMasterDataTracker.Where(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId == (int)masterdata).Max(n => n.DateTimePushConfirmed);
            }
            else
                return false;

            return _envelopeBuilder.IsUpdated(masterdata, date);
        }

        public string[] RepositoryList()
        {
            List<string> repoList = new List<string>();
            foreach (MasterDataCollective masterdata in Enum.GetValues(typeof(MasterDataCollective)))//change to linq l8r
            {
                //if ((int)masterdata > 46)
                //    continue;
                repoList.Add(masterdata.ToString());
            }
            return repoList.ToArray();
        }

        public MasterDataEnvelope GetTableContents(Guid costCentreApplicationId, int entityName)
        {
            _log.DebugFormat("GetTableContents: costCentreId: {0}, entityName: {1}", costCentreApplicationId, entityName);
            foreach (MasterDataCollective masterdata in Enum.GetValues(typeof(MasterDataCollective)))//change to linq l8r
            {
                if (((int)masterdata) == entityName)
                {
                    return getEnvelope(costCentreApplicationId, masterdata, null);
                }
            }
            throw new Exception("Cost Centre and/or entityName do not exist");
        }

        public TestCostCentreEnvelope GetTestCostCentre()
        {
            Guid cc=Guid.Empty ;
            var firstcc = _ctx.tblCostCentre.Where(n => n.CostCentreType == (int)CostCentreType.Distributor).FirstOrDefault();
            if (firstcc != null) cc = firstcc.Id;
            return new TestCostCentreEnvelope()
            {
                costCentreType = "Distributor",
                costCentreId = cc
            };
        }

        public MasterDataEnvelope GetInventory(Guid costCentreApplicationId)
        {
            CostCentre cct = null;
            CostCentreApplication ccApp = _costCentreApplicationRepository.GetById(costCentreApplicationId);
            if (ccApp != null)
                cct = _costCentreRepository.GetById(ccApp.CostCentreId);
            MasterDataEnvelope envelope = null;
            if (cct is DistributorSalesman)
            {
                if (cct != null)
                {
                    envelope = new MasterDataEnvelope()
                                   {
                                       masterDataName = "Inventory",
                                       masterData =
                                           _inventoryRepository.GetByWareHouseId(cct.Id).Select(n => n as MasterEntity).
                                           ToList()
                                   };
                }
            }else if(cct is Distributor)
            {
                List<Inventory> disInventory = new List<Inventory>() ;
                var discostcentre = _costCentreRepository.GetAll().Where(s => s.ParentCostCentre!=null).Where(p=>p.ParentCostCentre.Id==cct.Id || p.Id==cct.Id).OfType<Warehouse>().ToList();
                //var query = new QueryCostCentre();
                //var listOfCostCentreIds = new List<int>();
                //listOfCostCentreIds.Add((int)CostCentreType.Distributor);
                //listOfCostCentreIds.Add((int)CostCentreType.DistributorSalesman);
                //listOfCostCentreIds.Add((int)CostCentreType.DistributorPendingDispatchWarehouse);
                //listOfCostCentreIds.Add((int)CostCentreType.DistributorPendingReturnsWarehouse);
                //listOfCostCentreIds.Add((int)CostCentreType.Store);
                //listOfCostCentreIds.Add((int)CostCentreType.PurchasingClerk);
                //listOfCostCentreIds.Add((int)CostCentreType.Transporter);
                //query.ListOfCostCentreTypeIds=listOfCostCentreIds;
                //query.ShowInactive = true;
                //query.CostCentreId = cct.Id;
                //var discostcentreQuery = _costCentreRepository.Query(query);
                //var discostcentre=discostcentreQuery.Data.Where(s => s.ParentCostCentre != null).Where(p => p.ParentCostCentre.Id == cct.Id || p.Id == cct.Id).OfType<Warehouse>().ToList();

                foreach (var item in discostcentre)
                {
                    var inventory = _inventoryRepository.GetByWareHouseId(item.Id).ToList();
                    if (inventory != null && inventory.Count > 0)
                        disInventory = disInventory.Union(inventory).ToList();
                }
                
                envelope = new MasterDataEnvelope()
                {
                    masterDataName = "Inventory",
                    masterData =disInventory.Select(n => n as MasterEntity).ToList()
                };
            }
            DateTime date = DateTime.Now;
            int MasterDataId = -99;
            UpdateMasterDataTracker(costCentreApplicationId, MasterDataId);
            return envelope;
        }

        private void UpdateMasterDataTracker(Guid costCentreApplicationId, int MasterDataId)
        {
            tblClientMasterDataTracker phitem = _ctx.tblClientMasterDataTracker.FirstOrDefault(n => n.CostCentreApplicationId == costCentreApplicationId && n.MasterDataId == MasterDataId);
         
            if (phitem == null)
            {
                phitem = new tblClientMasterDataTracker
                             {
                                 Id = Guid.NewGuid(),
                                 IM_DateCreated = DateTime.Now,
                                 IM_DateLastUpdated = DateTime.Now,
                                 IM_Status =(int)EntityStatus.Active,// true,
                                 MasterDataId = MasterDataId,
                                 CostCentreApplicationId = costCentreApplicationId,
                                 DateTimePushed = DateTime.Now.AddYears(-40)

                             };
                _ctx.tblClientMasterDataTracker.AddObject(phitem);
            }

            if (phitem != null)
            {
                phitem.DateTimePushConfirmed = DateTime.Now;
                _ctx.SaveChanges();
            }
        }

        public ClientMasterDataTracker GetByMasterDataAndEntity(Guid costCentreApplicationId, int entityId)
        {
            var tbl = _ctx.tblClientMasterDataTracker.FirstOrDefault(s => s.MasterDataId == entityId && s.CostCentreApplicationId == costCentreApplicationId);
            ClientMasterDataTracker tracker = null;
            if(tbl!=null)
            {
                tracker = tbl.Map();
            }
            return tracker;
        }

        public MasterDataEnvelope GetPayments(Guid costCentreApplicationId)
        {
            CostCentre cct = null;
            CostCentreApplication ccApp = _costCentreApplicationRepository.GetById(costCentreApplicationId);
            if (ccApp != null)
                cct = _costCentreRepository.GetById(ccApp.CostCentreId);
            MasterDataEnvelope envelope = null;
            if (cct is DistributorSalesman)
            {
                if (cct != null)
                {
                    envelope = new MasterDataEnvelope()
                    {
                        masterDataName = "Payment",
                        masterData =
                            _paymentTrackerRepository.GetByCostCentre(cct.Id).Select(n => n as MasterEntity).
                            ToList()
                    };
                }
            }
            else if (cct is Distributor)
            {
                List<PaymentTracker> disInventory = new List<PaymentTracker>();
                var discostcentre = _costCentreRepository.GetAll().Where(s => s.ParentCostCentre != null).Where(p => p.ParentCostCentre.Id == cct.Id || p.Id == cct.Id).OfType<Warehouse>().ToList();
                foreach (var item in discostcentre)
                {
                    var inventory = _paymentTrackerRepository.GetByCostCentre(item.Id).ToList();
                    if (inventory != null && inventory.Count > 0)
                        disInventory = disInventory.Union(inventory).ToList();
                }

                envelope = new MasterDataEnvelope()
                {
                    masterDataName = "Payment",
                    masterData = disInventory.Select(n => n as MasterEntity).ToList()
                };
            }
            DateTime date = DateTime.Now;
            int MasterDataId = -98;
            UpdateMasterDataTracker(costCentreApplicationId, MasterDataId);
            return envelope;
        }

        public void IntializeApplication(Guid costCentreApplicationId)
        {
            foreach (var item in _ctx.tblClientMasterDataTracker.Where(s=>s.CostCentreApplicationId==costCentreApplicationId))
            {
               
                _ctx.tblClientMasterDataTracker.DeleteObject(item);
            }
            _ctx.SaveChanges();
        }



        public List<FarmerCummulative> FamersCummulative(Guid costCentreApplicationId)
        {
         
            DateTime now = DateTime.Now;
            var data = new List<FarmerCummulative>();
            var app = _ctx.tblCostCentreApplication.FirstOrDefault(s => s.id == costCentreApplicationId);
            //var app = _ctx.tblCostCentreApplication.FirstOrDefault(s => s.costcentreid == costCentreApplicationId);
            IQueryable<tblCommodityOwner> listofFarmers = _ctx.tblCommodityOwner;
            if (app != null)
            {

                var costcentre = _costCentreRepository.GetById(app.costcentreid);
                if (costcentre is PurchasingClerk)
                {
                    var routeids =
                        _ctx.tblPurchasingClerkRoute.Where(s => s.PurchasingClerkId == costcentre.Id).Select(
                            s => s.RouteId).Distinct();
                    var centreids =
                        _ctx.tblCentre.Where(s => routeids.Contains(s.RouteId.Value)).Select(s => s.Id).Distinct();
                    var farmids =
                        _ctx.tblMasterDataAllocation.Where(s => s.AllocationType == 4 && centreids.Contains(s.EntityBId))
                            .Select(s => s.EntityAId).Distinct();
                    var account =
                        _ctx.tblCommodityProducer.Where(s => farmids.Contains(s.Id)).Select(s => s.CostCentreId).
                            Distinct();
                    if (account != null)
                    {
                        listofFarmers = listofFarmers.Where(s => account.Contains(s.CostCentreId));
                    }

                }
                var date = new DateTime(now.Year, now.Month, 1);
                foreach (var owner in listofFarmers)
                {
                    var cum = new FarmerCummulative {CommodityOwnerId = owner.Id};
                    var weight = _ctx.tblSourcingLineItem.Where(s => s.tblSourcingDocument.CommodityOwnerId == owner.Id)
                        .Where(s => s.tblSourcingDocument.DocumentDate >= date)
                        .Sum(s => (decimal?) s.Weight.Value);
                    cum.ServerDate = now;
                    cum.CWeight = weight != null ? weight.Value : 0;
                    data.Add(cum);

                }
            }
            return data;
        }

      

    }
}
