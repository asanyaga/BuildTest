using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
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

namespace Distributr.DatabaseSetup
{
    public class InsertPHDTestData : InsertTestDataBase, IInsertTestData
    {

        public InsertPHDTestData(IOutletPriorityRepository outletPriorityRepository,
         IOutletVisitDayRepository outletVisitDayRepository, IProductDiscountGroupFactory productDiscountGroupFactory,
         ICertainValueCertainProductDiscountFactory certainValueCertainProductDiscountFactory,
         ICustomerDiscountFactory customerDiscountFactory,
         IPromotionDiscountFactory promotionDiscountFactory,
         IProductDiscountGroupRepository productDiscountGroupRepository,
         IPromotionDiscountRepository promotionDiscountRepository,
         IFreeOfChargeDiscountRepository freeOfChargeDiscountRepository,
         ICertainValueCertainProductDiscountRepository certainValueCertainProductDiscountRepository,IDiscountGroupRepository discountGroupRepository,IAssetStatusRepository assetStatusRepository,
        IAssetCategoryRepository assetCategoryRepository,
IProductTypeRepository productTypeRepository, IProductBrandRepository productBrandRepository, IProductFlavourRepository productFlavourRepository, IProductPackagingRepository productPackagingRepository, IProductPackagingTypeRepository productPackagingTypeRepository, IProductRepository productRepository, IRegionRepository regionRepository, ICostCentreRepository costCentreRepository, ICostCentreFactory costCentreFactory, IProductPricingRepository pricingRepository, IVATClassRepository vatClassRepository, IVATClassFactory vatClassFactory, ICountryRepository countryRepository, IProductPricingFactory productPricingFactory, IProductPricingTierRepository productPricingTierRepository, IOutletTypeRepository outletTypeRepository, IUserRepository userRepository, IOutletRepository outletRepository, IRouteRepository routeRepository, IRouteFactory routeFactory, ITransporterRepository transporterRepository, IProductFactory productFactory, IDistributorSalesmanRepository distributorSalesmanRepository, IProducerRepository producerRepository,  IDocumentFactory documentFactory, ISocioEconomicStatusRepository socioEconomicStatusRepository, IClientMasterDataTrackerRepository clientMasterDataTrackerRepository, IDistributorRepository distributorrepository, IOutletCategoryRepository outletCategoryRepository,  ITerritoryRepository territoryRepository, IAddOrderLineItemCommandHandler addOrderLineItemCommandHandler, IConfirmOrderCommandHandler confirmOrderCommandHandler,  IRejectOrderCommandHandler rejectOrderCommandHandler, IAreaRepository areaRepository, IContactRepository contactRepository, IAccountRepository accountRepository, IAccountTransactionRepository accountTransactionRepository, IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository, ICostCentreApplicationRepository costCentreApplicationRepository, IChannelPackagingRepository channelPackagingRepository, ICompetitorRepository competitorRepository, ICompetitorProductsRepository competitorProductRepository, IAssetRepository coolerRepository, IAssetTypeRepository coolerTypeRepository, IDistrictRepository districtRepository, IProvincesRepository provinceRepository, IReOrderLevelRepository reorderLevelRepository, ITargetPeriodRepository targetPeriodRepository, ITargetRepository targetRepository, IProductDiscountFactory productDiscountFactory, IProductDiscountRepository productDiscountRepository, ISaleValueDiscountFactory saleValueDiscountFactory, ISaleValueDiscountRepository saleValueDiscountRepository/*, IContainmentRepository containmentRepository*/, ISalesmanRouteRepository salesmanRouteRepository, IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRolesRepository,IContactTypeRepository contactTypeRepository)
        {
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
            //_containmentRepository = containmentRepository;
            _salesmanRouteRepository = salesmanRouteRepository;
            _userGroupRepository = userGroupRepository;
            _userGroupRolesRepository = userGroupRolesRepository;
           
            _contactTypeRepository = contactTypeRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _assetStatusRepository = assetStatusRepository;
            _discountGroupRepository = discountGroupRepository; 
            _discountGroupRepository = discountGroupRepository;
            _productDiscountGroupFactory = productDiscountGroupFactory;
            _certainValueCertainProductDiscountFactory = certainValueCertainProductDiscountFactory;
            _customerDiscountFactory = customerDiscountFactory;
            _promotionDiscountFactory = promotionDiscountFactory;
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _productDiscountGroupRepository = productDiscountGroupRepository;
            _freeOfChargeDiscountRepository = freeOfChargeDiscountRepository;
            _certainValueCertainProductDiscountRepository = certainValueCertainProductDiscountRepository;
            _outletPriorityRepository = outletPriorityRepository;
            _outletVisitDayRepository = outletVisitDayRepository;

        }

