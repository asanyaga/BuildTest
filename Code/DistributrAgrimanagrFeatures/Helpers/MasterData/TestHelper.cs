using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.MasterData;
using StructureMap;

namespace DistributrAgrimanagrFeatures.Helpers.MasterData
{
    public class TestHelper
    {
        public T Ioc<T>() where T : class
        {
            return ObjectFactory.GetInstance<T>();
        }

        public Guid AddSupplier()
        {
            Supplier supplier = new Supplier()
            {
                Id = Guid.NewGuid(),
                _DateCreated = DateTime.Now,
                _DateLastUpdated = DateTime.Now,
                _Status = EntityStatus.Active,
                Code = "sp1New",
                Description = "Description",
                Name = "SupplierUnitTest"
            };
            Guid supplierId = Ioc<ISupplierRepository>().Save(supplier);
            return supplierId;
        }

        public QueryStandard QueryStandard(string name, Guid supplierId, int? userType)
        {
            var queryStandard = new QueryStandard()
            {
                Name = name,
                ShowInactive = false,
                Skip = 0,
                Take = 10,
                SupplierId = supplierId,
                UserType = userType
            };
            return queryStandard;
        }


        public Country BuildCountry()
        {
            var id = Guid.NewGuid();
            var country = new Country(id)
            {
                _Status = EntityStatus.Active,
                Code = "Code 1",
                Currency = "Ksh.",
                Name = "Country Test"
            };
            return country;
        }

        public Bank BuildBank()
        {
            var bank = new Bank();
            bank.Id = Guid.NewGuid();
            bank._DateCreated = DateTime.Now;
            bank._DateLastUpdated = DateTime.Now;
            bank._Status = EntityStatus.Active;
            bank.Code = "001";
            bank.Description = "Bank 001";
            bank.Name = "Bank 001";
            return bank;
        }

        public Guid AddBankBranch(Guid id, string code, string description, string bankBranchName, EntityStatus status,
            Bank bank)
        {

            var bankBranch = new BankBranch();
            if (id == Guid.Empty)
            {
                bankBranch.Id = Guid.NewGuid();

            }
            else
            {
                bankBranch.Id = id;
            }
            bankBranch._DateCreated = DateTime.Now;
            bankBranch._DateLastUpdated = DateTime.Now;
            bankBranch._Status = status;
            bankBranch.Code = code;
            bankBranch.Description = description;
            bankBranch.Bank = bank;
            bankBranch.Name = bankBranchName;

            Guid bankBranchId = Ioc<IBankBranchRepository>().Save(bankBranch);
            return bankBranchId;
        }

        public Guid AddProduct(Guid id, string code, string description, EntityStatus status, decimal exFactoryPrice,
            ReturnableType returnableType, Product returnableProduct, string prodbrandDescription, string prodbrandCode,
            string prodbrandName,
            string prodFlavourDesc, string prodFlavourCode, string prodFlavourName, string prodPackagingTypeName,
            string prodPackagingTypeCode,
            string prodPackagingTypeDesc, string productPackagingName, string productPackagingCode,
            string productPackagingDesc,
            string productTypeCode, string productTypeDesc, string productTypeName, string vatClassName, string vatClass)
        {
            if (id == Guid.Empty)
            {
                var supplierId = AddSupplier();
                var supplier = Ioc<ISupplierRepository>().GetById(supplierId);
                var productBrand = new ProductBrand()
                {
                    Id = Guid.NewGuid(),
                    _DateCreated = DateTime.Now,
                    _DateLastUpdated = DateTime.Now,
                    _Status = EntityStatus.Active,
                    Description = prodbrandDescription,
                    Code = prodbrandCode,
                    Name = prodbrandName,
                    Supplier = supplier
                };

                var productFlavour = new ProductFlavour()
                {
                    Id = Guid.NewGuid(),
                    _DateCreated = DateTime.Now,
                    _DateLastUpdated = DateTime.Now,
                    _Status = EntityStatus.Active,
                    Description = prodFlavourDesc,
                    Code = prodFlavourCode,
                    Name = prodFlavourName,
                    ProductBrand = productBrand
                };

                var productPackagingType = new ProductPackagingType()
                {
                    Id = Guid.NewGuid(),
                    _DateCreated = DateTime.Now,
                    _DateLastUpdated = DateTime.Now,
                    _Status = EntityStatus.Active,
                    Description = prodPackagingTypeDesc,
                    Code = prodPackagingTypeCode,
                    Name = prodPackagingTypeName
                };

                var productPackaging = new ProductPackaging()
                {
                    Id = Guid.NewGuid(),
                    _DateCreated = DateTime.Now,
                    _DateLastUpdated = DateTime.Now,
                    _Status = EntityStatus.Active,
                    Description = productPackagingDesc,
                    Code = productPackagingCode,
                    Name = productPackagingName,

                };

                var productType = new ProductType()
                {
                    Id = Guid.NewGuid(),
                    _DateCreated = DateTime.Now,
                    _DateLastUpdated = DateTime.Now,
                    _Status = EntityStatus.Active,
                    Code = productTypeCode,
                    Description = productTypeDesc,
                    Name = productTypeName
                };

                Ioc<IProductBrandRepository>().Save(productBrand);
                Ioc<IProductFlavourRepository>().Save(productFlavour);
                Ioc<IProductPackagingTypeRepository>().Save(productPackagingType);
                Ioc<IProductPackagingRepository>().Save(productPackaging);
                Guid productTypeId = Ioc<IProductTypeRepository>().Save(productType);
                ProductType _productType = Ioc<IProductTypeRepository>().GetById(productTypeId);
                VATClass newVatClass = Ioc<IVATClassFactory>().CreateVATClass(vatClassName, vatClass, 2, DateTime.Now);
                Guid vatClassId = Ioc<IVATClassRepository>().Save(newVatClass);
                VATClass _vatClass = Ioc<IVATClassRepository>().GetById(vatClassId);


                if (returnableType == ReturnableType.Returnable)
                {
                    ReturnableProduct newReturnableProduct =
                        Ioc<IProductFactory>().CreateReturnableProduct(Guid.NewGuid());
                    newReturnableProduct._Status = status;
                    newReturnableProduct.Description = description;
                    newReturnableProduct.Brand = productBrand;
                    newReturnableProduct.ProductCode = code;
                    newReturnableProduct.ExFactoryPrice = exFactoryPrice;
                    newReturnableProduct.ReturnableType = returnableType;

                    newReturnableProduct.Flavour = productFlavour;
                    newReturnableProduct.Packaging = productPackaging;
                    newReturnableProduct.PackagingType = productPackagingType;
                    newReturnableProduct.VATClass = _vatClass;

                    //if (returnableProduct.Id != Guid.Empty)
                    // {
                    newReturnableProduct.ReturnAbleProduct = (ReturnableProduct)returnableProduct;
                    // }

                    Guid returnableproductId = Ioc<IProductRepository>().Save(newReturnableProduct);


                    return returnableproductId;
                }
                SaleProduct newSaleProduct = Ioc<IProductFactory>().CreateSaleProduct(Guid.NewGuid());
                newSaleProduct._Status = status;
                newSaleProduct.Description = description;
                newSaleProduct.Brand = productBrand;
                newSaleProduct.ProductCode = code;
                newSaleProduct.ExFactoryPrice = exFactoryPrice;
                newSaleProduct.ReturnableType = returnableType;

                newSaleProduct.Flavour = productFlavour;
                newSaleProduct.Packaging = productPackaging;
                newSaleProduct.PackagingType = productPackagingType;
                newSaleProduct.ProductType = _productType;
                newSaleProduct.VATClass = _vatClass;

                Guid productId = Ioc<IProductRepository>().Save(newSaleProduct);
                return productId;
            }
            else
            {
                Product saleProduct = Ioc<IProductRepository>().GetById(id);

                saleProduct._Status = status;
                saleProduct.Description = description;
                saleProduct.ProductCode = code;
                saleProduct.ExFactoryPrice = exFactoryPrice;
                saleProduct.ReturnableType = returnableType;

                Guid productId = Ioc<IProductRepository>().Save(saleProduct);
                return productId;
            }
        }

