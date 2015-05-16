using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;

namespace Distributr.Core.Utility
{
    public class EntityUsage
    {
        public List<InvoicePaymentInfo> OrderPaymentInfos;
        private List<Order> _salesmanOrders;
        private ITargetRepository _targetRepository;
        private IInventoryRepository _inventoryRepository;
        private IOrderRepository _orderRepository;
        private IInvoiceRepository _invoiceRepository;
        private IReceiptRepository _receiptRepository;
        private ICreditNoteRepository _creditNoteRepository;
        private IDispatchNoteRepository _dispatchNoteRepository;
        private ICostCentreRepository _costCentreRepository;
        private ICommodityOwnerRepository _commodityOwnerRepository;
        private ICommodityProducerRepository _commodityProducerRepository;
        private ICommodityPurchaseRepository _commodityPurchaseNoteRepository;

        public EntityUsage(List<Order> salesmanOrders, ITargetRepository targetRepository, IInventoryRepository inventoryRepository, IOrderRepository orderRepository, 
            IInvoiceRepository invoiceRepository, IReceiptRepository receiptRepository, ICreditNoteRepository creditNoteRepository,
            IDispatchNoteRepository dispatchNoteRepository, ICostCentreRepository costCentreRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository, ICommodityPurchaseRepository commodityPurchaseNoteRepository)
        {
            _dispatchNoteRepository = dispatchNoteRepository;
            _salesmanOrders = salesmanOrders;
            _targetRepository = targetRepository;
            _inventoryRepository = inventoryRepository;
            _orderRepository = orderRepository;
            _invoiceRepository = invoiceRepository;
            _receiptRepository = receiptRepository;
            _creditNoteRepository = creditNoteRepository;
            _costCentreRepository = costCentreRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _commodityPurchaseNoteRepository = commodityPurchaseNoteRepository;

            OrderPaymentInfos = new List<InvoicePaymentInfo>();
        }

        public EntityUsage(ITargetRepository targetRepository)
        {
            _targetRepository = targetRepository;
        }

        public string UserUsageInfo(User user)
        {
            if (user.UserType != UserType.DistributorSalesman)
                return "";
            return SalesmanUsage(user);
        }

        public string SalesmanUsage(User user)
        {

            _salesmanOrders = new List<Order>();
            string msg = "";

            _salesmanOrders = _orderRepository.GetAll().Where(n => n.DocumentIssuerUser.Id == user.Id).ToList();
            if (HasUndispatchedOrders(_salesmanOrders))
            {
                msg += "  - Selected salesman has undispatched order(s).";
            }
            if (HasInventory(user))
            {
                msg += "  - Selected salesman has unreturned inventory.";
            }
            return msg;
        }

        bool HasUndispatchedOrders(List<Order> orders)
        {
            int cnt = 0;
            cnt = orders.Where(n => n.Status == DocumentStatus.OrderPendingDispatch).Count();

            cnt += LoadPartiallyDispatchedOrders(orders).Count();

            return cnt > 0;
        }

        public bool HasInventory(User user)
        {
            var salesmanInv = _inventoryRepository.GetByWareHouseId(user.CostCentre);

            if (salesmanInv.Sum(n => n.Balance) > 0)
                return true;

            return false;
        }

        public bool CheckHasOutStandingPayments(Outlet outlet)
        {
            return OutletOrdersOutstandingPayments(outlet.Id).Count > 0;
        }

        public List<Order> OutletOrdersOutstandingPayments(Guid outletId)
        {
            List<Order> orders = new List<Order>();
            LoadAllPaymentInfos();

            var orderIds = OrderPaymentInfos.Where(n => n.AmountDue > 0).Select(n => n.OrderId).ToList();
            orderIds.Distinct().ToList()
                .ForEach(n =>
                {
                    Order order = null;
                    order = _orderRepository.GetAll().OfType<Order>().First(o => o.Id == n);
                    //use GetAll() to fetch from cache.
                    if (order != null && order.OrderType == OrderType.OutletToDistributor ||
                        order.OrderType == OrderType.DistributorPOS)
                    {
                        orders.Add(order);
                    }
                }
                );
            orders = orders.Where(n => n.IssuedOnBehalfOf.Id == outletId).ToList();
            return orders;
        }

        public string CheckRouteHasOutlets(Guid routeId, out List<Outlet> routeOutlets)
        { 
            string info = "";
                var _routeOutlets = new List<Outlet>();
                List<Outlet> outlets = _costCentreRepository.GetAll().OfType<Outlet>().ToList();
                routeOutlets = _routeOutlets = outlets.Where(n => n.Route.Id == routeId).ToList();

                if (_routeOutlets.Count > 0)
                {
                    string outletnames = "";
                    _routeOutlets.Aggregate(outletnames,
                                            (current, item) =>
                                            current + (item.Name + ".\n"));
                    info = "The following outlets have been assigned ths route :" + outletnames
                           +
                           "\nClick OK to deactivate the outlets automatically or cancel to stop deactivating this route.";
                }
                return info;
        }

