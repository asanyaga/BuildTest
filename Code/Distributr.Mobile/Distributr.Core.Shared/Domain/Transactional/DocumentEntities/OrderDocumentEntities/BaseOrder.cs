using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities
{
    public abstract class BaseOrder : Document
    {
        public BaseOrder(Guid id)
            : base(id)
        {

        }
        public Guid ParentId { get; set; }
        public CostCentre IssuedOnBehalfOf { get; set; }
        public DateTime DateRequired { get; set; }
        public OrderType OrderType { get; set; }
        public decimal SaleDiscount { get; set; }
        public string Note { get; set; }
        public string ShipToAddress { get; set; }



        public abstract void AddLineItem(SubOrderLineItem orderLineItem);
        public abstract void ApproveLineItem(SubOrderLineItem orderLineItem, decimal quantity, bool takeTheRestToLossSale);
        public abstract void ApproveLineItem(SubOrderLineItem orderLineItem);
        public abstract void RemoveLineItem(SubOrderLineItem orderLineItem);
        public abstract void EditLineItem(SubOrderLineItem orderLineItem);
        

        public abstract decimal TotalNet { get; }
        public abstract decimal TotalVat { get; }

        public abstract decimal TotalGross { get; }
        public abstract decimal TotalDiscount { get; }
        public abstract void Approve();
        public abstract void Reject();

        public abstract void Close();

       
        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        { throw new Exception("Should not be called here"); }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            throw new Exception("Should not be called here");
        }
        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            throw new Exception("Should not be called here");
        }
    }

}
