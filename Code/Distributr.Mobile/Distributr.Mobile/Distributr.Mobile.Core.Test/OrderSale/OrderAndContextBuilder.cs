using System;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Login;

namespace Distributr.Mobile.Core.Test.OrderSale
{
    public class OrderAndContext
    {
        public OrderAndContext(Sale order, IEnvelopeContext context)
        {
            Sale = order;
            Context = context;
        }

        public Sale Sale { get; private set; }
        public IEnvelopeContext Context { get; private set; }
    }

    public abstract class OrderSaleContextBuilder
    {
        protected OrderSaleContextBuilder(Outlet outlet, CostCentre costCentre, User user, Sale sale, Bank bank, BankBranch bankBranch)
        {
            Outlet = outlet;
            CostCentre = costCentre;
            User = user;
            Sale = sale;
            Bank = bank;
            BankBranch = bankBranch;
        }

        public Outlet Outlet { get; set; }
        public CostCentre CostCentre { get; set; }
        public User User { get; set; }
        public Sale Sale { get; set; }
        public Bank Bank { get; set; }
        public BankBranch BankBranch { get; set; }


        public abstract OrderSaleContextBuilder AddLineItem(SaleProduct product, decimal quantity, bool sellReturnables = false);

        public virtual OrderSaleContextBuilder PaidInfFullByCash()
        {
            return WithCashPayment(Sale.TotalValueIncludingVat, "full cash payment");
        }

        public virtual OrderSaleContextBuilder WithCashPayment(decimal amount, string reference = "cash payment")
        {
            Sale.AddCashPayment(reference, amount);
            return this;
        }

        public virtual OrderSaleContextBuilder PaidInFullByCheque()
        {
            return WithChequePayment(Sale.TotalValueIncludingVat);
        }

        public virtual OrderSaleContextBuilder WithChequePayment(decimal amount,  string chequeNumber = "12345678")
        {
            return WithChequePayment(amount, Bank, BankBranch, chequeNumber);
        }

        public virtual OrderSaleContextBuilder WithChequePayment(decimal amount, Bank bank, BankBranch bankBranch,  string chequeNumber = "12345678")
        {
            Sale.AddChequePayment(chequeNumber, amount, bank, bankBranch, DateTime.Now);
            return this;
        }

        public abstract OrderAndContext Build();
    }

    public class OrderAndContextBuilder : OrderSaleContextBuilder
    {

        public OrderAndContextBuilder(Outlet outlet, CostCentre costCentre, User user, Sale sale, Bank bank, BankBranch bankBranch)
            : base(outlet, costCentre, user, sale, bank, bankBranch)
        {
        }

        public override OrderSaleContextBuilder AddLineItem(SaleProduct product, decimal quantity, bool sellReturnables = false)
        {
            //Returnables are included automatically for all orders
            Sale.AddItem(product, quantity, quantity, sellReturnables);
            return this;
        }

        public override OrderAndContext Build()
        {
            return new OrderAndContext(Sale, new MakeOrderEnvelopeContext(100, Outlet, User, CostCentre, Sale));
        }
    }

    public class SaleAndContextBuilder : OrderSaleContextBuilder
    {
        public SaleAndContextBuilder(Outlet outlet, CostCentre costCentre, User user, Sale sale, Bank bank, BankBranch bankBranch)
            : base(outlet, costCentre, user, sale, bank, bankBranch)
        {
        }

        public override OrderSaleContextBuilder AddLineItem(SaleProduct product, decimal quantity, bool sellReturnables = false)
        {         
            Sale.AddItem(product, quantity, quantity, sellReturnables);            
            return this;            
        }

        public override OrderAndContext Build()
        {
            return new OrderAndContext(Sale, new MakeSaleEnvelopeContext(100, Outlet, User, CostCentre, Sale));
        }
    }
}