        public void InsertTestMasterData()
        {
            DateTime effectiveDate = DateTime.Now;
            DateTime endDate = effectiveDate.AddDays(3);
            Guid vatClass = AddVatClass("Vat 3", "Class 3", 0.16M, effectiveDate);
            //Add Supplier
            Guid supp1 = AddSupplier("001", "Eunice", "Panadol supplier");
            //Add product brand
            Guid pb1 = AddProductBrand(supp1, "Rapid Test Kit", "Rapid Test Kit", "001");
            /*Guid pb1 = AddProductBrand("Bibo Group", "Bibo Group", "002");
            Guid pb2 = AddProductBrand("Fanta Group", "Fanta Group", "007");
            Guid pb3 = AddProductBrand("Colas Group", "Colas Group", "005");
            Guid pb4 = AddProductBrand("Dasani Group", "Dasani Group", "030");*/
            //Add product flavour
            Guid pf1 = AddProductFlavour("Medication", "Medication", "006",pb1);
            /*Guid pf1 = AddProductFlavour("Fanta Black Currant", "Fanta Black Currant", "006");
            Guid pf2 = AddProductFlavour("Fanta Citrus ", "Fanta Citrus ", "014");
            Guid pf3 = AddProductFlavour("Coca Cola ", "Coca Cola", "016");
            Guid pf4 = AddProductFlavour("Dasani Still", "Dasani Still", "074");*/
            //Add Product Type
            Guid pt1 = AddProductType("Medication", "005");
            /*Guid pt1 = AddProductType("Carbonated Soft Drink");
            Guid pt2 = AddProductType("Water");
            Guid pt3 = AddProductType("Empty Shells");
            Guid pt4 = AddProductType("Mixed Packagings");*/
            //Add product PackagingType
            Guid ppt1 = AddProductPackagingType("Pack", "Pack", "006");
            /*Guid ppt1 = AddProductPackagingType("Bottle", "Bottle");
            Guid ppt2 = AddProductPackagingType("Can", "Can");
            Guid ppt3 = AddProductPackagingType("Tetra", "Tetra");*/
            //Add product Packaging
            Guid pp1 = AddProductPackaging("Drug 200mg", "Drug 200mg", "002");
            /*Guid pp1 = AddProductPackaging("Coke 300ml 24RB", "300ml RGBx24");
            Guid pp2 = AddProductPackaging("1500ml Dasani ", "1500ml Dasani");
            Guid pp3 = AddProductPackaging("1L RGB", "1L RGB");
            Guid pp4 = AddProductPackaging("300ml RGB", "300ml RGB");*/
            //add product
            Guid saleProduct = AddSaleProduct("001", "Determine", pf1, pb1, pp1, ppt1, pt1, 1, vatClass, Guid.Empty,33);
            Guid prod1 = AddSaleProduct("002", "Bioline", pf1, pb1, pp1, ppt1, pt1, 1, vatClass, Guid.Empty,44);
            Guid prod2 = AddSaleProduct("003", "Unigold", pf1, pb1, pp1, ppt1, pt1, 1, vatClass, Guid.Empty,43);
            
            /*Guid prod = AddSaleProduct("3630", "300ml RGBx24-Fanta Citrus 300ml 24 RB", pf2, pb2, pp4, ppt1, pt1, 1);
            Guid prod1 = AddSaleProduct("1030", "Coke 300ml 24 RB", pf3, pb3, pp4, ppt1, pt1, 1);
            Guid prod2 = AddSaleProduct("1080", "Dasani Water still 1.5l 12 s/w", pf4, pb4, pp2, ppt1, pt2, 1);*/
            //Returnable Product
            Guid returnableBottle = AddReturnableProduct("004", "Darkened Bottle", pf1, pb1, pp1, ppt1, pt1, 1, ReturnableType.GenericReturnable, vatClass, 1, Guid.Empty,343);
            Guid returnableCrate = AddReturnableProduct("005", "Padded Carton box", pf1, pb1, Guid.Empty, ppt1, pt1, 1, ReturnableType.GenericReturnable, vatClass, 24, Guid.Empty,343);//TODO: add quantity/capacity
            /*Guid prodReturn = AddReturnableProduct("3831B", "300ml RGBx24-Sprite  300ml 24 RB", pf1, pb1, pp1, ppt1, pt1, 1,ReturnableType.GenericReturnable );
            //Add Consolidated
            Guid consolidatedProdcut = AddConsolidatedProduct(prod, pb1, "Case XX", pf1, pp1, ppt1, "XXX");*/
            Guid caseOfSodaplusB = AddConsolidatedProduct(returnableCrate, pb1, "Crate of Fanta Passion", pf1, pp1, ppt1, "Crate of Soda", 1,3434);
            Guid dd = AddConsolidatedProductLineItem(caseOfSodaplusB, returnableBottle, 24);
            Guid dhsd = AddConsolidatedProductLineItem(caseOfSodaplusB, saleProduct, 24);

            //Add Containment
            //Guid containmentId = AddContainment(12, ppt1, prodReturn);
            AddProductPackagingInContainer(pp1, returnableCrate);

            //Add pricing tier
            Guid ppTier = AddPricingTier("Tier 5", "005", "Tier 5 Description");
            Guid ppTier2 = AddPricingTier("Tier 6", "006", "Tier 6 Description");
            Guid ppTier3 = AddPricingTier("Tier 7", "007", "Tier 7 Description");
            //Add Product Pricing
            Guid ppPricing = AddPricing(saleProduct, ppTier, 500, 700, DateTime.Now);
            Guid ppPricing2 = AddPricing(prod1, ppTier2, 40, 60, DateTime.Now.AddDays(-1));
            Guid ppPricing3 = AddPricing(prod2, ppTier3, 30, 50, DateTime.Now.AddDays(-2));

            Guid ppPricing4 = AddPricing(returnableBottle, ppTier3, 25, 20, DateTime.Now.AddDays(-2));
            Guid ppPricing5 = AddPricing(returnableCrate, ppTier3, 15, 10, DateTime.Now.AddDays(-2));
            /*Guid ppRetunableProductPricing = AddPricing(prodReturn, ppTier, 100, 200, DateTime.Now);
            Guid ppconsolidatedProductPricing = AddPricing(consolidatedProdcut, ppTier, 1000, 1500, DateTime.Now);*/
            //Add outlet category
            Guid outCategory = AddOutletCategory("Hospital", "002");
            Guid outCategory2 = AddOutletCategory("Dispensary", "003");
            Guid outCategory3 = AddOutletCategory("Health Centre", "004");
            Guid outCategory4 = AddOutletCategory("Clinic", "005");
            Guid outCategory5 = AddOutletCategory("Un-assigned", "006");
            //Add OutletType
            Guid outType = AddOutLetType("Referral", "002");
            Guid outType2 = AddOutletCategory("Emergency", "003");
            //Add Country
            Guid country = AddCountry("Kenya","KES","KES");
           
            //Add SocialEconomic Status
            Guid status = AddSocialeconomicStatus("Affluent");
            //Add Region
            Guid region = AddRegion("Nairobi Urban", "Nairobi Urban Region", country);
            //Add Area
            Guid area = AddArea("Nakuru", "Nakuru Area", region);
            //Add producer
            Guid prodr = AddProducerCC("Coke");
            Guid groupId = AddUserGroup("Admin");
            AddUserGroupRoles(groupId);
           

            //Add User ASM
            Guid usrASM = AddUser("KamemeXX", "12345678", "0720000000", "AADD123", prodr, UserType.ASM, groupId);
            //Add User Surveyor
            Guid usrSurv = AddUser("Kamau", "12345678", "0730000100", "AADD125", prodr, UserType.Surveyor, groupId);
            //Add User SalesRep
            Guid usrSRep = AddUser("Njoroge", "12345678", "0750000200", "AADD127", prodr, UserType.SalesRep, groupId);

            Guid usrSRep1 = AddUser("John1", "12345678", "0750000200", "AADD129", prodr, UserType.HQAdmin, groupId);

            //Add Distributor
            Guid distrib = AddDistributorCC(prodr, "Ernest Mburu", "Ernest Mburu", "A001155FF", "12456", region, usrSRep, usrSurv, usrASM, "12235WA", "123456", "123456","D001");
            Guid userDist1 = AddUser("Kameme", "12345678", "0720000000", "AADD122", distrib, UserType.WarehouseManager, groupId);
            AddDistributorCCApplication(distrib);
            AddDistributorCCApplication(distrib);
            AddDistributorPendingDispatchWarehouse(distrib, "Test name Pending Dispatch Warehouse");
            //Guid userDistribtrUser = AddUser("Tony", "doggie", "12345444", "AANNNNNN", distrib, UserType.WarehouseManager);
            //Add Route
            Guid route = AddRoute(region, "[Shop]", "001");
            Guid route2 = AddRoute(region, "[Shop 2]", "001");
            //Add Vat Class - taken to top
            //Guid vatClass = AddVatClass("Vat", "Class 1", 0.16M, dt);
            //Add Account
            //Guid account = AddAccount(distrib, (Guid)AccountType.Cash, 500);
            //Add Inventory
            //Guid inventory = AddInventory(distrib, 900, 90, prod);
            //Guid maritalStatus = AddMaritalStatus("004", "Single", "Never to marry");
           // Guid maritalStatus2 = AddMaritalStatusCC("8920", "Divorced", "She Gold");
            Guid contactType = AddContactType("006", "Distributor Contact", "all contacts");
            //Add Contact
            Guid contact = AddContact(distrib, "Michael", "0720 000 000", "020 22222", "0720 000 000", "1000", "2000", "Nairobi", "00100", "Nairobi", (int)ContactClassification.PrimaryContact,MaritalStatas.Single,contactType,(int)ContactOwnerType.Distributor,"");
            
            //Add Outlet
            //Outlet outlet = CreateOutlet(outCategory, outType, distrib, route, ppTier, "Kibatet Estate Dispensary");
            CreateOutlet(outCategory2, outType, distrib, route, ppTier, "Kibwari Dispensary", vatClass, "001", Guid.Empty);
            CreateOutlet(outCategory3, outType, distrib, route2, ppTier, "Kipchomo Dispensary", Guid.Empty, "002", Guid.Empty);
            CreateOutlet(outCategory4, outType, distrib, route, ppTier, "Kipkoigen Estate Dispensary", Guid.Empty, "003", Guid.Empty);
            CreateOutlet(outCategory, outType, distrib, route2, ppTier, "Koilot Health Centre", Guid.Empty, "004", Guid.Empty);

            CreateOutlet(outCategory2, outType, distrib, route2, ppTier, "Siret Dispensary", Guid.Empty, "005", Guid.Empty);
            CreateOutlet(outCategory3, outType, distrib, route, ppTier, "Siwo Dispensary", Guid.Empty, "006", Guid.Empty);
            CreateOutlet(outCategory4, outType, distrib, route2, ppTier, "Kimong Dispensary", Guid.Empty, "007", Guid.Empty);
            CreateOutlet(outCategory, outType, distrib, route, ppTier, "Chemase Health Centre", Guid.Empty, "008", Guid.Empty);
            CreateOutlet(outCategory2, outType, distrib, route2, ppTier, "Ngecheck Dispensary", Guid.Empty, "009", Guid.Empty);
            CreateOutlet(outCategory3, outType, distrib, route, ppTier, "P.C.E.A Ndalat Dispensary", Guid.Empty, "010", Guid.Empty);
            CreateOutlet(outCategory4, outType, distrib, route2, ppTier, "Bonjoge Dispensary", Guid.Empty, "011", Guid.Empty);
            CreateOutlet(outCategory, outType, distrib, route, ppTier, "Kabisaga Dispensary", Guid.Empty, "012", Guid.Empty);
            CreateOutlet(outCategory2, outType, distrib, route2, ppTier, "Kabiyet Health Centre", Guid.Empty, "013", Guid.Empty);
            CreateOutlet(outCategory3, outType, distrib, route, ppTier, "Kaigat SDA Health Centre", Guid.Empty, "014", Guid.Empty);

            //Add Distributor Salesman
            Guid salesmanId = CreateDistributorSalesman(distrib, route, "Logistics Officer1",groupId,"S001",DistributorSalesmanType.Salesman);
            Guid salesmanId2 = CreateDistributorSalesman(distrib, route, "Logistics Officer2", groupId, "S002", DistributorSalesmanType.Salesman);
            Guid salesmanId3 = CreateDistributorSalesman(distrib, route, "Logistics Officer3", groupId, "S003", DistributorSalesmanType.Salesman);
            Guid salesmanId4 = CreateDistributorSalesman(distrib, route, "Logistics Officer4", groupId, "S004", DistributorSalesmanType.Salesman);
            Guid salesmanId5 = CreateDistributorSalesman(distrib, route, "Logistics Officer5", groupId, "S005", DistributorSalesmanType.Salesman);
            Guid salesmanId6 = CreateDistributorSalesman(distrib, route, "Logistics Officer6", groupId, "S006", DistributorSalesmanType.Salesman);
            Guid salesmanId7 = CreateDistributorSalesman(distrib, route, "Logistics Officer7", groupId, "S007", DistributorSalesmanType.Salesman);
            Guid salesmanId8 = CreateDistributorSalesman(distrib, route, "Logistics Officer8", groupId, "S008", DistributorSalesmanType.Salesman);
            Guid salesmanId9 = CreateDistributorSalesman(distrib, route, "Logistics Officer9", groupId, "S009", DistributorSalesmanType.Salesman);
            Guid salesmanId10 = CreateDistributorSalesman(distrib, route, "Logistics Officer10", groupId, "S0010", DistributorSalesmanType.Salesman);
            Guid salesmanId11 = CreateDistributorSalesman(distrib, route, "Logistics Officer11", groupId, "S0011", DistributorSalesmanType.Salesman);
            Guid salesmanId12 = CreateDistributorSalesman(distrib, route, "Logistics Officer12", groupId, "S0012", DistributorSalesmanType.Salesman);
            Guid salesmanId13 = CreateDistributorSalesman(distrib, route, "Logistics Officer13", groupId, "S0013", DistributorSalesmanType.Salesman);
            Guid salesmanId14 = CreateDistributorSalesman(distrib, route, "Logistics Officer14", groupId, "S0014", DistributorSalesmanType.Salesman);
            Guid salesmanId15 = CreateDistributorSalesman(distrib, route, "Logistics Officer15", groupId, "S0015", DistributorSalesmanType.Salesman);
            Guid salesmanId16 = CreateDistributorSalesman(distrib, route, "Logistics Officer16", groupId, "S0016", DistributorSalesmanType.Salesman);
            Guid salesmanId17 = CreateDistributorSalesman(distrib, route, "Logistics Officer17", groupId, "S0017", DistributorSalesmanType.Salesman);
            Guid salesmanId18 = CreateDistributorSalesman(distrib, route, "Logistics Officer18", groupId, "S0018", DistributorSalesmanType.Salesman);
            Guid salesmanId19 = CreateDistributorSalesman(distrib, route, "Logistics Officer19", groupId, "S0019", DistributorSalesmanType.Salesman);
            Guid salesmanId20 = CreateDistributorSalesman(distrib, route, "Logistics Officer20", groupId, "S0020", DistributorSalesmanType.Salesman);


            Guid userDistributrSalesman = AddUser("John", "john", "12345444", "AANNNNNN", salesmanId, UserType.DistributorSalesman, groupId);

            //AddUser("John2", "john", "12345444", "AANNNNNN", salesmanId2, UserType.DistributorSalesman, groupId);
            //AddUser("John3", "john", "12345444", "AANNNNNN", salesmanId3, UserType.DistributorSalesman, groupId);
            //AddUser("John4", "john", "12345444", "AANNNNNN", salesmanId4, UserType.DistributorSalesman, groupId);
            //AddUser("John5", "john", "12345444", "AANNNNNN", salesmanId5, UserType.DistributorSalesman, groupId);
            //AddUser("John6", "john", "12345444", "AANNNNNN", salesmanId6, UserType.DistributorSalesman, groupId);
            //AddUser("John7", "john", "12345444", "AANNNNNN", salesmanId7, UserType.DistributorSalesman, groupId);
            //AddUser("John8", "john", "12345444", "AANNNNNN", salesmanId8, UserType.DistributorSalesman, groupId);
            //AddUser("John9", "john", "12345444", "AANNNNNN", salesmanId9, UserType.DistributorSalesman, groupId);
            //AddUser("John10", "john", "12345444", "AANNNNNN", salesmanId10, UserType.DistributorSalesman, groupId);
            //AddUser("John11", "john", "12345444", "AANNNNNN", salesmanId11, UserType.DistributorSalesman, groupId);
            //AddUser("John12", "john", "12345444", "AANNNNNN", salesmanId12, UserType.DistributorSalesman, groupId);
            //AddUser("John13", "john", "12345444", "AANNNNNN", salesmanId13, UserType.DistributorSalesman, groupId);
            //AddUser("John14", "john", "12345444", "AANNNNNN", salesmanId14, UserType.DistributorSalesman, groupId);
            //AddUser("John15", "john", "12345444", "AANNNNNN", salesmanId15, UserType.DistributorSalesman, groupId);
            //AddUser("John16", "john", "12345444", "AANNNNNN", salesmanId16, UserType.DistributorSalesman, groupId);
            //AddUser("John17", "john", "12345444", "AANNNNNN", salesmanId17, UserType.DistributorSalesman, groupId);
            //AddUser("John18", "john", "12345444", "AANNNNNN", salesmanId18, UserType.DistributorSalesman, groupId);
            //AddUser("John19", "john", "12345444", "AANNNNNN", salesmanId19, UserType.DistributorSalesman, groupId);
            //AddUser("Mary", "1234", "12345444", "AANNNNNN", salesmanId20, UserType.DistributorSalesman, groupId);

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
            Guid district = AddDistrict(province, "DAGORETTI");
            //Add Target Period
            Guid targetPeriod = AddTargetPeriod("SALES TARGET", DateTime.Now, DateTime.Now);
            //Add Competitor
            /*Guid competitor = AddCompetitor("QUENCHER", "INDUSTRIAL AREA", "788-NAIROBI", "0721345678", "MATENDECHERE", "NAIROBI", "675", "234");
            //Add Competitor Product
            Guid competitorProduct = AddCompetitorProduct("JUICE", "HIGHLANDS", competitor, pb1, pp1, pt1, ppt1, pf1);*/
            //Add Reorder Level
            Guid reorderLevel = AddReorderLevel(distrib,saleProduct,21);
           
            //Add Target
            Guid target = AddTarget(distrib, saleProduct, targetPeriod, 100, true);
            //Add Product Discount
            Guid prodDisc = AddProductDiscount(saleProduct, ppTier, 1, effectiveDate, endDate);
            //Add Sale Value Discount
            Guid saleValDisc = AddSaleValueDiscount(ppTier, 2, 18000, effectiveDate, endDate);
        }
    }
}
