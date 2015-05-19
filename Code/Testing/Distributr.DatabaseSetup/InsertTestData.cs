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
using NUnit.Framework;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;

namespace Distributr.DatabaseSetup
{
    public class InsertTestData : InsertTestDataBase, IInsertTestData
    {
        

        //protected IBankRepository _bankRepository;
        //protected IBankBranchRepository _bankBranchRepository;
        #region Constructors

        public InsertTestData(IOutletVisitReasonsTypeRepository visitReasonsTypeRepository,IActivityTypeRepository activityTypeRepository,IContainerTypeRepository containerTypeRepository, IRetireDocumentSettingRepository retireDocumentSettingRepository, IOutletPriorityRepository outletPriorityRepository,
        IOutletVisitDayRepository outletVisitDayRepository, IAssetStatusRepository assetStatusRepository,
        IAssetCategoryRepository assetCategoryRepository, IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRolesRepository, ISalesmanRouteRepository salesmanRouteRepository, IProductTypeRepository productTypeRepository, IProductBrandRepository productBrandRepository, IProductFlavourRepository productFlavourRepository, IProductPackagingRepository productPackagingRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductRepository productRepository, IRegionRepository regionRepository, ICostCentreRepository costCentreRepository, ICostCentreFactory costCentreFactory, IProductPricingRepository pricingRepository,
        IVATClassRepository vatClassRepository, IVATClassFactory vatClassFactory, ICountryRepository countryRepository,
        IProductPricingFactory productPricingFactory, IProductPricingTierRepository productPricingTierRepository,
        IOutletTypeRepository outletTypeRepository, IUserRepository userRepository, IOutletRepository outletRepository,
        IRouteRepository routeRepository, IRouteFactory routeFactory, ITransporterRepository transporterRepository,
        IProductFactory productFactory, IDistributorSalesmanRepository distributorSalesmanRepository,
        IProducerRepository producerRepository,  IDocumentFactory documentFactory,
        ISocioEconomicStatusRepository socioEconomicStatusRepository, IClientMasterDataTrackerRepository clientMasterDataTrackerRepository,
        IDistributorRepository distributorrepository, IOutletCategoryRepository outletCategoryRepository, 
        ITerritoryRepository territoryRepository, IAddOrderLineItemCommandHandler addOrderLineItemCommandHandler, IConfirmOrderCommandHandler confirmOrderCommandHandler,
        IRejectOrderCommandHandler rejectOrderCommandHandler, IAreaRepository areaRepository,
        IContactRepository contactRepository, IAccountRepository accountRepository, IAccountTransactionRepository accountTransactionRepository,
        IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository,
        ICostCentreApplicationRepository costCentreApplicationRepository, IChannelPackagingRepository channelPackagingRepository,
        ICompetitorRepository competitorRepository, ICompetitorProductsRepository competitorProductRepository, IAssetRepository coolerRepository,
        IAssetTypeRepository coolerTypeRepository, IDistrictRepository districtRepository, IProvincesRepository provinceRepository,
        IReOrderLevelRepository reorderLevelRepository, ITargetPeriodRepository targetPeriodRepository, ITargetRepository targetRepository,
        IProductDiscountFactory productDiscountFactory, IProductDiscountRepository productDiscountRepository, ISaleValueDiscountFactory saleValueDiscountFactory,
        ISaleValueDiscountRepository saleValueDiscountRepository, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository,
        ISupplierRepository supplierRepository, ICreatePaymentNoteCommandHandler createLossCommandHandler, IAddPaymentNoteLineItemCommandHandler addLossLineItemCommandHandler, IConfirmPaymentNoteCommandHandler confirmLossCommandHandler,
        IContactTypeRepository contactTypeRepository, IDiscountGroupRepository discountGroupRepository, IProductDiscountGroupFactory productDiscountGroupFactory,
        ICertainValueCertainProductDiscountFactory certainValueCertainProductDiscountFactory,
        ICustomerDiscountFactory customerDiscountFactory,
        IPromotionDiscountFactory promotionDiscountFactory,
        IProductDiscountGroupRepository productDiscountGroupRepository,
        IPromotionDiscountRepository promotionDiscountRepository,
        IFreeOfChargeDiscountRepository freeOfChargeDiscountRepository,
        ICertainValueCertainProductDiscountRepository certainValueCertainProductDiscountRepository,
        ITargetItemRepository targetItemRepository, ISettingsRepository settingsRepository, 
        ICentreRepository centreRepository, ICentreTypeRepository centreTypeRepository, 
        ICommoditySupplierRepository commoditySupplierRepository, ICommodityOwnerTypeRepository commodityOwnerTypeRepository, 
        ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository, 
        ICommodityRepository commodityRepository, ICommodityTypeRepository commodityTypeRepository,
        IEquipmentRepository equipmentRepository, IMasterDataAllocationRepository masterDataAllocationRepository,IVehicleRepository vehicleRepository,IHubRepository hubRepository,
        IPurchasingClerkRouteRepository purchasingClerkRouteRepository,IInfectionRepository infectionRepository,ISeasonRepository seasonRepository,IServiceProviderRepository serviceProviderRepository,IServiceRepository serviceRepository,IShiftRepository shiftRepository,ISalesmanSupplierRepository salesmanSupplierRepository
            )
        {
            _salesmanSupplierRepository = salesmanSupplierRepository;
            _productTypeRepository = productTypeRepository;
            _productBrandRepository = productBrandRepository;
            _productFlavourRepository = productFlavourRepository;
            _productPackagingRepository = productPackagingRepository;
            _productPackagingTypeRepository = productPackagingTypeRepository;
            _productRepository = productRepository;
            _regionRepository = regionRepository;
            _costCentreRepository = costCentreRepository;
            _costCentreFactory = costCentreFactory;
            _pricingRepository = pricingRepository;
            _vatClassRepository = vatClassRepository;
            _vatClassFactory = vatClassFactory;
            _countryRepository = countryRepository;
            _productPricingFactory = productPricingFactory;
            _ProductPricingTierRepository = productPricingTierRepository;
            _outletTypeRepository = outletTypeRepository;
            _userRepository = userRepository;
            _outletRepository = outletRepository;
            _routeRepository = routeRepository;
            _routeFactory = routeFactory;
            _transporterRepository = transporterRepository;
            _productFactory = productFactory;
            _distributorSalesmanRepository = distributorSalesmanRepository;
            _producerRepository = producerRepository;
            _documentFactory = documentFactory;
            _socioEconomicStatusRepository = socioEconomicStatusRepository;
            _clientMasterDataTrackerRepository = clientMasterDataTrackerRepository;
            _distributorrepository = distributorrepository;
            _outletCategoryRepository = outletCategoryRepository;
            //_createOrderCommandHandler = createOrderCommandHandler;
            _territoryRepository = territoryRepository;
            _addOrderLineItemCommandHandler = addOrderLineItemCommandHandler;
            _confirmOrderCommandHandler = confirmOrderCommandHandler;
            //_approveOrderCommandHandler = approveOrderCommandHandler;
            _rejectOrderCommandHandler = rejectOrderCommandHandler;
            _areaRepository = areaRepository;
            _contactRepository = contactRepository;
            _accountRepository = accountRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _costCentreApplicationRepository = costCentreApplicationRepository;
            _channelPackagingRepository = channelPackagingRepository;
            _competitorRepository = competitorRepository;
            _competitorProductRepository = competitorProductRepository;
            _coolerRepository = coolerRepository;
            _coolerTypeRepository = coolerTypeRepository;
            _districtRepository = districtRepository;
            _provinceRepository = provinceRepository;
            _reorderLevelRepository = reorderLevelRepository;
            _targetPeriodRepository = targetPeriodRepository;
            _targetRepository = targetRepository;
            _productDiscountFactory = productDiscountFactory;
            _productDiscountRepository = productDiscountRepository;
            _saleValueDiscountFactory = saleValueDiscountFactory;
            _saleValueDiscountRepository = saleValueDiscountRepository;
            _salesmanRouteRepository = salesmanRouteRepository;
            _userGroupRepository = userGroupRepository;
            _userGroupRolesRepository = userGroupRolesRepository;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _supplierRepository = supplierRepository;
            _createLossCommandHandler = createLossCommandHandler;
            _addLossLineItemCommandHandler = addLossLineItemCommandHandler;
            _confirmLossCommandHandler = confirmLossCommandHandler;
           
            _contactTypeRepository = contactTypeRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _assetStatusRepository = assetStatusRepository;
            _discountGroupRepository = discountGroupRepository;
            _productDiscountGroupFactory = productDiscountGroupFactory;
            _certainValueCertainProductDiscountFactory = certainValueCertainProductDiscountFactory;
            _customerDiscountFactory = customerDiscountFactory;
            _promotionDiscountFactory = promotionDiscountFactory;
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _promotionDiscountRepository = promotionDiscountRepository;
            _freeOfChargeDiscountRepository = freeOfChargeDiscountRepository;
            _certainValueCertainProductDiscountRepository = certainValueCertainProductDiscountRepository;
            _outletPriorityRepository = outletPriorityRepository;
            _outletVisitDayRepository = outletVisitDayRepository;
            _targetItemRepository = targetItemRepository;
            _settingsRepository = settingsRepository;
            _retireDocumentSettingRepository = retireDocumentSettingRepository;
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
            _centreTypeRepository = centreTypeRepository;
            _centreRepository = centreRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _commodityRepository = commodityRepository;
            _commodityTypeRepository = commodityTypeRepository;
            _equipmentRepository = equipmentRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _containerTypeRepository = containerTypeRepository;
            _hubRepository = hubRepository;
            _vehicleRepository = vehicleRepository;
            _purchasingClerkRouteRepository = purchasingClerkRouteRepository;
            _ShiftRepository = shiftRepository;
            _SeasonRepository = seasonRepository;
            _ServiceProviderRepository = serviceProviderRepository;
            _ServiceRepository = serviceRepository;
            _InfectionRepository = infectionRepository;
            _activityTypeRepository = activityTypeRepository;
            _outletVisitReasonsTypeRepository = visitReasonsTypeRepository;
        }

