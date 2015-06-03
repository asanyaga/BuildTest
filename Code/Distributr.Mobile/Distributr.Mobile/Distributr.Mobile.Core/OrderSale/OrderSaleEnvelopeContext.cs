using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.Data.References;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Login;

namespace Distributr.Mobile.Core.OrderSale
{
    public abstract class OrderSaleEnvelopeContext : IEnvelopeContext
    {
        private Outlet Outlet { get; set; }
        private CostCentre CostCentre { get; set; }
        private Order Order { get; set; }
        private DateTime Now { get; set; }
        protected ReferenceGenerator ReferenceGenerator { get; set; }

        protected OrderSaleEnvelopeContext(long sequenceNumber, Outlet outlet, User user, CostCentre costCentre, Order order)
        {
            Outlet = outlet;
            User = user;
            CostCentre = costCentre;
            Order = order;
            var dateTime = DateTime.Now;
            //Remove milliseconds
            Now = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            ReferenceGenerator = new ReferenceGenerator(sequenceNumber, user, outlet, Now);
            VisitId = Guid.NewGuid();
        }
        
        public Guid VisitId { get; set; }

        public Distributr.Core.Domain.Master.UserEntities.User User { get; private set; }

        public abstract int OrderTypeId { get; }

        public string ShipToAddress
        {
            get { return Order.ShipToAddress; }
        }

        public decimal SaleDiscount
        {
            get { return Order.SaleDiscount; }
        }

        public Guid ParentDocumentId
        {
            get { return Order.Id; }
        }

        public Guid IssuedOnBehalfOfCostCentre
        {
            get { return Outlet.Id; }
        }

        public Guid GeneratedByCostCentreId
        {
            get { return CostCentre.Id; }
        }

        public Guid RecipientCostCentreId
        {
            get { return CostCentre.ParentCostCentreId; }
        }

        public Guid GeneratedByCostCentreApplicationId
        {
            get { return new Guid(((User)User).CostCentreApplicationId); }            
        }

        public DateTime Timestamp
        {
            get { return Now; }
        }

        public Guid GeneratedByUserId
        {
            get { return User.Id; }
        }

        public Guid InvoiceId
        {
            get { return Order.InvoiceId; } 
        }

        public int InventoryAdjustmentNoteType
        {
            get { return (int)Distributr.Core.Domain.Transactional.DocumentEntities.InventoryAdjustmentNoteType.Available; }
        }

        public abstract string InventoryAdjustmentNoteReason { get; }

        public string ExternalDocumentReference()
        {
            return ReferenceGenerator.NextExternalDocumentReference();
        }

        public abstract string OrderSaleReference();

        public string ReceiptReference()
        {
            return ReferenceGenerator.NextReceiptReference();
        }

        public bool IsClosable()
        {
            return Order.IsClosable();
        }

        public string InvoiceReference()
        {
            if (string.IsNullOrEmpty(Order.InvoiceReference))
            {
                Order.InvoiceReference = ReferenceGenerator.NextInvoiceReference();
            }
            return Order.InvoiceReference;
        }
    }
}
