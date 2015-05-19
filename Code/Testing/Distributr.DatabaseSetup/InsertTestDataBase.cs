using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Data.Repository.MasterData.ProductRepositories;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Utility;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;
using Distributr.Core.Utility.Security;
using MaritalStatus = Distributr.Core.Domain.Master.CostCentreEntities.MaritalStatus;

namespace Distributr.DatabaseSetup
{
    public abstract class InsertTestDataBase
    {
        protected IProductTypeRepository _productTypeRepository;
        protected IProductBrandRepository _productBrandRepository;
        protected IProductFlavourRepository _productFlavourRepository;
        protected IProductPackagingRepository _productPackagingRepository;
        protected IProductPackagingTypeRepository _productPackagingTypeRepository;
        protected IProductRepository _productRepository;
        protected IRegionRepository _regionRepository;
        protected ICostCentreRepository _costCentreRepository;
        protected ICostCentreFactory _costCentreFactory;
        protected IProductPricingRepository _pricingRepository;
        protected IVATClassRepository _vatClassRepository;
        protected IVATClassFactory _vatClassFactory;
        protected ICountryRepository _countryRepository;
        protected IProductPricingFactory _productPricingFactory;
        protected IProductPricingTierRepository _ProductPricingTierRepository;
        protected IOutletTypeRepository _outletTypeRepository;
        protected IOutletVisitReasonsTypeRepository _outletVisitReasonsTypeRepository;
        protected IUserRepository _userRepository;
        protected IOutletRepository _outletRepository;
        protected IRouteRepository _routeRepository;
        protected IRouteFactory _routeFactory;
        protected ITransporterRepository _transporterRepository;
        protected IProductFactory _productFactory;
        protected IDistributorSalesmanRepository _distributorSalesmanRepository;
        //protected IUserTypeRepository _usertyperepository;
        protected IProducerRepository _producerRepository;
        protected IDocumentFactory _documentFactory;
        protected ISocioEconomicStatusRepository _socioEconomicStatusRepository;
        protected IClientMasterDataTrackerRepository _clientMasterDataTrackerRepository;

        protected IDistributorRepository _distributorrepository;
        protected IOutletCategoryRepository _outletCategoryRepository;//
        //protected ICreateOrderCommandHandler _createOrderCommandHandler;
        protected ITerritoryRepository _territoryRepository;
        //protected IAddOrderLineItemCommandHandler _addOrderLineItemCommandHandler;
        protected IConfirmOrderCommandHandler _confirmOrderCommandHandler;
        protected IRejectOrderCommandHandler _rejectOrderCommandHandler;
        protected IAreaRepository _areaRepository;
        protected IContactRepository _contactRepository;
        protected IAccountRepository _accountRepository;
        protected IAccountTransactionRepository _accountTransactionRepository;
        protected IInventoryRepository _inventoryRepository;
        protected IInventoryTransactionRepository _inventoryTransactionRepository;
        protected ICostCentreApplicationRepository _costCentreApplicationRepository;
        protected IChannelPackagingRepository _channelPackagingRepository;
        protected ICompetitorRepository _competitorRepository;
        protected ICompetitorProductsRepository _competitorProductRepository;
        protected IAssetRepository _coolerRepository;
        protected IAssetTypeRepository _coolerTypeRepository;
        protected IDistrictRepository _districtRepository;
        protected IProvincesRepository _provinceRepository;
        protected IReOrderLevelRepository _reorderLevelRepository;
        protected ITargetPeriodRepository _targetPeriodRepository;
        protected ITargetRepository _targetRepository;
        protected IProductDiscountFactory _productDiscountFactory;
        protected IProductDiscountRepository _productDiscountRepository;
        protected ISaleValueDiscountFactory _saleValueDiscountFactory;
        protected ISaleValueDiscountRepository _saleValueDiscountRepository;
        //protected IContainmentRepository _containmentRepository;
        protected ISalesmanRouteRepository _salesmanRouteRepository;
        protected ISalesmanSupplierRepository _salesmanSupplierRepository;
        protected IUserGroupRepository _userGroupRepository;
        protected IUserGroupRolesRepository _userGroupRolesRepository;
        protected IBankRepository _bankRepository;
        protected IBankBranchRepository _bankBranchRepository;
        protected ISupplierRepository _supplierRepository;
        //Loss
        protected ICreatePaymentNoteCommandHandler _createLossCommandHandler;
        protected IAddPaymentNoteLineItemCommandHandler _addLossLineItemCommandHandler;
        protected IConfirmPaymentNoteCommandHandler _confirmLossCommandHandler;
        protected IContactTypeRepository _contactTypeRepository;
      
        protected IAssetStatusRepository _assetStatusRepository;
        protected IAssetCategoryRepository _assetCategoryRepository;
        protected IDiscountGroupRepository _discountGroupRepository;
        protected IProductDiscountGroupFactory _productDiscountGroupFactory;
        protected ICertainValueCertainProductDiscountFactory _certainValueCertainProductDiscountFactory;
        protected ICustomerDiscountFactory _customerDiscountFactory;
        protected IPromotionDiscountFactory _promotionDiscountFactory;
        protected IProductDiscountGroupRepository _productDiscountGroupRepository;
        protected IPromotionDiscountRepository _promotionDiscountRepository;
        protected IFreeOfChargeDiscountRepository _freeOfChargeDiscountRepository;
        protected ICertainValueCertainProductDiscountRepository _certainValueCertainProductDiscountRepository;
        protected IOutletPriorityRepository _outletPriorityRepository;
        protected IOutletVisitDayRepository _outletVisitDayRepository;
        protected ITargetItemRepository _targetItemRepository;
        protected ISettingsRepository _settingsRepository;
        protected IRetireDocumentSettingRepository _retireDocumentSettingRepository;

        protected ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        protected ICommodityOwnerRepository _commodityOwnerRepository;
        protected ICommoditySupplierRepository _commoditySupplierRepository;
        protected ICommodityProducerRepository _commodityProducerRepository;
        protected ICentreTypeRepository _centreTypeRepository;
        protected ICentreRepository _centreRepository;
        protected ICommodityRepository _commodityRepository;
        protected ICommodityTypeRepository _commodityTypeRepository;
        protected IEquipmentRepository _equipmentRepository;
        protected IMasterDataAllocationRepository _masterDataAllocationRepository;
        protected IContainerTypeRepository _containerTypeRepository;
        protected IVehicleRepository _vehicleRepository;
        protected IHubRepository _hubRepository;
        protected IPurchasingClerkRouteRepository _purchasingClerkRouteRepository;

        protected IServiceProviderRepository _ServiceProviderRepository;
        protected IServiceRepository _ServiceRepository;
        protected ISeasonRepository _SeasonRepository;
        protected IShiftRepository _ShiftRepository;
        protected IInfectionRepository _InfectionRepository;
        protected IActivityTypeRepository _activityTypeRepository;

