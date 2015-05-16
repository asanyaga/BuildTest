using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ProductRepositoryFixtures
{
    [TestFixture]
    public class ProductRepositoryFixture
    {

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
        }


        [Test]
        public void ProductRepositoryUnitTest()
        {

            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START PRODUCT REPOSITORY UNIT TEST....");

                string code = "Product Code";
                string description = "Fresh Milk";
                decimal exFactoryPrice = 24;
                ReturnableType returnableType = ReturnableType.GenericReturnable;
                EntityStatus status = EntityStatus.Active;

                //Create SaleProduct
                Trace.WriteLine("...Creating Sale Product");
                Guid productId = _testHelper.AddProduct(Guid.Empty, code, description, status, exFactoryPrice, returnableType, null,
                    "productBrandDesc", "productBrandCode", "prodBrandName", "prodFlavour", "prodFlavourCode",
                    "prodFlavourName", "prodPackagingTypeName", "prodPackagingTypeCode", "prodPackagingTypeDesc", "prodPackagingName", "prodPackagingCode", "prodPackagingDesc", "productTypeCode", "productTypeDesc", "productTypeName", "vatClassName", "vatClass");

                Trace.WriteLine(string.Format(">Created Product with Id[{0}]", productId));

                //GetById
                Product createdProduct = _testHelper.Ioc<IProductRepository>().GetById(productId);
                Assert.IsNotNull(createdProduct);
                Assert.AreEqual(createdProduct.ProductCode, code);
                Assert.AreEqual(createdProduct.Description, description);
                Assert.AreEqual(createdProduct._Status, status);
                Assert.AreEqual(createdProduct.ReturnableType, returnableType);
                Assert.AreEqual(createdProduct.ExFactoryPrice, exFactoryPrice);

                Trace.WriteLine(string.Format(">Product GetById [ Id[{0}],Product Name[{1}], Product Code[{2}], Status[{3}],Returnable Type[{4}],Exfactory Price[{5}]]",
                        productId, createdProduct.Description, createdProduct.ProductCode, createdProduct._Status, createdProduct.ReturnableType, createdProduct.ExFactoryPrice));


                //Create Returnable Product
                Trace.WriteLine("...Creating Returnable Product");
                Guid _productId = _testHelper.AddProduct(Guid.Empty, "productcode", "soda", EntityStatus.Active, 15, ReturnableType.Returnable, null,
                   "productBrandDesc2", "productBrandCode2", "prodBrandName2", "prodFlavour2", "prodFlavourCode2",
                   "prodFlavourName2", "prodPackagingTypeName2", "prodPackagingTypeCode2", "prodPackagingTypeDesc2", "prodPackagingName2", "prodPackagingCode2","prodPackagingDesc2", "productTypeCode2", "productTypeDesc2", "productTypeName2", "vatClassName2", "vatClass2");
                Product _product = _testHelper.Ioc<IProductRepository>().GetById(_productId);

                var rtProduct = new
                {
                    code = "Returnable Product Code",
                    description = "Soda",
                    exFactoryPrice = 15,
                    returnableType = ReturnableType.Returnable,
                    status = EntityStatus.Active,
                };
                Guid returnableProdId = _testHelper.AddProduct(Guid.Empty, rtProduct.code, rtProduct.description, rtProduct.status, rtProduct.exFactoryPrice, rtProduct.returnableType, _product, "productBrandDesc1", "productBrandCode1", "prodBrandName1", "prodFlavour1", "prodFlavourCode1",
                    "prodFlavourName1", "prodPackagingTypeName1", "prodPackagingTypeCode1", "prodPackagingTypeDesc1", "prodPackagingName1", "prodPackagingCode1", "prodPackagingDesc1", "productTypeCode1", "productTypeDesc1", "productTypeName1", "vatClassName1", "vatClass1");
                ReturnableProduct createdReturnableProduct = _testHelper.Ioc<IProductRepository>().GetById(returnableProdId) as ReturnableProduct;

                if (createdReturnableProduct != null)
                {
                    ReturnableProduct createdReturnableProduct_product = _testHelper.Ioc<IProductRepository>().GetReturnableProduct(createdReturnableProduct.ReturnAbleProduct.Id) as ReturnableProduct;
                    Trace.WriteLine(string.Format("ProdId[{0}],ProductName[{1}],ReturnableProdId[{2}],ReturnableProductName[{3}]", returnableProdId, createdReturnableProduct_product.Description, createdReturnableProduct_product.ReturnAbleProduct.Id, createdReturnableProduct_product.ReturnAbleProduct.Description));
                }

                //Update
                var productToUpdate = createdProduct;
                productToUpdate._DateCreated = DateTime.Now;
                productToUpdate._DateLastUpdated = DateTime.Now;
                productToUpdate._Status = EntityStatus.Active;
                productToUpdate.ProductCode = "Product Code Update";
                productToUpdate.Description = "Milk001";
                productToUpdate.ReturnableType = ReturnableType.None;
                productToUpdate.ExFactoryPrice = 40;

                Guid updatedProductId = _testHelper.AddProduct(productToUpdate.Id, productToUpdate.ProductCode, productToUpdate.Description, productToUpdate._Status, productToUpdate.ExFactoryPrice, productToUpdate.ReturnableType, null,
                    "productBrandDesc", "productBrandCode", "prodBrandName", "prodFlavour", "prodFlavourCode",
                    "prodFlavourName", "prodPackagingTypeName", "prodPackagingTypeCode", "prodPackagingTypeDesc", "prodPackagingName", "prodPackagingCode", "prodPackagingDesc", "productTypeCode", "productTypeDesc", "productTypeName", "vatClassName", "vatClass");

                Product updatedProduct = _testHelper.Ioc<IProductRepository>().GetById(updatedProductId);

                Assert.AreEqual(productToUpdate.Id, updatedProduct.Id);
                Assert.AreEqual(productToUpdate.ProductCode, updatedProduct.ProductCode);
                Assert.AreEqual(productToUpdate.Description, updatedProduct.Description);
                Assert.AreEqual(productToUpdate.ReturnableType, updatedProduct.ReturnableType);
                Assert.AreEqual(productToUpdate._Status, updatedProduct._Status);
                Assert.AreEqual(productToUpdate.ExFactoryPrice, updatedProduct.ExFactoryPrice);
                Trace.WriteLine(string.Format(">Updated ProductId[{0}]", updatedProductId));
                Trace.WriteLine(string.Format(">Product Updated to.. [ Id[{0}],Product Name[{1}], Product Code[{2}], Status[{3}],Returnable Type[{4}],Exfactory Price[{5}]]",
                      updatedProduct.Id, updatedProduct.Description, updatedProduct.ProductCode, updatedProduct._Status, updatedProduct.ReturnableType, updatedProduct.ExFactoryPrice));

                //Query
                QueryStandard queryStandard = _testHelper.QueryStandard(updatedProduct.Description, updatedProduct.Brand.Supplier.Id,
                    (int?)UserType.Supplier);
                var queryResult = _testHelper.Ioc<IProductRepository>().Query(queryStandard);
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format(">Product Query Count[{0}]", queryResult.Count));

                //Set As Inactive
                _testHelper.Ioc<IProductRepository>().SetInactive(updatedProduct);
                Product inactiveProduct = _testHelper.Ioc<IProductRepository>().GetById(updatedProductId);
                Assert.AreEqual(inactiveProduct._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format(">Product Status[{0}]", inactiveProduct._Status));

                //Set As Active
                _testHelper.Ioc<IProductRepository>().SetActive(updatedProduct);
                Product activeProduct = _testHelper.Ioc<IProductRepository>().GetById(updatedProductId);
                Assert.AreEqual(activeProduct._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format(">Product Status[{0}]", activeProduct._Status));

                //Set As Deleted
                _testHelper.Ioc<IProductRepository>().SetAsDeleted(updatedProduct);
                Product deletedProduct = _testHelper.Ioc<IProductRepository>().GetById(updatedProductId);
                Assert.AreEqual(deletedProduct._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format(">Product Status[{0}]", deletedProduct._Status));
                Trace.WriteLine(string.Format("Product Repository Unit Tests Successful"));
            }
        }
    }
}