        public Commodity BuildCommodity()
        {
            var commodityType = BuildCommodityType();
            var toSaveCommodityType = Ioc<ICommodityTypeRepository>().Save(commodityType);
            var savedCommodityType = Ioc<ICommodityTypeRepository>().GetById(toSaveCommodityType);
            var id = Guid.NewGuid();
            var commodity = new Commodity(id)
            {
                Code = "Code 1",
                Name = "Commodity 1",
                Description = "Commodity 1",
                CommodityType = savedCommodityType,
                _Status = EntityStatus.Active
            };
            return commodity;
        }

        public CommodityType BuildCommodityType()
        {
            var id = Guid.NewGuid();
            var commodityType = new CommodityType(id)
            {
                Code = "Code 1",
                Name = "Commodity Type 1",
                Description = "Commodity Type 1",
                _Status = EntityStatus.Active
            };
            return commodityType;
        }

        public ProductBrand BuilProductBrand()
        {
            var supplierId = AddSupplier();
            var supplier = Ioc<ISupplierRepository>().GetById(supplierId);
            var productBrand = new ProductBrand();

            productBrand.Id = Guid.NewGuid();
            productBrand._DateCreated = DateTime.Now;
            productBrand._DateLastUpdated = DateTime.Now;
            productBrand._Status = EntityStatus.Active;
            productBrand.Description = "Milk";
            productBrand.Code = "Milk 110";
            productBrand.Name = "Milk 110";
            productBrand.Supplier = supplier;

            return productBrand;
        }

        public ProductFlavour BuildProductFlavour()
        {
            ProductBrand productBrand = BuilProductBrand();
            var productFlavour = new ProductFlavour();
            productFlavour.Id = Guid.NewGuid();
            productFlavour._DateCreated = DateTime.Now;
            productFlavour._DateLastUpdated = DateTime.Now;
            productFlavour._Status = EntityStatus.Active;
            productFlavour.Description = "UHT 001";
            productFlavour.Code = " 001";
            productFlavour.Name = "UHT 001";
            productFlavour.ProductBrand = productBrand;

            return productFlavour;
        }

        public ProductPackagingType BuildProductPackagingType()
        {
            var productPackagingType = new ProductPackagingType();
            productPackagingType.Id = Guid.NewGuid();
            productPackagingType._DateCreated = DateTime.Now;
            productPackagingType._DateLastUpdated = DateTime.Now;
            productPackagingType._Status = EntityStatus.Active;
            productPackagingType.Description = "Tetra";
            productPackagingType.Code = "Tetra code";
            productPackagingType.Name = "Tetre";

            return productPackagingType;
        }

        public ProductPackaging BuildProductPackaging()
        {
            var productPackaging = new ProductPackaging();

            productPackaging.Id = Guid.NewGuid();
            productPackaging._DateCreated = DateTime.Now;
            productPackaging._DateLastUpdated = DateTime.Now;
            productPackaging._Status = EntityStatus.Active;
            productPackaging.Description = "Pack1";
            productPackaging.Code = "Code001";
            productPackaging.Name = "Pack1";

            return productPackaging;
        }

        public ProductType BuildProductType()
        {
            var productType = new ProductType();

            productType.Id = Guid.NewGuid();
            productType._DateCreated = DateTime.Now;
            productType._DateLastUpdated = DateTime.Now;
            productType._Status = EntityStatus.Active;
            productType.Code = "Code 001";
            productType.Description = "Tea001";
            productType.Name = "Tea001";

            return productType;
        }


        public ProductPricingTier BuildProductPricingTier()
        {
            ProductPricingTier productPricingTier = new ProductPricingTier()
            {
                Id = Guid.NewGuid(),
                _DateCreated = DateTime.Now,
                _DateLastUpdated = DateTime.Now,
                _Status = EntityStatus.Active,
                Code = "ppTierCode001",
                Description = "Tier0001",
                Name = "Tier0001"
            };
            return productPricingTier;
        }

        public ProductDiscount BuildProductDiscount()
        {

            Guid productId = AddProduct(Guid.Empty, "10013", "Soda3", EntityStatus.Active, 25, ReturnableType.GenericReturnable, null,
                   "productBrandDesc3", "productBrandCode3", "prodBrandName3", "prodFlavour3", "prodFlavourCode3",
                   "prodFlavourName3", "prodPackagingTypeName3", "prodPackagingTypeCode3", "prodPackagingTypeDesc3", "prodPackagingName3", "prodPackagingCode3", "prodPackagingDesc3", "productTypeCode3", "productTypeDesc3", "productTypeName3", "vatClassName3", "vatClass3");

            ProductPricingTier productPricingTier = BuildProductPricingTier();
            Guid productPricingTierId = Ioc<IProductPricingTierRepository>().Save(productPricingTier);

            DateTime date = DateTime.Now;
            DateTime enddate = date.AddDays(2);
            ProductDiscount discount = Ioc<IProductDiscountFactory>().CreateProductDiscount(productId, productPricingTierId, 10, date, enddate, false, 0);

            return discount;

        }


        public ProductPricing BuildProductPricing()
        {
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35, ReturnableType.GenericReturnable, null,
             "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
             "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004", "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");

            ProductPricingTier productPricingTier = BuildProductPricingTier();
            Guid productPricingTierId = Ioc<IProductPricingTierRepository>().Save(productPricingTier);

            DateTime date = DateTime.Now;
            date = date.AddDays(-2);
            ProductPricing price = Ioc<IProductPricingFactory>().CreateProductPricing(productId, productPricingTierId, 10, 10, date);
            return price;
        }

        public Supplier BuildSupplier()
        {
            var id = Guid.NewGuid();
            var supplier = new Supplier(id)
            {
                _Status = EntityStatus.Active,
                Code = "Code 1",
                Description = "Supplier 1",
                Name = "Supplier 1"
            };

            return supplier;
        }

