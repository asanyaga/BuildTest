using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Data.Resources;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Workflow;
using StructureMap;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    public class MainOrderRepository : DocumentRepository, IMainOrderRepository
    {

        public MainOrderRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository,
                                   IUserRepository userRepository, IProductRepository productRepository)
            : base(ctx, costCentreRepository, userRepository, productRepository)
        {
          
        }

        public void CancelDocument(Guid documentId)
        {
            throw new NotImplementedException();
        }

        public MainOrder GetById(Guid Id)
        {
            List<tblDocument> orders = _ctx.tblDocument.Where(s => s.OrderParentId == Id).ToList();

            MainOrder order = Map(orders);
            return order;

        }

        public MainOrder GetByDocumentReference(string docReference)
        {
            List<tblDocument> orders =
                _ctx.tblDocument.Where(s => s.DocumentReference.ToLower() == docReference.ToLower()).ToList();
            MainOrder order = Map(orders);
            return order;
        }

        public IPagedDocumentList<MainOrderSummary> PagedPurchaseDocumentList(int page, int pageSize, DateTime startdate,
                                                                              DateTime endDate,
                                                                              DocumentStatus? documentStatus,
                                                                              Guid? distributrId = null,
                                                                              string search = "")
        {
            IQueryable<tblDocument> orders =
                _GetAll(DocumentType.Order, startdate, endDate).Where(s => s.OrderParentId == s.Id);
            orders = orders.Where(s => s.OrderOrderTypeId == (int) OrderType.DistributorToProducer);
            if (!string.IsNullOrEmpty(search))
                orders = orders.Where(p => p.DocumentReference.ToLower().Contains(search.ToLower()));

            if (distributrId.HasValue && distributrId != Guid.Empty)
            {
                orders =
                    orders.Where(
                        s =>
                        s.DocumentRecipientCostCentre == distributrId.Value ||
                        s.DocumentIssuerCostCentreId == distributrId.Value);
            }
            if (documentStatus.HasValue && documentStatus != 0)
            {
                orders = orders.Where(s => s.DocumentStatusId == (int) documentStatus);
            }

            PagedDocumentList<MainOrderSummary> pages;
            pages = new PagedDocumentList<MainOrderSummary>(orders.Count(), page, pageSize);
            int skip = Skip(page, pageSize);
            var data = orders.OrderByDescending(d => d.IM_DateCreated).Skip(skip).Take(pageSize).ToList();
            pages.AddRange(data.Select(s => MapSummary(s)).ToList());
            return pages;
        }

        public bool HasOrdersPendingDispatch(Guid salesmanId)
        {
            return _ctx.tblDocument.Count(s => s.DocumentIssuerUserId == salesmanId) > 0;

        }

        public IEnumerable<MainOrderSummary> GetApproveOrderAndDateProcessedList(DateTime startdate, DateTime endDate,
                                                                                 Guid? salesmanId)
        {
            IQueryable<tblDocument> invoices = _GetAll(DocumentType.Invoice, startdate, endDate);
            var orderIds = invoices.Select(s => s.DocumentParentId);
            var orders =
                _ctx.tblDocument.Where(
                    s =>
                    orderIds.Contains(s.Id) && s.IM_DateLastUpdated >= startdate &&
                    s.OrderOrderTypeId == (int) OrderType.OutletToDistributor).AsQueryable(); //
            if (salesmanId != null && salesmanId != Guid.Empty)
            {
                orders =
                    orders.Where(
                        p => p.DocumentIssuerCostCentreId == salesmanId || p.DocumentRecipientCostCentre == salesmanId);
            }

            return orders.ToList().Select(s => MapSummary(s, false, true)).ToList();
        }

        public IEnumerable<MainOrderSummary> GetPendingOrderAndDateProcessedList(DateTime startdate, DateTime endDate,
                                                                                 Guid? salesmanId)
        {
            IQueryable<tblDocument> orders =
                _GetAll(DocumentType.Order, startdate, endDate).Where(s => s.OrderParentId == s.Id);
            orders = orders.Where(
                s =>
                s.DocumentStatusId == (int) DocumentStatus.Confirmed &&
                s.OrderOrderTypeId == (int) OrderType.OutletToDistributor)
                .Where(s => s.tblLineItems.All(p => p.LineItemStatusId == (int) MainOrderLineItemStatus.Confirmed));

            if (salesmanId != null && salesmanId != Guid.Empty)
            {
                orders =
                    orders.Where(
                        p => p.DocumentIssuerCostCentreId == salesmanId || p.DocumentRecipientCostCentre == salesmanId);
            }

            return orders.ToList().Select(s => MapSummary(s, false, true)).ToList();
        }

        public IEnumerable<MainOrderSummary> GetApproveOrderAndDateProcessedList(DateTime startdate, DateTime endDate, Guid? salesmanId, Guid? outletId)
        {
            IQueryable<tblDocument> invoices = _GetAll(DocumentType.Invoice, startdate, endDate);
            var orderIds = invoices.Select(s => s.DocumentParentId);
            var orders =
                _ctx.tblDocument.Where(
                    s =>
                    orderIds.Contains(s.Id) && s.IM_DateLastUpdated >= startdate &&
                    s.OrderOrderTypeId == (int)OrderType.OutletToDistributor).AsQueryable(); //
            if (salesmanId != null && salesmanId != Guid.Empty)
            {
                orders =orders.Where(p => p.DocumentIssuerCostCentreId == salesmanId ||p.DocumentRecipientCostCentre==salesmanId );

            }

            if (outletId != null && outletId != Guid.Empty)
            {
                orders =
                    orders.Where(
                        p => p.OrderIssuedOnBehalfOfCC == outletId);
            }

            return orders.ToList().Select(s => MapSummary(s, false, true)).ToList();
        }

        public QueryResult<MainOrderSummary> Query(QueryStandard query, DateTime startdate, DateTime endDate, DocumentStatus? documentStatus, Guid? distributrId = null)
        {
            IQueryable<tblDocument> orders;
            orders = _GetAll(DocumentType.Order, startdate, endDate).Where(s => s.OrderParentId == s.Id);
            orders = orders.Where(s => s.OrderOrderTypeId == (int)OrderType.DistributorToProducer);
            

            if (distributrId.HasValue && distributrId != Guid.Empty)
            {
                orders =
                    orders.Where(
                        s =>
                        s.DocumentRecipientCostCentre == distributrId.Value ||
                        s.DocumentIssuerCostCentreId == distributrId.Value);
            }
            if (documentStatus.HasValue && documentStatus != 0)
            {
                orders = orders.Where(s => s.DocumentStatusId == (int)documentStatus);
            }

            var queryResult = new QueryResult<MainOrderSummary>();

            if (!string.IsNullOrWhiteSpace(query.Name))
                orders = orders.Where(p => p.DocumentReference.ToLower().Contains(query.Name.ToLower()));

            queryResult.Count = orders.Count();

            orders = orders.OrderBy(k => k.Id);

            if (query.Skip.HasValue && query.Take.HasValue)
                orders = orders.Skip(query.Skip.Value).Take(query.Take.Value);

            var result = orders.ToList();

            queryResult.Data = result.Select(k=>MapSummary(k)).ToList();
            query.ShowInactive = false;
            return queryResult;
        }

        private MainOrder Map(List<tblDocument> orders)
        {
            var tblMainOrder = orders.FirstOrDefault(s => s.Id == s.OrderParentId);

            if (tblMainOrder == null) return null;
            var mo = PrivateConstruct<MainOrder>(tblMainOrder.Id);
            mo.OrderStatus = tblMainOrder.OrderStatusId == null
                                 ? OrderStatus.None
                                 : (OrderStatus) tblMainOrder.OrderStatusId.Value;
            mo.ShipToAddress = tblMainOrder.ShipToAddress;
            mo.Note = tblMainOrder.Note;
            mo.ExternalDocumentReference = tblMainOrder.ExtDocumentReference;
            if (tblMainOrder.OrderParentId != null) 
                mo.ParentId = tblMainOrder.OrderParentId.Value;
            if (tblMainOrder.OrderOrderTypeId != null) 
                mo.OrderType = (OrderType) tblMainOrder.OrderOrderTypeId.Value;
            if (tblMainOrder.SaleDiscount != null) 
                mo.SaleDiscount = tblMainOrder.SaleDiscount.Value;
            if (tblMainOrder.OrderDateRequired != null)
                mo.DateRequired = tblMainOrder.OrderDateRequired.Value;
            MapPaymentInfo(tblMainOrder, mo);
            _Map(tblMainOrder, mo);

            mo.PaidAmount = GetPaidAmount(tblMainOrder);

            mo.OutstandingAmount = GetOutstandingAmount(tblMainOrder);

            if (tblMainOrder.OrderIssuedOnBehalfOfCC != null)
                mo.IssuedOnBehalfOf = _costCentreRepository.GetById(tblMainOrder.OrderIssuedOnBehalfOfCC.Value);

            foreach (var order in orders)
            {
                SubOrder subOrder = PrivateConstruct<SubOrder>(order.Id);
                List<SubOrderLineItem> subborderLineItem = order.tblLineItems.Select(MapSubOrderLineItem).ToList();
                foreach (var item in subborderLineItem)
                {
                    subOrder.AddLineItem(item);
                }
                _Map(order, subOrder);
                if (order.OrderParentId != null) subOrder.ParentId = order.OrderParentId.Value;
                if (order.OrderIssuedOnBehalfOfCC != null)
                    subOrder.IssuedOnBehalfOf = _costCentreRepository.GetById(order.OrderIssuedOnBehalfOfCC.Value);
                if (order.OrderOrderTypeId != null)
                    subOrder.OrderType = (OrderType) order.OrderOrderTypeId.Value;
                if (tblMainOrder.OrderDateRequired != null)
                    subOrder.DateRequired = order.OrderDateRequired != null
                                                ? order.OrderDateRequired.Value
                                                : DateTime.MinValue;

                AddSubDocument(subOrder, mo);
                subOrder.EnableAddCommands();
            }

            return mo;
        }

        private static void MapPaymentInfo(tblDocument tblMainOrder, MainOrder mo)
        {
            foreach (var paymentInfo in tblMainOrder.tblOrderPaymentInfo)
            {

                var item = new PaymentInfo
                               {
                                   Amount = paymentInfo.Amount,
                                   ConfirmedAmount = paymentInfo.ConfirmedAmount,
                                   Id = paymentInfo.Id,
                                   Description = paymentInfo.Description,
                                   IsConfirmed = paymentInfo.IsConfirmed.Value,
                                   IsProcessed = paymentInfo.IsProcessed.Value,
                                   MMoneyPaymentType = paymentInfo.MMoneyPaymentType,
                                   NotificationId = paymentInfo.NotificationId,
                                   PaymentModeUsed = (PaymentMode) paymentInfo.PaymentMode,
                                   PaymentRefId = paymentInfo.PaymentRefId,
                                   Bank = paymentInfo.BankCode,
                                   BankBranch = paymentInfo.BranchCode,
                                   DueDate = paymentInfo.ChequeDueDate
                               };
                if (paymentInfo.ChequeDueDate.HasValue)
                    paymentInfo.ChequeDueDate = paymentInfo.ChequeDueDate.Value;

                mo.AddPayment(item);
            }
        }

        private SubOrderLineItem


            MapSubOrderLineItem(tblLineItems s)
        {
            SubOrderLineItem item = DocumentLineItemPrivateConstruct<SubOrderLineItem>(s.id);


            item.Description = s.Description;
            if (s.LineItemSequenceNo != null) item.LineItemSequenceNo = s.LineItemSequenceNo.Value;
            if (s.Vat != null)
                item.LineItemVatValue = s.Vat.Value;
            item.Product = s.ProductID == null ? null : _productRepository.GetById(s.ProductID.Value);
            //if (s.ProductDiscount != null) item.ProductDiscount = s.ProductDiscount.Value;
            if (s.Quantity != null) item.Qty = s.Quantity.Value;
            if (s.Value != null) item.Value = s.Value.Value;
            if (s.ProductDiscount != null) item.ProductDiscount = s.ProductDiscount.Value;
            if (s.DiscountLineItemTypeId != null) item.DiscountType = (DiscountType) s.DiscountLineItemTypeId;
            if (s.LostSaleQuantity != null) item.LostSaleQuantity = s.LostSaleQuantity.Value;
            if (s.ApprovedQuantity != null) item.ApprovedQuantity = s.ApprovedQuantity.Value;
            if (s.BackOrderQuantity != null) item.BackOrderQuantity = s.BackOrderQuantity.Value;
            if (s.DispatchedQuantity != null) item.DispatchedQuantity = s.DispatchedQuantity.Value;
            item.InitialQuantity = s.InitialQuantity;
            if (s.LineItemStatusId != null) item.LineItemStatus = (MainOrderLineItemStatus) s.LineItemStatusId;
            if (s.OrderLineItemType != null) item.LineItemType = (MainOrderLineItemType) s.OrderLineItemType;
            return item;
        }

        private void AddSubDocument(SubOrder subOrder, MainOrder order)
        {

            MethodInfo info = typeof (MainOrder)
                .GetMethod("AddSubOrder", BindingFlags.NonPublic | BindingFlags.Instance);
            info.Invoke(order, new[] {subOrder});
        }

        public List<MainOrder> GetAll()
        {
            throw new NotImplementedException();


        }

        public List<MainOrder> GetAll(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MainOrderSummary> GetMainOrderSummariyList(DateTime startdate, DateTime endDate,
                                                                      OrderType orderType,
                                                                      DocumentStatus? documentStatus = null,
                                                                      string search = "")
        {
            bool calculateOutStandingPayment = false;
            IQueryable<tblDocument> orders =
                _GetAll(DocumentType.Order, startdate, endDate).Where(s => s.OrderParentId == s.Id);
            if (!string.IsNullOrEmpty(search))
                orders = orders.Where(p => p.DocumentReference.ToLower().StartsWith(search.ToLower()));
            if ((int) orderType != 0)
                //Go:we want outstanding for all order types,we pass 0 for outanding listing for all transactions
                orders = orders.Where(s => s.OrderOrderTypeId == (int) orderType);
            switch (documentStatus)
            {
                case DocumentStatus.Confirmed:
                    // calculateOutStandingPayment = true;
                    orders = GetPendingApproval(orders);
                    break;
                case DocumentStatus.Approved:
                    calculateOutStandingPayment = true;
                    orders = GetPendingDispatch(orders, startdate, endDate);
                    break;
                case DocumentStatus.Dispatched:
                    calculateOutStandingPayment = true;
                    orders = GetDispatched(orders);
                    break;
                case DocumentStatus.OrderBackOrder:
                    orders = GetBackOrder(orders);
                    break;
                case DocumentStatus.OrderLossSale:
                    orders = GetOrderLossSale(orders);
                    break;
                case DocumentStatus.Rejected:
                    orders = GetRejected(orders);
                    break;
                case DocumentStatus.Closed:
                    calculateOutStandingPayment = true;
                    orders = GetClosed(orders);
                    break;
                case DocumentStatus.Outstanding:
                    calculateOutStandingPayment = true;
                    orders = GetOutStandingPayment(orders, startdate, endDate);
                    break;
                case DocumentStatus.UnconfirmedReceiptPayment:
                    calculateOutStandingPayment = true;
                    orders = GetUnconfirmedPayment(orders);
                    break;
                case DocumentStatus.FullyPaidOrders:
                    calculateOutStandingPayment = true;
                    orders = GetFullyPaidOrders(orders);
                    break;
                default:
                    orders = GetPendingConfirmation(orders);
                    break;

            }
            var data =
                orders.OrderByDescending(p => p.DocumentDateIssued).ToList().Select(
                    s => MapSummary(s, calculateOutStandingPayment));
            var count = data.Count();
            return data;

        }

        public IPagedDocumentList<MainOrderSummary> PagedDocumentList(int page, int pageSize, DateTime startdate,
                                                                      DateTime endDate, OrderType orderType,
                                                                      DocumentStatus? documentStatus = null,
                                                                      string search = "")
        {
            endDate=new DateTime(endDate.Year,endDate.Month,endDate.Day,23,59,0);
            bool calculateOutStandingPayment = false;
            IQueryable<tblDocument> orders =
                _GetAll(DocumentType.Order, startdate, endDate).Where(s => s.OrderParentId == s.Id);
            if (!string.IsNullOrEmpty(search))
                orders = orders.Where(p => p.DocumentReference.ToLower().StartsWith(search.ToLower()));
            if ((int) orderType != 0)
                //Go:we want outstanding for all order types,we pass 0 for outanding listing for all transactions
                orders = orders.Where(s => s.OrderOrderTypeId == (int) orderType);


            switch (documentStatus)
            {
                case DocumentStatus.Confirmed:
                    // calculateOutStandingPayment = true;
                   // orders = GetPendingApproval(orders);
                    return GetPendingApproval(page, pageSize, startdate, endDate,orderType);
                    break;
                case DocumentStatus.Approved:
                    orders = GetPendingDispatch(orders, startdate, endDate);
                    break;
                case DocumentStatus.Dispatched:
                    orders = GetDispatched(orders);
                    break;
                case DocumentStatus.OrderBackOrder:
                    orders = GetBackOrder(orders);
                    break;
                case DocumentStatus.OrderLossSale:
                    orders = GetOrderLossSale(orders);
                    break;
                case DocumentStatus.Rejected:
                    orders = GetRejected(orders);
                    break;
                case DocumentStatus.Closed:
                    calculateOutStandingPayment = true;
                    orders = GetClosed(orders);
                    break;
                case DocumentStatus.Outstanding:
                    calculateOutStandingPayment = true;
                    orders = GetOutStandingPayment(orders,startdate,endDate);
                    break;
                case DocumentStatus.UnconfirmedReceiptPayment:
                    calculateOutStandingPayment = true;
                    orders = GetUnconfirmedPayment(orders);
                    break;
                case DocumentStatus.FullyPaidOrders:
                    calculateOutStandingPayment = true;
                    orders = GetFullyPaidOrders(orders);
                    break;
                default:
                    orders = GetPendingConfirmation(orders);
                    break;

            }
            var count = orders.Count();

            PagedDocumentList<MainOrderSummary> pages;
            pages = new PagedDocumentList<MainOrderSummary>(count, page, pageSize);
            int skip = Skip(page, pageSize);
            var data = orders.OrderByDescending(d => d.IM_DateCreated).Skip(skip).Take(pageSize).ToList();

            pages.AddRange(data.Select(s => MapSummary(s, calculateOutStandingPayment)).ToList());
            return pages;
        }



        public List<MainOrderSummary> GetPendingDispatch(Guid? routeId, Guid? salesmanId)
        {
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            IQueryable<tblDocument> orders =
                _GetAll(DocumentType.Order, startDate, endDate).Where(s => s.OrderParentId == s.Id).Where(
                    s => s.OrderOrderTypeId == (int) OrderType.OutletToDistributor);
            if (routeId.HasValue && !salesmanId.HasValue)
            {
                List<Guid> salesmanIds =
                    _ctx.tblSalemanRoute.Where(s => s.RouteId == routeId.Value).Select(s => s.SalemanId).ToList();
                orders =
                    orders.Where(s =>
                                 salesmanIds.Contains(s.DocumentIssuerCostCentreId) ||
                                 salesmanIds.Contains(s.DocumentRecipientCostCentre));
            }
            else if (salesmanId.HasValue)
            {
                orders =
                    orders.Where(s =>
                                 s.DocumentIssuerCostCentreId == salesmanId.Value ||
                                 s.DocumentRecipientCostCentre == salesmanId.Value);
            }
            orders = GetPendingDispatch(orders, startDate, endDate);
            return orders.ToList().Select(s => MapSummary(s, false)).ToList();
        }

        public List<MainOrderSummary> GetPurchaseOrderPendingReceive()
        {
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            IQueryable<tblDocument> orders = _GetAll(DocumentType.Order, startDate, endDate)
                .Where(s => s.OrderParentId == s.Id)
                .Where(s => s.OrderOrderTypeId == (int) OrderType.DistributorToProducer);
            //orders = GetPendingDispatch(orders,startDate,endDate).Where(s => s.DocumentStatusId != (int) DocumentStatus.Closed);

            string sql = string.Format(Resources.MainOrderResource.PurchaseOrdersPendingDispatch, startDate.ToString("yyyy-MM-dd HH:mm:ss"), endDate.ToString("yyyy-MM-dd HH:mm:ss"));

            var data = _ctx.ExecuteStoreQuery<Guid>(sql).ToList();

            var pendingOrders = orders.Where(s => data.Contains(s.Id)).Where(s => s.DocumentStatusId != (int)DocumentStatus.Closed);

            orders = pendingOrders.AsQueryable();
           
            return orders.ToList().Select(s => MapSummary(s, false)).ToList();
        }

        private IQueryable<tblDocument> GetUnconfirmedPayment(IQueryable<tblDocument> orders)
        {
            var unconfirmedOrders = from o in orders
                                    let receiptsSum = (decimal?) (from d in _ctx.tblDocument
                                                                  where d.DocumentTypeId == (int) DocumentType.Receipt
                                                                  where d.DocumentParentId == o.Id

                                                                  from l in d.tblLineItems
                                                                      .Where(
                                                                          p =>
                                                                          p.OrderLineItemType ==
                                                                          (int) OrderLineItemType.DuringConfirmation)
                                                                  select new {l.Value})
                                                                     .Sum(x => x.Value) ?? 0
                                    select o;

            return unconfirmedOrders;

        }

        private IQueryable<tblDocument> GetFullyPaidOrders(IQueryable<tblDocument> orders)
        {
            var fullyPaid = from o in orders
                            let invoicesSum = (decimal?) (from d in _ctx.tblDocument
                                                          where d.DocumentTypeId == (int) DocumentType.Invoice
                                                          where d.InvoiceOrderId == o.Id
                                                          from l in d.tblLineItems
                                                          select new {l.Quantity, l.Value, l.Vat})
                                                             .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0
                            let receiptsSum = (decimal?) (from d in _ctx.tblDocument
                                                          where d.DocumentTypeId == (int) DocumentType.Receipt
                                                          where d.DocumentParentId == o.Id
                                                          from l in d.tblLineItems
                                                          select new {l.Value})
                                                             .Sum(x => x.Value) ?? 0
                            let creditNoteSum = (decimal?) (from d in _ctx.tblDocument
                                                            where d.DocumentTypeId == (int) DocumentType.CreditNote
                                                            where d.DocumentParentId == o.Id
                                                            from l in d.tblLineItems
                                                            select new {l.Quantity, l.Value, l.Vat})
                                                               .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0
                            let salediscount = (decimal?) o.SaleDiscount ?? 0

                            where (salediscount + receiptsSum + creditNoteSum) >= invoicesSum && (invoicesSum > 0)
                            select o;



            return fullyPaid;
        }

        private IQueryable<tblDocument> GetOutStandingPayment(IQueryable<tblDocument> orders, DateTime startDate, DateTime endDate, int pageSize = 10)
        {
            
            string sql = string.Format(Resources.MainOrderResource.Outstanding, startDate.ToString("yyyy-MM-dd HH:mm:ss"), endDate.ToString("yyyy-MM-dd HH:mm:ss"));
            var data = _ctx.ExecuteStoreQuery<Guid>(sql).ToList();
            var outstandingOrders = orders.Where(s => data.Contains(s.Id)).AsQueryable();
            return outstandingOrders;
           
        }

        private IQueryable<tblDocument> GetClosed(IQueryable<tblDocument> orders)
        {
            IQueryable<Guid> ids = orders.Where(s => s.DocumentStatusId == (int) DocumentStatus.Closed)
                .Select(t => t.Id).Distinct();
            orders = orders.Where(s => ids.Contains(s.Id));
            return orders;
        }

        private IQueryable<tblDocument> GetRejected(IQueryable<tblDocument> orders)
        {
            IQueryable<Guid> ids = orders
                .SelectMany(s => s.tblLineItems)
                .Select(t => t.tblDocument.Id).Distinct();
            IQueryable<Guid> withRejected = _ctx.tblDocument
                .Where(s => s.DocumentStatusId == (int) DocumentStatus.Rejected)
                .SelectMany(s => s.tblLineItems)
                .Where(p => ids.Contains(p.tblDocument.OrderParentId.Value))
                .Select(t => t.tblDocument.OrderParentId.Value).Distinct();
            orders = orders.Where(s => withRejected.Contains(s.Id));
            return orders;

        }

        private IQueryable<tblDocument> GetOrderLossSale(IQueryable<tblDocument> orders)
        {
            IQueryable<Guid> ids = orders
                .SelectMany(s => s.tblLineItems)
                .Select(t => t.tblDocument.Id).Distinct();
            IQueryable<Guid> withlosssale = _ctx.tblDocument
                .SelectMany(s => s.tblLineItems)
                .Where(s => s.LostSaleQuantity > 0)
                .Where(p => ids.Contains(p.tblDocument.OrderParentId.Value))
                .Select(t => t.tblDocument.OrderParentId.Value).Distinct();
            orders = orders.Where(s => withlosssale.Contains(s.Id));

            return orders;
        }

        private IQueryable<tblDocument> GetBackOrder(IQueryable<tblDocument> orders)
        {
            IQueryable<Guid> ids = orders
                .SelectMany(s => s.tblLineItems)
                // .Where(s => s.LineItemStatusId == (int)MainOrderLineItemStatus.Approved)
                .Select(t => t.tblDocument.Id).Distinct();
            IQueryable<Guid> withbackorder = _ctx.tblDocument
                .Where(p => p.Id != p.OrderParentId.Value)
                .SelectMany(s => s.tblLineItems)
                .Where(s => s.LineItemStatusId == (int) MainOrderLineItemStatus.Confirmed)
                .Where(p => ids.Contains(p.tblDocument.OrderParentId.Value))
                .Select(t => t.tblDocument.OrderParentId.Value).Distinct();
            orders = orders.Where(s => withbackorder.Contains(s.Id));
            return orders;
        }

        private IQueryable<tblDocument> GetPendingConfirmation(IQueryable<tblDocument> orders)
        {
            IQueryable<Guid> ids = orders
                .SelectMany(s => s.tblLineItems)
                .Where(s => s.LineItemStatusId == (int) MainOrderLineItemStatus.New)
                .Select(t => t.tblDocument.Id).Distinct();
            orders = orders.Where(s => ids.Contains(s.Id));
            return orders;
        }

        private IQueryable<tblDocument> GetDispatched(IQueryable<tblDocument> orders)
        {
            IQueryable<Guid> ids = orders.Where(s => s.DocumentStatusId != (int) DocumentStatus.Closed)
                .SelectMany(s => s.tblLineItems)
                .Where(s => s.LineItemStatusId == (int) MainOrderLineItemStatus.Dispatched)
                .Select(t => t.tblDocument.Id).Distinct();
            orders = orders.Where(s => ids.Contains(s.Id));
            return orders;
        }

        private IPagedDocumentList<MainOrderSummary> GetPendingApproval(int page, int pageSize, DateTime startdate,
            DateTime endDate, OrderType orderType)
        {
            int pageStart = ((page - 1)*pageSize) + 1;
            int pageEnd = (pageStart + pageSize) - 1;
            string sql = string.Format(MainOrderResource.PendingApproval, pageStart, pageEnd, (int) orderType,
                startdate.ToString("yyyy-MM-dd HH:mm:ss"), endDate.ToString("yyyy-MM-dd HH:mm:ss"));
            var result = _ctx.ExecuteStoreQuery<MainOrderSummary>(sql).ToList();
            int count = 0;
            if (result.Any())
            {
                count = result.FirstOrDefault().RowCount;
            }
            var pages = new PagedDocumentList<MainOrderSummary>(count, page, pageSize);
            pages.AddRange(result);
            return pages;

        }

        private static IQueryable<tblDocument> GetPendingApproval(IQueryable<tblDocument> orders)
        {

            IQueryable<Guid> ids = orders.Where(s => s.DocumentStatusId == (int) DocumentStatus.Confirmed)
                .SelectMany(s => s.tblLineItems)
                .Where(s => s.LineItemStatusId == (int) MainOrderLineItemStatus.Confirmed)
                .Select(t => t.tblDocument.Id).Distinct();
            orders = orders.Where(s => ids.Contains(s.Id));
            return orders;
        }

        private IQueryable<tblDocument> GetPendingDispatch(IQueryable<tblDocument> orders, DateTime startDate, DateTime endDate)
        {
            
            string sql = string.Format(Resources.MainOrderResource.PendingDispatch, startDate.ToString("yyyy-MM-dd HH:mm:ss"), endDate.ToString("yyyy-MM-dd HH:mm:ss"));
            //_ctx.CommandTimeout = 120;
           var data = _ctx.ExecuteStoreQuery<Guid>(sql).ToList();
            int input = 2;
           
            var pendingorders = orders.Where(s => data.Contains(s.Id)).AsQueryable();
            return pendingorders;
            


            //IQueryable<Guid> ids = _ctx.tblDocument.Where(s => mainOrderIds.Contains(s.DocumentParentId.Value))
            //    .SelectMany(s => s.tblLineItems)
            //    .Where(s => s.LineItemStatusId == (int) MainOrderLineItemStatus.Approved)
            //    .Select(t => t.tblDocument.DocumentParentId.Value).Distinct();
            //orders = orders.Where(s => ids.Contains(s.Id));
            //return orders;
        }


        internal MainOrderSummary MapSummary(tblDocument tblDocument, bool calculateOutstanding = false,
                                             bool getapproveddate = false)
        {
            string salesman = "";
            var issuer =
                _ctx.tblCostCentre.Where(s => s.Id == tblDocument.DocumentIssuerCostCentreId).Select(
                    s => new {s.Name, s.CostCentreType.Value, s.Id}).FirstOrDefault();
            var recepient =
                _ctx.tblCostCentre.Where(s => s.Id == tblDocument.DocumentRecipientCostCentre).Select(
                    s => new {s.Name, s.CostCentreType.Value, s.Id}).FirstOrDefault();
            var outlet =
                _ctx.tblCostCentre.Where(s => s.Id == tblDocument.OrderIssuedOnBehalfOfCC).Select(
                    s => new {s.Name, s.CostCentreType.Value, s.Id}).FirstOrDefault();

            var mainOrderSummary = new MainOrderSummary();
            if (issuer != null && recepient != null && (CostCentreType) issuer.Value == CostCentreType.Distributor)
            {
                salesman = recepient.Name + "(" + issuer.Name + ")";
                mainOrderSummary.SalesmanId = recepient.Id;
            }
            else if (issuer != null && recepient != null)
            {
                salesman = issuer.Name + "(" + recepient.Name + ")";
                mainOrderSummary.SalesmanId = issuer.Id;
            }
            if (outlet != null)
            {
                mainOrderSummary.Outlet = outlet.Name;
            }
            if (getapproveddate)
            {
                var invoicedoc = _ctx.tblDocument.FirstOrDefault(s => s.DocumentParentId == tblDocument.Id);
                if (invoicedoc != null)
                    mainOrderSummary.DateProcessed = invoicedoc.DocumentDateIssued;
            }
            var lineitems =
                tblDocument.tblLineItems.Where(s => s.LineItemStatusId != (int) MainOrderLineItemStatus.Removed);

            mainOrderSummary.OrderId = tblDocument.Id;
            mainOrderSummary.Status = (DocumentStatus) tblDocument.DocumentStatusId;
            
            
            mainOrderSummary.TotalVat = lineitems.Sum(n => (n.Vat ?? 0)*(n.Quantity ?? 0));



            mainOrderSummary.GrossAmount = (lineitems.Sum(n => (n.Value ?? 0) * (n.Quantity ?? 0)).GetTruncatedValue() +
                                           mainOrderSummary.TotalVat.GetTruncatedValue()).GetTotalGross();

            //mainOrderSummary.GrossAmount = lineitems.Sum(n => DiscountProWorkflow.GetTruncatedValue((n.Value ?? 0) * (n.Quantity ?? 0))) +
            //                               mainOrderSummary.TotalVat;

            mainOrderSummary.OrderReference = tblDocument.DocumentReference;

            mainOrderSummary.Required = tblDocument.OrderDateRequired != null
                                            ? tblDocument.OrderDateRequired.Value
                                            : DateTime.Now;

            decimal salediscount = tblDocument.SaleDiscount.HasValue ? tblDocument.SaleDiscount.Value : 0;
            mainOrderSummary.SaleDiscount = salediscount;
            mainOrderSummary.NetAmount = (mainOrderSummary.GrossAmount) - mainOrderSummary.SaleDiscount;
            mainOrderSummary.Salesman = salesman;
            mainOrderSummary.ExternalRefNo = tblDocument.ExtDocumentReference ?? "";
            if (tblDocument.OrderIssuedOnBehalfOfCC != null)
                mainOrderSummary.OutletId = tblDocument.OrderIssuedOnBehalfOfCC.Value;
            if (calculateOutstanding)
            {
                var invoiceList = (from d in _ctx.tblDocument
                    where d.DocumentTypeId == (int) DocumentType.Invoice
                    where d.InvoiceOrderId == tblDocument.Id
                    from l in d.tblLineItems
                    select new {l.Quantity, l.Value, l.Vat}).ToList();

                var invoiceAmount =invoiceList.Sum(x => ((x.Quantity.Value * x.Value.Value) + (x.Vat.Value * x.Quantity.GetValueOrDefault(0))).GetTruncatedValue()).GetTotalGross();
                //var invoiceAmount = (decimal?) (from d in _ctx.tblDocument
                //                                where d.DocumentTypeId == (int) DocumentType.Invoice
                //                                where d.InvoiceOrderId == tblDocument.Id
                //                                from l in d.tblLineItems
                //                                select new {l.Quantity, l.Value, l.Vat})
                //                                   .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0;

                var receiptAmount = (decimal?) (from d in _ctx.tblDocument
                                                where d.DocumentTypeId == (int) DocumentType.Receipt
                                                where d.DocumentParentId == tblDocument.Id
                                                from l in d.tblLineItems
                                                select new {l.Value})
                                                   .Sum(x => x.Value) ?? 0;
                var creditNoteSum = (decimal?) (from d in _ctx.tblDocument
                                                where d.DocumentTypeId == (int) DocumentType.CreditNote
                                                where d.DocumentParentId == tblDocument.Id
                                                from l in d.tblLineItems
                                                select new {l.Quantity, l.Value, l.Vat})
                                                   .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0;

                decimal totalInvoiceAmount =(invoiceAmount - salediscount).GetTotalGross();
                decimal totalPaidAmount = (receiptAmount + creditNoteSum);
                decimal outstandingAmount = totalInvoiceAmount - totalPaidAmount;
                mainOrderSummary.OutstandingAmount = outstandingAmount>0?outstandingAmount:0;
                mainOrderSummary.PaidAmount = receiptAmount;

            }
            return mainOrderSummary;


        }

        protected virtual decimal GetPaidAmount(tblDocument order)
        {
            decimal paidamount =
                _ctx.ExecuteStoreQuery<decimal>(string.Format(MainOrderResource.PaidAmount, order.Id)).FirstOrDefault();
            return paidamount;
            //decimal salediscount = order.SaleDiscount.HasValue ? order.SaleDiscount.Value : 0;
            //var invoiceAmount = (decimal?) (from d in _ctx.tblDocument
            //                                where d.DocumentTypeId == (int) DocumentType.Invoice
            //                                where d.InvoiceOrderId == order.Id
            //                                from l in d.tblLineItems
            //                                select new {l.Quantity, l.Value, l.Vat})
            //                                   .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0;

            //var receiptAmount = (decimal?) (from d in _ctx.tblDocument
            //                                where d.DocumentTypeId == (int) DocumentType.Receipt
            //                                where d.DocumentParentId == order.Id
            //                                from l in d.tblLineItems
            //                                select new {l.Value})
            //                                   .Sum(x => x.Value) ?? 0;
            //var creditNoteSum = (decimal?) (from d in _ctx.tblDocument
            //                                where d.DocumentTypeId == (int) DocumentType.CreditNote
            //                                where d.DocumentParentId == order.Id
            //                                from l in d.tblLineItems
            //                                select new {l.Quantity, l.Value, l.Vat})
            //                                   .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0;


            //decimal totalPaidAmount = (receiptAmount + creditNoteSum);
           // return totalPaidAmount;
        }

        protected virtual decimal GetOutstandingAmount(tblDocument order)
        {

            decimal salediscount = order.SaleDiscount.HasValue ? order.SaleDiscount.Value : 0;

            var invoiceList = (from d in _ctx.tblDocument
                               where d.DocumentTypeId == (int)DocumentType.Invoice
                               where d.InvoiceOrderId == order.Id
                               from l in d.tblLineItems
                               select new { l.Quantity, l.Value, l.Vat }).ToList();

            var invoiceAmount = invoiceList.Sum(x => ((x.Quantity.Value * x.Value.Value) + (x.Vat.Value * x.Quantity.GetValueOrDefault(0))).GetTruncatedValue()).GetTotalGross();
            //var invoiceAmount = (decimal?) (from d in _ctx.tblDocument
            //                                where d.DocumentTypeId == (int) DocumentType.Invoice
            //                                where d.InvoiceOrderId == order.Id
            //                                from l in d.tblLineItems
            //                                select new {l.Quantity, l.Value, l.Vat})
            //                                   .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0;

            var receiptAmount = (decimal?) (from d in _ctx.tblDocument
                                            where d.DocumentTypeId == (int) DocumentType.Receipt
                                            where d.DocumentParentId == order.Id
                                            from l in d.tblLineItems
                                            select new {l.Value})
                                               .Sum(x => x.Value) ?? 0;
            var creditNoteSum = (decimal?) (from d in _ctx.tblDocument
                                            where d.DocumentTypeId == (int) DocumentType.CreditNote
                                            where d.DocumentParentId == order.Id
                                            from l in d.tblLineItems
                                            select new {l.Quantity, l.Value, l.Vat})
                                               .Sum(x => (x.Quantity*x.Value) + (x.Vat*x.Quantity)) ?? 0;

            decimal totalInvoiceAmount = (invoiceAmount - salediscount).GetTotalGross();
            decimal totalPaidAmount = (receiptAmount + creditNoteSum);

            decimal outstandingAmnt = totalInvoiceAmount - totalPaidAmount;

            return outstandingAmnt;
        }




        public virtual int GetCount()
        {
            throw new NotImplementedException();
        }


        internal virtual int GetCount(Expression<Func<tblDocument, bool>> tCriteria = null)
        {
            if (tCriteria == null)
                return GetCount();
            return _ctx.tblDocument.Count(tCriteria.Compile());
        }

       
    }
}
