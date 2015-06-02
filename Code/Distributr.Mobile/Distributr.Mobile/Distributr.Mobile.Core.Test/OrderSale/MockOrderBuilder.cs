using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Moq;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    public class MockOrderBuilder
    {
        private Order Order { get; set; }
        private readonly BankBranch bankBranch = new BankBranch() { Code = "EFG" };
        private readonly Bank bank = new Bank() { Code = "ABC" };

        public MockOrderBuilder()
        {
            Order = new Order(Guid.NewGuid(), new Outlet());
        }

        public static SaleProduct AProductWithPrice(decimal price)
        {
            var product = CreateProduct<SaleProduct>("sale product", price);
            product.ContainerCapacity = 24;
            product.ReturnableProduct = CreateProduct<ReturnableProduct>("item returnable", 10);
            product.ReturnableProductMasterId = product.ReturnableProduct.Id;

            product.ReturnableContainer = CreateProduct<ReturnableProduct>("container returnable", 40);
            product.ReturnableContainerMasterId = product.ReturnableContainer.Id;
            
            return product;
        }

        public static T CreateProduct<T>(string description, decimal price) where T : Product
        {
            var product = new Mock<T>();
            product.Setup(p => p.ProductPrice(It.IsAny<ProductPricingTier>()))
                .Returns(price);
            product.Object.Description = description;

            var id = Guid.NewGuid();            
            product.Setup(p => p.Id).Returns(id);

            return product.Object;
        }

        public static VATClass AVatClassWithRate(decimal rate)
        {
            var vatClassItem = new VATClass.VATClassItem()
            {
                EffectiveDate = DateTime.Now.AddDays(-1),
                Rate = rate
            };
            var vatClass = new VATClass { VATClassItems = new List<VATClass.VATClassItem>() { vatClassItem } };

            return vatClass;
        }

        public MockOrderBuilder WithLineItem(decimal price = 30m, decimal vatRate = 0.10m, decimal caseQuantity = 0m,
            decimal eachQuantity = 1m, decimal eachReturnableQuantity = 0, decimal caseReturnableQuantity = 0)
        {
            var aProduct = AProductWithPrice(price);
            aProduct.VATClass = AVatClassWithRate(vatRate);
            var wrapper = new ProductWrapper()
            {
                SaleProduct = aProduct, 
                EachQuantity = eachQuantity, 
                CaseQuantity = caseQuantity, 
                EachReturnableQuantity = eachReturnableQuantity, 
                CaseReturnableQuantity = caseReturnableQuantity
            };

            Order.AddOrUpdateSaleLineItem(wrapper);            
            return this;
        }

        public MockOrderBuilder WithChequePayment(string chequeNumber = "00000001", string bankCode = "ABCD", string bankBranchCode = "EFGH", decimal amount = 1m)
        {
            bank.Code = bankCode;
            bankBranch.Code = bankBranchCode;
            Order.AddChequePayment(chequeNumber, amount, bank, bankBranch, DateTime.Now);
            return this;
        }

        public MockOrderBuilder WithCashPayment(string paymentReference = "cash payment", decimal amount = 1m)
        {
            Order.AddCashPayment(paymentReference, amount);
            return this;
        }

        public Order Build()
        {
            return Order;
        }

    }
}