        public CommodityOwner BuildCommodityOwner()
        {
            var supplier = BuildCommoditySupplierCC();
            var toSaveSupplier = Ioc<ICommoditySupplierRepository>().Save(supplier);
            var savedSupplier = Ioc<ICommoditySupplierRepository>().GetById(toSaveSupplier) as CommoditySupplier;

            var commodityOwnerType = BuildCommodityOwnerType();
            var toSaveCommodityOwnerType = Ioc<ICommodityOwnerTypeRepository>().Save(commodityOwnerType);
            var savedCommodityOwnerType = Ioc<ICommodityOwnerTypeRepository>().GetById(toSaveCommodityOwnerType);

            var id = Guid.NewGuid();
            var commodityOwner = new CommodityOwner(id)
            {
                Code = "Code 1",
                Surname = " Commodity Owner 1",
                FirstName = " Commodity Owner 1",
                LastName = " Commodity Owner 1",
                IdNo = "12345678",
                PinNo = "12345678",
                DateOfBirth = DateTime.Now.AddYears(-20),
                MaritalStatus = MaritalStatas.Single,
                Gender = Gender.Male,
                PhysicalAddress = "Address 1",
                PostalAddress = "Address 2",
                Email = "test@test.com",
                PhoneNumber = "0722000000",
                BusinessNumber = "ABC000",
                FaxNumber = "123456",
                OfficeNumber = "123456789",
                Description = "Commodity Owner 1",
                CommodityOwnerType = savedCommodityOwnerType,
                CommoditySupplier = savedSupplier
            };
            return commodityOwner;
        }

