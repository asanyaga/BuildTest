using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Util;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Outlets;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModels.Admin.Outlets;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility.MasterData;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Outlets
{
    public class OutletViewModelBuilder : IOutletViewModelBuilder
    {
        private IOutletRepository _outletRepository;
        private IRouteRepository _routeRepository;
        private IOutletCategoryRepository _outletCategoryRepository;
        private IOutletTypeRepository _outletTypeRepository;
        private IDistributorRepository _distributorRepository;
        private IUserViewModelBuilder _userViewModelBuilder;
        private IUserRepository _userRepository;
        private ICostCentreRepository _costCenterRepository;
        private ICostCentreFactory _costCentreFactory;
        private IDiscountGroupRepository _discountGroupRepository;
        private IVATClassRepository _vatClassRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private EntityUsage _entityUsage;

        public OutletViewModelBuilder(EntityUsage entityUsage, IDiscountGroupRepository discountGroupRepository,
                                      IOutletRepository outletRepository,
                                      IRouteRepository routeRepository,
                                      IOutletCategoryRepository outletCategoryRepository,
                                      IOutletTypeRepository outletTypeRepository,
                                      IDistributorRepository distributorRepository,
                                      IUserViewModelBuilder userViewModelBuilder, IUserRepository userRepository,
                                      ICostCentreFactory costCentreFactory, ICostCentreRepository costCenterRepository,
                                      IVATClassRepository vatClassRepository,
                                      IProductPricingTierRepository productPricingTierRepository)
        {
            _outletRepository = outletRepository;
            _routeRepository = routeRepository;
            _outletCategoryRepository = outletCategoryRepository;
            _outletTypeRepository = outletTypeRepository;
            _distributorRepository = distributorRepository;
            _userViewModelBuilder = userViewModelBuilder;
            _userRepository = userRepository;
            _costCentreFactory = costCentreFactory;
            _costCenterRepository = costCenterRepository;
            _discountGroupRepository = discountGroupRepository;
            _vatClassRepository = vatClassRepository;
            _productPricingTierRepository = productPricingTierRepository;

            _entityUsage = entityUsage;
        }

        private OutletViewModel Map(Outlet outlet)
        {
            //Dictionary<int, string> routes = _routeRepository.GetAll().ToList().ToDictionary(n => n.Id, n => n.Name);
            //Dictionary<int, string> outletCategory = _outletCategoryRepository.GetAll().ToList().ToDictionary(n => n.Id, n => n.Name);
            //Dictionary<int, string> outletType = _outletTypeRepository.GetAll().ToList().ToDictionary(n => n.Id, n => n.Name);
            //Dictionary<int, string> pricingTier = _productPricingTierRepository.GetAll().ToList().ToDictionary(n => n.Id, n => n.Name);
            //Dictionary<int, string> vatClass = _vatClassRepository.GetAll().ToList().ToDictionary(n => n.Id, n => n.Name);
            var outletCateg = _outletCategoryRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id,
                                                                                                            n => n.Name);
            var outletTypes = _outletTypeRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id,
                                                                                                        n => n.Name);
            var pricingTiers =
                _productPricingTierRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
            var route = _routeRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
            var vatClasses = _vatClassRepository.GetAll().ToList().OrderBy(n => n.VatClass).ToDictionary(n => n.Id,
                                                                                                         n => n.Name);
            var discountGroups = _discountGroupRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(
                n => n.Id, n => n.Name);
            OutletViewModel outletViewModel = new OutletViewModel();
            {
                outletViewModel.Id = outlet.Id;
                outletViewModel.Name = outlet.Name;
                if (outlet.OutletCategory != null)
                {
                    outletViewModel.OutletCategoryId = _outletCategoryRepository.GetById(outlet.OutletCategory.Id).Id;
                }

                outletViewModel.OutletCategoryList = new SelectList(outletCateg, "key", "value",
                                                                    outlet.OutletCategory == null
                                                                        ? Guid.Empty
                                                                        : outlet.OutletCategory.Id);
                if (outlet.OutletCategory != null)
                {
                    outletViewModel.OutletCategoryName =
                        _outletCategoryRepository.GetById(outlet.OutletCategory.Id).Name;
                }
                outletViewModel.OutLetCode = outlet.CostCentreCode;
                if (outlet.OutletType != null)
                {
                    outletViewModel.OutletTypeId = _outletTypeRepository.GetById(outlet.OutletType.Id).Id;
                }
                outletViewModel.OutletTypeList = new SelectList(outletTypes, "key", "value",
                                                                outlet.OutletType == null
                                                                    ? Guid.Empty
                                                                    : outlet.OutletType.Id);
                if (outlet.OutletType != null)
                {
                    outletViewModel.OutletTypeName = _outletTypeRepository.GetById(outlet.OutletType.Id).Name;
                }
                if (outlet.Route != null)
                {
                    outletViewModel.RouteId = _routeRepository.GetById(outlet.Route.Id).Id;
                }
                if (outlet.Route != null)
                {
                    outletViewModel.RouteName = _routeRepository.GetById(outlet.Route.Id).Name;
                }
                if (outlet.UsrSalesRep != null)
                {
                    outletViewModel.SalesRepName = _userRepository.GetById(outlet.UsrSalesRep.Id).Username;
                }
                if (outlet.VatClass != null)
                {
                    outletViewModel.vatClassName = _vatClassRepository.GetById(outlet.VatClass.Id).Name;
                }
                if (outlet.OutletProductPricingTier != null)
                {
                    outletViewModel.pricingTierName =
                        _productPricingTierRepository.GetById(outlet.OutletProductPricingTier.Id).Name;
                }
                outletViewModel.vatClassId = outlet.VatClass == null ? Guid.Empty : outlet.VatClass.Id;
                outletViewModel.VatClassList = new SelectList(vatClasses, "Key", "Value",
                                                              outlet.VatClass == null ? Guid.Empty : outlet.VatClass.Id);
                outletViewModel.pricingTierId = outlet.OutletProductPricingTier == null
                                                    ? Guid.Empty
                                                    : outlet.OutletProductPricingTier.Id;
                outletViewModel.PricingTiersList = new SelectList(pricingTiers, "Key", "Value",
                                                                  outlet.OutletProductPricingTier == null
                                                                      ? Guid.Empty
                                                                      : outlet.OutletProductPricingTier.Id);
                outletViewModel.OutLetCode = outlet.CostCentreCode;
                outletViewModel.DiscountGroup = outlet.DiscountGroup == null ? Guid.Empty : outlet.DiscountGroup.Id;
                outletViewModel.DiscountGroupList = new SelectList(discountGroups, "Key", "Value",
                                                                   outlet.DiscountGroup == null
                                                                       ? Guid.Empty
                                                                       : outlet.DiscountGroup.Id);
                outletViewModel.DiscountGroupName = outlet.DiscountGroup == null ? "" : outlet.DiscountGroup.Name;
                outletViewModel.distributor = outlet.ParentCostCentre == null ? Guid.Empty : outlet.ParentCostCentre.Id;
                if (outlet.ParentCostCentre.Id != null)
                {
                    var outlet_ = _costCenterRepository.GetById(outlet.ParentCostCentre.Id);
                    if (outlet_ != null) outletViewModel.DistributorName = outlet_.Name;
                }
                var distRegion = ((Distributor) _distributorRepository.GetById(outletViewModel.distributor)).Region.Id;
                var routes = _routeRepository.GetAll().Where(n => n.Region.Id == distRegion).ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);
                outletViewModel.RouteList = new SelectList(routes, "key", "value",
                                           outlet.Route == null ? Guid.Empty : outlet.Route.Id);
                outletViewModel.IsActive = outlet._Status == EntityStatus.Active ? true : false;
                outletViewModel.DateCreated = outlet._DateCreated;
            }
            return outletViewModel;
        }

        public IList<OutletViewModel> GetAll(bool inactive = false)
        {
            List<Outlet> outlets = _outletRepository.GetAll(inactive).OfType<Outlet>().ToList();
            return outlets.Select(n => new OutletViewModel
                                           {
                                               Id = n.Id,
                                               Name = n.Name,
                                               RouteName = _routeRepository.GetById(n.Route.Id).Name,
                                               OutletTypeName = _outletTypeRepository.GetById(n.OutletType.Id).Name,
                                               OutletCategoryName =
                                                   _outletCategoryRepository.GetById(n.OutletCategory.Id).Name,
                                               IsActive = n._Status == EntityStatus.Active ? true : false,
                                               SurveyorName = _userViewModelBuilder.Get(n.UsrSurveyor.Id).Username,
                                               SalesRepName = _userViewModelBuilder.Get(n.UsrSalesRep.Id).Username,
                                               DistributorName =
                                                   _costCenterRepository.GetById(n.ParentCostCentre.Id).Name,
                                               DateCreated = n._DateCreated,
                                               //DistributorName=_distributorRepository.GetDistributor().Name 

                                           }).ToList();

        }

        public OutletViewModel Get(Guid Id)
        {
            Outlet outlet = _outletRepository.GetById(Id) as Outlet;
            if (outlet == null) return null;
            return Map(outlet);
        }

        public void SetInActive(Guid Id)
        {
            Outlet outlet = new Outlet(Id);
            //string usage = CheckOutletUsage(outlet.Id);
            //if (!string.IsNullOrEmpty(usage))
            //    throw new Exception(usage);
            _outletRepository.SetInactive(outlet);
        }

        public void Activate(Guid id)
        {
            Outlet o = _outletRepository.GetById(id) as Outlet;
            _outletRepository.SetActive(o);
        }

        public void Delete(Guid id)
        {
            Outlet o = _outletRepository.GetById(id) as Outlet;
            //string usage = CheckOutletUsage(o.Id);
            //if (!string.IsNullOrEmpty(usage))
            //    throw new Exception(usage);
            _outletRepository.SetAsDeleted(o);
        }

        public string CheckOutletUsage(Guid outletId)
        {
            Outlet o = _outletRepository.GetById(outletId) as Outlet;
            return OutletUsage(o);
        }

        public string OutletUsage(Outlet outlet)
        {
            string msg = "";
            //var outletOrders = _orderRepository.GetAllOrders().Where(n => n.IssuedOnBehalfOf.Id == outlet.Id);

            //if (outletOrders.Count() > 0)
            //{
            //    msg += "  - There are orders for outlet " + outlet.Name + " which will not be visible after the outlet is deactivated.";
            //}

           if (_entityUsage.CheckHasOutStandingPayments(outlet))
            {
                msg += "Cannot deactivate outlet " + outlet.Name + " because of outstanding payments.";
            }
            return msg;
        }

        public void SetActive(Guid id)
        {
            Outlet outlet = new Outlet(id);
            _outletRepository.SetActive(outlet);
        }

        IEnumerable<OutletViewModel> IOutletViewModelBuilder.GetAll(bool inactive)
        {
            DateTime start = DateTime.Now;
            var outlet = _outletRepository.GetAll(inactive).OfType<Outlet>().Skip(0).Take(10);
            TimeSpan diff = DateTime.Now.Subtract(start);
            var result = outlet.Select(n => Map(n)).ToList();
            ;
            TimeSpan mapdiff = DateTime.Now.Subtract(start);
            return result;
        }

        public OutletViewModel GetLastCreatedOutlet(bool inactive)
        {
            var outlet =
                _outletRepository.GetAll(inactive).OfType<Outlet>().OrderByDescending(n => n._DateCreated).First();
            return Map(outlet);
        }
        public void Save(OutletViewModel outletviewmodel)
        {
            if (outletviewmodel.DistributorName != null || outletviewmodel.RouteName != null ||
                outletviewmodel.OutletCategoryName != null || outletviewmodel.OutletTypeName != null ||
                outletviewmodel.pricingTierName != null || outletviewmodel.vatClassName != null)
            {
                VATClass vc =
                    _vatClassRepository.GetAll().FirstOrDefault(
                        n => n.VatClass.ToLower() == outletviewmodel.vatClassName.ToLower());
                ProductPricingTier ppt =
                    _productPricingTierRepository.GetAll().FirstOrDefault(
                        n => n.Code.ToLower() == outletviewmodel.pricingTierCode.ToLower());
                DiscountGroup dg =
                    _discountGroupRepository.GetAll().FirstOrDefault(
                        n => n.Name.ToLower() == outletviewmodel.DiscountGroupName.ToLower());
                OutletType ot =
                    _outletTypeRepository.GetAll().FirstOrDefault(
                        n => n.Code.ToLower() == outletviewmodel.OutletTypeCode.ToLower());
                OutletCategory oc =
                    _outletCategoryRepository.GetAll().FirstOrDefault(
                        n => n.Code.ToLower() == outletviewmodel.OutletCategoryCode.ToLower());
                Route rt =
                    _routeRepository.GetAll().FirstOrDefault(n => n.Code.ToLower() == outletviewmodel.Code.ToLower());


                Outlet outlet = null;

                if (outletviewmodel.Id == Guid.Empty)
                {
                    Distributor ParentCC = _costCenterRepository.GetById(outletviewmodel.distributor) as Distributor;
                        // _distributorRepository.GetDistributor();
                    outlet =
                        _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, ParentCC) as Outlet;
                    outlet._SetStatus(EntityStatus.Active);
                }
                else
                    outlet = _costCenterRepository.GetById(outletviewmodel.Id) as Outlet;
                outlet.CostCentreCode = outletviewmodel.OutLetCode;
                outlet.Name = outletviewmodel.Name;
                outlet.Route = _routeRepository.GetById(rt.Id);
                outlet.OutletType = _outletTypeRepository.GetById(ot.Id);
                outlet.OutletCategory = _outletCategoryRepository.GetById(oc.Id);
                if (dg != null)
                {
                    outlet.DiscountGroup = _discountGroupRepository.GetById(dg.Id);
                }
                outlet.OutletProductPricingTier = _productPricingTierRepository.GetById(ppt.Id);

                _outletRepository.Save(outlet);
            }
            else
            {
                Outlet outlet = null;

                if (outletviewmodel.Id == Guid.Empty)
                {
                    Distributor ParentCC = _costCenterRepository.GetById(outletviewmodel.distributor) as Distributor;
                        // _distributorRepository.GetDistributor();
                    outlet =
                        _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, ParentCC) as Outlet;
                    outlet._SetStatus(EntityStatus.Active);
                }
                else
                    outlet = _costCenterRepository.GetById(outletviewmodel.Id) as Outlet;
                outlet.CostCentreCode = outletviewmodel.OutLetCode;
                outlet.Name = outletviewmodel.Name;
                outlet.Route = _routeRepository.GetById(outletviewmodel.RouteId);
                outlet.OutletType = _outletTypeRepository.GetById(outletviewmodel.OutletTypeId);
                outlet.OutletCategory = _outletCategoryRepository.GetById(outletviewmodel.OutletCategoryId);
                outlet.DiscountGroup = outletviewmodel.DiscountGroup == null
                                           ? null
                                           : _discountGroupRepository.GetById(outletviewmodel.DiscountGroup.Value);
                outlet.OutletProductPricingTier = _productPricingTierRepository.GetById(outletviewmodel.pricingTierId);
                if (outletviewmodel.vatClassId != Guid.Empty)
                    outlet.VatClass = _vatClassRepository.GetById(outletviewmodel.vatClassId);

                _outletRepository.Save(outlet);
            }

        }

        private ShipToAddress Map(OutletViewModel.ShipToAddressItem address)
        {
            if (address.Id == Guid.Empty)
                address.Id = Guid.NewGuid();
            var shipToAddress = new ShipToAddress(address.Id);
            shipToAddress.Name = address.Name;
            shipToAddress.PhysicalAddress = address.PhysicalAdress;
            shipToAddress.PostalAddress = address.PostalAddress;
            shipToAddress.Description = address.Description;
            shipToAddress.Latitude = address.Latitude;
            shipToAddress.Longitude = address.Longitude;
            return shipToAddress;
        }

        public Dictionary<Guid, string> Route()
        {
            return _routeRepository.GetAll().OrderBy(n => n.Name)
                .Select(s => new {s.Id, s.Name}
                ).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> Route(Guid regionId)
        {
            var allRoutes = _routeRepository.GetAll();
            var regRoutes = allRoutes
                .Where(n => n.Region.Id == regionId)
                .OrderBy(n => n.Name)
                .Select(s => new { s.Id, s.Name })
                .ToList().ToDictionary(d => d.Id, d => d.Name);
            return regRoutes;
        }

        public Dictionary<Guid, string> OutletCategory()
        {
            return _outletCategoryRepository.GetAll().OrderBy(n => n.Name)
                .Select(s => new {s.Id, s.Name}
                ).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> OutletType()
        {
            return _outletTypeRepository.GetAll().OrderBy(n => n.Name)
                .Select(s => new {s.Id, s.Name}
                ).ToList().ToDictionary(d => d.Id, d => d.Name);
        }
        
        public Dictionary<Guid, string> OutletUser()
        {
            return
                _userRepository.GetAll().Where(h=>h.UserType == UserType.DistributorSalesman).OrderBy(l => l.Username).Select(k => new {k.Id, k.Username}).ToList().
                    ToDictionary(j => j.Id, j => j.Username);
        }

        //public IList<OutletViewModel> Search(string searchParam, bool inactive = false)
        //{
        //    List<Outlet> outlets = _outletRepository.GetAll(inactive).Where(n=>(n.Name.ToLower().StartsWith(searchParam.ToLower()))).OfType<Outlet>().ToList();
        //    return outlets.Select(n => new OutletViewModel
        //    {
        //        Id = n.Id,
        //        Name = n.Name,
        //        RouteName = _routeRepository.GetById(n.Route.Id).Name,
        //        OutletTypeName = _outletTypeRepository.GetById(n.OutletType.Id).Name,
        //        OutletCategoryName = _outletCategoryRepository.GetById(n.OutletCategory.Id).Name,
        //        DistributorName=_distributorRepository.GetDistributor().Name ,
        //        SurveyorName = n.UsrSurveyor == null ? "" : n.UsrSurveyor.Username,
        //        SalesRepName = n.UsrSalesRep == null ? "" : n.UsrSalesRep.Username,
        //        ASM = n.UsrASM == null ? "" : n.UsrASM.Username,
        //        IsActive = n._Status


        //    }).ToList();
        //}


        public Dictionary<Guid, string> GetDistributor()
        {
            return _distributorRepository.GetAll().OfType<Distributor>()
                .Select(n => new {n.Id, n.Name}).ToList().ToDictionary(n => n.Id, n => n.Name);
        }


        //public IList<OutletViewModel> GetByDistributor(int distId, bool inactive = false)
        //{
        //    List<Outlet> outlets = _outletRepository.GetAll(inactive).OfType<Outlet>().Where(n=>n.ParentCostCentre.Id==distId).ToList();
        //    return outlets.Select(n => new OutletViewModel
        //    {
        //        Id = n.Id,
        //        Name = n.Name,
        //        RouteName = _routeRepository.GetById(n.Route.Id).Name,
        //        OutletTypeName = _outletTypeRepository.GetById(n.OutletType.Id).Name,
        //        OutletCategoryName = _outletCategoryRepository.GetById(n.OutletCategory.Id).Name,
        //        IsActive = n._Status,
        //        SurveyorName = n.UsrSurveyor == null ? "" : n.UsrSurveyor.Username,
        //        SalesRepName = n.UsrSalesRep == null ? "" : n.UsrSalesRep.Username,
        //        ASM = n.UsrASM == null ? "" : n.UsrASM.Username,
        //        DistributorName=_costCenterRepository.GetById(n.ParentCostCentre.Id).Name 


        //    }).ToList();
        //}

        //public IList<OutletViewModel> GetByDistributor(int distId, bool inactive = false)
        //{
        //    List<Outlet> outlets = _outletRepository.GetAll(inactive).OfType<Outlet>().Where(n=>n.ParentCostCentre.Id==distId).ToList();
        //    return outlets.Select(n => new OutletViewModel
        //    {
        //        Id = n.Id,
        //        Name = n.Name,
        //        RouteName = _routeRepository.GetById(n.Route.Id).Name,
        //        OutletTypeName = _outletTypeRepository.GetById(n.OutletType.Id).Name,
        //        OutletCategoryName = _outletCategoryRepository.GetById(n.OutletCategory.Id).Name,
        //        IsActive = n._Status,
        //        SurveyorName = n.UsrSurveyor == null ? "" : n.UsrSurveyor.Username,
        //        SalesRepName = n.UsrSalesRep == null ? "" : n.UsrSalesRep.Username,
        //        ASM = n.UsrASM == null ? "" : n.UsrASM.Username,
        //        DistributorName=_costCenterRepository.GetById(n.ParentCostCentre.Id).Name 


        //    }).ToList();
        //}

        public Dictionary<Guid, string> GetDiscountGroup()
        {
            return
                _discountGroupRepository.GetAll().OrderBy(n => n.Name).ToList().Select(n => new {n.Id, n.Name}).
                    ToDictionary(n => n.Id, n => n.Name);
        }


        public Dictionary<Guid, string> GetPricingTier()
        {
            return
                _productPricingTierRepository.GetAll().OrderBy(n => n.Name).ToList().Select(n => new {n.Id, n.Name}).
                    ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> GetVatClass()
        {
            return
                _vatClassRepository.GetAll().OrderBy(n => n.VatClass).ToList().Select(n => new {n.Id, n.VatClass}).
                    ToDictionary(n => n.Id, n => n.VatClass);
        }

        public Dictionary<Guid, string> ASM()
        {
            Dictionary<Guid, string> ASMDictionary = new Dictionary<Guid, string>();
            ASMDictionary.Add(Guid.Empty, "----- None Selected -----");
            IEnumerable<User> ASMList = _userRepository.GetAll().Where(n => n.UserType == UserType.ASM);
            foreach (User asm in ASMList)
            {
                ASMDictionary.Add(asm.Id, asm.Username);
            }
            return ASMDictionary;
        }

        public Dictionary<Guid, string> SalesRep()
        {
            Dictionary<Guid, string> SalesRepDictionary = new Dictionary<Guid, string>();
            SalesRepDictionary.Add(Guid.Empty, "----- None Selected -----");
            IEnumerable<User> SalesRepList = _userRepository.GetAll().Where(n => n.UserType == UserType.SalesRep);
            foreach (User SalesRep in SalesRepList)
            {
                SalesRepDictionary.Add(SalesRep.Id, SalesRep.Username);
            }
            return SalesRepDictionary;
        }

        public Dictionary<Guid, string> Surveyor()
        {
            Dictionary<Guid, string> SurveyorDictionary = new Dictionary<Guid, string>();
            SurveyorDictionary.Add(Guid.Empty, "----- None Selected -----");
            IEnumerable<User> SurveyorList = _userRepository.GetAll().Where(n => n.UserType == UserType.Surveyor);
            foreach (User Surveyor in SurveyorList)
            {
                SurveyorDictionary.Add(Surveyor.Id, Surveyor.Username);
            }
            return SurveyorDictionary;
        }

        public IPagedList<OutletViewModel> GetOutlet(int currentPageIndex, int defaultPageSize = 10,
                                                     bool inactive = false, string searchParam = "", Guid? distId = null)
        {
            DateTime start = DateTime.Now;
            var outlet = _outletRepository.GetAll(inactive).OfType<Outlet>();
            if (searchParam != "")
            {
                outlet = outlet.Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower())));
            }
            if (distId != null && distId != Guid.Empty)
            {
                outlet = outlet.Where(n => n.ParentCostCentre.Id == distId);
            }
            var takeoutlet = outlet.Skip(currentPageIndex*defaultPageSize).Take(defaultPageSize);
            TimeSpan diff = DateTime.Now.Subtract(start);
            var result = takeoutlet.Select(n => Map(n)).ToList();
            ;
            TimeSpan mapdiff = DateTime.Now.Subtract(start);
            return result.ToPagedList(currentPageIndex, defaultPageSize, outlet.Count());
        }

        public QueryResult<OutletViewModel> Query(QueryStandard q, Guid? distId = null)
        {
            var queryResult = _outletRepository.Query(q, distId);

            var result = new QueryResult<OutletViewModel>();

            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();

            return result;
        }


        //public OutletViewModel GetOutletsSkipTake(bool inactive = false)
        //{
        //    OutletViewModel outletVM = new OutletViewModel
        //    {
        //        Items=_outletRepository.GetAll(inactive).OfType<Outlet>()
        //        .Select(n=>new OutletViewModel.OutletViewModelItems
        //    {
        //         Code=n.CostCentreCode,
        //         DiscountGroup = n.DiscountGroup == null ? Guid.Empty: n.DiscountGroup.Id,
        //          DiscountGroupName=n.DiscountGroup==null?"":n.DiscountGroup.Name,
        //           distributor=n.ParentCostCentre.Id,
        //            DistributorName=_costCenterRepository.GetById(n.ParentCostCentre.Id).Name,
        //             ErrorText="",
        //              Id=n.Id,
        //         pricingTierId = n.OutletProductPricingTier == null ? Guid.Empty: n.OutletProductPricingTier.Id,
        //                pricingTierName=n.OutletProductPricingTier==null?"":n.OutletProductPricingTier.Name,
        //                 IsActive=n._Status,
        //                 Name=n.Name,
        //                  OutLetCode=n.CostCentreCode,
        //    }).ToList()
        //    };
        //    return outletVM;
        //}


        public IList<OutletViewModel> GetByDistributor(Guid distId, bool inactive = false)
        {
            DateTime start = DateTime.Now;
            var items = _outletRepository.GetAll(inactive).OfType<Outlet>().Where(n => n.ParentCostCentre.Id == distId);
            TimeSpan diff = DateTime.Now.Subtract(start);
            return items.Select(n => new OutletViewModel
                                         {
                                             Code = n.CostCentreCode,
                                             DiscountGroup = n.DiscountGroup == null ? Guid.Empty : n.DiscountGroup.Id,
                                             DiscountGroupName = n.DiscountGroup == null ? "" : n.DiscountGroup.Name,
                                             distributor = n.ParentCostCentre.Id,
                                             DistributorName = _costCenterRepository.GetById(n.ParentCostCentre.Id).Name,
                                             ErrorText = "",
                                             Id = n.Id,
                                             pricingTierId =
                                                 n.OutletProductPricingTier == null
                                                     ? Guid.Empty
                                                     : n.OutletProductPricingTier.Id,
                                             pricingTierName =
                                                 n.OutletProductPricingTier == null
                                                     ? ""
                                                     : n.OutletProductPricingTier.Name,
                                             IsActive = n._Status == EntityStatus.Active ? true : false,
                                             Name = n.Name,
                                             OutLetCode = n.CostCentreCode,
                                         }).ToList();
        }


        public IList<OutletViewModel> Search(string searchParam, bool inactive = false)
        {
            var items =
                _outletRepository.GetAll(inactive).OfType<Outlet>().Where(
                    n => (n.Name.ToLower().StartsWith(searchParam.ToLower())));
            return items.Select(n => new OutletViewModel
                                         {
                                             Code = n.CostCentreCode,
                                             DiscountGroup = n.DiscountGroup == null ? Guid.Empty : n.DiscountGroup.Id,
                                             DiscountGroupName = n.DiscountGroup == null ? "" : n.DiscountGroup.Name,
                                             distributor = n.ParentCostCentre.Id,
                                             DistributorName = _costCenterRepository.GetById(n.ParentCostCentre.Id).Name,
                                             ErrorText = "",
                                             Id = n.Id,
                                             pricingTierId =
                                                 n.OutletProductPricingTier == null
                                                     ? Guid.Empty
                                                     : n.OutletProductPricingTier.Id,
                                             pricingTierName =
                                                 n.OutletProductPricingTier == null
                                                     ? ""
                                                     : n.OutletProductPricingTier.Name,
                                             IsActive = n._Status == EntityStatus.Active ? true : false,
                                             Name = n.Name,
                                             OutLetCode = n.CostCentreCode,
                                         }).ToList();
        }

        public Guid GetRegionIdForDistributor(Guid distId)
        {
            var dist = _distributorRepository.GetById(distId);
            if (dist == null) return Guid.Empty;
            return ((Distributor) dist).Region.Id;
        }
    }
}