        #endregion

        public void InsertTestMasterData()
        {
            DateTime effectiveDate = DateTime.Now;
            DateTime endDate = effectiveDate.AddDays(3);
            Guid vatClass = AddVatClass("Vat 1", "Class 1", 0.16M, effectiveDate);
           // Guid vatClassr2 = AddVatClass("Vat 1", "Class 1", 0.16M, dt);
            //Add Supplier
            Guid supp1 = AddSupplier("001", "Michael", "egg supplier");
            Guid supp2 = AddSupplier("002", "Dennis", "Chicken supplier");

            Guid ppTier = AddPricingTier("Tier 1", "Code", "Description");
            // sample consolidated Product
            Guid saleProductBrand = AddProductBrand(supp1, "Bibo Groupd", "Bibo Groupd", "1002");

            Guid saleProductFlavor = AddProductFlavour("Fanta Passion", "Fanta Passion ", "1106", saleProductBrand);

            Guid saleProductFlavor1 = AddProductFlavour("Krest", "Krest", "1107", saleProductBrand);

            ////Guid milkFlavor = AddProductFlavour("Whole Milk Brookside Flavour", "Whole Milk Brookside Flavour", "1108", saleProductBrand);//cn

            Guid saleProductPackaging = AddProductPackaging("Fanta 300ml 24RB0", "Fanta 300ml 24RB0", "001");
            Guid saleProductPackagingTest = AddProductPackaging("Fantadd 300mlll 247RB0", "Fantha 3007ml 245RB0", "002");
            Guid saleProductPackaging2 = AddProductPackaging("Krest 300ml 24RB0", "Krest 300ml 24RB0", "003");
            ////Guid milkPacket = AddProductPackaging("500ml Milk Packets", "500ml Milk Packets", "004");//cn
            Guid saleProductPackType = AddProductPackagingType("Bottles", "Bottles", "001");
            ////Guid milkPackType = AddProductPackagingType("Paper Boxes", "Paper Boxes", "002");//cn
            Guid saleProductType = AddProductType("Ready To Drink (RTD) Cordiald", "001");
            Guid returnProductType = AddProductType("Bottle", "002");
            Guid returnProductType2 = AddProductType("Crate", "002");

            Guid returnableCrate = AddReturnableProduct("RP002", "300ml Fanta  Crate", saleProductFlavor, saleProductBrand, /*saleProductPackaging*/ Guid.Empty, saleProductPackType, returnProductType2, 1, ReturnableType.Returnable, vatClass, 24, Guid.Empty, 45);

            ////Guid returnableMilkCrate = AddReturnableProduct("MilkCrate01", "500ml Milk Crate",milkFlavor, saleProductBrand, milkPacket, milkPackType, returnProductType, 1, ReturnableType.Returnable, vatClass, 25, Guid.Empty, 100);//cn

            Guid returnableBottle = AddReturnableProduct("RP003", "300ml Fanta  Bottle", saleProductFlavor, saleProductBrand, saleProductPackaging, saleProductPackType, returnProductType, 1, ReturnableType.Returnable, vatClass, 1, returnableCrate,50);

            Guid returnableBottle2 = AddReturnableProduct("RP004", "300ml Krest  Bottle", saleProductFlavor1, saleProductBrand, saleProductPackaging2, saleProductPackType, returnProductType, 1, ReturnableType.Returnable, vatClass, 1, returnableCrate,60);

            Guid saleProduct = AddSaleProduct("SP004", "300ml Fanta Passion", saleProductFlavor, saleProductBrand, saleProductPackaging, saleProductPackType, saleProductType, 1, vatClass, returnableBottle,10);
            Guid saleProduct2 = AddSaleProduct("SP005", "300ml Krest", saleProductFlavor1, saleProductBrand, saleProductPackaging2, saleProductPackType, saleProductType, 1, vatClass, returnableBottle2,12);

            ////Guid saleProductMilk = AddSaleProduct("SP006", "500ml Brookside Milk", milkFlavor, saleProductBrand, milkPacket, milkPackType, saleProductType, 1, vatClass, returnableMilkCrate, 20);//cn

            Guid caseOfSodaplusB = AddConsolidatedProduct(returnableCrate, saleProductBrand, "Crate of Fanta Passion", saleProductFlavor, saleProductPackaging, saleProductPackType,"CP002", 1,78);
            Guid dd = AddConsolidatedProductLineItem(caseOfSodaplusB, returnableBottle, 24);
            Guid dhsd = AddConsolidatedProductLineItem(caseOfSodaplusB, saleProduct, 24);

            AddProductPackagingInContainer(saleProductPackagingTest, returnableCrate);

            Guid ppPricing1 = AddPricing(saleProduct, ppTier, 20, 25, DateTime.Now);
            Guid ppPricing2 = AddPricing(saleProduct2, ppTier, 20, 25, DateTime.Now);
            ////Guid ppMilkPricing = AddPricing(saleProductMilk, ppTier, 20, 25, DateTime.Now);//cn
            Guid ppRetunableProductPricing1 = AddPricing(returnableBottle, ppTier, 10, 15, DateTime.Now);
            Guid ppRetunableProductPricing2 = AddPricing(returnableBottle2, ppTier, 10, 15, DateTime.Now);
            Guid ppconsolidatedProductPricingw = AddPricing(returnableCrate, ppTier, 100, 200, DateTime.Now);
            Guid ppconsolidatedProductPricing1 = AddPricing(caseOfSodaplusB, ppTier, 2000, 2500, DateTime.Now);
            ////Guid ppMilkRetunableProductPricing2 = AddPricing(returnableMilkCrate, ppTier, 150, 200, DateTime.Now);//cn

            
            
            //Add product brand
            Guid pb1 = AddProductBrand(supp1, "Bibo Group", "Bibo Group", "002");
            Guid pb2 = AddProductBrand(supp1, "Fanta Group", "Fanta Group", "007");
            Guid pb3 = AddProductBrand(supp1, "Colas Group", "Colas Group", "005");
            Guid pb4 = AddProductBrand(supp1, "Dasani Group", "Dasani Group", "030");
            //Add product flavour
            Guid pf1 = AddProductFlavour("Fanta Black Currant", "Fanta Black Currant", "006", pb1);
            Guid pf2 = AddProductFlavour("Fanta Citrus ", "Fanta Citrus ", "014", pb2);
            Guid pf3 = AddProductFlavour("Coca Cola ", "Coca Cola", "016", pb3);
            Guid pf4 = AddProductFlavour("Dasani Still", "Dasani Still", "074", pb4);
            //Add Product Type
            Guid pt1 = AddProductType("Carbonated Soft Drink", "002");
            Guid pt2 = AddProductType("Water", "003");
            Guid pt3 = AddProductType("Empty Shells", "004");
            Guid pt4 = AddProductType("Mixed Packagings", "005");
            //Add product PackagingType
            Guid ppt1 = AddProductPackagingType("Bottle", "Bottle", "002");
            Guid ppt2 = AddProductPackagingType("Can", "Can", "003");
            Guid ppt3 = AddProductPackagingType("Tetra", "Tetra", "003");
            //Add product Packaging
            Guid pp1 = AddProductPackaging("Coke 300ml 24RB", "300ml RGBx24", "002");
            Guid pp2 = AddProductPackaging("1500ml Dasani ", "1500ml Dasani", "003");
            Guid pp3 = AddProductPackaging("1L RGB", "1L RGB", "004");
            Guid pp4 = AddProductPackaging("300ml RGB", "300ml RGB", "005");
            //add product
            Guid prod = AddSaleProduct("SP001", "300ml RGBx24-Fanta Citrus 300ml 24 RB", pf2, pb2, pp4, ppt1, pt1, 1, vatClass, returnableBottle,56);
            Guid prod1 = AddSaleProduct("SP002", "Coke 300ml 24 RB", pf3, pb3, pp4, ppt1, pt1, 1, vatClass, returnableBottle,89);
            Guid prod2 = AddSaleProduct("SP003", "Dasani Water still 1.5l 12 s/w", pf4, pb4, pp2, ppt1, pt2, 1, vatClass, returnableBottle,45);

            

            //Returnable Product
            Guid prodReturn = AddReturnableProduct("RP001", "300ml RGBx24-Sprite  300ml 24 RB", pf1, pb1, pp1, ppt1, pt1, 1, ReturnableType.GenericReturnable, vatClass, 1, returnableCrate,78);
            //Add Consolidated
            Guid consolidatedProdcut = AddConsolidatedProduct(prod, pb1, "Case XX", pf1, pp1, ppt1,"CP001", 3,78);
            //Add pricing tier
            //int ppTier = AddPricingTier("Tier 1","001", "Description");
            Guid ppTier2 = AddPricingTier("Tier 2", "002", "Tier 2");
            Guid ppTier3 = AddPricingTier("Tier 3", "003", "Tier 3");
            //Add Product Pricing
            AddPricing(prod, ppTier, 40, 30, DateTime.Now.AddMinutes(-10));
            AddPricing(prod1, ppTier, 50, 35, DateTime.Now.AddMinutes(-20));
            AddPricing(prod2, ppTier, 60, 40, DateTime.Now.AddMinutes(-30));

            /*int ppPricing2 = AddPricing(prod1, ppTier2, 400, 600, DateTime.Now);
            int ppPricing3 = AddPricing(prod2, ppTier3, 300, 500, DateTime.Now);
            int ppRetunableProductPricing = AddPricing(prodReturn, ppTier, 100, 200, DateTime.Now);
            int ppconsolidatedProductPricing = AddPricing(consolidatedProdcut, ppTier, 1000, 1500, DateTime.Now);*/
            //Add outlet category
            Guid outCategory = AddOutletCategory("Gold", "002");
            Guid outCategory2 = AddOutletCategory("Bronze", "003");
            Guid outCategory3 = AddOutletCategory("Silver", "004");
            Guid outCategory4 = AddOutletCategory("Un-assigned", "005");
           
            
            
            //Add OutletType
            Guid outType = AddOutLetType("Duka", "002");
            Guid outType2 = AddOutLetType("Kiosk", "003");
            Guid outType3 = AddOutLetType("Hotels", "004");
            Guid outType4 = AddOutLetType("Bars", "005");
            //Add Country
            Guid country = AddCountry("Kenya","KES","KE");
           
            //Add SocialEconomic Status
            Guid status = AddSocialeconomicStatus("Rich");
            //Add Region
            Guid regionNairobiUrban = AddRegion("Nairobi Urban", "Nairobi Urban Region", country);
            //Add Area
            Guid area = AddArea("Nakuru", "Nakuru Area", regionNairobiUrban);
            //Add producer
            Guid prodrCoke = AddProducerCC("Coke");
            //Add User ASM
            Guid groupId = AddUserGroup("Admin");
            AddUserGroupRoles(groupId);

            Guid usrASM = AddUser("KamemeXX", "12345678", "0720000000", "AADD123", prodrCoke, UserType.ASM, groupId);
            //Add User Surveyor
            Guid usrSurv = AddUser("Kamau", "12345678", "0730000100", "AADD125", prodrCoke, UserType.Surveyor, groupId);
            //Add User SalesRep
            Guid usrSRep = AddUser("Njoroge", "12345678", "0750000200", "AADD127", prodrCoke, UserType.SalesRep, groupId);

            Guid usrSRep1 = AddUser("John1", "12345678", "0750000200", "AADD129", prodrCoke, UserType.HQAdmin, groupId);

            //Add Distributor
            Guid distribErnestMburu = AddDistributorCC(prodrCoke, "Ernest Mburu", "Ernest Mburu", "A001155FF", "12456", regionNairobiUrban, usrSRep, usrSurv, usrASM, "12235WA", "123456", "123456", "D001");
            Guid userDist1 = AddUser("Kameme", "12345678", "0720000000", "AADD122", distribErnestMburu, UserType.WarehouseManager, groupId);
            AddDistributorCCApplication(distribErnestMburu);
            AddDistributorCCApplication(distribErnestMburu);
            AddDistributorPendingDispatchWarehouse(distribErnestMburu, "Test name Pending Dispatch Warehouse");

            //int userDistribtrUser = AddUser("Tony", "doggie", "12345444", "AANNNNNN", distrib, UserType.WarehouseManager);
            //Add Route
            Guid routeShop = AddRoute(regionNairobiUrban, "[Shop]", "001");
            //Add Vat Class
            Guid vatClass2 = AddVatClass("Vat 2", "Class 2", 0.16M, effectiveDate);
            //Add Account
            //int account = AddAccount(distrib, (int)AccountType.Cash, 500);
            //Add Inventory
            //int inventory = AddInventory(distrib, 900, 90, prod);
            //Guid maritalStatus = AddMaritalStatus("002", "Widowed", "She Passed");
            //Guid maritalStatus2 = AddMaritalStatusCC("8520", "Divorcee", "She Flew");
            Guid contactType = AddContactType("005", "Outlet Contact", "all contacts");
            //Add Contact
            Guid contact = AddContact(distribErnestMburu, "Michael", "254722557538", "020 22222", "254722557538", "1000", "2000", "Nairobi", "00100", "Nairobi", (int)ContactClassification.PrimaryContact, MaritalStatas.Single, contactType, (int)ContactOwnerType.Distributor, "jgitau@virtualcity.co.ke");
            Guid discountGroup = AddDiscountGroup("Outlet Discount", "OD2344");
            //Add Outlet
            Outlet outlet = CreateOutlet(outCategory, outType, distribErnestMburu, routeShop, ppTier, "Yaya Duka", vatClass, "O001", Guid.Empty, 36.7867442F, -1.2930796F,ppTier);
            CreateOutlet(outCategory, outType, distribErnestMburu, routeShop, ppTier, "Town Shop", vatClass, "O002", discountGroup, 36.756486F, -1.2930796F);
            Outlet outlet2 = CreateOutlet(outCategory, outType, distribErnestMburu, routeShop, ppTier, "Eastleight Supermarket", vatClass, "O003", Guid.Empty, 36.746486F, -1.2930796F);
            Outlet outlet3 = CreateOutlet(outCategory, outType, distribErnestMburu, routeShop, ppTier, "Kiambu Mall", vatClass, "O004", Guid.Empty, 36.726486F, -1.2930796F);
            CreateOutlet(outCategory, outType, distribErnestMburu, routeShop, ppTier, "Ngara Hotel", vatClass, "O005", Guid.Empty, 36.706486F, -1.2930796F);
            CreateOutletPriority(outlet.Id, routeShop, 1);
            CreateOutletVistDay(outlet.Id, DayOfWeek.Monday);
            //Add Distributor Salesman
            Guid salesmanId = CreateDistributorSalesman(distribErnestMburu, routeShop, "John", groupId,"S001",DistributorSalesmanType.Salesman);
            Guid salesmanId2 = CreateDistributorSalesman(distribErnestMburu, routeShop, "Maina", groupId, "S002", DistributorSalesmanType.Salesman);
            Guid salesmanId3 = CreateDistributorSalesman(distribErnestMburu, routeShop, "Nzomo", groupId, "S003", DistributorSalesmanType.Salesman);
            Guid salesmanId4 = CreateDistributorSalesman(distribErnestMburu, routeShop, "Wallace", groupId, "S004", DistributorSalesmanType.Salesman);
            Guid salesmanId5 = CreateDistributorSalesman(distribErnestMburu, routeShop, "Stockist", groupId, "S005", DistributorSalesmanType.Stockist);
            Guid salesmanId6 = CreateDistributorSalesman(distribErnestMburu, routeShop, "StockistSalesman", groupId, "S006", DistributorSalesmanType.StockistSalesman);
            //int userDistributrSalesman = AddUser("John", "john", "12345444", "AANNNNNN", salesmanId, UserType.DistributorSalesman, groupId);

            //Create data into salesman supplier table
            Guid salesmanSupplier = CreateSalemanSupplier(salesmanId, supp1);
            Guid salesmanSupplier1 = CreateSalemanSupplier(salesmanId2, supp1);
            Guid salesmanSupplier2 = CreateSalemanSupplier(salesmanId3, supp1);
            Guid salesmanSupplier3 = CreateSalemanSupplier(salesmanId3, supp2);
            Guid salesmanSupplier4= CreateSalemanSupplier(salesmanId4, supp2);
            Guid salesmanSupplier5 = CreateSalemanSupplier(salesmanId5, supp2);


            /*salesmanId = CreateDistributorSalesman(distrib, route, "Mary Salesman");
            userDistributrSalesman = AddUser("Mary", "mary", "12345444", "AANNNNNN", salesmanId, UserType.DistributorSalesman, groupId);

            salesmanId = CreateDistributorSalesman(distrib, route, "Tom Salesman");
            userDistributrSalesman = AddUser("Tom", "tom", "12345444", "AANNNNNN", salesmanId, UserType.DistributorSalesman, groupId);

            salesmanId = CreateDistributorSalesman(distrib, route, "Jerry Salesman");
            userDistributrSalesman = AddUser("Jerry", "jerry", "12345444", "AANNNNNN", salesmanId, UserType.DistributorSalesman, groupId);*/

            //Add Channel Packagings
            Guid channelPacks = AddChannelPacks(pp1, outType, true);

            //Add Cooler Type
            Guid assetType = AddAssetType("COOLER TYPE", "CT087976");
            Guid assetStatus = AddAssetStatus("Good", "Good");
            Guid assetStatus1 = AddAssetStatus("Broken", "Broken");
            Guid assetCategory = AddAssetCategory("Cooler Small", "Small", assetType);
            //Add Cooler
            Guid cooler = AddAsset(assetType, assetStatus, assetCategory, "C8689", "300L", "SN987656", "AN098977");

            //Add Province
            Guid province = AddProvince("NAIROBI", "NAIROBI REGION", country);
            //Add District
            Guid district = AddDistrict(province, "DAGORETI");
            //Add Target Period
            DateTime now = DateTime.Now;
            DateTime now2 = DateTime.Now.AddMonths(1);
            DateTime now3 = DateTime.Now.AddMonths(2);
            DateTime now4 = DateTime.Now.AddMonths(3);
            DateTime now5 = DateTime.Now.AddMonths(4);
            DateTime now6 = DateTime.Now.AddMonths(5);
            Guid targetPeriod = AddTargetPeriod("SALES TARGET Month 1", now, now2);
            Guid targetPeriod2 = AddTargetPeriod("Sales Target Month 2", now2.AddDays(1), now3);
            Guid targetPeriod3 = AddTargetPeriod("SALES TARGET Month 3", now3.AddDays(1), now4);
            Guid targetPeriod4 = AddTargetPeriod("SALES TARGET Month 4", now4.AddDays(1), now5);
            Guid targetPeriod5 = AddTargetPeriod("SALES TARGET Month 5", now5.AddDays(1), now6);
            //Add Competitor
            Guid competitor = AddCompetitor("QUENCHER", "INDUSTRIAL AREA", "788-NAIROBI", "0721345678", "MATENDECHERE", "NAIROBI", "675", "234");
            //Add Competitor Product
            Guid competitorProduct = AddCompetitorProduct("JUICE", "HIGHLANDS", competitor, pb1, pp1, pt1, ppt1, pf1);
            //Add Reorder Level
            Guid reorderLevel = AddReorderLevel(distribErnestMburu, prod, 21);
           
            //Add Target
            Guid target = AddTarget(outlet.Id, prod, targetPeriod, 100, true);
            Guid target2 = AddTarget(outlet2.Id, prod, targetPeriod2, 90000, false);
            Guid target3 = AddTarget(outlet3.Id, prod, targetPeriod3, 5000, true);
          //  Guid targetItem = AddTargetItem(prod, 2, target);
            //Add Product Discount
            DateTime startdate = DateTime.Now.AddDays(-10);
            DateTime enddate = DateTime.Now.AddDays(90);
            Guid prodDisc = AddProductDiscount(prod, ppTier, 0.1M, startdate, enddate);
            Guid prodDisc2 = AddProductDiscount(saleProduct, ppTier, 0.12M, startdate, enddate);
            //Add Sale Value Discount
            Guid saleValDisc = AddSaleValueDiscount(ppTier, 0.15M, 1000, startdate, enddate);
            //add Discount Group
           
            DiscountGroup gd = _discountGroupRepository.GetById(discountGroup);
            Guid customerDiscount = AddProductGroupDiscount(gd, new ProductRef { ProductId = saleProduct }, 0.2M, startdate, enddate);
            Guid freeofCharge = AddFreeOfChargeDiscountDiscount(new ProductRef { ProductId = prod }, startdate, enddate);
            Guid promo = AddPromotionDiscount(new ProductRef { ProductId = prod1 }, saleProduct, 10, 3, 0.25M, startdate, enddate);
            Guid promo1 = AddPromotionDiscount(new ProductRef { ProductId = saleProduct2 }, saleProduct2, 10, 3, 0.25M, startdate, enddate);
            Guid certainProduct = AddCertainValueCertainProductDiscount(new ProductRef { ProductId = prod }, 20, 18000, startdate, enddate);
            Guid certainProduct2 = AddCertainValueCertainProductDiscount(new ProductRef { ProductId = saleProduct }, 10, 1000, startdate, enddate);
            Guid bank = AddBank("KCB", "KENYA COMMERCIAL BANK", "0267K");
            Guid bankBranch = AddBankBranch("MOI AVENUE", "HEAD OFFICE", "09875", bank);

            Guid reason1 = AddOutletVisitReasonType("SHop Closed", "SHop Closed", OutletVisitAction.NoAction);
            Guid reason2 = AddOutletVisitReasonType("Sufficient Stock", "Sufficient Stock", OutletVisitAction.UnProductive);

            Guid setting = AddAppSetting("http://localhost:55193/pgbridge/", SettingsKeys.PaymentGatewayWSUrl);
            AddDocumentSetting();
            Guid retire = AddRetireSetting(RetireType.Paid, 0);

            #region Agrimanagr

            //Add producer
            Guid prodrAgrimanagerCompany = AddProducerCC("Agrimanagr Company Ltd.");

            Guid agrimHQAmdin = AddUser("AgriHQAdmin", "12345678", "0750000201", "RADD129", prodrAgrimanagerCompany, UserType.AgriHQAdmin, groupId);

            Guid regionTransNzoia = AddRegion("Trans Nzoia", "TN Dist", country);
            Guid routeEldKitale = AddRoute(regionTransNzoia, "Eldoret Kitale Highway", "Eld-Ktl");

            Guid hubKitaleMain = AddHub("HUB", "Kitale Main Hub", regionTransNzoia, "HUB12343VAT", new CostCentreRef {Id = prodrAgrimanagerCompany});

            Guid routeKitale = AddRoute(regionNairobiUrban, "Kitale", "KTL001");
            
            Guid centreType = AddCentreType("Ceral", "Cereal");
            Guid centreMois = AddCentre("Moi's Bridge Cereal", "MTDC", centreType, hubKitaleMain, routeEldKitale);
            Guid allocationrouteKitaleMois = AddMasterDataAllocation(routeKitale, centreMois, MasterDataAllocationType.RouteCentreAllocation);
            Guid allocationrouteEldKitlcentreMois = AddMasterDataAllocation(routeEldKitale, centreMois, MasterDataAllocationType.RouteCentreAllocation);
           // Guid maritalStatuss = AddMaritalStatus("Single", "Single", "Single");
            Guid commOwnerType = AddCommodityOwnerType("CerealOwnerType", "Cereal Owner Type");
            Guid commSuppl = AddCommoditySupplier("KirFarm456",bankBranch, bank,
                                                  CommoditySupplierType.Individual, "Kirwa Supplier",
                                                  CostCentreType.CommoditySupplier, "Kirwa Supplier",
                                                  new CostCentreRef {Id = hubKitaleMain}, "KirwaPIN12");
         
            Guid commOwner = AddCommodityOwner("Kirwa Buss num", "Kirwa", "Rui", "12345","1234567890", "PIN123",commOwnerType,commSuppl,"Kipchirchir","Jacob");


            Guid routeRoisambu = AddRoute(regionTransNzoia, "Roisambu", "ROi001");

            Guid regionRoisambu = AddRegion("Roisambu", "Roisambu", country);
            Guid centreType2 = AddCentreType("Cereal2", "Cereal2");
            Guid centreRoisambu = AddCentre("Roisambu Cereal", "ROIC", centreType2, hubKitaleMain,routeRoisambu);
            Guid allocationrouteRoisambucentreRoisambu = AddMasterDataAllocation(routeRoisambu, centreRoisambu, MasterDataAllocationType.RouteCentreAllocation);
            Guid allocationrouteEldKitlcentreRoisambu = AddMasterDataAllocation(routeEldKitale, centreRoisambu, MasterDataAllocationType.RouteCentreAllocation);
            Guid commOwnerType2 = AddCommodityOwnerType("Milk Producers", "Milk Producers Type");
            Guid commSuppl2 = AddCommoditySupplier("MPROIS4563", bankBranch, bank,
                                                  CommoditySupplierType.Cooperative, "ROI Coop Suppliers",
                                                  CostCentreType.CommoditySupplier, "Roisambu Suppliers Ltd",
                                                  new CostCentreRef { Id = hubKitaleMain }, "ROIPIN12");

            Guid commProducer = AddCommodityProducer("1500", "KirFarm", "Kirwa Farm", "Reg 123", commSuppl);
            Guid commProducer2 = AddCommodityProducer("3400", "Roisambu Milk Farm", "Roisambu Milk Farm", "Reg 123 M", commSuppl2);
            Guid commOwner2 = AddCommodityOwner("Wamalwa 11242", "Wamash", "Wafula", "142345",  "2323147", "PIN1234", commOwnerType2, commSuppl2, "Situma","Robert");
            Guid producerToCenter1 = AddMasterDataAllocation(commProducer2, centreRoisambu, MasterDataAllocationType.CommodityProducerCentreAllocation);
            Guid producerToCenter2 = AddMasterDataAllocation(commProducer, centreRoisambu, MasterDataAllocationType.CommodityProducerCentreAllocation);
            Guid producerToCenter3 = AddMasterDataAllocation(commProducer2, centreMois, MasterDataAllocationType.CommodityProducerCentreAllocation);
            Guid producerToCenter4 = AddMasterDataAllocation(commProducer, centreMois, MasterDataAllocationType.CommodityProducerCentreAllocation);
            Guid commodType = AddCommodityType("Cereal", "Cereal", "Cereal");
            
            Guid commodType2 = AddCommodityType("Dairy Product", "Dairy Product", "Dairy Product");

            Guid commodity1 = AddCommodity("Maize", "Maize",commodType);
           Guid commodity4 = AddCommodity("Fake-Maize", "Fake-Maize", commodType);
            Guid grade1 = _commodityRepository.GetAllGradeByCommodityId(commodity1).FirstOrDefault().Id;
            Guid commodity2 = AddCommodity("Milk", "Milk", commodType2);

           Guid commodity3 = AddCommodity("Wheat", "Wheat", commodType);
           Guid containerType=AddContainType("20 L Aluminium", 2, "Metallic", ContainerUseType.WeighingContainer, grade1);
           Guid containerType1 = AddContainType("90 kg Maize Bag", 5, "sisal", ContainerUseType.StorageContainer,null);
           Guid containerType2 = AddContainType("12 kg Tea bag", 3, "sisal", ContainerUseType.WeighingContainer, null);
           Guid containerType3 = AddContainType("5 L Aluminium", 0.5M, "Metallic", ContainerUseType.WeighingContainer, grade1);
           Guid containerType4 = AddContainType("12 kg Tea bag sc", 1, "sisal", ContainerUseType.StorageContainer, null);
           Guid container1 = AddSourcingContainer("Milk Can 1", hubKitaleMain, "Milk Can 1", "Container 1", "Aluminium", "11", "Milk Can 1",  containerType);
           Guid container2 = AddSourcingContainer("Milk Can 2", hubKitaleMain, "Milk Can 2", "Container 2", "Aluminium", "12", "Milk Can 2",  containerType1);
           Guid container3 = AddSourcingContainer("Maize Bag 1", hubKitaleMain, "Maize Bag 1", "Container 3", "Sisal", "121", "Maize Bag 1", containerType2);
           Guid container4 = AddSourcingContainer("Maize Bag 2", hubKitaleMain, "Maize Bag 2", "Container 4", "Nylon", "122", "Maize Bag 2",  containerType3);
           Guid container5 = AddSourcingContainer("Wheat Bag 3", hubKitaleMain, "Wheat Bag 3", "Container 5", "Sisal", "123", "Wheat Bag 3", containerType4);
           Guid genericContainer = AddSourcingContainer("Generic Cont", hubKitaleMain, "Generic Container", "Container 6", "Steel", "ADS", "Generic Container", containerType4);

           AddVehicle("veh1", "veh1-code", "Fake", hubKitaleMain, "desc", "veh1-reg");
           AddVehicle("veh2", "veh2-code", "Fake", hubKitaleMain, "desc", "veh2-reg");
           AddVehicle("veh3", "veh3-code", "Fake", hubKitaleMain, "desc", "veh3-reg");
           AddVehicle("veh4", "veh4-code", "Fake", hubKitaleMain, "desc", "veh4-reg");
            Guid beans = AddCommodity("Beans", "Beans", commodType);

            AddPrinter("Epson 100", "EP100", "Epson Printer 100", "Epson", "Epson", "Printer", hubKitaleMain);
            AddPrinter("Hp 100", "HP100", "Hp Printer 100", "Hp", "Hp", "Printer", hubKitaleMain);
            AddPrinter("Pidion", "Pidion", "Pidion", "Pidion", "Pidion", "Printer", hubKitaleMain);

            AddWeighingScale("Matz Scale 100", "SC100", "Matz 100", "Matzuda", "Matzuda S series", "Matz Scale", hubKitaleMain);
            AddWeighingScale("Matz Scale 200", "SC200", "Matz 200", "Matzuda", "Matzuda S series", "Matz Scale", hubKitaleMain);
            AddWeighingScale("Matz Scale 300", "SC300", "Matz 300", "Matzuda", "Matzuda S series", "Matz Scale", hubKitaleMain);
           

            AddStore("Main Store", "Main Store", new CostCentreRef {Id = hubKitaleMain}, "vatClass");

            AddUser("Driver1", "12345678", "045585521", "1234", hubKitaleMain, UserType.Driver, groupId);
            AddUser("Driver2", "12345678", "235585522", "1235", hubKitaleMain, UserType.Driver, groupId);


            CreatePurchasingClerk(hubKitaleMain, routeRoisambu, "Hubuser", groupId, "1234");

            AddUser("Hubmanager", "12345678", "045585522", "12345", hubKitaleMain, UserType.HubManager, groupId);

           AddServiceProvider("James Okello", "JMK", "0712348654", "AO785423", "27894456", "James Account", "0001455223387", Gender.Male, "At Your Service",bank,bankBranch);
           AddServiceProvider("Davidson Company", "Davidson", "0752348654", "AO057468", "21894456", "Davidson Limited Account", "00457962341", Gender.Male, "Service at your farm", bank, bankBranch);
           AddServiceProvider("Miriam Makeba", "MMK", "02112348654", "A08645312", "19894456", "Miriam Account", "00531455223387", Gender.Female, "Service is our agenda", bank, bankBranch);

            AddService("Weeding", "WD001", "Removing plants which are not required", 500m);
            AddService("Spraying", "SP002", "Desc", 400m);
            AddService("Planting", "PL003", "Desc Planting", 300m);
            AddService("Harvesting", "HARV004", "Desc Harvesting", 600m);
            AddService("Sorting", "SRT005", "Desc Sorting", 350m);
            AddService("Snipping", "SNP006", "Desc Snipping", 250m);
            AddService("Transporting", "TRP007", "Desc Transporting", 450m);
            AddService("Scouting", "SCT008", "Desc Scouting", 600m);

            AddSeason("Season1", "SSN1", "First Season", commProducer, DateTime.Now, DateTime.Now.AddMonths(6));

            AddInfection("Disease 1", "DS001", "First Disease", InfectionType.Disease);
            AddInfection("Disease 2", "DS002", "Second Disease", InfectionType.Disease);
            AddInfection("Pest 1", "PST001", "First Pest", InfectionType.Pest);
            AddInfection("Pest 2", "PST002", "Second Pest", InfectionType.Pest);

            var date = DateTime.Now;
            var end = date.AddHours(8);
            AddShift("Morning Shift", "MRNShift", "Morning Shift", date,end);
            AddShift("Evening Shift", "EVEShift", "Evening Shift", end, end.AddHours(8));

            AddActivityType("Preparation", "Preparation", "Preparation", false, false, true, false);
            AddActivityType("Planting", "Planting", "Planting", true, false, true, false);
            AddActivityType("Scouting", "Scouting", "Planting", false, true, false, false);
            AddActivityType("Weeding", "Weeding", "Weeding", false, false, true, false);
            AddActivityType("Harvesting", "Harvesting", "Harvesting", false,false, true, true);
            AddActivityType("Sorting", "Sorting", "Sorting", false, false, true, true);
            #endregion
        }

       


        //public int AddBank(string name, string desc, string code)
       // {
       //     Bank bank = new Bank(Guid.NewGuid())

       //     {
       //         Name = name,
       //         Description = desc,
       //         Code = code
                
       //     };
       //     return _bankRepository.Save(bank );
       // }
       //public int AddBankBranch(string name, string desc, string code,int BankId)
       // {
       //     BankBranch bankBranch = new BankBranch(Guid.NewGuid())

       //     {
       //         Name = name,
       //         Description = desc,
       //         Code = code,
       //         BankId=_bankRepository.GetById(BankId ).Id 

       //     };
       //     return _bankBranchRepository.Save(bankBranch);
        // }
    }
}