        public string CheckRouteHasOrders(Guid routeId)
        {
            string info = "";

            List<Order> allOrders = _orderRepository.GetAll()
                .OfType<Order>()
                .Where(n =>
                           {
                               Route route = null;
                               try
                               {
                                   route = ((Outlet) n.IssuedOnBehalfOf).Route;
                               }
                               catch
                               {
                               }
                               if (route != null)
                                   return route.Id == routeId;
                               return false;
                           }).ToList();
            if (allOrders.Count > 0)
            {
                info +=
                    "\nThere are orders for this route which will not be visible after this route is deactivated." +
                    "\nDo you still want to deactivate this route?";
            }

            return info;
        }

        public void LoadAllPaymentInfos()
        {
            OrderPaymentInfos.Clear();
            var invoiceList = new List<Invoice>();
            var creditNoteList = new List<CreditNote>();
            //invoiceList = _documentRepository.GetAll().OfType<Invoice>()
            //    .Where(n => n.Status != DocumentStatus.Rejected).ToList();
            invoiceList = _invoiceRepository.GetAll().OfType<Invoice>().Where(n => n.Status != DocumentStatus.Rejected).ToList();

            //InvoiceCreditNotes = _creditNoteService.GetAll();

            foreach (var inv in invoiceList)
            {
                //var invoiceReceipts = _documentRepository.GetAll().OfType<Receipt>().Where(n => n.InvoiceId == inv.Id);
                var invoiceReceipts = _receiptRepository.GetReceipts(inv.Id);
                var receiptsTotal = new decimal();
                if (invoiceReceipts != null)
                {
                    foreach (var r in invoiceReceipts)
                    {
                        receiptsTotal = receiptsTotal + r.Total;
                    }
                }

                //var invoiceCreditNotes = _documentRepository.GetAll().OfType<CreditNote>().Where(n => n.InvoiceId == inv.Id);
                var invoiceCreditNotes = _creditNoteRepository.GetCreditNotesByInvoiceId(inv.Id);
                var creditNotesTotals = new decimal();
                if (invoiceCreditNotes != null)
                {
                    foreach (var cn in invoiceCreditNotes)
                    {
                        creditNotesTotals = creditNotesTotals + cn.Total;
                    }
                }

                OrderPaymentInfos.Add(new InvoicePaymentInfo
                {
                    InvoiceId = inv.Id,
                    OrderId = inv.OrderId,
                    InvoiceAmount = inv.TotalGross,
                    AmountPaid = receiptsTotal,
                    CreditNoteAmount = creditNotesTotals,
                    AmountDue = (inv.TotalGross - creditNotesTotals) - receiptsTotal,
                    InvoiceDate = inv.DocumentDateIssued
                });
            }
        }

        public IEnumerable<Order> LoadPartiallyDispatchedOrders(List<Order> orders)
        {
            orders = orders.Where(n => n.OrderType == OrderType.OutletToDistributor && n.Status == DocumentStatus.OrderDispatchedToPhone).ToList();
            orders = orders.Where(n => n.LineItems.Any(l => l.LineItemType == OrderLineItemType.ProcessedBackOrder)).ToList();

            List<Order> withUndispatched = new List<Order>();
            foreach (var ord in orders)
            {
                var dns = _dispatchNoteRepository.GetAll().Where(n => n.OrderId == ord.Id && n.DispatchType == DispatchNoteType.DispatchToPhone).ToList();

                if (hasItemsToBeDispatched(ord, dns))
                {
                    withUndispatched.Add(ord);
                }
            }

            return withUndispatched;
        }

        bool hasItemsToBeDispatched(Order ord, List<DispatchNote> dns)
        {
            bool has = false;
            foreach (var procesdBO in ord.LineItems.Where(n => n.LineItemType == OrderLineItemType.ProcessedBackOrder))
            {
                //get oli's confirmed line item
                var confirmed = ord.LineItems.FirstOrDefault(n => n.Description == procesdBO.Description
                                                                  && n.LineItemType == OrderLineItemType.PostConfirmation);
                if (confirmed == null)
                {
                    confirmed = ord.LineItems.FirstOrDefault(n => n.Id.ToString() == procesdBO.Description && n.LineItemType == OrderLineItemType.Discount);
                }

                var liBo = ord.LineItems.FirstOrDefault(n => n.Description == procesdBO.Description && n.LineItemType == OrderLineItemType.BackOrder);

                decimal liDispatchedQty = dns.SelectMany(n => n.LineItems.Where(l => l.Description == confirmed.Description)).Sum(s => s.Qty);
                if (liDispatchedQty < (confirmed.Qty - (liBo.Qty - procesdBO.Qty)))
                {
                    has = true;
                }

            }

            return has;
        }

        public bool TargetPeriodUsed(TargetPeriod targetPeriod)
        {
            var targetUsingThisPeriod = _targetRepository.GetAll(true).Where(n => n.TargetPeriod.Id == targetPeriod.Id);
            if (targetUsingThisPeriod.Count() > 0)
                return true;

            return false;
        }

    }
}
