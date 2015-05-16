using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{

    class CostCentreRefRepository : ICostCentreRefRepository
    {
         CokeDataContext _ctx;

        public CostCentreRefRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public List<CostCentreDetailRef> GetDistributorAndSalesmenAll()
        {
            return _ctx.tblCostCentre.Where(
                s =>
                    s.CostCentreType == (int) CostCentreType.DistributorSalesman ||
                    s.CostCentreType == (int) CostCentreType.Distributor).Select(s=>new{s.Id,s.Name,s.CostCentreType}).ToList()
                .Select(s => new CostCentreDetailRef
                {
                    Id = s.Id,
                    Name = s.Name,
                    CostCentreType = ((CostCentreType) s.CostCentreType).ToString()

                }).ToList();
        }
    }
    class CostCentreRepository : RepositoryMasterBase<CostCentre>, ICostCentreRepository
    {
        public ICostCentreFactory _costCentreFactory;
        public CokeDataContext _ctx;
        public ICacheProvider _cacheProvider;
        public IUserRepository _userRepository;
        public IContactRepository _contactRepository;

        public CostCentreRepository(ICostCentreFactory costCentreFactory, CokeDataContext ctx, ICacheProvider cacheProvider, IUserRepository userRepository, IContactRepository contactRepository)
        {
            _ctx = ctx;
            _costCentreFactory = costCentreFactory;
            _cacheProvider = cacheProvider;
            _userRepository = userRepository;
            _contactRepository = contactRepository;
            _log.Debug("CostCenter constructor bootstrap");
        }

        public Guid Save(CostCentre entity, bool? isSync = null)
        {
            _log.Debug("Save Cost Centre");
            EntityStatus entityStatus;
            tblCostCentre ccToSave =_ctx.tblCostCentre.FirstOrDefault(n => n.Id == entity.Id);
            DateTime dt = DateTime.Now;
            if (ccToSave == null)
            {
                ccToSave = new tblCostCentre
                {
                    CostCentreType = (int)entity.CostCentreType,
                    IM_DateCreated = dt,
                     Id= entity.Id,
                };

                if (!(entity is Outlet))
                    ccToSave.IM_Status = (int) EntityStatus.Active;

                _ctx.tblCostCentre.AddObject(ccToSave);
            }
            
            if (entity is Outlet)
            {
                entityStatus = entity._Status == EntityStatus.New ? EntityStatus.New : entity._Status;
            }
            else
            {
                entityStatus = entity._Status == EntityStatus.New ? EntityStatus.Active : entity._Status;
                if (ccToSave.IM_Status != (int) entityStatus)
                    ccToSave.IM_Status = (int) entity._Status;
            }
            ccToSave.Cost_Centre_Code = entity.CostCentreCode;
            ccToSave.IM_DateLastUpdated = dt;

            //costcentre fields
            ccToSave.Name = entity.Name;
            if (entity.ParentCostCentre != null)
                ccToSave.ParentCostCentreId = entity.ParentCostCentre.Id;
            foreach (Contact cnt in entity.Contact)
            {
                _contactRepository.Save(cnt);
            }

            //warehouse fields
            if (entity is Warehouse)
            {
                Warehouse wh = entity as Warehouse;

            }

            //outlet fields
            if (entity is Outlet)
            {

                Outlet outlet = entity as Outlet;
                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(outlet);
                }
                if (!vri.IsValid)
                {

                    LogErrors(outlet,vri);
                    _log.Debug("Outlet not valid");
                    throw new DomainValidationException(vri, "Outlet Entity Not valid");
                }
                outlet.CostCentreType = CostCentreType.Outlet;
                ccToSave.CostCentreType = (int)CostCentreType.Outlet;
                if (ccToSave.IM_Status != (int)entityStatus)
                    ccToSave.IM_Status = (int)entity._Status;
                if (outlet.Route != null)
                    ccToSave.RouteId = outlet.Route.Id;
                if (outlet.OutletCategory != null)
                    ccToSave.Outlet_Category_Id = outlet.OutletCategory.Id;
                if (outlet.OutletType != null)
                    ccToSave.Outlet_Type_Id = outlet.OutletType.Id;
                if (outlet.DiscountGroup != null)
                    ccToSave.Outlet_DiscountGroupId = outlet.DiscountGroup.Id;
                else
                {
                    ccToSave.Outlet_DiscountGroupId = null;

                }

                if (outlet.OutletProductPricingTier != null)
                    ccToSave.Tier_Id = outlet.OutletProductPricingTier.Id;
                if (outlet.SpecialPricingTier != null)
                    ccToSave.SpecialPricingTierId = outlet.SpecialPricingTier.Id;

                //cn:: resetting an outlets VATClass to null. i.e. remove the VAT Class
                if (outlet.VatClass != null)
                    ccToSave.VATClass_Id = outlet.VatClass.Id;
                else
                    ccToSave.VATClass_Id = null;

                if (outlet.UsrSurveyor != null)
                    ccToSave.Surveyor_Id = outlet.UsrSurveyor.Id;
                if (outlet.UsrASM != null)
                    ccToSave.Distributor_ASM_Id = outlet.UsrASM.Id;
                if (outlet.UsrSalesRep != null)
                    ccToSave.SalesRep_Id = outlet.UsrSalesRep.Id;

                ccToSave.StandardWH_Latitude = outlet.Latitude;
                ccToSave.StandardWH_Longtitude = outlet.Longitude;
                ccToSave.Cost_Centre_Code = outlet.CostCentreCode;

                if (outlet.ShipToAddresses != null)
                {
                    foreach (var addressItem in outlet.ShipToAddresses)
                    {
                        var address = _ctx.tblShipToAddress.FirstOrDefault(n => n.Id == addressItem.Id);
                        if (address == null)
                        {
                            address = new tblShipToAddress
                                {
                                    IM_DateCreated = dt,
                                    IM_Status = (int) EntityStatus.Active
                                };
                            _ctx.tblShipToAddress.AddObject(address);
                        }
                        var addressEntityStatus = (addressItem._Status == EntityStatus.New)
                                                      ? EntityStatus.Active
                                                      : addressItem._Status;
                        if (address.IM_Status != (int) addressEntityStatus)
                            address.IM_Status = (int) addressEntityStatus;
                        address.CostCentreId = entity.Id;
                        address.Id = addressItem.Id;
                        address.Name = addressItem.Name;
                        address.PhysicalAddress = addressItem.PhysicalAddress;
                        address.PostalAddress = addressItem.PostalAddress;
                        address.Description = addressItem.Description;
                        address.Latitude = addressItem.Latitude;
                        address.Longitude = addressItem.Longitude;
                        address.IM_DateLastUpdated = dt;
                        ccToSave.tblShipToAddress.Add(address);
                    }
                }
            }

            //standard warehouse fields
            if (entity is StandardWarehouse)
            {
                StandardWarehouse swh = entity as StandardWarehouse;
                ccToSave.StandardWH_Latitude = swh.Latitude;
                ccToSave.StandardWH_Longtitude = swh.Longitude;
                ccToSave.StandardWH_VatRegistrationNo = swh.VatRegistrationNo;
            }

            //distributor salesman
            if (entity is DistributorSalesman)
            {

                DistributorSalesman distributorsalesman = entity as DistributorSalesman;
                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(distributorsalesman);
                }
                if (!vri.IsValid)
                {
                    LogErrors(distributorsalesman, vri);
                    _log.Debug("DistributorSalesman not valid");
                    throw new DomainValidationException(vri, "Distributor Saleman Entity Not valid");
                }
                distributorsalesman.CostCentreType = CostCentreType.DistributorSalesman;
               
                ccToSave.Cost_Centre_Code = distributorsalesman.CostCentreCode;
                ccToSave.CostCentreType2 = (int) distributorsalesman.Type;
                //if (distributorsalesman.Route != null)
                //    ccToSave.RouteId = distributorsalesman.Route.Id;
                DateTime date = DateTime.Now;
                foreach (SalesmanRoute r in distributorsalesman.Routes)
                { 
                    if (r == null) continue;
                    tblSalemanRoute tblsr  = _ctx.tblSalemanRoute.FirstOrDefault(p => p.Id == r.Id); ;
                    if (tblsr == null)
                    {
                        tblsr = new tblSalemanRoute()
                                    {
                                        IM_DateCreated = date,
                                        IM_Status = (int)EntityStatus.Active,//true,
                                    };
                        _ctx.tblSalemanRoute.AddObject(tblsr);


                    }
                    
                    tblsr.IM_DateLastUpdated = date;
                    tblsr.RouteId = r.Route.Id;
                    tblsr.SalemanId = distributorsalesman.Id;
                }

            }

            //transporter
            if (entity is Transporter)
            {
                Transporter transporter = entity as Transporter;
                ccToSave.Transporter_Drivername = transporter.DriverName;
                ccToSave.Transporter_VehicleRegistrationNo = transporter.VehicleRegistrationNo;
            }
            //Producer
            if (entity is Producer)
            {
                Producer producer = entity as Producer;
            }
            //Distributor
            if (entity is Distributor)
            {
                Distributor distributor = entity as Distributor;
                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(distributor);
                }
                if (!vri.IsValid)
                {
                    LogErrors(entity, vri);
                    _log.Debug("Distributor not valid");
                    throw new DomainValidationException(vri, "Distributor Entity Not valid");
                }
                //distributor = costCentre as Distributor;
                ccToSave.Distributor_Owner = distributor.Owner;
                ccToSave.Cost_Centre_Code = distributor.CostCentreCode;
                if(distributor.ASM != null)
                ccToSave.Distributor_ASM_Id =distributor.ASM.Id;
                ccToSave.Distributor_PIN = distributor.PIN;
                //ccToSave.Distributor_RegionId = distributor.Region.Id;
                if (distributor.Region != null)
                    ccToSave.Distributor_RegionId =distributor.Region==null ? Guid.Empty: distributor.Region.Id;
                if(distributor.SalesRep!=null)
                ccToSave.SalesRep_Id =  distributor.SalesRep.Id;
                if(distributor.Surveyor != null)
                ccToSave.Surveyor_Id =  distributor.Surveyor.Id;
                if (distributor.ProductPricingTier != null)
                    ccToSave.Tier_Id = distributor.ProductPricingTier.Id;

                ccToSave.MerchantNumber = distributor.MerchantNumber;
                ccToSave.PaybillNumber = distributor.PaybillNumber;

                distributor.CostCentreType = CostCentreType.Distributor;


                //ccToSave.CostCentreType = (int)CostCentreType.Distributor;
            }
            if (entity is Hub)
            {
                Hub hub = entity as Hub;
                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(hub);
                }
                if (!vri.IsValid)
                {
                    LogErrors(entity, vri);
                    _log.Debug("Hub not valid");
                    throw new DomainValidationException(vri, "Hub Entity Not valid");
                }
                ccToSave.Cost_Centre_Code = hub.CostCentreCode;
                if (hub.Region != null)
                    ccToSave.Distributor_RegionId = hub.Region == null ? Guid.Empty : hub.Region.Id;
                hub.CostCentreType = CostCentreType.Hub;
                
            }
            if (entity is CommoditySupplier)
            {

                CommoditySupplier commoditySupplier = entity as CommoditySupplier;
                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(commoditySupplier);
                }
                if (!vri.IsValid)
                {
                    LogErrors(entity, vri);
                    _log.Debug("CommoditySupplier not valid");
                    throw new DomainValidationException(vri, "CommoditySupplier Entity Not valid");
                }
                commoditySupplier.CostCentreType = CostCentreType.CommoditySupplier;
                ccToSave.CostCentreType =(int)commoditySupplier.CostCentreType;
                ccToSave.JoinDate = commoditySupplier.JoinDate;
                ccToSave.CostCentreType2 = (int)commoditySupplier.CommoditySupplierType;
                ccToSave.AccountNumber = commoditySupplier.AccountNo;
                ccToSave.AccountName = commoditySupplier.AccountName;
                ccToSave.Cost_Centre_Code = commoditySupplier.CostCentreCode;
                ccToSave.BankId = commoditySupplier.BankId;
                ccToSave.BankBranchId = commoditySupplier.BankBranchId;
                ccToSave.Revenue_PIN = commoditySupplier.PinNo;
                //if (outlet.Route != null)
                //    ccToSave.RouteId = outlet.Route.Id;

            }
            if (entity is PurchasingClerk)
            {

                PurchasingClerk purchasingClerk = entity as PurchasingClerk;

                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(purchasingClerk);
                }
                if (!vri.IsValid)
                {
                    LogErrors(entity, vri);
                    _log.Debug("PurchasingClerk not valid");
                    throw new DomainValidationException(vri, "PurchasingClerk Entity Not valid");
                }
                if (purchasingClerk.User != null)
                {
                    //Saving Field Clerk User, Validation in user repository
                    _ctx.SaveChanges();
                    purchasingClerk.User.CostCentre = purchasingClerk.Id;
                    _userRepository.Save(purchasingClerk.User, true);
                }
                purchasingClerk.CostCentreType = CostCentreType.PurchasingClerk;
                ccToSave.CostCentreType = (int)purchasingClerk.CostCentreType;
                ccToSave.Cost_Centre_Code = purchasingClerk.CostCentreCode;

                DateTime date = DateTime.Now;
                foreach (PurchasingClerkRoute r in purchasingClerk.PurchasingClerkRoutes)
                {
                    tblPurchasingClerkRoute tblsr = _ctx.tblPurchasingClerkRoute.FirstOrDefault(p => p.Id == r.Id); ;
                    if (tblsr == null)
                    {
                        tblsr = new tblPurchasingClerkRoute()
                        {
                            Id = r.Id,
                            IM_DateCreated = date,
                            IM_Status = (int)EntityStatus.Active,
                        };
                        _ctx.tblPurchasingClerkRoute.AddObject(tblsr);
                    }

                    tblsr.IM_DateLastUpdated = date;
                    tblsr.RouteId = r.Route !=null?r.Route.Id:Guid.Empty;
                    tblsr.PurchasingClerkId = purchasingClerk.Id;
                }


            }

            //DistributorPendingDispatchWarehouse
            if (entity is DistributorPendingDispatchWarehouse)
            {
                DistributorPendingDispatchWarehouse dpdw = entity as DistributorPendingDispatchWarehouse;
                //check that it has a parent field of type warehouse
                bool hasParent = false;
                if (dpdw != null)
                {
                    CostCentre cc = GetById(dpdw.ParentCostCentre.Id);
                    if (cc is Distributor)
                        hasParent = true;
                }
                if (!hasParent)
                    throw new Exception("DistributorPendingDispatchWarehouse needs to have a parent costcentre that is a distributor");
            }

            if (entity is Store)
            {
                Store store = entity as Store;
                var vri = new ValidationResultInfo();
                if (isSync == null || !isSync.Value)
                {
                    vri = Validate(store);
                }
                if (!vri.IsValid)
                {
                    LogErrors(entity, vri);
                    _log.Debug("Store not valid");
                    throw new DomainValidationException(vri, "");
                }
                ccToSave.Cost_Centre_Code = store.CostCentreCode;

                store.CostCentreType = CostCentreType.Store;

            }


            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, ccToSave.Id));
            _log.Debug("saved");
            return ccToSave.Id;
        }

        public CostCentre GetById(Guid Id, bool includeDeactivated = false)
        {
            CostCentre entity = (CostCentre)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCostCentre.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected List<CostCentre> GetByParentId(Guid parentCostCentreId, bool includeDeactivated = false)
        {
            IEnumerable<CostCentre> items = null;

            if (includeDeactivated)
                items = _ctx.tblCostCentre.Where(n => n.ParentCostCentreId == parentCostCentreId)
                    .ToList()
                    .Select(n => Map(n));
            else
                items = _ctx.tblCostCentre.Where(n => n.ParentCostCentreId == parentCostCentreId && n.IM_Status==(int)EntityStatus.Active)
                .ToList()
                .Select(n => Map(n));

            return items.ToList();
        }

        protected CostCentre Map(tblCostCentre tblCC)
        {
            CostCentre cc = null;
            CostCentreType cct = (CostCentreType)tblCC.CostCentreType;
            switch (cct)
            {
                case CostCentreType.Producer:
                    cc = new Producer(tblCC.Id) { CostCentreType = CostCentreType.Producer };
                    break;
                case CostCentreType.Transporter:
                    cc = new Transporter(tblCC.Id) { CostCentreType = CostCentreType.Transporter };
                    break;
                case CostCentreType.Distributor:
                    Distributor distributor = new Distributor(tblCC.Id) { CostCentreType = CostCentreType.Distributor };
                    if (tblCC.tblRegion != null)
                        distributor.Region = MapToDomain.Map(tblCC.tblRegion);
                    if (tblCC.Tier_Id != null)
                        distributor.ProductPricingTier =
                            _ctx.tblPricingTier.FirstOrDefault(n => n.id == tblCC.Tier_Id).Map();
                    distributor.CostCentreCode = tblCC.Cost_Centre_Code;
                    if (tblCC.PaybillNumber != null)
                        distributor.PaybillNumber = tblCC.PaybillNumber;
                    if (tblCC.MerchantNumber != null)
                        distributor.MerchantNumber = tblCC.MerchantNumber;
                    cc = distributor as CostCentre;
                    break;
                case CostCentreType.DistributorPendingDispatchWarehouse:
                    DistributorPendingDispatchWarehouse dpdw = new DistributorPendingDispatchWarehouse(tblCC.Id)
                                                                   {
                                                                       CostCentreType =
                                                                           CostCentreType.
                                                                           DistributorPendingDispatchWarehouse
                                                                   };
                    cc = dpdw as CostCentre;
                    break;
                case CostCentreType.DistributorSalesman:
                    cc = new DistributorSalesman(tblCC.Id) { CostCentreType = CostCentreType.DistributorSalesman };
                    
                    break;
                case CostCentreType.Outlet:
                    //cc = new Outlet(tblCC.Id) { CostCentreType = CostCentreType.Outlet };
                    //TODO WAIT FOR OUTLET DEPENDENCIES
                    cc = new Outlet(tblCC.Id) { CostCentreType = CostCentreType.Outlet };
                    Outlet outlet = cc as Outlet;
                    if (tblCC.tblOutletCategory != null)
                        outlet.OutletCategory = MapToDomain.Map(tblCC.tblOutletCategory);
                    if (tblCC.tblOutletType != null)
                        outlet.OutletType = MapToDomain.Map(tblCC.tblOutletType);
                    if (tblCC.tblDiscountGroup != null)
                        outlet.DiscountGroup = MapToDomain.Map(tblCC.tblDiscountGroup);
                    if (tblCC.Tier_Id !=Guid.Empty)
                    {
                        var tire = _ctx.tblPricingTier.FirstOrDefault(n => n.id == tblCC.Tier_Id);

                        if(tire !=null)
                        outlet.OutletProductPricingTier = tire.Map();
                    }
                    if (tblCC.SpecialPricingTierId != Guid.Empty)
                    {
                        var tire = _ctx.tblPricingTier.FirstOrDefault(n => n.id == tblCC.SpecialPricingTierId);

                        if (tire != null)
                            outlet.SpecialPricingTier = tire.Map();
                    }
                    
                    if (tblCC.VATClass_Id != null)
                        outlet.VatClass = MapToDomain.Map(tblCC.tblVATClass);
                    outlet.CostCentreCode = tblCC.Cost_Centre_Code;
                    outlet.Latitude = tblCC.StandardWH_Latitude;
                    outlet.Longitude = tblCC.StandardWH_Longtitude;

                    if (tblCC.tblRoutes != null)
                    {
                        Route r = null;
                        _log.Debug("Outlet Route ID:" + tblCC.tblRoutes.RouteID);
                        r = MapRoute(tblCC.tblRoutes);
                        outlet.Route = r;
                    }

                    var shipToAddresses = 
                        tblCC.tblShipToAddress
                        .Select(n =>
                            new ShipToAddress(n.Id, n.IM_DateCreated, n.IM_DateLastUpdated, (EntityStatus) n.IM_Status)
                                {
                                    Name = n.Name,
                                    Code = n.Code,
                                    Description = n.Description,
                                    PhysicalAddress = n.PhysicalAddress,
                                    PostalAddress = n.PostalAddress,
                                    Latitude = n.Latitude ?? 0,
                                    Longitude = n.Longitude ?? 0,
                                    
                                })
                        .ToList();

                    shipToAddresses.ForEach(x => outlet.AddShipToAddress(x));    
                    cc = outlet;// as CostCentre;
                    break;
                case CostCentreType.Hub:
                    Hub hub = new Hub(tblCC.Id) { CostCentreType = CostCentreType.Hub };
                    if (tblCC.tblRegion != null)
                        hub.Region = MapToDomain.Map(tblCC.tblRegion);
                    hub.CostCentreCode = tblCC.Cost_Centre_Code;
                    cc = hub;
                    break;
                case CostCentreType.CommoditySupplier:
                    CommoditySupplier commoditySupplier = new CommoditySupplier(tblCC.Id) { CostCentreType = CostCentreType.CommoditySupplier };
                    commoditySupplier.CostCentreCode = tblCC.Cost_Centre_Code;

                    cc = commoditySupplier;
                    break;
                case CostCentreType.PurchasingClerk:
                    cc = new PurchasingClerk(tblCC.Id) { CostCentreType = CostCentreType.PurchasingClerk };
                    break;
                case CostCentreType.Store:
                    cc = new Store(tblCC.Id){CostCentreType = CostCentreType.Store, CostCentreCode = tblCC.Cost_Centre_Code};
                    break;

            }
           
            if (cc is Producer)
            {
                Producer prodc = cc as Producer;
                prodc.Latitude = tblCC.StandardWH_Latitude;
                prodc.Longitude = tblCC.StandardWH_Longtitude;

            }
            if (cc is Outlet)
            {
                Outlet outLets = cc as Outlet;
                outLets.UsrSalesRep = _userRepository.GetById(tblCC.SalesRep_Id ?? Guid.Empty);
                outLets.UsrSurveyor = _userRepository.GetById(tblCC.Surveyor_Id ?? Guid.Empty);
                outLets.UsrASM = _userRepository.GetById(tblCC.Distributor_ASM_Id ?? Guid.Empty);
               
                outLets.CostCentreCode = tblCC.Cost_Centre_Code;

            }
            if (cc is Transporter)
            {
                Transporter trans = cc as Transporter;
                trans.DriverName = tblCC.Transporter_Drivername;
                trans.VehicleRegistrationNo = tblCC.Transporter_VehicleRegistrationNo;

            }
            if (cc is StandardWarehouse)
            {
                StandardWarehouse swh = cc as StandardWarehouse;
                swh.Latitude = tblCC.StandardWH_Latitude;
                swh.Longitude = tblCC.StandardWH_Longtitude;
                swh.VatRegistrationNo = tblCC.StandardWH_VatRegistrationNo;
            }
            if (cc is Distributor)
            {
                Distributor dist = cc as Distributor;
                dist.AccountNo = tblCC.Cost_Centre_Code;
                dist.ASM = _userRepository.GetById(tblCC.Distributor_ASM_Id ?? Guid.Empty);
                dist.Owner = tblCC.Distributor_Owner;
                dist.PIN = tblCC.Distributor_PIN;
                dist.SalesRep = _userRepository.GetById(tblCC.SalesRep_Id ?? Guid.Empty);
                dist.Surveyor = _userRepository.GetById(tblCC.Surveyor_Id ?? Guid.Empty);
                dist.CostCentreCode = tblCC.Cost_Centre_Code;
            }
            if (cc is DistributorSalesman)
            {
                DistributorSalesman ds = cc as DistributorSalesman;
                ds.CostCentreCode = tblCC.Cost_Centre_Code;
                ds.Type = (DistributorSalesmanType) tblCC.CostCentreType2;
                if (_ctx.tblSalemanRoute.Any(a=>a.SalemanId==ds.Id)== true)
                {
                  List<SalesmanRoute> routes=_ctx.tblSalemanRoute.Where(a => a.SalemanId == ds.Id).ToList()
                      .Select(n => ds.ParentCostCentre != null ? 
                          (n != null ? Map(n, ds.ParentCostCentre.Id,ds.Id) : null) : null).ToList();
                    ds.Routes = routes;

                }
            }
            if (cc is Hub)
            {
                Hub hub = cc as Hub;
               hub.Region = tblCC.tblRegion.Map();
            }
            if (cc is CommoditySupplier)
            {
                CommoditySupplier commoditySupplier = cc as CommoditySupplier;
                commoditySupplier.JoinDate = tblCC.JoinDate.HasValue?tblCC.JoinDate.Value:default(DateTime);
                commoditySupplier.CommoditySupplierType = (CommoditySupplierType)tblCC.CostCentreType2;
                commoditySupplier.AccountNo = tblCC.AccountNumber;
                commoditySupplier.AccountName = tblCC.AccountName;
                commoditySupplier.CostCentreCode = tblCC.Cost_Centre_Code;
                if (tblCC.BankId != null) commoditySupplier.BankId = tblCC.BankId.Value;
                if (tblCC.BankBranchId != null) commoditySupplier.BankBranchId = tblCC.BankBranchId.Value;
                commoditySupplier.PinNo = tblCC.Revenue_PIN;
            }
            if (cc is PurchasingClerk)
            {
                PurchasingClerk purchasingClerk = cc as PurchasingClerk;
                purchasingClerk.CostCentreCode = tblCC.Cost_Centre_Code;
                purchasingClerk.User = _userRepository.GetByCostCentre(purchasingClerk.Id).FirstOrDefault();

                if (_ctx.tblPurchasingClerkRoute.Any(a => a.PurchasingClerkId == purchasingClerk.Id) == true)
                {
                    if (tblCC.ParentCostCentreId.HasValue)
                        purchasingClerk.ParentCostCentre = new CostCentreRef { Id = tblCC.ParentCostCentreId.Value };
                    
                    var rotues = _ctx.tblPurchasingClerkRoute
                        .Where(a => a.PurchasingClerkId == purchasingClerk.Id);
                    foreach(tblPurchasingClerkRoute  route in rotues)
                    {
                        var pcRoute = Map(route, purchasingClerk.ParentCostCentre.Id, purchasingClerk.Id);
                        purchasingClerk.PurchasingClerkRoutes.Add(pcRoute);
                    }
                }
            }
            if(cc is Store)
            {

                Store str = cc as Store;
              


            }
            if (cc != null)
            {
                cc.Name = tblCC.Name;
                //generic costcentre c
                if (tblCC.ParentCostCentreId.HasValue)
                    cc.ParentCostCentre = new CostCentreRef { Id = tblCC.ParentCostCentreId.Value };

                //metadata fields
                cc._SetDateCreated(tblCC.IM_DateCreated);
                cc._SetDateLastUpdated(tblCC.IM_DateLastUpdated);
                cc._SetStatus((EntityStatus)tblCC.IM_Status);

               
            }
            if (cc == null)
                throw new NotSupportedException("Unknown costcentre type");
            return cc;
        }

        public PurchasingClerkRoute Map(tblPurchasingClerkRoute sr, Guid distributorId, Guid purchasingClerkId)
        {
            PurchasingClerkRoute clerkRoute = new PurchasingClerkRoute(sr.Id)
            {
                Route = MapRoute(sr.tblRoutes),
                PurchasingClerkRef = new CostCentreRef { Id = purchasingClerkId }
            
            };
            clerkRoute._SetDateCreated(sr.IM_DateCreated);
            clerkRoute._SetDateLastUpdated(sr.IM_DateLastUpdated);
            clerkRoute._SetStatus((EntityStatus)sr.IM_Status);

            return clerkRoute;
        }

        public SalesmanRoute Map(tblSalemanRoute sr, Guid distributorId, Guid distributorSalesmanId)
        {
            SalesmanRoute retArea = new SalesmanRoute(sr.Id)
            {
                Route = MapRoute(sr.tblRoutes),
                DistributorSalesmanRef = new CostCentreRef { Id = distributorSalesmanId }
            };
            retArea._SetDateCreated(sr.IM_DateCreated);
            retArea._SetDateLastUpdated(sr.IM_DateLastUpdated);
            retArea._SetStatus((EntityStatus)sr.IM_Status);
            return retArea;
        }
        
        
        private Route MapRoute(tblRoutes tbl)
        {
            Route r = new Route(tbl.RouteID)
            {
                Name = tbl.Name,
                Code = tbl.Code,
                Region  = tbl.tblRegion.Map()
            };

            r._SetDateCreated(tbl.IM_DateCreated);
            r._SetDateLastUpdated(tbl.IM_DateLastUpdated);
            r._SetStatus((EntityStatus)tbl.IM_Status);
            return r;
        }

        public void SetActive(CostCentre entity)
        {
            _log.Debug("Activating Cost Centre " + entity.Id + "Name: " + entity.Name);
            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                _log.Debug("Costcentre not valid");
                throw new DomainValidationException(vri, "Entity Not valid");
            }


            _log.Debug("Activating contacts for " + entity.Name);
            var myContacts = _contactRepository.GetByContactsOwnerId(entity.Id, true);
            if (myContacts != null)
            {
                foreach (Contact contact in myContacts)
                {
                    _contactRepository.SetActive(contact);
                }
            }

            List<tblCostCentre> pendingDispatch = _ctx.tblCostCentre
                .Where(n => n.ParentCostCentreId == entity.Id
                            && n.CostCentreType == (int)CostCentreType.DistributorPendingDispatchWarehouse)
                .ToList();

            if (pendingDispatch != null)
            {
                foreach (var pd in pendingDispatch)
                {
                    pd.IM_Status = (int)EntityStatus.Active;
                    pd.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, pd.Id));
                }
            }

            tblCostCentre cc = _ctx.tblCostCentre.FirstOrDefault(n => n.Id == entity.Id);
            if (cc != null)
            {
                cc.IM_Status = (int)EntityStatus.Active;
                cc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, cc.Id));
            }
        }

        public void SetInactive(CostCentre entity)
        { 
            //cn:: deactivate ur contacts
            _log.Debug("Deactivating contacts for " + entity.Name);
            var myContacts = _contactRepository.GetByContactsOwnerId(entity.Id);
            if (myContacts != null)
            {
                foreach (Contact contact in myContacts)
                {
                    _contactRepository.SetInactive(contact);
                }
            }

            _log.Debug("Deactivating Cost Centre ID: " + entity.Id + "Name: " + entity.Name);

            if (entity.CostCentreType == CostCentreType.Outlet)
            {
                var startDate = "01/01/2001 00:00:00";
                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.OutstandingOutletOrders,startDate,endDate, entity.Id);
                var data = _ctx.ExecuteStoreQuery<object>(sql).ToList();
                if (data.Count >= 1)
                {
                    throw new Exception("Outlet cannot be Deactivated since it has an Outstanding Payment"); 
                }
            }

            List<tblCostCentre> pendingDispatch = _ctx.tblCostCentre
                .Where(n => n.ParentCostCentreId == entity.Id
                            && n.CostCentreType == (int)CostCentreType.DistributorPendingDispatchWarehouse)
                .ToList();

            if (pendingDispatch != null)
            {
                foreach (var pd in pendingDispatch)
                {
                    pd.IM_Status = (int)EntityStatus.Inactive;
                    pd.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, pd.Id));
                }
            }

            tblCostCentre tblcostcentre = _ctx.tblCostCentre.FirstOrDefault(n => n.Id == entity.Id);
            if (tblcostcentre != null)
            {
                tblcostcentre.IM_Status = (int)EntityStatus.Inactive;//false;
                tblcostcentre.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblcostcentre.Id));
            }
        }

        public void SetAsDeleted(CostCentre entity)
        {
            _log.Debug("Deactivating contacts for " + entity.Name);
            var myContacts = _contactRepository.GetByContactsOwnerId(entity.Id);
            if (myContacts != null)
            {
                foreach (Contact contact in myContacts)
                {
                    _contactRepository.SetAsDeleted(contact);
                }
            }

            if (entity.CostCentreType == CostCentreType.Outlet)
            {
                var startDate = "01/01/2001 00:00:00";
                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = string.Format(Resources.OutletOrders.OutletOrdersResource.OutstandingOutletOrders, startDate, endDate, entity.Id);
                var data = _ctx.ExecuteStoreQuery<object>(sql).ToList();
                if (data.Count >= 1)
                {
                    throw new Exception("Outlet cannot be Deleted since it has an Outstanding Payment");
                }
            }

            List<tblCostCentre> pendingDispatch = _ctx.tblCostCentre
                .Where(n => n.ParentCostCentreId == entity.Id
                            && n.CostCentreType == (int) CostCentreType.DistributorPendingDispatchWarehouse)
                .ToList();

            if (pendingDispatch != null)
            {
                foreach (var pd in pendingDispatch)
                {
                    pd.IM_Status = (int)EntityStatus.Deleted;
                    pd.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, pd.Id));
                }
            }

            tblCostCentre tblcostcentre = _ctx.tblCostCentre.FirstOrDefault(n => n.Id == entity.Id);
            if (tblcostcentre != null)
            {
                tblcostcentre.IM_Status = (int)EntityStatus.Deleted;
                tblcostcentre.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblcostcentre.Id));
            }

            _log.Debug("Deleting Cost Centre ID: " + entity.Id + "Name: " + entity.Name);
        }

        protected override string _cacheKey
        {
            get { return "CostCentre-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CostCentreList"; }
        }

        public override IEnumerable<CostCentre> GetAll(bool includeDeactivated = false)
        {

            IList<CostCentre> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CostCentre>(ids.Count);
                foreach (Guid id in ids)
                {
                    CostCentre entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCostCentre.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CostCentre p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }
                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

        public virtual List<CostCentre> GetByRegionId(Guid regionId, bool includeDeactivated)
        {
            throw new NotImplementedException();
        }
      

        public CostCentre GetByCode(string code,CostCentreType costcentretype, bool showInActive=false)
        {
            tblCostCentre tbl = null;
            tbl = showInActive
                      ? _ctx.tblCostCentre.FirstOrDefault(
                          s =>
                          s.IM_Status != (int) EntityStatus.Deleted && s.Cost_Centre_Code != null &&
                          s.Cost_Centre_Code.ToLower() == code.ToLower() && s.CostCentreType == (int)costcentretype)
                      : _ctx.tblCostCentre.FirstOrDefault(
                          s =>
                          s.Cost_Centre_Code != null && s.Cost_Centre_Code.ToLower() == code.ToLower() &&
                          s.IM_Status == (int) EntityStatus.Active && s.CostCentreType == (int)costcentretype);
            if (tbl != null)
            { 
               return Map(tbl);
            }
            return null;
            
        }

        public void SaveMapping(CostCentreMapping map)
        {
            if (map != null)
            {
                var costcentre = _ctx.tblCostCentre.FirstOrDefault(s => s.Id == map.Id);
                if (costcentre != null)
                {
                    var mapping = costcentre.tblCostCentreMapping;
                    if(mapping==null)
                    {
                        mapping= new tblCostCentreMapping();
                        mapping.Id = costcentre.Id;
                        _ctx.tblCostCentreMapping.AddObject(mapping);
                        mapping.IM_DateCreated = DateTime.Now;
                    }
                    mapping.MapToCostCentreId = map.MappToId;
                    mapping.IM_DateLastUpdated = DateTime.Now;
                    mapping.IM_Status = (int)EntityStatus.Active;
                }
                _ctx.SaveChanges();
            }
        }

        public QueryResult<CostCentre> Query(QueryBase query)
        {
           
            var q = query as QueryCostCentre;
            IQueryable<tblCostCentre> costCentreQuery;
            if (q.ShowInactive)
                costCentreQuery = _ctx.tblCostCentre.Where(p => p.IM_Status != (int)EntityStatus.Deleted && q.ListOfCostCentreTypeIds.Contains((int)p.CostCentreType)).AsQueryable();
            else
                costCentreQuery = _ctx.tblCostCentre.Where(s => s.IM_Status == (int)EntityStatus.Active && q.ListOfCostCentreTypeIds.Contains((int)s.CostCentreType)).AsQueryable();

            var queryResult = new QueryResult<CostCentre>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                costCentreQuery = costCentreQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Cost_Centre_Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = costCentreQuery.Count();
            costCentreQuery = costCentreQuery.OrderBy(s => s.Name).ThenBy(s => s.Cost_Centre_Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                costCentreQuery = costCentreQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = costCentreQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<CostCentre>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(CostCentre itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            if (itemToValidate is Distributor)
            {
                Distributor distributor = itemToValidate as Distributor;
                if (distributor.Region == null)
                    vri.Results.Add(new ValidationResult("Distributor Should Have Region" + " (" + distributor.Region + ")"));

                bool hasDuplicateCode = GetAll(true)
                    .OfType<Distributor>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Any(p => p.CostCentreCode == itemToValidate.CostCentreCode);
                if (hasDuplicateCode)
                    vri.Results.Add(new ValidationResult("Duplicate Distributor Code found" + " (" + itemToValidate.CostCentreCode + ")"));

                bool hasDuplicatePaybillNumber = GetAll(true).OfType<Distributor>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Where(d => !string.IsNullOrEmpty(d.PaybillNumber) &&  d.Id != distributor.Id)
                    .Any(d =>d.PaybillNumber == distributor.PaybillNumber);
                if (hasDuplicatePaybillNumber)
                    vri.Results.Add(new ValidationResult("Duplicate paybill number found." + " (" + distributor.PaybillNumber + ")"));

                bool hasDuplicateMerchantNumber = GetAll(true).OfType<Distributor>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Where(d => !string.IsNullOrEmpty(d.MerchantNumber) && d.Id != distributor.Id)
                    .Any(x => x.MerchantNumber == distributor.MerchantNumber);
                if (hasDuplicateMerchantNumber)
                    vri.Results.Add(new ValidationResult("Duplicate merchant number found." + " (" + distributor.MerchantNumber + ")"));
            }

            if (itemToValidate is Hub)
            {
                Hub hub = itemToValidate as Hub;
                if (hub.Region == null)
                    vri.Results.Add(new ValidationResult("Hub Should Have Region"));

                bool hasDuplicateCode = GetAll(true)
                    .OfType<Hub>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Any(p => p.CostCentreCode == itemToValidate.CostCentreCode);
                if (hasDuplicateCode)
                    vri.Results.Add(new ValidationResult("Duplicate Hub Code found"));
            }
            if (itemToValidate is Outlet)
            {

                Outlet outlet = itemToValidate as Outlet;

                if (outlet.Route == null)
                    vri.Results.Add(new ValidationResult("An Outlet should have a Route!"));
                if(outlet.ParentCostCentre==null)
                    vri.Results.Add(new ValidationResult("Outlet distributor is required"));
                
                if (outlet.OutletCategory == null)
                    vri.Results.Add(new ValidationResult("An Outlet should have an Outlet Category!"));
                if (outlet.OutletType == null)
                    vri.Results.Add(new ValidationResult("An Outlet should have an Outlet Type!"));

              bool hasDuplicateCode= _ctx.tblCostCentre.Any(
                    n =>
                    n.CostCentreType == (int) CostCentreType.Outlet && n.Id != itemToValidate.Id &&
                    n.Cost_Centre_Code == itemToValidate.CostCentreCode);
                
                if (hasDuplicateCode)
                    vri.Results.Add(new ValidationResult("Duplicate Code found"));
            }
            if (itemToValidate is DistributorSalesman)
            {
                DistributorSalesman distributorSalesman = itemToValidate as DistributorSalesman;
                if (itemToValidate.CostCentreCode != null)
                {
                    bool hasDuplicateCode = _ctx.tblCostCentre.Any(n => n.CostCentreType == (int)CostCentreType.DistributorSalesman && n.Id != itemToValidate.Id && n.Cost_Centre_Code==itemToValidate.CostCentreCode); //GO=>GetAll(true).OfType<DistributorSalesman>()=>not neccessary for validations
                        
                    if (hasDuplicateCode)
                        vri.Results.Add(new ValidationResult(string.Format("Duplicate Code found=>{0}",itemToValidate.CostCentreCode)));
                    if(itemToValidate.ParentCostCentre==null)
                        vri.Results.Add(new ValidationResult("Distributor is required"));
                   
                }
            }
            if (itemToValidate is PurchasingClerk)
            {
                PurchasingClerk purchasingClerk = itemToValidate as PurchasingClerk;
            }
            if(itemToValidate is Store)
            {
                Store store = itemToValidate as Store;
                
                bool hasDuplicateCode = _ctx.tblCostCentre.Where(s=>s.CostCentreType==(int)CostCentreType.Store)
                    .Where(s => s.Id != itemToValidate.Id)
                    .Any(p => p.Cost_Centre_Code == itemToValidate.CostCentreCode);
                if (hasDuplicateCode)
                    vri.Results.Add(new ValidationResult("Duplicate Store Code found"));

                bool hasDuplicateName = GetAll(true)
                    .OfType<Store>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Any(p => p.Name == itemToValidate.Name);
                if (hasDuplicateName)
                    vri.Results.Add(new ValidationResult("Duplicate Store Name found"));
            }

            if (itemToValidate is CommoditySupplier)
            {
                CommoditySupplier commoditySupplier = itemToValidate as CommoditySupplier;
                
                bool hasDuplicateCode = GetAll(true)
                    .OfType<CommoditySupplier>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Any(p => p.CostCentreCode == itemToValidate.CostCentreCode);
                if (hasDuplicateCode)
                    vri.Results.Add(new ValidationResult("Duplicate Commodity Supplier Code found"));

                bool hasDuplicateName = GetAll(true)
                    .OfType<CommoditySupplier>()
                    .Where(s => s.Id != itemToValidate.Id)
                    .Any(p => p.Name == itemToValidate.Name);
                if (hasDuplicateName)
                    vri.Results.Add(new ValidationResult("Duplicate Commodity Supplier Name found"));
            }

            return vri;
        }
    }
}
