using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.OrderSale;
using Moq;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    public class MockOrderBuilder
    {
        private Sale Order { get; set; }
        public static readonly BankBranch BankBranch = new BankBranch() { Code = "EFG" };
        public static readonly Bank Bank = new Bank() { Code = "ABC", Branches = new List<BankBranch>(){BankBranch}};

        public MockOrderBuilder()
        {
            Order = new Sale(Guid.NewGuid(), new Outlet());
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

        public MockOrderBuilder WithSaleLineItem(decimal price = 30m, decimal vatRate = 0.10m, decimal quantity = 1, bool sellReturnables = false)
        {
            var aProduct = AProductWithPrice(price);
            aProduct.VATClass = AVatClassWithRate(vatRate);

            Order.AddItem(aProduct, quantity, quantity, sellReturnables);

            return this;            
        }

        public MockOrderBuilder WithOrderLineItem(decimal price = 30m, decimal vatRate = 0.10m, decimal quantity = 1)
        {
            var aProduct = AProductWithPrice(price);
            aProduct.VATClass = AVatClassWithRate(vatRate);

            Order.AddItem(aProduct, quantity);

            return this;
        }

        public MockOrderBuilder WithChequePayment(string chequeNumber = "00000001", string bankCode = "ABCD", string bankBranchCode = "EFGH", decimal amount = 1m)
        {
            Bank.Code = bankCode;
            BankBranch.Code = bankBranchCode;
            Order.AddChequePayment(chequeNumber, amount, Bank, BankBranch, DateTime.Now);
            return this;
        }

        public MockOrderBuilder WithCashPayment(string paymentReference = "cash payment", decimal amount = 1m)
        {
            Order.AddCashPayment(paymentReference, amount);
            return this;
        }

        public Sale Build()
        {
            return Order;
        }

    }
}
