using System;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Login;

namespace Distributr.Mobile.Core.Test.OrderSale
{

    public class OrderAndContext
    {
        public OrderAndContext(Order order, IEnvelopeContext context)
        {
            Order = order;
            Context = context;
        }

        public Order Order { get; private set; }
        public IEnvelopeContext Context { get; private set; }
    }

    public abstract class OrderSaleContextBuilder
    {
        protected OrderSaleContextBuilder(Outlet outlet, CostCentre costCentre, User user, Order order, Bank bank, BankBranch bankBranch)
        {
            Outlet = outlet;
            CostCentre = costCentre;
            User = user;
            Order = order;
            Bank = bank;
            BankBranch = bankBranch;
        }

        public Outlet Outlet { get; set; }
        public CostCentre CostCentre { get; set; }
        public User User { get; set; }
        public Order Order { get; set; }
        public Bank Bank { get; set; }
        public BankBranch BankBranch { get; set; }


        public abstract OrderSaleContextBuilder AddLineItem(SaleProduct product, decimal eachQuantity, decimal caseQuantity, decimal eachReturnableQuantity = 0, decimal caseReturnableQuantity = 0);

        public virtual OrderSaleContextBuilder PaidInfFullByCash()
        {
            return WithCashPayment(Order.TotalValueIncludingVat, "full cash payment");
        }

        public virtual OrderSaleContextBuilder WithCashPayment(decimal amount, string reference = "cash payment")
        {
            Order.AddCashPayment(reference, amount);
            return this;
        }

        public virtual OrderSaleContextBuilder PaidInFullByCheque()
        {
            return WithChequePayment(Order.TotalValueIncludingVat);
        }

        public virtual OrderSaleContextBuilder WithChequePayment(decimal amount,  string chequeNumber = "12345678")
        {
            return WithChequePayment(amount, Bank, BankBranch, chequeNumber);
        }

        public virtual OrderSaleContextBuilder WithChequePayment(decimal amount, Bank bank, BankBranch bankBranch,  string chequeNumber = "12345678")
        {
            Order.AddChequePayment(chequeNumber, amount, bank, bankBranch, DateTime.Now);
            return this;
        }

        public abstract OrderAndContext Build();
    }

    public class OrderAndContextBuilder : OrderSaleContextBuilder
    {

        public OrderAndContextBuilder(Outlet outlet, CostCentre costCentre, User user, Order order, Bank bank, BankBranch bankBranch)
            : base(outlet, costCentre, user, order, bank, bankBranch)
        {
        }

        public override OrderSaleContextBuilder AddLineItem(SaleProduct product, decimal eachQuantity, decimal caseQuantity, decimal eachReturnableQuantity = 0, decimal caseReturnableQuantity = 0)
        {
            var wrapper = new ProductWrapper()
            {
                SaleProduct = product,
                EachQuantity = eachQuantity,
                CaseQuantity = caseQuantity,
            };
            //Returnables are included automatically for all orders
            Order.AddOrUpdateOrderLineItem(wrapper);
            return this;
        }

        public override OrderAndContext Build()
        {
            return new OrderAndContext(Order, new MakeOrderEnvelopeContext(100, Outlet, User, CostCentre, Order));
        }
    }

    public class SaleAndContextBuilder : OrderSaleContextBuilder
    {
        public SaleAndContextBuilder(Outlet outlet, CostCentre costCentre, User user, Order order, Bank bank, BankBranch bankBranch)
            : base(outlet, costCentre, user, order, bank, bankBranch)
        {
        }

        public override OrderSaleContextBuilder AddLineItem(SaleProduct product, decimal eachQuantity, decimal caseQuantity, decimal eachReturnableQuantity = 0, decimal caseReturnableQuantity = 0)
        {
            var wrapper = new ProductWrapper()
            {
                SaleProduct = product,
                EachQuantity = eachQuantity,
                CaseQuantity = caseQuantity,
                EachReturnableQuantity = eachReturnableQuantity,
                CaseReturnableQuantity = caseReturnableQuantity
            };
         
            Order.AddOrUpdateSaleLineItem(wrapper);
            
            return this;            
        }

        public override OrderAndContext Build()
        {
            return new OrderAndContext(Order, new MakeSaleEnvelopeContext(100, Outlet, User, CostCentre, Order));
        }
    }
}