        protected Guid AddRetireSetting(RetireType type, int duration)
        {
            RetireDocumentSetting pb = new RetireDocumentSetting(Guid.NewGuid())
            {
                Duration = duration,
                RetireType = type,
               

            };
            return _retireDocumentSettingRepository.Save(pb);
        }
         protected Guid CreateOutletPriority(Guid outletId, Guid route, int priority)
         {
             OutletPriority p = new OutletPriority(Guid.NewGuid())
                                    {
                                        EffectiveDate=DateTime.Now.AddDays(-1),
                                        Outlet= new CostCentreRef{ Id=outletId},
                                        Route=_routeRepository.GetById(route),
                                        Priority=priority

                                    };
             return _outletPriorityRepository.Save(p);
         }
         protected Guid CreateOutletVistDay(Guid outletId, DayOfWeek day)
         {
             OutletVisitDay p = new OutletVisitDay(Guid.NewGuid())
             {
                 EffectiveDate = DateTime.Now.AddDays(-1),
                 Outlet = new CostCentreRef { Id = outletId },
                 Day = day
             };
             return _outletVisitDayRepository.Save(p);
         }
        protected Guid CreateDistributorSalesman(Guid distrib, Guid route, string salesmanName, Guid usergroupid,string code,DistributorSalesmanType type)
        {
            DistributorSalesman salesman = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman, _costCentreRepository.GetById(distrib)) as DistributorSalesman;
            salesman.Name = salesmanName;// "Test Salesman";
            salesman.Type = type;
            salesman.CostCentreCode = code;
            Guid id = _costCentreRepository.Save(salesman);
            Route r = _routeRepository.GetById(route);
            AddUser(salesmanName, "12345678", "32323",Guid.NewGuid().ToString(), id, UserType.DistributorSalesman, usergroupid);
            CreateSalemanRoute(distrib, id, r.Id);
            
            
            return id;
        }

