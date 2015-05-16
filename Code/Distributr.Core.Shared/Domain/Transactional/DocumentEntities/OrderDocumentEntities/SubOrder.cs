using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Workflow;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities
{
    public class SubOrder : BaseOrder
    {
        public SubOrder(Guid id)
            : base(id)
        {
            _lineItems = new List<SubOrderLineItem>();
            _BackOrderlineItems = Tuple.Create(Guid.NewGuid(), new List<SubOrderLineItem>());
        }

        public OrderStatus OrderStatus { set; get; }

      
        public override void AddLineItem(SubOrderLineItem orderLineItem)
        {
            if (Status != DocumentStatus.New)
                throw new Exception("Cannot add line on a confirmed order");
            _lineItems.Add(orderLineItem);
            _AddAddLineItemCommandToExecute(orderLineItem);

        }

        public override void ApproveLineItem(SubOrderLineItem orderLineItem, decimal quantity, bool takeTheRestToLossSale)
        {
            if (quantity < orderLineItem.Qty && !takeTheRestToLossSale)
            {
                var bo = new SubOrderLineItem(Guid.NewGuid());
                bo.Product = orderLineItem.Product;
                bo.Qty = orderLineItem.Qty - quantity;
                bo.Value = orderLineItem.Value;
                bo.LineItemStatus = MainOrderLineItemStatus.New;
                bo.LineItemType = orderLineItem.LineItemType;
                bo.Description = orderLineItem.Description;
                bo.ProductDiscount = orderLineItem.ProductDiscount;
                bo.DiscountType = orderLineItem.DiscountType;
                bo.LineItemVatValue = orderLineItem.LineItemVatValue;

                _BackOrderlineItems.Item2.Add(bo);
            }

            _AddApproveLineItemCommandToExecute(orderLineItem, quantity, takeTheRestToLossSale);
        }

        public override void ApproveLineItem(SubOrderLineItem orderLineItem)
        {
            _AddApproveLineItemCommandToExecute(orderLineItem, orderLineItem.Qty, false);

        }

        public override void RemoveLineItem(SubOrderLineItem orderLineItem)
        {

            if (Status != DocumentStatus.Confirmed)
                throw new Exception("Cannot Remove lineitem on an order that isn't confirmed ");
            SubOrderLineItem li = LineItems.FirstOrDefault(n => n.Id == orderLineItem.Id);
            if (li != null)
            {
                orderLineItem.LineItemStatus = MainOrderLineItemStatus.Removed;
                _lineItems.Remove(orderLineItem);
                _AddRemoveLineItemCommandToExecute(orderLineItem);
            }
           
        }

        public override void EditLineItem(SubOrderLineItem orderLineItem)
        {
            if (Status != DocumentStatus.Confirmed)
                throw new Exception("Cannot edit lineitem on an order that isn't confirmed ");
            SubOrderLineItem li = LineItems.FirstOrDefault(n => n.Id ==orderLineItem.Id);
            if(li==null)
            {
                orderLineItem.LineItemStatus = MainOrderLineItemStatus.Confirmed;
                _lineItems.Add(orderLineItem);
                _AddAddLineItemCommandToExecute(orderLineItem);
            }else
            {
                li.Qty = orderLineItem.Qty;
                _AddEditLineItemCommandToExecute(orderLineItem);
            }


        }
        private Tuple<Guid, List<SubOrderLineItem>> _BackOrderlineItems;
        private List<SubOrderLineItem> _lineItems;
        public List<SubOrderLineItem> LineItems
        {
            get { return _lineItems; }
            //set { _lineItems = value; }
        }



        public override decimal TotalNet
        {
            get
            {
                return LineItems.Sum(n => n.Value * n.Qty);
            }
        }
        public override decimal TotalVat
        {
            get
            {
                return LineItems.Sum(n => n.LineItemVatTotal);
            }
        }
        public override decimal TotalGross { get { return (TotalNet + TotalVat) - SaleDiscount; } }

        public override decimal TotalDiscount
        {
            get { throw new NotImplementedException(); }
        }

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an order that is not new");

            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        public override void Approve()
        {
            DocumentStatus[] validStatus = new DocumentStatus[] { DocumentStatus.Confirmed };

            if (!validStatus.Contains(Status))
                throw new InvalidDocumentOperationException("Cannot Approve an order that is not Confirmed");
            if (LineItems.Count() == 0)
                throw new InvalidDocumentOperationException("Must add at least one lineitem to order before approving");
            _AddApproveOrderCommandToExecute();
            if (_BackOrderlineItems.Item2.Any())
            {
                _AddCreateCommandToExecute(true);
                foreach (var subOrderLineItem in _BackOrderlineItems.Item2)
                {
                    _AddAddLineItemCommandToExecute(subOrderLineItem, true);
                }
                _AddConfirmCommandToExecute(true);

            }
            this.DisableAddCommands();
        }

        public override void Reject()
        {
            if (Status != DocumentStatus.Confirmed)
                throw new InvalidDocumentOperationException("Order needs to be Confirmed");
            _AddRejectOrderCommandToExecute();
        }

        private void _AddRejectOrderCommandToExecute()
        {
          
            if (!_CanAddCommands) return;

            var ali = new RejectMainOrderCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
               DocumentIssuerCostCentreApplicationId,
               Note,
               DocumentIssuerUser.Id
                );
            _AddCommand(ali);
                
        }

        public override void Close()
        {
            _AddCloseCommandToExecute();
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            if (!_CanAddCommands) return;

            var coc = new CreateMainOrderCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentReference,
                DocumentDateIssued,
                DateRequired,
                IssuedOnBehalfOf.Id,
                DocumentIssuerCostCentre.Id,
                DocumentRecipientCostCentre.Id,
                DocumentIssuerUser.Id,
                (int)OrderType, (int)OrderStatus, ParentId,ShipToAddress,Note,SaleDiscount);
            if (isHybrid)
            {
                coc.DocumentId = _BackOrderlineItems.Item1;

            }
            coc.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(coc);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as SubOrderLineItem;
            if (!_CanAddCommands) return;

            var ali = new AddMainOrderLineItemCommand(
                item.Id,
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                item.LineItemSequenceNo,
                item.Value,
                item.Product.Id,
                item.Qty,
                item.LineItemVatValue,
                item.ProductDiscount,
                item.Description,
                (int)item.LineItemType,
                (int)item.DiscountType
                );
            if (isHybrid)
            {
                ali.CommandId = Guid.NewGuid();
                ali.DocumentId = _BackOrderlineItems.Item1;
            }
            _AddCommand(ali);

        }
        protected  void _AddEditLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as SubOrderLineItem;
            if (!_CanAddCommands) return;

            var ali = new ChangeMainOrderLineItemCommand(Guid.NewGuid(),
                Id,
                item.Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                item.Qty
                );
            if (isHybrid)
            {
                ali.CommandId = Guid.NewGuid();
                ali.DocumentId = _BackOrderlineItems.Item1;
            }
            _AddCommand(ali);

        }
        protected void _AddRemoveLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as SubOrderLineItem;
            if (!_CanAddCommands) return;

            var ali = new RemoveMainOrderLineItemCommand(Guid.NewGuid(),
                Id,
                item.Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId
                );
            if (isHybrid)
            {
                ali.CommandId = Guid.NewGuid();
                ali.DocumentId = _BackOrderlineItems.Item1;
            }
            _AddCommand(ali);

        }
        protected void _AddApproveLineItemCommandToExecute<T>(T lineItem, decimal quantity, bool takeTheRestToLossSale)
        {
            var item = lineItem as SubOrderLineItem;
            if (!_CanAddCommands) return;
            //check if line item is not approved

            var ali = new ApproveOrderLineItemCommand(
                Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id, DocumentIssuerCostCentre.Id,
                0,
               DocumentIssuerCostCentreApplicationId,
               DocumentParentId,
               item.Id,
               quantity
                );
            if (takeTheRestToLossSale)
                ali.LossSaleQuantity = item.Qty - quantity;
            _AddCommand(ali);

        }
        protected void _AddCloseCommandToExecute(bool isHybrid = false)
        {
            if (!_CanAddCommands) return;
            var co = new CloseOrderCommand(Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId);
           
            _AddCommand(co);
        }
        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            if (!_CanAddCommands) return;
            var co = new ConfirmMainOrderCommand(Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentParentId
                );
            if (isHybrid)
            {
                co.DocumentId = _BackOrderlineItems.Item1;
            }
            _AddCommand(co);
        }
        protected void _AddApproveOrderCommandToExecute()
        {
            if (!_CanAddCommands) return;
            var co = new ApproveMainOrderCommand(Guid.NewGuid(),
                Id,
                DocumentIssuerUser.Id,
                DocumentIssuerCostCentre.Id,
                0,
                DocumentIssuerCostCentreApplicationId,
                DocumentIssuerUser.Id, DateTime.Now
                );
            _AddCommand(co);
        }
       

        protected void _AddDispatchPendingLineItemsCommandToExecute()
        {
            if (!_CanAddCommands) return;
            var co = new OrderDispatchApprovedLineItemsCommand(Guid.NewGuid(),
                                                               Id,
                                                               DocumentIssuerUser.Id,
                                                               DocumentIssuerCostCentre.Id,
                                                               0,
                                                               DocumentIssuerCostCentreApplicationId
                );
            _AddCommand(co);
        }
        public List<SubOrderLineItem> _allLineItems()
        {
            return _lineItems;
        }

        public void _SetLineItems(List<SubOrderLineItem> items)
        {
            _lineItems = items;
        }

       

       
        public void DispatchPendingLineItems()
        {
            if (Status != DocumentStatus.Approved)
                throw new InvalidDocumentOperationException("Only an order with order Approved Status can be Dispatched");
            // Status = DocumentStatus.OrderDispatchedToPhone;
            _AddDispatchPendingLineItemsCommandToExecute();
        }


        public void AddOrderPaymentInfoLineItem(PaymentInfo paymentInfo)
        {
            if (!_CanAddCommands) return;
            var aopi = new AddOrderPaymentInfoCommand();
            aopi.CommandId = Guid.NewGuid();
            aopi.InfoId = paymentInfo.Id;
            aopi.Amount = paymentInfo.Amount;
            aopi.ConfirmedAmount = paymentInfo.ConfirmedAmount;
            aopi.CommandCreatedDateTime = DateTime.Now;
            aopi.CommandGeneratedByCostCentreApplicationId = DocumentIssuerCostCentreApplicationId;
            aopi.CommandGeneratedByCostCentreId = DocumentIssuerCostCentre.Id;
            aopi.CommandGeneratedByUserId = DocumentIssuerUser.Id;
            aopi.CommandSequence = 0;
            aopi.CostCentreApplicationCommandSequenceId = 0;
            aopi.Description = paymentInfo.Description;
            aopi.DocumentId = Id;
            aopi.IsConfirmed = paymentInfo.IsConfirmed;
            aopi.IsProcessed = paymentInfo.IsProcessed;
            aopi.MMoneyPaymentType = paymentInfo.MMoneyPaymentType;
            aopi.NotificationId = paymentInfo.NotificationId;
            aopi.PaymentModeId = (int)paymentInfo.PaymentModeUsed;
            aopi.PaymentRefId = paymentInfo.PaymentRefId;
            aopi.PDCommandId = ParentId;

            aopi.Bank = paymentInfo.Bank;
            aopi.BankBranch = paymentInfo.BankBranch;
            if (paymentInfo.DueDate.HasValue)
                aopi.DueDate = paymentInfo.DueDate.Value;
            _AddCommand(aopi);
        }
    }
}