        private CommoditySupplier BuildCommoditySupplierCC()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var hub = BuildHub(savedRegion);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);

            var commoditySuppliercc = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.CommoditySupplier, savedHub);

            var bankBranch = BuildBankBranch();
            var toSaveBankBranch = Ioc<IBankBranchRepository>().Save(bankBranch);
            var savedBankBranch = Ioc<IBankBranchRepository>().GetById(toSaveBankBranch);

            var commoditySupplier = commoditySuppliercc as CommoditySupplier;
            commoditySupplier.Name = "Commodity Supplier 1";
            commoditySupplier.CostCentreCode = "Code 1";
            commoditySupplier.AccountNo = "12345678";
            commoditySupplier.BankBranchId = savedBankBranch.Id;
            commoditySupplier.BankId = savedBankBranch.Bank.Id;
            commoditySupplier.CommoditySupplierType = CommoditySupplierType.Individual;
            commoditySupplier.JoinDate = DateTime.Now;
            commoditySupplier.PinNo = "12345678";
            commoditySupplier.AccountNo = "12345678";
            commoditySupplier.AccountName = "Account 1";

            return commoditySupplier;
        }

        public BankBranch BuildBankBranch()
        {
            var bank = BuildBank();
            var toSaveBank = Ioc<IBankRepository>().Save(bank);
            var savedBank = Ioc<IBankRepository>().GetById(toSaveBank);

            var id = Guid.NewGuid();
            var bankBranch = new BankBranch(id)
            {
                Name = "Bank Branch 1",
                Code = "Code 1",
                Description = "Bank Branch 1",
                Bank = savedBank
            };

            return bankBranch;

        }

        public CommodityOwnerType BuildCommodityOwnerType()
        {
            var id = Guid.NewGuid();
            var commodityOwnerType = new CommodityOwnerType(id)
            {
                Code = "Code 1",
                Name = "Commodity Owner Type 1",
                Description = "Commodity Owner Type 1",
                _Status = EntityStatus.Active
            };
            return commodityOwnerType;
        }

        public CommoditySupplier BuildCommositySupplier()
        {
            var id = Guid.NewGuid();
            var commoditySupplier = new CommoditySupplier(id)
            {
                AccountName = "Account 1",
                Name = "Commodity Supplier 1",

            };
            return commoditySupplier;
        }

        public OutletVisitReasonsType BuildOutletVisitReasonsType()
        {
            var id = Guid.NewGuid();
            var outletVisitReasonType = new OutletVisitReasonsType(id)
            {
                Name = "Outlet Visit Reason Type 1",
                Description = "Outlet Visit Reason Type 1",
                OutletVisitAction = OutletVisitAction.NoAction,
                _Status = EntityStatus.Active
            };

            return outletVisitReasonType;
        }

        public OutletCategory BuildOutletCartegory()
        {
            var id = Guid.NewGuid();
            var outletCartegory = new OutletCategory(id)
            {
                Name = "Outlet Cartegory 1",
                _Status = EntityStatus.Active
            };
            return outletCartegory;
        }

        public OutletType BuildOutletType()
        {
            var id = Guid.NewGuid();
            var outletType = new OutletType(id)
            {
                Name = "Outlet Type 1",
                Code = "Code 1",
                _Status = EntityStatus.Active
            };
            return outletType;
        }

        public AssetStatus BuildAssetStatus()
        {
            var id = Guid.NewGuid();
            var assetStatus = new AssetStatus(id)
            {
                Name = "Asset Status 1",
                Description = "Asset Status 1",
                _Status = EntityStatus.Active
            };
            return assetStatus;
        }

        public AssetType BuildAssetType()
        {
            var id = Guid.NewGuid();
            var assetType = new AssetType(id)
            {
                Name = "Asset Type 1",
                Description = "Asset Type 1",
                _Status = EntityStatus.Active
            };
            return assetType;
        }

        public AssetCategory BuildAssetCategory()
        {
            var assetType = BuildAssetType();
            var toSaveAssetType = Ioc<IAssetTypeRepository>().Save(assetType);
            var savedAssetType = Ioc<IAssetTypeRepository>().GetById(toSaveAssetType);
            var id = Guid.NewGuid();
            var assetCategory = new AssetCategory(id)
            {
                Name = "Asset Cartegory 1",
                Description = "Asset Cartegory 1",
                _Status = EntityStatus.Active,
                AssetType = savedAssetType
            };
            return assetCategory;
        }

        public Asset BuildAsset()
        {
            var assetStatus = BuildAssetStatus();
            var toSaveAssetStatus = Ioc<IAssetStatusRepository>().Save(assetStatus);
            var savedAssetStatus = Ioc<IAssetStatusRepository>().GetById(toSaveAssetStatus);

            var assetCartegory = BuildAssetCategory();
            var toSaveAssetCartegory = Ioc<IAssetCategoryRepository>().Save(assetCartegory);
            var savedAssetCartegory = Ioc<IAssetCategoryRepository>().GetById(toSaveAssetCartegory);

            var id = Guid.NewGuid();
            var asset = new Asset(id)
            {
                Code = "Code 1",
                Name = "Asset Cartegory 1",
                Capacity = "One",
                SerialNo = "123456789",
                AssetNo = "123456789",
                _Status = EntityStatus.Active,
                AssetType = savedAssetCartegory.AssetType,
                AssetStatus = savedAssetStatus,
                AssetCategory = savedAssetCartegory
            };
            return asset;
        }

        public DiscountGroup BuildDiscountGroup()
        {
            DiscountGroup discountGroup = new DiscountGroup(Guid.NewGuid());
            discountGroup.Name = "GroupDisc A";
            discountGroup.Code = "GD_001";
            discountGroup._Status = EntityStatus.Active;
            return discountGroup;
        }

        public ProductGroupDiscount BuildProductDiscountGroup()
        {
            DiscountGroup discountGroup = BuildDiscountGroup();
            Guid discountGroupId = Ioc<IDiscountGroupRepository>().Save(discountGroup);
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35, ReturnableType.GenericReturnable, null,
             "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
             "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004", "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");

            DateTime date = DateTime.Now;
            date = date.AddDays(-2);
            ProductGroupDiscount productGroupDiscount =
                Ioc<IProductDiscountGroupFactory>().CreateProductGroupDiscount(
                    Ioc<IDiscountGroupRepository>().GetById(discountGroupId), new ProductRef { ProductId = productId }, 14,
                    DateTime.Now, DateTime.Now.AddDays(2), false, 0);
            productGroupDiscount._Status = EntityStatus.Active;
            return productGroupDiscount;
        }

        public FreeOfChargeDiscount BuildFreeOfChargeDiscount()
        {
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35, ReturnableType.GenericReturnable, null,
             "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
             "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004", "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");
            FreeOfChargeDiscount foc = new FreeOfChargeDiscount(Guid.NewGuid())
            {
                isChecked = true,
                ProductRef = new ProductRef { ProductId = productId }
            };
            foc.StartDate = DateTime.Now;
            foc.EndDate = foc.StartDate.AddDays(2);
            foc._Status = EntityStatus.Active;

            return foc;
        }

        public CustomerDiscount BuildCustomerDiscount()
        {
            Outlet outlet = BuildOutlet();
            Guid outletId = Ioc<IOutletRepository>().Save(outlet);
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35,
                ReturnableType.GenericReturnable, null,
                "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
                "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004",
                "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");

            CustomerDiscount custDiscount =
                Ioc<ICustomerDiscountFactory>().CreateCustomerDiscount(new CostCentreRef { Id = outletId },
                    new ProductRef { ProductId = productId }, 14,
                    DateTime.Now);
            custDiscount._Status = EntityStatus.Active;
            return custDiscount;
        }

        public Territory BuildTerritory()
        {
            var id = Guid.NewGuid();
            var territory = new Territory(id)
            {
                Name = "Territory 1",
                _Status = EntityStatus.Active
            };
            return territory;
        }

        public Province BuildProvince()
        {
            var country = BuildCountry();
            var toSaveCountry = Ioc<ICountryRepository>().Save(country);
            var savedCountry = Ioc<ICountryRepository>().GetById(toSaveCountry);

            var id = Guid.NewGuid();
            var territory = new Province(id)
            {
                Name = "Province 1",
                Description = "Province 1",
                _Status = EntityStatus.Active,
                Country = savedCountry
            };
            return territory;
        }

        public District BuildDistrict()
        {
            var province = BuildProvince();
            var toSaveProvince = Ioc<IProvincesRepository>().Save(province);
            var savedProvince = Ioc<IProvincesRepository>().GetById(toSaveProvince);

            var id = Guid.NewGuid();
            var district = new District(id)
            {
                DistrictName = "District 1",
                _Status = EntityStatus.Active,
                Province = savedProvince
            };
            return district;
        }

        public Region BuildRegion()
        {
            var country = BuildCountry();
            var toSaveCountry = Ioc<ICountryRepository>().Save(country);
            var savedCountry = Ioc<ICountryRepository>().GetById(toSaveCountry);

            var id = Guid.NewGuid();
            var district = new Region(id)
            {
                Name = "Region 1",
                Description = "Region 1",
                _Status = EntityStatus.Active,
                Country = savedCountry
            };
            return district;
        }

        public ContactType BuildContactType()
        {
            var id = Guid.NewGuid();
            var contactType = new ContactType(id)
            {
                Code = "Code 1",
                Name = "Contact type 1",
                Description = "Contact type 1",
                _Status = EntityStatus.Active,
            };
            return contactType;
        }

        public Competitor BuildCompetitor()
        {
            var id = Guid.NewGuid();
            var competitor = new Competitor(id)
            {
                Name = "Competitor 1",
                ContactPerson = "Competitor 1",
                City = "City 1",
                Lattitude = "Latitude 1",
                Longitude = "Longitude 1",
                PhysicalAddress = "Physical Address 1",
                PostalAddress = "Postal Address 1",
                Telephone = "Telephone 1",
                _Status = EntityStatus.Active,
            };
            return competitor;
        }

        public TargetPeriod BuildTargetPeriod()
        {
            var id = Guid.NewGuid();
            var competitor = new TargetPeriod(id)
            {
                Name = "Target Period 1",
                StartDate = DateTime.Now.AddMonths(7),
                EndDate = DateTime.Now.AddMonths(8),
                _Status = EntityStatus.Active,
            };
            return competitor;
        }

        public Outlet BuildOutlet()
        {
            var route = BuildRoute();
            var toSaveRoute = Ioc<IRouteRepository>().Save(route);
            var savedRoute = Ioc<IRouteRepository>().GetById(toSaveRoute);

            var distributorCC = BuildDistributorCC(route.Region);
            var toSaveDistributorCC = Ioc<ICostCentreRepository>().Save(distributorCC);
            var savedDistributorCC = Ioc<ICostCentreRepository>().GetById(toSaveDistributorCC);

            var vatClass = BuildVatClass();
            var toSaveVatClass = Ioc<IVATClassRepository>().Save(vatClass);
            var savedVatClass = Ioc<IVATClassRepository>().GetById(toSaveVatClass);

            var productPricingTier = BuildProductPricingTier();
            var toSaveProductPricingTier = Ioc<IProductPricingTierRepository>().Save(productPricingTier);
            var savedProductPricingTier = Ioc<IProductPricingTierRepository>().GetById(toSaveProductPricingTier);

            var outletCartegory = BuildOutletCartegory();
            var toSaveOutletCartegory = Ioc<IOutletCategoryRepository>().Save(outletCartegory);
            var savedOutletCartegory = Ioc<IOutletCategoryRepository>().GetById(toSaveOutletCartegory);

            var outletType = BuildOutletType();
            var toSaveOutletType = Ioc<IOutletTypeRepository>().Save(outletType);
            var savedOutletType = Ioc<IOutletTypeRepository>().GetById(toSaveOutletType);

            var outletCC = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, savedDistributorCC);

            var outlet = outletCC as Outlet;
            outlet.Name = "Outlet 1";
            outlet.OutletCategory = savedOutletCartegory;
            outlet.OutletType = savedOutletType;
            outlet.Route = savedRoute;
            outlet.VatClass = savedVatClass;
            outlet.OutletProductPricingTier = savedProductPricingTier;
            outlet.CostCentreCode = "Code 1";
            outlet._Status = EntityStatus.Active;

            return outlet;
        }

        private VATClass BuildVatClass()
        {
            var vatClass = Ioc<IVATClassFactory>().CreateVATClass("Vat Test 1", "Class Test 1", 2, DateTime.Now.AddDays(-1));

            return vatClass;
        }

        public Distributor BuildDistributorCC(Region region)
        {
            var savedProducer = BuildProducer();

            var salesRep = BuildUserSalesRep(savedProducer.Id);
            var surveyor = BuildUserSurveyor(savedProducer.Id);
            surveyor.Group = salesRep.Group;
            var asm = BuildUserASM(savedProducer.Id);
            asm.Group = salesRep.Group;

            var sw = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, savedProducer) as StandardWarehouse;

            var distributor = sw as Distributor;
            distributor.Name = "Distributor 1";
            distributor.Owner = "Distributor Owner 1";
            distributor.PIN = "12345678";
            distributor.AccountNo = "12345678";
            distributor.SalesRep = salesRep;
            distributor.Surveyor = surveyor;
            distributor.ASM = asm;
            distributor.VatRegistrationNo = "12345678";
            distributor.PaybillNumber = "12345678";
            distributor.MerchantNumber = "12345678";
            distributor.CostCentreCode = "Code 1";
            distributor.Region = region;
            distributor._Status = EntityStatus.Active;

            return distributor;
        }

        public Contact BuildContact()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var distributorCC = BuildDistributorCC(savedRegion);
            var toSaveDistributorCC = Ioc<ICostCentreRepository>().Save(distributorCC);
            var savedDistributorCC = Ioc<ICostCentreRepository>().GetById(toSaveDistributorCC);

            var contactType = BuildContactType();
            var toSaveContactType = Ioc<IContactTypeRepository>().Save(contactType);
            var savedContactType = Ioc<IContactTypeRepository>().GetById(toSaveContactType);

            var id = Guid.NewGuid();
            var contact = new Contact(id)
            {
                PhysicalAddress = "Physical Address 1",
                ContactOwnerType = ContactOwnerType.Distributor,
                ContactOwnerMasterId = savedDistributorCC.Id,
                City = "City 1",
                PostalAddress = "Postal Address 1",
                Firstname = "First Name 1",
                MStatus = MaritalStatas.Unknown,
                ContactType = savedContactType,
                MobilePhone = "12345678",
                ContactClassification = ContactClassification.PrimaryContact,
                BusinessPhone = "12345678",
                Fax = "12345678",
                _Status = EntityStatus.Active
            };

            return contact;
        }

        private User BuildUserSalesRep(Guid costCentreId)
        {
            var userGroup = BuildUserGroup();
            var toSaveUserGroup = Ioc<IUserGroupRepository>().Save(userGroup);
            var savedUserGroup = Ioc<IUserGroupRepository>().GetById(toSaveUserGroup);

            var id = Guid.NewGuid();
            var salesRep = new User(id)
            {
                Username = "SalesRepUser",
                Password = "12345678",
                CostCentre = costCentreId,
                UserType = UserType.Surveyor,
                Mobile = "0722000000",
                Group = savedUserGroup
            };

            return salesRep;
        }

        private User BuildUserSurveyor(Guid costCentreId)
        {
            /*var userGroup = BuildUserGroup();
            var toSaveUserGroup = Ioc<IUserGroupRepository>().Save(userGroup);
            var savedUserGroup = Ioc<IUserGroupRepository>().GetById(toSaveUserGroup);*/

            var id = Guid.NewGuid();
            var salesRep = new User(id)
            {
                Username = "SurveyorUser",
                Password = "12345678",
                CostCentre = costCentreId,
                UserType = UserType.Surveyor,
                Mobile = "0722000000",
                //Group = savedUserGroup
            };

            return salesRep;
        }

        private User BuildUserASM(Guid costCentreId)
        {
            /*var userGroup = BuildUserGroup();
            var toSaveUserGroup = Ioc<IUserGroupRepository>().Save(userGroup);
            var savedUserGroup = Ioc<IUserGroupRepository>().GetById(toSaveUserGroup);*/

            var id = Guid.NewGuid();
            var salesRep = new User(id)
            {
                Username = "ASMUser",
                Password = "12345678",
                CostCentre = costCentreId,
                UserType = UserType.ASM,
                Mobile = "0722000000",
                //Group = savedUserGroup
            };

            return salesRep;
        }

        private UserGroup BuildUserGroup()
        {
            var id = Guid.NewGuid();
            var userGroup = new UserGroup(id)
            {
                Name = "User Group 1",
                Descripition = "User Group 1",
                _Status = EntityStatus.Active
            };
            return userGroup;
        }

        private CostCentre BuildProducer()
        {
            var p = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null) as StandardWarehouse;
            p.Name = "Producer 1";
            p.CostCentreCode = "Producer 1";

            var toSaveProducer = Ioc<ICostCentreRepository>().Save(p);
            var savedProducer = Ioc<ICostCentreRepository>().GetById(toSaveProducer);

            return savedProducer;
        }

        public Route BuildRoute()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var id = Guid.NewGuid();
            var route = new Route(id)
            {
                Code = "Code 1",
                Name = "Route 1",
                Region = savedRegion,
                _Status = EntityStatus.Active
            };

            return route;
        }

        public Target BuildTarget()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var distributorCC = BuildDistributorCC(savedRegion);
            var toSaveDistributorCC = Ioc<ICostCentreRepository>().Save(distributorCC);
            var savedDistributorCC = Ioc<ICostCentreRepository>().GetById(toSaveDistributorCC);

            var targetPeriod = BuildTargetPeriod();
            var toSaveTargetPeriod = Ioc<ITargetPeriodRepository>().Save(targetPeriod);
            var savedTargetPeriod = Ioc<ITargetPeriodRepository>().GetById(toSaveTargetPeriod);


            var id = Guid.NewGuid();
            var target = new Target(id)
            {
                CostCentre = savedDistributorCC,
                TargetPeriod = savedTargetPeriod,
                IsQuantityTarget = true,
                TargetValue = 5,
                _Status = EntityStatus.Active
            };

            return target;
        }

        public CertainValueCertainProductDiscount BuildCertainValueCertainProductDiscount()
        {
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35, ReturnableType.GenericReturnable, null,
             "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
             "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004", "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");

            DateTime effDate = DateTime.Now.AddDays(2);
            DateTime endDate = effDate.AddDays(4);
            CertainValueCertainProductDiscount cvcp =
                Ioc<ICertainValueCertainProductDiscountFactory>().CreateCertainValueCertainProductDiscount(
                    new ProductRef { ProductId = productId }, 17, 500, effDate, endDate);
            cvcp._Status = EntityStatus.Active;
            return cvcp;
        }

        public PromotionDiscount BuildPromotionDiscount()
        {
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35, ReturnableType.GenericReturnable, null,
            "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
            "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004", "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");

            Guid freeofChargeProduct = AddProduct(Guid.Empty, "10013", "Soda3", EntityStatus.Active, 25, ReturnableType.GenericReturnable, null,
                "productBrandDesc3", "productBrandCode3", "prodBrandName3", "prodFlavour3", "prodFlavourCode3",
                "prodFlavourName3", "prodPackagingTypeName3", "prodPackagingTypeCode3", "prodPackagingTypeDesc3", "prodPackagingName3", "prodPackagingCode3", "prodPackagingDesc3", "productTypeCode3", "productTypeDesc3", "productTypeName3", "vatClassName3", "vatClass3");

            DateTime effDate = DateTime.Now.AddDays(2);
            DateTime endDate = effDate.AddDays(4);
            PromotionDiscount foc =
                Ioc<IPromotionDiscountFactory>().CreateFreeOfChargeDiscount(new ProductRef { ProductId = productId },
                                                                            freeofChargeProduct, 1, 1, effDate, 12, endDate);
            foc._Status = EntityStatus.Active;
            return foc;
        }

        public TargetItem BuildTargetItem()
        {
            Guid productId = AddProduct(Guid.Empty, "10", "SodaBig", EntityStatus.Active, 35, ReturnableType.GenericReturnable, null,
             "Fanta Large", "099", "Fanta Large", "Fanta Orange l", "orange001",
             "Fanta Orange ", "Bottle3", "code003", "Bottle Big", "Bottle Big", "Code003", "packaging004", "Soft drink005", "Soft drink large", "Soft drink L", "vatClassName4", "vatClass4");

            var target = BuildTarget();
            var toSaveTarget = Ioc<ITargetRepository>().Save(target);
            var savedTarget = Ioc<ITargetRepository>().GetById(toSaveTarget);

            var id = Guid.NewGuid();
            var targetItem = new TargetItem(id)
            {
                Product = new ProductRef { ProductId = productId },
                Quantity = 20,
                Target = savedTarget,
                _Status = EntityStatus.Active
            };

            return targetItem;
        }

        public ChannelPackaging BuildChannelPack()
        {
            var productPackaging = BuildProductPackaging();
            var toSaveProductPackaging = Ioc<IProductPackagingRepository>().Save(productPackaging);
            var savedProductPackaging = Ioc<IProductPackagingRepository>().GetById(toSaveProductPackaging);

            var outletType = BuildOutletType();
            var toSaveOutletType = Ioc<IOutletTypeRepository>().Save(outletType);
            var savedOutletType = Ioc<IOutletTypeRepository>().GetById(toSaveOutletType);

            var id = Guid.NewGuid();
            var cPack = new ChannelPackaging(id)
            {
                IsChecked = true,
                OutletType = outletType,
                Packaging = savedProductPackaging,
                _Status = EntityStatus.Active
            };

            return cPack;
        }

        public CompetitorProducts BuildCompetitorProduct()
        {
            var competitor = BuildCompetitor();
            var toSaveCompetitor = Ioc<ICompetitorRepository>().Save(competitor);
            var savedCompetitor = Ioc<ICompetitorRepository>().GetById(toSaveCompetitor);

            var brand = BuilProductBrand();
            var toSaveBrand = Ioc<IProductBrandRepository>().Save(brand);
            var savedBrand = Ioc<IProductBrandRepository>().GetById(toSaveBrand);

            var flavour = BuildProductFlavour();
            var toSaveFlavour = Ioc<IProductFlavourRepository>().Save(flavour);
            var savedFlavour = Ioc<IProductFlavourRepository>().GetById(toSaveFlavour);

            var productPackaging = BuildProductPackaging();
            var toSaveProductPackaging = Ioc<IProductPackagingRepository>().Save(productPackaging);
            var savedProductPackaging = Ioc<IProductPackagingRepository>().GetById(toSaveProductPackaging);

            var packagingType = BuildProductPackagingType();
            var toSavePackagingType = Ioc<IProductPackagingTypeRepository>().Save(packagingType);
            var savedPackagingType = Ioc<IProductPackagingTypeRepository>().GetById(toSavePackagingType);

            var productType = BuildProductType();
            var toSaveProductType = Ioc<IProductTypeRepository>().Save(productType);
            var savedProductType = Ioc<IProductTypeRepository>().GetById(toSaveProductType);

            var id = Guid.NewGuid();
            var compeProduct = new CompetitorProducts(id)
            {
                Competitor = savedCompetitor,
                Brand = savedBrand,
                Flavour = savedFlavour,
                PackagingType = savedPackagingType,
                Packaging = savedProductPackaging,
                ProductType = savedProductType,
                ProductDescription = "Competitor Product 1",
                ProductName = "Competitor Product 1",
                _Status = EntityStatus.Active
            };

            return compeProduct;
        }

        public Producer BuildProducerCostCentre()
        {
            var p = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null) as StandardWarehouse;
            p.Name = "ProducerCC 1";
            p.CostCentreCode = "ProdCC001";
            return p as Producer;
        }

        public Transporter BuildTransporterCostCentre()
        {
            Producer p = BuildProducerCostCentre();
            Guid pId = Ioc<ICostCentreRepository>().Save(p);
            Transporter transporter =
                Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter,
                                                           Ioc<ICostCentreRepository>().GetById(pId)) as Transporter;
            transporter.Name = "TransporterName";
            transporter.CostCentreCode = "TCode001";
            return transporter;
        }


        public Distributor BuildDistributrCostCentre()
        {
            var p = BuildProducerCostCentre();
            var producerId = Ioc<ICostCentreRepository>().Save(p);

            /*var ct = BuildContactType();
            var contactType = Ioc<IContactTypeRepository>().Save(ct);*/

            var r = BuildRegion();
            var rId = Ioc<IRegionRepository>().Save(r);

            var salesRep = BuildUserSalesRep(producerId);
            var salesRepId = Ioc<IUserRepository>().Save(salesRep);

            var surveyor = BuildUserSurveyor(producerId);
            var surveyorId = Ioc<IUserRepository>().Save(surveyor);

            var asmUser = BuildUserASM(producerId);
            var asmUserId = Ioc<IUserRepository>().Save(asmUser);


            var sw = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor,
                                                           Ioc<ICostCentreRepository>().GetById(producerId)) as StandardWarehouse;

            var distributor = sw as Distributor;
            distributor.Name = "Distributor 1";
            distributor.Owner = "John";
            distributor.PIN = "A00911";
            distributor.AccountNo = "000999344";
            distributor.Region = Ioc<IRegionRepository>().GetById(rId);
            distributor.SalesRep = Ioc<IUserRepository>().GetById(salesRepId);
            distributor.Surveyor = Ioc<IUserRepository>().GetById(surveyorId);
            distributor.ASM = Ioc<IUserRepository>().GetById(asmUserId);
            distributor.VatRegistrationNo = "RegNo600A";
            distributor.PaybillNumber = "PayBill0001";
            distributor.MerchantNumber = "MerchNo001";
            distributor.CostCentreCode = "DistCode001";

            return distributor;
        }


        public CostCentre BuildDistributrSalesmanCostCentre()
        {
            Distributor distributor = BuildDistributrCostCentre();
            Guid parentId = Ioc<ICostCentreRepository>().Save(distributor);
            CostCentre cc = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(),
                                                                       CostCentreType.DistributorSalesman,
                                                                       Ioc<ICostCentreRepository>().GetById(parentId));
            cc.Name = "SalesmanCC";
            cc.CostCentreCode = "SalesmanCC_001";
            return cc;
        }

        public Hub BuildHub(Region region)
        {
            Producer p = BuildProducerCostCentre();
            Guid parentId = Ioc<ICostCentreRepository>().Save(p);

            CostCentre hubcc = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Hub,
                                                                             Ioc<ICostCentreRepository>().GetById(
                                                                                 parentId));
            Hub hub = hubcc as Hub;
            hub.Name = "TestHub001";
            hub.Region = region;
            hub.CostCentreCode = "HubCostCenterCode_001";
            return hub;
        }

        public ActivityType BuildActivityType()
        {
            var id = Guid.NewGuid();

            var activityType = new ActivityType(id)
            {
                Code = "Code 1",
                Name = "Activity Type 1",
                Description = "Activity Type 1",
                IsInfectionsRequired = true,
                IsInputRequired = true,
                IsServicesRequired = true,
                IsProduceRequired = true,
                _Status = EntityStatus.Active
            };

            return activityType;
        }

        public CentreType BuildCentreType()
        {
            var id = Guid.NewGuid();
            var centreType = new CentreType(id)
            {
                Name = "Centre Type 1",
                Description = "Centre Type 1",
                Code = "Code 1",
                _Status = EntityStatus.Active
            };

            return centreType;
        }

        public Centre BuildCentre()
        {
            var centreType = BuildCentreType();
            var toSaveCentreType = Ioc<ICentreTypeRepository>().Save(centreType);
            var savedCentreType = Ioc<ICentreTypeRepository>().GetById(toSaveCentreType);

            var route = BuildRoute();
            var toSaveRoute = Ioc<IRouteRepository>().Save(route);
            var savedRoute = Ioc<IRouteRepository>().GetById(toSaveRoute);

            var hub = BuildHub(route.Region);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);

            var id = Guid.NewGuid();
            var centre = new Centre(id)
            {
                Code = "Code 1",
                Name = "Centre 1",
                Description = "Centre 1",
                CenterType = savedCentreType,
                Route = savedRoute,
                Hub = savedHub as Hub,
                _Status = EntityStatus.Active
            };

            return centre;
        }

        public CostCentre BuildDistributorSalesmanWarehouse()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var parentCCId = BuildDistributorCC(savedRegion);
            var toSaveParentCCId = Ioc<ICostCentreRepository>().Save(parentCCId);
            var savedParentCCId = Ioc<ICostCentreRepository>().GetById(toSaveParentCCId);

            CostCentre cc = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(),
                                                                       CostCentreType.DistributorSalesman,
                                                                      savedParentCCId);
            cc.Name = "Distributor CC";
            cc.CostCentreCode = "Distributor CC";
            cc._Status = EntityStatus.Active;

            return cc;
        }


        public User BuildTestUserPurchasingClerk(Guid costcenterId, Guid groupId)
        {
            // UserGroup ugroup = BuildUserGroup();
            //  Guid groupId = Ioc<IUserGroupRepository>().Save(ugroup);
            UserGroup group = Ioc<IUserGroupRepository>().GetById(groupId);
            User user = new User(Guid.NewGuid())
            {
                Username = "Kimani",
                Password = "12345678",
                CostCentre = costcenterId,
                Mobile = "0745896532",
                UserType = UserType.PurchasingClerk,
                Group = group,
            };

            return user;
        }
        public PurchasingClerk BuildPurchasingClerk(Guid ccparentId, Guid userGroup)
        {
            var user = BuildTestUserPurchasingClerk(ccparentId, userGroup);
            Guid userId = Ioc<IUserRepository>().Save(user);
            var purchasingClerk = new PurchasingClerk(Guid.NewGuid())
            {
                Name = "Wambui",
                CostCentreCode = "pc_Code03",
                ParentCostCentre = Ioc<ICostCentreRepository>().GetById(ccparentId).ParentCostCentre,
                CostCentreType = CostCentreType.PurchasingClerk,
                User = Ioc<IUserRepository>().GetById(userId),
                _Status = EntityStatus.Active
            };
            return purchasingClerk;
        }

        public Store BuildStore(Guid ccparendId)
        {
            CostCentre storecc = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Store,
                Ioc<ICostCentreRepository>().GetById(
                    ccparendId));
            Store store = storecc as Store;
            store.Name = "MaizeStore";
            store.CostCentreCode = "MaizeStore001";
            return store;
        }

        public CommoditySupplier BuildCommoditySupplier()
        {
            var route = BuildRoute();
            var toSaveRoute = Ioc<IRouteRepository>().Save(route);
            var savedRoute = Ioc<IRouteRepository>().GetById(toSaveRoute);


            var hub = BuildHub(savedRoute.Region);
            Guid parentId = Ioc<IHubRepository>().Save(hub);
            CostCentre commoditySuppliercc = Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.CommoditySupplier,
                                                                             Ioc<ICostCentreRepository>().GetById(
                                                                                 parentId));
            CommoditySupplier commoditySupplier = commoditySuppliercc as CommoditySupplier;
            commoditySupplier.Name = "NewCommodityWSupplier";
            commoditySupplier.CostCentreCode = "CCCode_001";
            commoditySupplier.AccountNo = "000OA";
            commoditySupplier.AccountName = "NewCommodityWSupplierAccount";
            commoditySupplier.BankBranchId = Guid.NewGuid();
            commoditySupplier.BankId = Guid.NewGuid();
            commoditySupplier.CommoditySupplierType = CommoditySupplierType.Individual;
            commoditySupplier.JoinDate = DateTime.Now;
            commoditySupplier.PinNo = "A000100OL";

            return commoditySupplier;

        }

        public ContainerType BuildContainerType()
        {
            var commodity = BuildCommodity();
            var toSaveCommodity = Ioc<ICommodityRepository>().Save(commodity);
            var savedCommodity = Ioc<ICommodityRepository>().GetById(toSaveCommodity);

            var id = Guid.NewGuid();
            var container = new ContainerType(id)
            {
                Name = "Container Type 1",
                BubbleSpace = 0,
                CommodityGrade = commodity.CommodityGrades.FirstOrDefault(),
                FreezerTemp = 0,
                Height = 0,
                Length = 0,
                LoadCarriage = 0,
                TareWeight = 0,
                Volume = 0,
                Width = 0,
                Code = "Code 1",
                Description = "Container Type 1",
                Make = "Container Type 1",
                Model = "Container Type 1",
                ContainerUseType = ContainerUseType.StorageContainer,
                _Status = EntityStatus.Active
            };

            return container;
        }

        public Printer BuildPrinter(CostCentre savedHub)
        {
            /*var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var hub = BuildHub(savedRegion);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);*/

            var id = Guid.NewGuid();
            var printer = new Printer(id)
            {
                Name = "Printer 1",
                Code = "Code 1",
                Description = "Printer 1",
                Make = "Printer 1",
                Model = "Printer 1",
                EquipmentType = EquipmentType.Printer,
                CostCentre = savedHub as Hub,
                EquipmentNumber = "12345678",
                _Status = EntityStatus.Active
            };

            return printer;
        }

        public WeighScale BuildWeighScale(CostCentre savedHub)
        {
            /*var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var hub = BuildHub(savedRegion);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);*/

            var id = Guid.NewGuid();
            var weighScale = new WeighScale(id)
            {
                Name = "WeighScale 1",
                Code = "Code 1",
                Description = "WeighScale 1",
                Make = "WeighScale 1",
                Model = "WeighScale 1",
                EquipmentType = EquipmentType.WeighingScale,
                CostCentre = savedHub as Hub,
                EquipmentNumber = "12345678",
                _Status = EntityStatus.Active
            };

            return weighScale;
        }

        public SourcingContainer BuildContainer(CostCentre savedHub)
        {
            /*var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var hub = BuildHub(savedRegion);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);*/

            var containerType = BuildContainerType();
            var toSaveContainerType = Ioc<IContainerTypeRepository>().Save(containerType);
            var savedContainerType = Ioc<IContainerTypeRepository>().GetById(toSaveContainerType);

            var id = Guid.NewGuid();
            var container = new SourcingContainer(id)
            {
                Name = "Container 1",
                Code = "Code 1",
                Description = "Container 1",
                Make = "Container 1",
                Model = "Container 1",
                EquipmentType = EquipmentType.Container,
                CostCentre = savedHub as Hub,
                EquipmentNumber = "12345678",
                ContainerType = savedContainerType,
                _Status = EntityStatus.Active
            };

            return container;
        }

        public List<Equipment> BuildEquipments()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var hub = BuildHub(savedRegion);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);

            var printer = BuildPrinter(savedHub) as Equipment;
            var weighingScale = BuildWeighScale(savedHub) as Equipment;
            var container = BuildContainer(savedHub) as Equipment;

            var equipments = new List<Equipment> { printer, weighingScale, container };

            return equipments;
        }



        public Vehicle BuildVehicle()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var hub = BuildHub(savedRegion);
            var toSaveHub = Ioc<IHubRepository>().Save(hub);
            var savedHub = Ioc<IHubRepository>().GetById(toSaveHub);

            var id = Guid.NewGuid();

            var vehicle = new Vehicle(id)
            {
                Code = "Code 1",
                Name = "Vehicle 1",
                CostCentre = savedHub as Hub,
                Description = "Vehicle 1",
                EquipmentNumber = "123456789",
                EquipmentType = EquipmentType.Vehicle,
                Make = "12345678",
                Model = "Vehicle 1",
                _Status = EntityStatus.Active
            };

            return vehicle;
        }

        public Account BuildAccount()
        {
            var cc = BuildDistributrCostCentre();
            Guid costCenterId = Ioc<ICostCentreRepository>().Save(cc);
            var acc = new Account
            {
                Id = Guid.NewGuid(),
                CostcentreId = costCenterId,
                AccountType = AccountType.Cash,
                Balance = 500,
            };
            return acc;
        }

        public AccountTransaction BuildAccountTransaction()
        {
            Account account = BuildAccount();
            Guid accountId = Ioc<IAccountRepository>().Add(account);
            AccountTransaction accTr = new AccountTransaction(Guid.NewGuid())
            {
                Account = Ioc<IAccountRepository>().GetById(accountId),
                Amount = 400,
                DocumentType = DocumentType.Invoice,
                DocumentId = Guid.NewGuid(),
                DateInserted = DateTime.Now
            };
            return accTr;
        }





        public Infection BuildInfection()
        {
            var id = Guid.NewGuid();

            var infection = new Infection(id)
            {
                Name = "Infection 1",
                Code = "Code 1",
                InfectionType = InfectionType.Default,
                Description = "Infection 1",
                _Status = EntityStatus.Active
            };

            return infection;
        }

        public Inventory BuildInventory()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var distributorCC = BuildDistributorCC(savedRegion);
            var toSaveDistributorCC = Ioc<ICostCentreRepository>().Save(distributorCC);
            var savedDistributorCC = Ioc<ICostCentreRepository>().GetById(toSaveDistributorCC);

            Guid productId = AddProduct(Guid.Empty, "10013", "Soda3", EntityStatus.Active, 25, ReturnableType.GenericReturnable, null,
                   "productBrandDesc3", "productBrandCode3", "prodBrandName3", "prodFlavour3", "prodFlavourCode3",
                   "prodFlavourName3", "prodPackagingTypeName3", "prodPackagingTypeCode3", "prodPackagingTypeDesc3", "prodPackagingName3", "prodPackagingCode3", "prodPackagingDesc3", "productTypeCode3", "productTypeDesc3", "productTypeName3", "vatClassName3", "vatClass3");
            var product = Ioc<IProductRepository>().GetById(productId);

            var id = Guid.NewGuid();
            var inventory = new Inventory(id)
            {
                Warehouse = savedDistributorCC as StandardWarehouse,
                Product = product,
                Balance = 5,
                Value = 5
            };

            return inventory;
        }

        public MarketAudit BuildMarketAudit()
        {
            var id = Guid.NewGuid();

            var marketAudit = new MarketAudit(id)
            {
                Question = "Question 1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                _Status = EntityStatus.Active
            };

            return marketAudit;
        }

        public MasterDataAllocation BuildMasterDataAllocation()
        {
            var route = BuildRoute();
            Guid routeId = Ioc<IRouteRepository>().Save(route);
            var prodCc = BuildProducerCostCentre();
            Guid ccId = Ioc<ICostCentreRepository>().Save(prodCc);
            var allocation = new MasterDataAllocation(Guid.NewGuid())
            {
                AllocationType =
                    MasterDataAllocationType.RouteCostCentreAllocation,
                EntityAId = routeId,
                EntityBId = ccId,
            };
            return allocation;
        }

        //        public void object BuildOrder()
        //        {
        //           var order = Ioc<IOrderFactory>().Create()
        //        }

        public OutletAudit BuildOutletAudit()
        {
            var id = Guid.NewGuid();

            var outletAudit = new OutletAudit(id)
            {
                Question = "Question 1",
                _Status = EntityStatus.Active
            };

            return outletAudit;
        }

        public ReOrderLevel BuildReorderLevel()
        {
            var region = BuildRegion();
            var toSaveRegion = Ioc<IRegionRepository>().Save(region);
            var savedRegion = Ioc<IRegionRepository>().GetById(toSaveRegion);

            var distributorCC = BuildDistributorCC(savedRegion);
            var toSaveDistributorCC = Ioc<ICostCentreRepository>().Save(distributorCC);
            var savedDistributorCC = Ioc<ICostCentreRepository>().GetById(toSaveDistributorCC);
            var productId = AddProduct(Guid.Empty, "10013", "Soda3", EntityStatus.Active, 25, ReturnableType.GenericReturnable, null,
                   "productBrandDesc3", "productBrandCode3", "prodBrandName3", "prodFlavour3", "prodFlavourCode3",
                   "prodFlavourName3", "prodPackagingTypeName3", "prodPackagingTypeCode3", "prodPackagingTypeDesc3", "prodPackagingName3", "prodPackagingCode3", "prodPackagingDesc3", "productTypeCode3", "productTypeDesc3", "productTypeName3", "vatClassName3", "vatClass3");
            var product = Ioc<IProductRepository>().GetById(productId);

            var id = Guid.NewGuid();

            var reorder = new ReOrderLevel(id)
            {
                ProductReOrderLevel = 10,
                ProductId = product,
                DistributorId = savedDistributorCC,
                _Status = EntityStatus.Active
            };

            return reorder;
        }
    }
}