        protected Outlet CreateOutlet(Guid outCategory, Guid outType, Guid distrib, Guid route, Guid pricingTier, string outletName, Guid vatClass, string code, Guid discountGroup, float longitude=0F, float latitude =0F,Guid? specialtierId=null)
        {
            Outlet outlet = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, _costCentreRepository.GetById(distrib)) as Outlet;
            outlet._SetStatus(EntityStatus.Active);
            outlet.Route = _routeRepository.GetById(route);
            outlet.OutletCategory = _outletCategoryRepository.GetById(outCategory);
            outlet.OutletType = _outletTypeRepository.GetById(outType);
            outlet.Name = outletName;// "Kibatet Estate Dispensary";
            if (discountGroup != Guid.Empty)
                outlet.DiscountGroup = _discountGroupRepository.GetById(discountGroup);
           // outlet.outLetCode = code;
            outlet.CostCentreCode = code;
            outlet.Latitude = latitude.ToString();
            outlet.Longitude = longitude.ToString();
                outlet.VatClass = _vatClassRepository.GetById(vatClass);
            outlet.OutletProductPricingTier = _ProductPricingTierRepository.GetById(pricingTier);
            if (specialtierId != null)
                outlet.SpecialPricingTier = _ProductPricingTierRepository.GetById(specialtierId.Value);
            var shipTo1 = new ShipToAddress(Guid.NewGuid())
                {
                    Name =outletName +  "ShipTo1",
                    PhysicalAddress = "Address1",
                    PostalAddress = "PO Box 1",
                };
            var shipTo2 = new ShipToAddress(Guid.NewGuid())
            {
                Name = outletName + "ShipTo2",
                PhysicalAddress = "Address2",
                PostalAddress = "PO Box 2",
            };
            outlet.AddShipToAddress(shipTo1);
            outlet.AddShipToAddress(shipTo2);
            _costCentreRepository.Save(outlet);
            return outlet;
        }

        protected Guid AddProductBrand(Guid supplierId, string name, string desc, string code)
        {
            ProductBrand pb = new ProductBrand(Guid.NewGuid())
            {
                Name = name,
                Description = desc,
                Code = code,
                 Supplier=_supplierRepository.GetById(supplierId)

            };
            pb._SetStatus(EntityStatus.Active);
            return _productBrandRepository.Save(pb);
        }

        protected Guid AddProductFlavour(string name, string desc, string code, Guid brandId)
        {
            ProductFlavour pf = new ProductFlavour(Guid.NewGuid())

            {
                Name = name,
                Description = desc,
                Code = code,
                ProductBrand = _productBrandRepository.GetById(brandId),
            };
            pf._SetStatus(EntityStatus.Active);
            return _productFlavourRepository.Save(pf);
        }

        protected Guid AddProductType(string name, string code)
        {
            ProductType pt = new ProductType(Guid.NewGuid())

            {
                Name = name,
                 Code=code

            };
            pt._SetStatus(EntityStatus.Active);
            return _productTypeRepository.Save(pt); ;
        }

        protected Guid AddProductPackagingType(string name, string desc, string code)
        {
            ProductPackagingType ppt = new ProductPackagingType(Guid.NewGuid())

            {
                Name = name,
                Description = desc,
                 Code=code

            };

            ppt._SetStatus(EntityStatus.Active);
            return _productPackagingTypeRepository.Save(ppt);
        }

        protected Guid AddProductPackaging(string name, string desc, string code/*, int containmentId*/)
        {
            ProductPackaging pp = new ProductPackaging(Guid.NewGuid())

            {
                Name = name,
                Description = desc,
                 Code=code

            };
            /*if(containmentId!=0){
                pp.Containment = _containmentRepository.GetById(containmentId);
            }*/

            pp._SetStatus(EntityStatus.Active);
            return _productPackagingRepository.Save(pp);
        }

        protected Guid AddSaleProduct(string ProductCode, string Description, Guid FlavourId, Guid BrandId, Guid PackageId, Guid PackageTypeId, Guid ProductTypeId, int DomainTypeId, Guid vatClassId, Guid returnableProductId, decimal exfactoryPrice)
        {
            SaleProduct p = _productFactory.CreateSaleProduct(Guid.NewGuid());
            p.Brand = _productBrandRepository.GetById(BrandId);
            p.Description = Description;
            p.Flavour = _productFlavourRepository.GetById(FlavourId);
            p.Packaging = _productPackagingRepository.GetById(PackageId);
            p.PackagingType = _productPackagingTypeRepository.GetById(PackageTypeId);
            p.ProductCode = ProductCode;
            p.ExFactoryPrice = exfactoryPrice;
            p.ProductType = _productTypeRepository.GetById(ProductTypeId);
            p.VATClass = _vatClassRepository.GetById(vatClassId);
            if (returnableProductId != Guid.Empty)
            {
                p.ReturnableProduct = _productRepository.GetById(returnableProductId) as ReturnableProduct;
            }

            p._SetStatus(EntityStatus.Active);
            Guid test = _productRepository.Save(p);
            _productRepository.GetById(test);
            return test;
        }

        protected void AddProductPackagingInContainer(Guid pp1, Guid returnableProductId)
        {
            ProductPackaging pp=_productPackagingRepository.GetById(pp1);
            //pp.Containment = _containmentRepository.GetById(containmentId);//TODO: change to returnableproduct
            pp.ReturnableProductRef = new ProductRef { ProductId = returnableProductId };
            _productPackagingRepository.Save(pp);
        }

        protected Guid AddReturnableProduct(string ProductCode, string Description, Guid FlavourId, Guid BrandId, Guid PackageId, Guid PackageTypeId, Guid ProductTypeId, int DomainTypeId, ReturnableType returnableType, Guid vatClassId, int capacity, Guid returnableProductId,decimal exfactoryPrice)
        {
            ReturnableProduct p = _productFactory.CreateReturnableProduct(Guid.NewGuid());
            p.Brand = _productBrandRepository.GetById(BrandId);
            p.Description = Description;
            p.Flavour = _productFlavourRepository.GetById(FlavourId);
            p.ExFactoryPrice = exfactoryPrice;

            if (PackageId != Guid.Empty)
            {
                p.Packaging = _productPackagingRepository.GetById(PackageId);
            }            
            p.PackagingType = _productPackagingTypeRepository.GetById(PackageTypeId);
            p.ProductCode = ProductCode;
            p.Capacity = capacity;
            p.ReturnableType = returnableType;
            p.VATClass = _vatClassRepository.GetById(vatClassId);
            if (returnableProductId != Guid.Empty)
            {
                p.ReturnAbleProduct = _productRepository.GetById(returnableProductId) as ReturnableProduct;
            }
            Guid test = _productRepository.Save(p);
            _productRepository.GetById(test);
            return test;
        }

        /*protected int AddReturnableProduct(string ProductCode, string Description, int FlavourId, int BrandId, int PackageId, int PackageTypeId, int ProductTypeId, int DomainTypeId, ReturnableType returnableType, int vatClassId)
        {
            ReturnableProduct p = _productFactory.CreateReturnableProduct();
            p.Brand = _productBrandRepository.GetById(BrandId);
            p.Description = Description;
            p.Flavour = _productFlavourRepository.GetById(FlavourId);
            p.Packaging = _productPackagingRepository.GetById(PackageId);
            p.PackagingType = _productPackagingTypeRepository.GetById(PackageTypeId);
            p.ProductCode = ProductCode;
            p.ReturnableType = returnableType;
            p.VATClass = _vatClassRepository.GetById(vatClassId);
            int test = _productRepository.Save(p);
            _productRepository.GetById(test);
            return test;
        }*/

        /*protected int AddConsolidatedProduct(int productid, int brandId, string description, int flavourId, int packageId, int packageTypeId, string productCode)
        {
            ConsolidatedProduct c = _productFactory.CreateConsolidatedProduct(_productRepository.GetById(productid), 3);
            c.Brand = _productBrandRepository.GetById(brandId);
            c.Description = description;
            c.Packaging = _productPackagingRepository.GetById(packageId);
            c.PackagingType = _productPackagingTypeRepository.GetById(packageTypeId);
            c.ProductCode = productCode;
            //p.ProductType = _productTypeRepository.GetById(productTypeId);
            return _productRepository.Save(c);

        }*/

        protected Guid AddConsolidatedProduct(Guid productid, Guid brandId, string description, Guid flavourId, Guid packageId, Guid packageTypeId, string productCode, int quantity, decimal exfactoryPrice)
        {

            ConsolidatedProduct c = _productFactory.CreateConsolidatedProduct(Guid.NewGuid(), _productRepository.GetById(productid), quantity);
            c.Brand = _productBrandRepository.GetById(brandId);
            c.Description = description;
            c.ExFactoryPrice = exfactoryPrice;
            c.Packaging = _productPackagingRepository.GetById(packageId);
            c.PackagingType = _productPackagingTypeRepository.GetById(packageTypeId);
            c.ProductCode = productCode;
            //p.ProductType = _productTypeRepository.GetById(productTypeId);
            return _productRepository.Save(c);

        }

        protected Guid AddConsolidatedProductLineItem(Guid id, Guid productid, int quantity)
        {
            Product p = _productRepository.GetById(productid);
            ConsolidatedProduct c = _productRepository.GetById(id) as ConsolidatedProduct;
            c.ProductDetails.Add(new ConsolidatedProduct.ProductDetail { Product = p, QuantityPerConsolidatedProduct = quantity });
            return _productRepository.Save(c);

        }

        protected Guid AddPricingTier(string name, string code, string description)
        {
            ProductPricingTier pptier = new ProductPricingTier(Guid.NewGuid())
            {
                Name = name,
                Code = code,
                Description = description
            };
            return _ProductPricingTierRepository.Save(pptier);
        }

        /*protected int AddContainment(int quantity, int productPackagingType, int returnableProduct)
        {
            Containment pptier = new Containment(Guid.NewGuid())
            {
                ProductPackagingType=_productPackagingTypeRepository.GetById(productPackagingType),
                ProductRef = new ProductRef { ProductId = returnableProduct },
                Quantity=quantity
            };
            return _containmentRepository.Save(pptier);
        }*/

        protected Guid AddPricing(Guid ProductRefId, Guid TierId, decimal exfactory, decimal sellingprice, DateTime date)
        {

            ProductPricing price = _productPricingFactory.CreateProductPricing(ProductRefId, TierId, exfactory, sellingprice, date);
            return _pricingRepository.Save(price);
        }

        protected Guid AddUserGroup(string name)
        {
            UserGroup dg = new UserGroup(Guid.NewGuid())
            {
                Name = name,
                Descripition = "Administrator"
            };
            Guid id = _userGroupRepository.Save(dg);
            return id;
        }
        protected void AddUserGroupRoles(Guid groupid)
        {
            UserGroup usergroup = _userGroupRepository.GetById(groupid);
            foreach (var val in RolesHelper.GetRoles())
            {
                Guid id = Guid.NewGuid();
                bool canAcess = true;
               
                UserGroupRoles r = new UserGroupRoles(id)
                {
                    UserGroup = usergroup,
                    UserRole = val.Id,
                    CanAccess = canAcess
                };
                _userGroupRolesRepository.Save(r);
            }
          
            
        }

        protected Guid AddOutletCategory(string name, string code)
        {
            OutletCategory oc = new OutletCategory(Guid.NewGuid())
            {
                Name = name,
                 Code=code

            };
            return _outletCategoryRepository.Save(oc);
        }

        protected Guid AddOutLetType(string name, string code)
        {
            OutletType ot = new OutletType(Guid.NewGuid())

            {
                Name = name,
                 Code=code
            };
            return _outletTypeRepository.Save(ot);
        }

        protected Guid AddCountry(string name, string currency, string code)
        {
            Country country = new Country(Guid.NewGuid(), DateTime.Now, DateTime.Now, EntityStatus.Active)
            {
                Name = name,
                Currency=currency,
                Code=code
            };

            return _countryRepository.Save(country);
        }

        protected Guid AddSocialeconomicStatus(string Status)
        {
            return _socioEconomicStatusRepository.Save(new SocioEconomicStatus(Guid.NewGuid())
            {
                EcoStatus = Status
            });
        }

        protected Guid AddRegion(string name, string desc, Guid country)
        {
            Region reg = new Region(Guid.NewGuid())
            {
                Name = name,
                Description = desc,
                Country = _countryRepository.GetById(country),


            };
            return _regionRepository.Save(reg);
        }

        protected Guid AddProducerCC(string name)
        {
            StandardWarehouse p = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null) as StandardWarehouse;
            p.Name = name;
            return _costCentreRepository.Save(p);
        }
        protected Guid AddDistributorCC(Guid producerId, string name, string owner, string pin, string accNo, Guid region, Guid SaleRep, Guid surveyor, Guid ASM, string VatReg, string paybillNumber, string merchantNumber,string code)
        {
            // int producerId = AddProducerCC();
            CostCentre sw = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, _costCentreRepository.GetById(producerId)) as CostCentre;
            Distributor distributor = sw as Distributor;
            distributor.Name = name;
            distributor.Owner = owner;
            distributor.CostCentreCode = code;
            distributor.PIN = pin;
            distributor.AccountNo = accNo;
            distributor.Region = _regionRepository.GetById(region);
            distributor.SalesRep = _userRepository.GetById(SaleRep);
            distributor.Surveyor = _userRepository.GetById(surveyor);
            distributor.ASM = _userRepository.GetById(ASM);
            distributor.VatRegistrationNo = VatReg;
            distributor.MerchantNumber = paybillNumber;
            distributor.PaybillNumber = merchantNumber;
            return _distributorrepository.Save(distributor);

        }

        protected Guid AddDistributorPendingDispatchWarehouse(Guid distributorId, string name)
        {
            CostCentre d = _costCentreRepository.GetById(distributorId);
            CostCentre dpdw = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorPendingDispatchWarehouse, d);
            dpdw.Name = name;
            return _costCentreRepository.Save(dpdw);
        }

        protected Guid AddDistributorCCApplication(Guid distributorId)
        {
            CostCentreApplication cca = new CostCentreApplication(Guid.NewGuid())
            {
                CostCentreId = distributorId,
                Description = "Test CC App"
            };

            return _costCentreApplicationRepository.Save(cca);
        }

        protected Guid AddRoute(Guid regionId, string name, string code)
        {
            Route rt = _routeFactory.CreateRoute(_regionRepository.GetById(regionId), name, code,Guid.NewGuid()) as Route;
            rt._SetStatus(EntityStatus.Active);
            return _routeRepository.Save(rt);
        }

        /*protected int AddUser(string Username, string password, string mobile, string pin, int costCenter, UserType usertype)
        {
            User usr = new User(Guid.NewGuid())
            {
                Username = Username,
                Password = password,
                Mobile = mobile,
                PIN = pin,
                UserType = usertype,
                CostCentre = costCenter


            };
            return _userRepository.Save(usr);
        }*/

        protected Guid CreateSalemanRoute(Guid distributorid, Guid salemanwarehouseid, Guid routeId)
        {
            CostCentre costcenterDistibutor = _costCentreRepository.GetById(distributorid);
            CostCentre costcenterDistibutorSaleman = _costCentreRepository.GetById(salemanwarehouseid);
            Route route = _routeRepository.GetById(routeId);
            SalesmanRoute sr = new SalesmanRoute(Guid.NewGuid())
            {
                DistributorSalesmanRef = new CostCentreRef { Id = costcenterDistibutorSaleman.Id },
                Route = route
            };
            sr._SetStatus(EntityStatus.Active);
            Guid id = _salesmanRouteRepository.Save(sr);
            return id;
        }

        protected Guid CreateSalemanSupplier(Guid salemanwarehouseid, Guid supplierId)
        {
            //CostCentre costcenterDistibutor = _costCentreRepository.GetById(distributorid);
            CostCentre costcenterDistibutorSaleman = _costCentreRepository.GetById(salemanwarehouseid);
            Supplier supplier = _supplierRepository.GetById(supplierId);
            SalesmanSupplier ss = new SalesmanSupplier(Guid.NewGuid())
            {
                DistributorSalesmanRef = new CostCentreRef { Id = costcenterDistibutorSaleman.Id },
                Supplier = supplier
            };
            ss._SetStatus(EntityStatus.Active);
            Guid id = _salesmanSupplierRepository.Save(ss);
            return id;
        }

        protected Guid AddUser(string Username, string password, string mobile, string pin, Guid costCenter, UserType usertype, Guid groupId)
        {
            User usr = new User(Guid.NewGuid())
            {
                Username = Username,
                Password =EncryptorMD5.GetMd5Hash(password),
                Mobile = mobile,
                PIN = pin,
                UserType = usertype,
                CostCentre = costCenter,
                Group = _userGroupRepository.GetById(groupId),
                

            };
            usr._SetStatus(EntityStatus.Active);
            return _userRepository.Save(usr);
        }
        protected Guid AddActivityType(string name, string code, string description, bool isInputRequired, bool isInfectionsRequired, bool isServiceRequired, bool isProduceRequired)
        {
            var activitytype = new ActivityType(Guid.NewGuid())
            {
                Name = name,
                Code = code,
                IsInputRequired = isInputRequired,
                IsInfectionsRequired = isInfectionsRequired,
                Description = description,
                IsProduceRequired = isProduceRequired,
                IsServicesRequired = isServiceRequired,
               
            };
            activitytype._SetStatus(EntityStatus.Active);
            return _activityTypeRepository.Save(activitytype);
        }
        protected Guid AddShift(string name, string code, string description, DateTime startTime, DateTime endTime)
        {
            var shift = new Shift(Guid.NewGuid())
            {
                Name = name,
                Code = code,
                StartTime = startTime,
                EndTime = endTime,
                Description = description
            };
            shift._SetStatus(EntityStatus.Active);
            return _ShiftRepository.Save(shift);
        }

        protected Guid AddSeason(string name, string code, string description, Guid commodityProducerId,DateTime startDate,DateTime endDate)
        {
            var season = new Season(Guid.NewGuid())
            {
                Name = name,
                Code = code,
                CommodityProducer =_commodityProducerRepository.GetById(commodityProducerId),
                StartDate = startDate,
                EndDate = endDate,
                Description = description
            };
            season._SetStatus(EntityStatus.Active);
            return _SeasonRepository.Save(season);
        }

        protected Guid AddService(string name, string code, string description,decimal cost)
        {
            var service = new CommodityProducerService(Guid.NewGuid())
            {
                Name = name,
                Code = code,
                Cost = cost,
                Description = description
            };
            service._SetStatus(EntityStatus.Active);
            return _ServiceRepository.Save(service);
        }

        protected Guid AddInfection(string name, string code, string description, InfectionType type)
        {
            var  infection = new Infection(Guid.NewGuid())
            {
                Name = name,
               Code = code,
               InfectionType = type,
               Description = description
            };
            infection._SetStatus(EntityStatus.Active);
            return _InfectionRepository.Save(infection);
        }

        protected Guid AddServiceProvider(string name, string code, string mobile, string pin, string idNo, string accountName,string accountNumber,Gender gender,string description, Guid bankId,Guid bankBranchId)
        {
            var  provider = new ServiceProvider(Guid.NewGuid())
            {Name = name,
                Code = code,
                MobileNumber = mobile,
                PinNo = pin,
                IdNo = idNo,
                AccountName = accountName,
                AccountNumber = accountNumber,
                Gender = gender,
                Bank =_bankRepository.GetById(bankId),
                BankBranch =_bankBranchRepository.GetById(bankBranchId),
                Description = description

            };
            provider._SetStatus(EntityStatus.Active);
            return _ServiceProviderRepository.Save(provider);
        }

        protected static string Hash(string input)
        {
            return Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(input)));
        }
        protected Guid AddArea(string name, string desc, Guid reg)
        {
            Area area = new Area(Guid.NewGuid())
            {
                Name = name,
                Description = desc,
                region = _regionRepository.GetById(reg)
            };
            area._SetStatus(EntityStatus.Active);
            return _areaRepository.Save(area);
        }
        protected Guid AddVatClass(string name, string className, decimal Rate, DateTime date)
        {
            VATClass vC = _vatClassFactory.CreateVATClass(name, className, Rate, date);
            vC._SetStatus(EntityStatus.Active);
            return _vatClassRepository.Save(vC);
        }
        protected Guid AddProductDiscount(Guid product, Guid tier, decimal rate, DateTime effDate, DateTime endDate)
        {
            ProductDiscount pd = _productDiscountFactory.CreateProductDiscount(product, tier, rate, effDate, endDate,false,0);
            pd._SetStatus(EntityStatus.Active);
            return _productDiscountRepository.Save(pd);
        }
        protected Guid AddSaleValueDiscount(Guid tier, decimal rate, decimal saleValue, DateTime effDate, DateTime endDate)
        {
            SaleValueDiscount svd = _saleValueDiscountFactory.CreateSaleValueDiscount(tier, rate, saleValue, effDate, endDate);
            svd._SetStatus(EntityStatus.Active);
            return _saleValueDiscountRepository.Save(svd);
        }
        protected Guid AddProductGroupDiscount(DiscountGroup group, ProductRef productRef, decimal rate, DateTime effDate, DateTime endDate)
        {
            ProductGroupDiscount svd = _productDiscountGroupFactory.CreateProductGroupDiscount(group, productRef, rate, effDate, endDate,false,0);
            svd._SetStatus(EntityStatus.Active);
            return _productDiscountGroupRepository.Save(svd);
        }
        protected Guid AddPromotionDiscount(ProductRef parentProduct, Guid freeProduct, int ParentQuantity, int FreeQuantity, decimal rate, DateTime effDate, DateTime endDate)
        {
            PromotionDiscount svd = _promotionDiscountFactory.CreateFreeOfChargeDiscount(parentProduct, freeProduct, ParentQuantity, FreeQuantity, effDate, rate, endDate);
            svd._SetStatus(EntityStatus.Active);
            return _promotionDiscountRepository.Save(svd);
        }
        protected Guid AddFreeOfChargeDiscountDiscount(ProductRef product, DateTime startDate, DateTime endDate)
        {
            var svd = new FreeOfChargeDiscount(Guid.NewGuid())
                                           {
                                               isChecked=true,
                                               StartDate = startDate,
                                               EndDate = endDate,
                                               ProductRef=product,
                                           };
            svd._SetStatus(EntityStatus.Active);
            return _freeOfChargeDiscountRepository.Save(svd);
        }
        protected Guid AddCertainValueCertainProductDiscount(ProductRef product, int Quantity, decimal rate, DateTime effDate, DateTime endDate)
        {
            CertainValueCertainProductDiscount svd = _certainValueCertainProductDiscountFactory.CreateCertainValueCertainProductDiscount(product, Quantity, rate, effDate, endDate);
            svd._SetStatus(EntityStatus.Active);
            return _certainValueCertainProductDiscountRepository.Save(svd);
        }
        protected Guid AddDiscountGroup(string Name, string code)
        {
            DiscountGroup svd = new DiscountGroup(Guid.NewGuid())
                                    {
                                        Name = Name,
                                        Code = code,
                                    };
            svd._SetStatus(EntityStatus.Active);
            return _discountGroupRepository.Save(svd);
        }
        protected Guid AddAccount(Guid CostCenteId, int AccType, decimal Balance)
        {
            Account acc = new Account
            {
                CostcentreId = CostCenteId,
                AccountType = AccountType.Cash,
                Balance = Balance
            };
            return _accountRepository.Add(acc);
        }
        protected Guid AddInventory(Guid wareHouseId, decimal Val, int Bal, Guid ProductId)
        {
            Inventory inv = new Inventory(Guid.NewGuid())
            {
                Warehouse = _costCentreRepository.GetById(wareHouseId) as Warehouse,
                Value = Val,
                Balance = Bal,
                Product = _productRepository.GetById(ProductId)
            };
            inv._SetStatus(EntityStatus.Active);
            return _inventoryRepository.AddInventory(inv);
        }
        protected Guid AddContact(Guid cCId, string contactPerson, string telephone, string fax, string mobile, string address1, string address2, string locality, string postalCode, string city, int classification, MaritalStatas maritalStatus, Guid contactType, int contactOwner,string email)
        {

            Contact contact = new Contact(Guid.NewGuid())
            {
                ContactOwnerMasterId = cCId,
                Firstname = contactPerson,
                BusinessPhone = telephone,
                Fax = fax,
                MobilePhone = mobile,
                PhysicalAddress = address1,
                PostalAddress = address2,
                Company = locality,
                Email = email,
                City = city,
                ContactClassification = ContactClassification.PrimaryContact,
                ContactOwnerType = ContactOwnerType.Distributor,
                 MStatus=MaritalStatas.Single,
                 ContactType=_contactTypeRepository.GetById(contactType)
            };
            contact._SetStatus(EntityStatus.Active);
            return _contactRepository.Save(contact);

        }
        protected Guid AddChannelPacks(Guid ProductPackaging, Guid OutletType, bool IsChecked)
        {
            ChannelPackaging channelPackaging = new ChannelPackaging(Guid.NewGuid())
            {
                Packaging = _productPackagingRepository.GetById(ProductPackaging),
                OutletType = _outletTypeRepository.GetById(OutletType),
                IsChecked = IsChecked
            };
            channelPackaging._SetStatus(EntityStatus.Active);
            return _channelPackagingRepository.Save(channelPackaging);
        }
        protected Guid AddCompetitor(string Name, string PhysicalAddress, string PostalAddress, string Telephone, string ContactPerson, string City, string Longitude, string Latitude)
        {
            Competitor competitor = new Competitor(Guid.NewGuid())
            {
                Name = Name,
                PhysicalAddress = PhysicalAddress,
                PostalAddress = PostalAddress,
                Telephone = Telephone,
                ContactPerson = ContactPerson,
                City = City,
                Longitude = Longitude,
                Lattitude = Latitude

            };
            competitor._SetStatus(EntityStatus.Active);
            return _competitorRepository.Save(competitor);
        }
        protected Guid AddCompetitorProduct(string ProductName, string ProductDescription, Guid Competitor, Guid Brand, Guid Packaging, Guid ProductType, Guid PackagingType, Guid Flavor)
        {
            CompetitorProducts competitorProduct = new CompetitorProducts(Guid.NewGuid())
            {
                ProductName = ProductName,
                ProductDescription = ProductDescription,
                Competitor = _competitorRepository.GetById(Competitor),
                Brand = _productBrandRepository.GetById(Brand),
                Packaging = _productPackagingRepository.GetById(Packaging),
                ProductType = _productTypeRepository.GetById(ProductType),
                PackagingType = _productPackagingTypeRepository.GetById(PackagingType),
                Flavour = _productFlavourRepository.GetById(Flavor)
            };
            competitorProduct._SetStatus(EntityStatus.Active);
            return _competitorProductRepository.Save(competitorProduct);
        }
        protected Guid AddAsset(Guid assetTypeId,Guid assetStatusId,Guid assetCategoryId, string Code, string Capacity, string SerialNo, string AssetNo)
        {
            Asset cooler = new Asset(Guid.NewGuid())
                               {
                                   AssetType = _coolerTypeRepository.GetById(assetTypeId),
                                   AssetCategory = _assetCategoryRepository.GetById(assetCategoryId),
                                   AssetStatus = _assetStatusRepository.GetById(assetStatusId),
                                   Code = Code,
                                   Capacity = Capacity,
                                   SerialNo = SerialNo,
                                   AssetNo = AssetNo,
                                   Name = AssetNo
                               };
            cooler._SetStatus(EntityStatus.Active);
            return _coolerRepository.Save(cooler);
        }
        protected Guid AddAssetType(string Name, string Code)
        {
            AssetType coolerType = new AssetType(Guid.NewGuid())
            {
                Name = Name,
                Description = Code
            };
            coolerType._SetStatus(EntityStatus.Active);
            return _coolerTypeRepository.Save(coolerType);
        }
        protected Guid AddDistrict(Guid Province, string DistrictName)
        {
            District district = new District(Guid.NewGuid())
            {
                Province = _provinceRepository.GetById(Province),
                DistrictName = DistrictName
            };
            district._SetStatus(EntityStatus.Active);
            return _districtRepository.Save(district);
        }
        protected Guid AddProvince(string Name, string Description, Guid CountryId)
        {
            Province province = new Province(Guid.NewGuid())
            {
                Name = Name,
                Description = Description,
                Country = _countryRepository.GetById(CountryId)
            };
            province._SetStatus(EntityStatus.Active);
            return _provinceRepository.Save(province);
        }
        protected Guid AddReorderLevel(Guid DistributorId, Guid ProductId, decimal ProductReorderLevel)
        {
            ReOrderLevel reorderLevel = new ReOrderLevel(Guid.NewGuid())
            {
                DistributorId = _costCentreRepository.GetById(DistributorId),
                ProductId = _productRepository.GetById(ProductId),
                ProductReOrderLevel = ProductReorderLevel

            };
            reorderLevel._SetStatus(EntityStatus.Active);
            return _reorderLevelRepository.Save(reorderLevel);
        }
        protected Guid AddTargetPeriod(string Name, DateTime StartDate, DateTime EndDate)
        {
            TargetPeriod targetPeriod = new TargetPeriod(Guid.NewGuid())
            {
                Name = Name,
                StartDate = StartDate,
                EndDate = EndDate,
            };
            targetPeriod._SetStatus(EntityStatus.Active);
            return _targetPeriodRepository.Save(targetPeriod);
        }
        protected Guid AddTarget(Guid DistributorId, Guid Product, Guid TargetPeriod, decimal TargetAmount, bool IsQuantityTarget)
        {
            Target target = new Target(Guid.NewGuid())
            {
                CostCentre = _distributorrepository.GetById(DistributorId),
                // product=_productRepository.GetById(Product ),
                TargetPeriod = _targetPeriodRepository.GetById(TargetPeriod),
                 TargetValue = TargetAmount,
                IsQuantityTarget = IsQuantityTarget,
            };
            target._SetStatus(EntityStatus.Active);
            return _targetRepository.Save(target);
        }
        protected Guid AddTargetItem(Guid prod, decimal qty, Guid target)
        {
            TargetItem targetitem = new TargetItem(Guid.NewGuid())
            {
              
                Product=new ProductRef{ProductId=prod},
                Target = _targetRepository.GetById(target),
                Quantity = qty,
              
            };
            targetitem._SetStatus(EntityStatus.Active);
            return _targetItemRepository.Save(targetitem);
        }
        protected Guid AddBank(string name, string desc, string code)
        {
            Bank bank = new Bank(Guid.NewGuid())

            {
                Name = name,
                Description = desc,
                Code = code
                
            };
            bank._SetStatus(EntityStatus.Active);
            return _bankRepository.Save(bank );
        }
        protected Guid AddOutletVisitReasonType(string name, string desc, OutletVisitAction type)
        {
            OutletVisitReasonsType reasonsType = new OutletVisitReasonsType(Guid.NewGuid())

            {
                Name = name,
                Description = desc,
                OutletVisitAction = type
                
            };
            reasonsType._SetStatus(EntityStatus.Active);
            return _outletVisitReasonsTypeRepository.Save(reasonsType);
        }
       
        protected Guid AddBankBranch(string name, string desc, string code, Guid BankId)
        {
            BankBranch bankBranch = new BankBranch(Guid.NewGuid())

            {
                Name = name,
                Description = desc,
                Code = code,
                Bank=_bankRepository.GetById(BankId ) 

            };
            bankBranch._SetStatus(EntityStatus.Active);
            return _bankBranchRepository.Save(bankBranch);
        }
        protected Guid AddSupplier(string code, string name, string description)
        {
        Supplier supplier=new Supplier(Guid.NewGuid())
        {
         Name=name,
         Code=code,
          Description=description
        };
        supplier._SetStatus(EntityStatus.Active);
        return _supplierRepository.Save(supplier);
        }

      
       
        protected Guid AddContactType(string code, string name, string description)
       {
           ContactType cType = new ContactType(Guid.NewGuid())
           {
                Name=name,
                 Code=code,
                 Description=description
           };
           cType._SetStatus(EntityStatus.Active);
           return _contactTypeRepository.Save(cType);
       }
        protected Guid AddAssetCategory( string name, string description, Guid assetTypeId)
        {
            AssetCategory cType = new AssetCategory(Guid.NewGuid())
            {
                Name = name,
                AssetType= _coolerTypeRepository.GetById(assetTypeId),
                Description = description
            };
            cType._SetStatus(EntityStatus.Active);
            return _assetCategoryRepository.Save(cType);
        }
        protected Guid AddAssetStatus(string name, string description)
        {
            AssetStatus cType = new AssetStatus(Guid.NewGuid())
            {
                Name = name,
             
                Description = description
            };
            cType._SetStatus(EntityStatus.Active);
            return _assetStatusRepository.Save(cType);
        }

        protected Guid AddAppSetting(string value, SettingsKeys key)
        {
            AppSettings setting = new AppSettings(Guid.NewGuid())
                                      {
                                          Key = key,
                                          Value = value
                                      };
            setting._SetStatus(EntityStatus.Active);
            return _settingsRepository.Save(setting);
        }
        protected void AddDocumentSetting()
        {
            var docOrderInitial = new AppSettings(Guid.NewGuid());
            docOrderInitial.Key = SettingsKeys.DocOrderInitial;
            docOrderInitial.Value ="O";
            _settingsRepository.Save(docOrderInitial);

            var docSaleInitial = new AppSettings(Guid.NewGuid());
            docSaleInitial.Key = SettingsKeys.DocSaleInitial;
            docSaleInitial.Value = "S";
            _settingsRepository.Save(docSaleInitial);

            var  docInvoiceInitial = new AppSettings(Guid.NewGuid());
            docInvoiceInitial.Key = SettingsKeys.DocInvoiceInitial;
            docInvoiceInitial.Value = "I";
            _settingsRepository.Save(docInvoiceInitial);


            var  docReceiptInitial = new AppSettings(Guid.NewGuid());
            docReceiptInitial.Key = SettingsKeys.DocReceiptInitial;
            docReceiptInitial.Value = "R";
            _settingsRepository.Save(docReceiptInitial);


            var  docReferenceRule = new AppSettings(Guid.NewGuid());
            docReferenceRule.Key = SettingsKeys.DocReferenceRule;
            docReferenceRule.Value ="{D}_{SN}_{OC}_{DT}_{TM}_{SQ}";
            _settingsRepository.Save(docReferenceRule);
        }

        protected Guid AddMasterDataAllocation(Guid entityAId, Guid entityBId, MasterDataAllocationType allocationType)
        {
            MasterDataAllocation allocation = new MasterDataAllocation(Guid.NewGuid())
            {
                AllocationType = allocationType,
                EntityAId = entityAId,
                EntityBId = entityBId,
            };

            allocation._SetStatus(EntityStatus.Active);
            return _masterDataAllocationRepository.Save(allocation);
        }

        #region Agrmanager Data

        protected Guid AddHub(string code, string name, Guid regionId, string varNo, CostCentreRef parentCC)
        {
            Hub hub = new Hub(Guid.NewGuid())
                          {
                              CostCentreCode = code,
                              CostCentreType = CostCentreType.Hub,
                              Name = name,
                              Region = _regionRepository.GetById(regionId),
                              VatRegistrationNo = varNo,
                              _Status = EntityStatus.Active,
                              ParentCostCentre = parentCC
                          };
            return _costCentreRepository.Save(hub);
        }

        protected Guid AddCentreType(string name, string code)
        {
            CentreType ct = new CentreType(Guid.NewGuid())
            {
                Code = code,
                Description = "Desc",
                Name = name,
                _Status = EntityStatus.Active,
            };
            return _centreTypeRepository.Save(ct);
        }

        protected Guid AddCentre(string name, string code, Guid ct, Guid hubId, Guid routeId)
        {
            Centre c = new Centre(Guid.NewGuid())
            {
                CenterType = _centreTypeRepository.GetById( ct),
                Name = name,
                Code = code,
                Description = "DEsc",
                _Status = EntityStatus.Active,
                Route = _routeRepository.GetById(routeId),
                Hub = _costCentreRepository.GetById(hubId) as Hub
            };
            return _centreRepository.Save(c);
        }
        protected Guid AddCommodityOwnerType(string code,string name)
        {
            CommodityOwnerType cot = new CommodityOwnerType(Guid.NewGuid())
                                         {
                                             Code = code,
                                             Name = name,
                                             Description = "desc",
                                             _Status = EntityStatus.Active,
                                         };
            return _commodityOwnerTypeRepository.Save(cot);
        }

        protected Guid AddCommoditySupplier(string acc,Guid bbn, Guid bn, CommoditySupplierType stype,string code, CostCentreType ctype, string name, CostCentreRef parent, string pin)
        {
            CommoditySupplier cs = new CommoditySupplier(Guid.NewGuid())
                                       {
                                           AccountName = acc,
                                           AccountNo = acc,
                                           BankBranchId = bbn,
                                           BankId = bn,
                                           CommoditySupplierType = stype,
                                           CostCentreCode = code,
                                           CostCentreType = ctype,
                                           JoinDate = DateTime.Now,
                                           Name = name,
                                           ParentCostCentre = parent,
                                           PinNo = pin,
                                           _Status = EntityStatus.Active,
                                       };
            return _commoditySupplierRepository.Save(cs);
        }

        protected Guid AddCommodityProducer(string acres, string code, string name, string regNu, Guid commSupplierId)
        {
            CommodityProducer cp = new CommodityProducer(Guid.NewGuid())
            {
                Acrage = acres,
                Code = code,
                CommoditySupplier = _commoditySupplierRepository.GetById(commSupplierId) as CommoditySupplier,
                Description = "Farm",
                Name = name,
                PhysicalAddress = "PhAdd",
                RegNo = regNu,
                CommodityProducerCentres = _centreRepository.GetAll().Take(2).ToList(),
                _Status = EntityStatus.Active,
            };

            return _commodityProducerRepository.Save(cp);

        }

        protected Guid AddCommodityOwner(string bussNu, string code, string lastName, string idNo, string offNu, string pin, Guid cotId,  Guid supplierId, string surname, string firstname)
        {
            CommodityOwner farmer = new CommodityOwner(Guid.NewGuid())
                                        {
                                            BusinessNumber = bussNu,
                                            Code = code,
                                            LastName = lastName,
                                            IdNo = idNo,
                                            MaritalStatus =MaritalStatas.Unknown /*_maritalStatusRepository.GetById( ms)*/,
                                            OfficeNumber = offNu,
                                            PhoneNumber = offNu,
                                            PhysicalAddress = "Kitale",
                                            PinNo = pin,
                                            PostalAddress = "209",
                                            Surname = surname,
                                            FirstName = firstname, 
                                            Description = "Farmer",
                                            CommodityOwnerType = _commodityOwnerTypeRepository.GetById(cotId),
                                            Email = "e@a.com",
                                            _Status = EntityStatus.Active,
                                            CommoditySupplier = _commoditySupplierRepository.GetById(supplierId) as CommoditySupplier
                                        };
            return _commodityOwnerRepository.Save(farmer);
        }

        List<CommodityGrade> AddCommodityGrades(Commodity comm)
        {
            List<CommodityGrade> grades = new List<CommodityGrade>();
            CommodityGrade grade = new CommodityGrade(Guid.NewGuid())
                                       {
                                           Code = comm.Code + " A",
                                           Description = comm.Name + " Grade A",
                                           Name = comm.Name + " Grade A",
                                           Commodity = comm,
                                           UsageTypeId = 1,
                                           _Status = EntityStatus.Active,

                                       };
            grades.Add(grade);
            CommodityGrade grade2 = new CommodityGrade(Guid.NewGuid())
            {
                Code = comm.Code +" B",
                Description = comm.Name +" Grade B",
                Name = comm.Name + " Grade B",
                Commodity = comm,
                UsageTypeId = 1,
                _Status = EntityStatus.Active,

            };
            grades.Add(grade2);
            CommodityGrade grade3 = new CommodityGrade(Guid.NewGuid())
            {
                Code = comm.Code + " C",
                Description = comm.Name + " Grade C",
                Name = comm.Name + " Grade C",
                Commodity = comm,
                UsageTypeId = 1,
                _Status = EntityStatus.Active,

            };
            grades.Add(grade3);

            return grades;
        }

        protected Guid AddCommodityType(string code, string desc, string name)
        {
            CommodityType ct = new CommodityType(Guid.NewGuid())
                                   {
                                       Code = code,
                                       Description = desc,
                                       Name = name,
                                       _Status = EntityStatus.Active
                                   };
            return _commodityTypeRepository.Save(ct);
        }

        protected Guid AddCommodity(string name, string code, Guid commodityTypeid)
        {
            Commodity comm = new Commodity(Guid.NewGuid())
                                 {
                                     Name = name,
                                     Code = code,
                                     Description = name,
                                     _Status = EntityStatus.Active,
                                      CommodityType = _commodityTypeRepository.GetById(commodityTypeid)
                                 };
            comm.CommodityGrades = AddCommodityGrades(comm);
            return _commodityRepository.Save(comm);
        }

        protected Guid AddSourcingContainer(string code, Guid costCentre, string desc, string equipNu, string make, string mode, string name,  Guid containerTypeId)
        {
            SourcingContainer con = new SourcingContainer(Guid.NewGuid())
                                        {
                                            EquipmentType = EquipmentType.Container,
                                            _Status = EntityStatus.Active,
                                            Code = code,
                                            CostCentre = _costCentreRepository.GetById(costCentre) as Hub,
                                            Description = desc,
                                            EquipmentNumber = equipNu,
                                            Make = make,
                                            Model = mode,
                                            Name = name,
                                            
                                            //BubbleSpace = bubblespace,
                                            ContainerType = _containerTypeRepository.GetById(containerTypeId)
                                           // Lenght = length,
                                            //LoadCariage = loadCarriage,
                                            //Height = height,
                                            //FreezerTemp = freezer,
                                            //TareWeight = tarewight,
                                            //Volume = volume,
                                            //Width = width,
                                            //ContainerType = containerType
                                        };

            return _equipmentRepository.Save(con);
        }

        protected Guid AddContainType(string name,decimal tareWeight,string make,ContainerUseType useType,Guid? gradeId)
        {
            ContainerType container = new ContainerType(Guid.NewGuid())
            {
                Name = name,
                BubbleSpace = 0,
                CommodityGrade =gradeId.HasValue? _commodityRepository.GetGradeByGradeId(gradeId.Value):null,
                FreezerTemp = 0,
                Height = 0,
                Length = 0,
                LoadCarriage = 0,
                TareWeight = tareWeight,
                Volume = 0,
                Width = 0,
                Code = name,
                Description = name,
                Make = name,
                Model = name,
                ContainerUseType = useType,

            };
            return _containerTypeRepository.Save(container);
        }

        protected Guid AddVehicle(string name, string code,string make, Guid hub,string description,string regNo)
        {
           
            var vehicle = new Vehicle(Guid.NewGuid())
            {
                Code = code,
                Name = name,
                CostCentre =_hubRepository.GetById(hub)as Hub,
                Description = description,
                EquipmentNumber = regNo,
                EquipmentType = EquipmentType.Vehicle
            };
           
            return _vehicleRepository.Save(vehicle);
        }
      

        protected Guid AddPrinter(string equipmentNo, string code, string name, string make, string model, string desc, Guid hubId)
        {
            Printer printer = new Printer(Guid.NewGuid())
                {
                    EquipmentNumber = equipmentNo,
                    Code = code,
                    Name = name,
                    Make = make,
                    Model = model,
                    EquipmentType = EquipmentType.Printer,
                    Description = desc,
                    CostCentre = _costCentreRepository.GetById(hubId) as Hub
                };
            return _equipmentRepository.Save(printer);
        }

        protected Guid AddWeighingScale(string equipmentNo, string code, string name, string make, string model, string desc, Guid hubId)
        {
            WeighScale scale = new WeighScale(Guid.NewGuid())
            {
                EquipmentNumber = equipmentNo,
                Code = code,
                Name = name,
                Make = make,
                Model = model,
                EquipmentType = EquipmentType.WeighingScale,
                Description = desc,
                CostCentre = _costCentreRepository.GetById(hubId) as Hub
            };
            return _equipmentRepository.Save(scale);
        }

       

        protected Guid AddStore(string code, string name, CostCentreRef parent, string vatId)
        {
            Store store = new Store(Guid.NewGuid())
                        {
                            CostCentreCode = code,
                            Name = name,
                            _Status = EntityStatus.Active,
                            CostCentreType = CostCentreType.Store,
                            ParentCostCentre = parent,
                            VatRegistrationNo = vatId, 
                        };

           return  _costCentreRepository.Save(store);
        }

        protected Guid CreatePurchasingClerk(Guid hub, Guid route, string salesmanName, Guid usergroupid, string code)
        {
          
            PurchasingClerk purchasingClerk = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.PurchasingClerk, _costCentreRepository.GetById(hub)) as PurchasingClerk;
            purchasingClerk.Name = salesmanName;
            purchasingClerk.CostCentreCode = code;
            User usr = AddPurchaseClerkUser(salesmanName, "12345678", "32323", Guid.NewGuid().ToString(), purchasingClerk.Id, UserType.PurchasingClerk, usergroupid);
            purchasingClerk.User = usr;
            Guid id = _costCentreRepository.Save(purchasingClerk);
            
            Route r = _routeRepository.GetById(route);

            _userRepository.Save(usr);

            AddPurchasingClerkRoute(hub, id, r.Id);

            return id;
        }

        protected User AddPurchaseClerkUser(string Username, string password, string mobile, string pin, Guid costCenter, UserType usertype, Guid groupId)
        {
            User usr = new User(Guid.NewGuid())
            {
                Username = Username,
                Password = EncryptorMD5.GetMd5Hash(password),
                Mobile = mobile,
                PIN = pin,
                UserType = usertype,
                CostCentre = costCenter,
                Group = _userGroupRepository.GetById(groupId),


            };
            usr._SetStatus(EntityStatus.Active);
           // _userRepository.Save(usr)
            return usr;
        }

        protected Guid AddPurchasingClerkRoute(Guid hubId, Guid purchasingClerkId, Guid routeId)
        {
            CostCentre costcenterHub = _costCentreRepository.GetById(hubId);
            CostCentre costcenterPurchasingClerk = _costCentreRepository.GetById(purchasingClerkId);
            Route route = _routeRepository.GetById(routeId);
            PurchasingClerkRoute pcr = new PurchasingClerkRoute(Guid.NewGuid())
            {
                PurchasingClerkRef = new CostCentreRef { Id = costcenterPurchasingClerk.Id },
                Route = route
            };
            pcr._SetStatus(EntityStatus.Active);
            Guid id = _purchasingClerkRouteRepository.Save(pcr);
            return id;
        }

        #endregion




    }
   
}
