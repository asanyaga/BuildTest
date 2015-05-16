using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class OrderRepository : DocumentRepository, IOrderRepository
    {
        public OrderRepository(CokeDataContext ctx, ICostCentreRepository costCenterRepository, IUserRepository userRepository, IProductRepository productRepository) :
            base(ctx, costCenterRepository, userRepository, productRepository)
        {

        }

        public List<Order> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetByRecipientCostCentre(Guid recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            IEnumerable<tblDocument> tblOrders = _GetAll(DocumentType.Order);
            var orders = tblOrders.Select(Map);
            return orders as List<Order>;
        }

        public List<Order> GetByIssuedOnBehalfOfCostCentre(Guid recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetAll(DateTime startDate, DateTime endDate)
        {
            var docs = _GetAll(DocumentType.Order, startDate, endDate);
            return docs.ToList().Select(Map).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.Order);
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }

        public void Save(Order documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            Order order = documentEntity as Order;
            docToSave.OrderDateRequired = order.DateRequired;
            docToSave.OrderIssuedOnBehalfOfCC = order.IssuedOnBehalfOf == null ? Guid.Empty : order.IssuedOnBehalfOf.Id;
            docToSave.OrderOrderTypeId = (int)order.OrderType;
            docToSave.SaleDiscount = order.SaleDiscount;
            docToSave.Note = order.Note;

            foreach (OrderLineItem lineItem in order._allLineItems())
            {
                tblLineItems ll = null;
                if (docToSave.tblLineItems.Any(p => p.id == lineItem.Id))
                    ll = docToSave.tblLineItems.First(p => p.id.ToString() == lineItem.Id.ToString());
                else
                {
                    ll = new tblLineItems();
                    ll.id = lineItem.Id;
                    docToSave.tblLineItems.Add(ll);
                }

                ll.ProductID = lineItem.Product.Id;
                ll.DocumentID = documentEntity.Id;
                ll.Description = lineItem.Description;
                ll.Quantity = lineItem.Qty;
                ll.LineItemSequenceNo = lineItem.LineItemSequenceNo;
                ll.Value = lineItem.Value;
                ll.Vat = lineItem.LineItemVatValue;
                ll.OrderLineItemType = (int)lineItem.LineItemType;
                ll.ProductDiscount = lineItem.ProductDiscount;
                ll.DiscountLineItemTypeId = (int)lineItem.DiscountType;
                ll.Description = lineItem.Description;
            }
            _ctx.SaveChanges();
        }

        public Order GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var o = Map(tblDoc);
            return o;
        }

        private Order Map(tblDocument tblDoc)
        {
            var o = new Order(tblDoc.Id);
            _Map(tblDoc, o);
            o.DateRequired = tblDoc.OrderDateRequired.Value;
            o.IssuedOnBehalfOf = _costCentreRepository.GetById(tblDoc.OrderIssuedOnBehalfOfCC.Value);
            o.OrderType = (OrderType)tblDoc.OrderOrderTypeId.Value;
            o.SaleDiscount = tblDoc.SaleDiscount.Value;
            o.Note = tblDoc.Note;
            //tblDoc.tblLineItems.Any();
            o._SetLineItems(tblDoc.tblLineItems.Select(s => MapOrderLineItem(s)).ToList());
            return o;
        }
        internal OrderLineItem MapOrderLineItem(tblLineItems s)
        {
            return new OrderLineItem(s.id)
            {
                Description = s.Description,
                LineItemSequenceNo = s.LineItemSequenceNo.Value,
                LineItemVatValue = s.Vat.Value,
                Product = s.ProductID == null ? null : _productRepository.GetById(s.ProductID.Value),
                Qty = s.Quantity.Value,
                Value = s.Value.Value,
                LineItemType = (OrderLineItemType)s.OrderLineItemType.Value,
                ProductDiscount = s.ProductDiscount.Value,
                DiscountType = (DiscountType)s.DiscountLineItemTypeId
            };
        }
        public List<Order> GetAll()
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.Order);
            return tblDocument.ToList().Select(Map).ToList();

        }

        public List<Order> GetAll(OrderType orderType, DateTime startDate, DateTime endDate, string searchText = "")
        {
            searchText = searchText.ToLower().Trim();
            IQueryable<tblDocument> docs = _GetAll(DocumentType.Order, startDate, endDate);
            docs =docs.Where(n => n.DocumentTypeId == (int) orderType && n.DocumentReference.ToLower().Contains(searchText));
            return docs.ToList().Select(Map).ToList();
        }

        public List<Order> GetAllDistributorPurchaseOrder(Guid distributorId)
        {
            var data = _GetAll(DocumentType.Order)
                .Where(n => n.DocumentIssuerCostCentreId == distributorId
                            && n.OrderOrderTypeId == (int) OrderType.DistributorToProducer
                );
            return data.ToList().Select(Map).ToList();
        }

        public List<Order> GetDistributorPurchaseOrdersToProducer(Guid distributorId, DocumentStatus status)
        {
            var docs = _GetAll(DocumentType.Order)
                .Where(n => n.DocumentIssuerCostCentreId == distributorId
                            && n.OrderOrderTypeId == (int) OrderType.DistributorToProducer
                            && n.DocumentStatusId == (int) status);

            return docs.ToList().Select(Map).ToList();
        }

        public List<Order> GetByOrderTypeAndDocumentStatus(OrderType orderType, DocumentStatus docStatus, bool ofThisStatus = true, DateTime startDate = new DateTime(), DateTime endDate = new DateTime(), string searchText = "")
        {
            if (searchText.Trim() != "")
                return SearchByOrderTypeAndDocumentStatus(orderType, docStatus, searchText, ofThisStatus, startDate, endDate);

            IQueryable<tblDocument> data;
            if (ofThisStatus)
            {
                if (startDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order)
                        .Where(n => n.OrderOrderTypeId == (int)orderType
                               && n.DocumentStatusId == (int)docStatus
                        );
                else
                {
                    data = _GetAll(DocumentType.Order, startDate,endDate)
                        .Where(n => n.OrderOrderTypeId == (int)orderType
                               && n.DocumentStatusId == (int)docStatus);
                }
                return data.Select(Map).Select(n => n as Order).ToList();
            }
            if (startDate.Equals(new DateTime()))
                data = _GetAll(DocumentType.Order)
                        .Where(n => n.OrderOrderTypeId == (int)orderType
                                  && n.DocumentStatusId != (int)docStatus
                    );
            else
            {
                data = _GetAll(DocumentType.Order, startDate,endDate)
                        .Where(n => n.OrderOrderTypeId == (int)orderType
                                  && n.DocumentStatusId != (int)docStatus);
            }
            return data.ToList().Select(Map).ToList();
        }

        public int GetOrderCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            return GetOrdersCount(startDate, endDate, (int)OrderType.OutletToDistributor);
        }

        public int GetSaleCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            return GetOrdersCount(startDate, endDate, (int)OrderType.DistributorPOS);
        }

        public int GetPurchaseOrderCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            return GetOrdersCount(startDate, endDate, (int)OrderType.DistributorToProducer);
        }

        public int GetStockistPurchaseOrderCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            return GetOrdersCount(startDate, endDate, (int)OrderType.SalesmanToDistributor);
        }

        public int CountAllOrders(DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            return GetOrdersCount(startDate, endDate);
        }

        public List<Order> GetAllPagenated(int skip, int take, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            IQueryable<tblDocument> data;
            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order);
                else
                    data = _GetAll(DocumentType.Order,startDate, endDate);
            }
            else
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order)
                        .Where(n =>n.OrderOrderTypeId == orderType);
                else
                    data = _GetAll(DocumentType.Order, startDate, endDate)
                        .Where(n => n.OrderOrderTypeId == orderType);
            }
            data = data.OrderByDescending(n => n.DocumentDateIssued)
                       .Skip((skip - 1)*take).Take(take);
            return data.ToList().Select(Map).ToList();
        }

        //REFACTOR this can be simplified
        public List<Order> GetByDocumentStatusPagenated(int skip, int take, int documentStatus = -1, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime(), string searchText = "")
        {
            if (searchText != "")
                return SearchByDocumentStatusPagenated(skip, take, searchText, documentStatus, orderType, startDate, endDate);

            IQueryable<tblDocument> data;
            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order)
                        .Where(n =>n.DocumentStatusId == documentStatus);
                else
                {
                    data = _GetAll(DocumentType.Order, startDate, endDate)
                        .Where(n =>n.DocumentStatusId == documentStatus);
                }
            }
            else
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order)
                        .Where(n =>n.DocumentStatusId == documentStatus && n.OrderOrderTypeId == orderType);
                else
                {
                    data = _GetAll(DocumentType.Order, startDate, endDate)
                        .Where(n =>n.DocumentStatusId == documentStatus&& n.OrderOrderTypeId == orderType);
                }
            }
            data = data.OrderByDescending(n => n.DocumentDateIssued)
                      .Skip((skip - 1) * take).Take(take);

            return data.Select(Map).Select(n => n as Order).ToList();
        }


        //REFACTOR this can be simplified
        public List<Order> GetByDocumentStatusPagenated(int skip, int take, int documentStatus1, bool hasStatus1 = true, int? documentStatus2 = -1, bool? hasStatus2 = false, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime(), string searchText = "")
        {
            IQueryable<tblDocument> data;

            if (searchText != "")
                return SearchByDocumentStatusPagenated(skip, take, searchText, documentStatus1, hasStatus1,
                                                                      documentStatus2, hasStatus2, orderType, startDate,
                                                                      endDate);
            //cn:: imrpove this if u can
            //1. orders with status 1
            //2. orders not with status 1
            //3. orders with status 1 and 2
            //4. orders with status 1 but not 2

            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order)
                            .Where(n => (hasStatus1 ? n.DocumentStatusId == documentStatus1 : n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>(n.DocumentStatusId == documentStatus1 || n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>(n.DocumentStatusId != documentStatus1 && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }
                    }
                }
                else//dates set
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order, startDate, endDate )
                            .Where(n => (hasStatus1 ? n.DocumentStatusId == documentStatus1 : n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order, startDate, endDate)
                                .Where(n =>(n.DocumentStatusId == documentStatus1|| n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order, startDate, endDate)
                               .Where(n =>
                                       (n.DocumentStatusId != documentStatus1&& n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
            }
            else //orderType set
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order)
                            .Where(n =>
                                    n.OrderOrderTypeId == orderType
                                   &&(hasStatus1
                                        ? n.DocumentStatusId == documentStatus1
                                        : n.DocumentStatusId != documentStatus1)
                            );
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>
                                    n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId == documentStatus1
                                           || n.DocumentStatusId == documentStatus2)
                                );
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>
                                    n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId != documentStatus1
                                           && n.DocumentStatusId != documentStatus2)
                                );
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
                else//dates set
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order, startDate, endDate)
                            .Where(n =>
                                    n.OrderOrderTypeId == orderType
                                   && (hasStatus1
                                           ? n.DocumentStatusId == documentStatus1
                                           : n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order, startDate, endDate)
                                .Where(n =>
                                        n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId == documentStatus1
                                           || n.DocumentStatusId == documentStatus2)
                                       
                                );
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order, startDate, endDate)
                                .Where(n =>
                                        n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId != documentStatus1
                                           && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
            }
            data = data.OrderByDescending(n => n.DocumentDateIssued)
                      .Skip((skip - 1) * take).Take(take);
            return data.ToList().Select(Map).ToList();
        }

        //REFACTOR this can be simplified
        public int GetCountByDocumentStatus(int documentStatus1, bool hasStatus1 = true, int? documentStatus2 = -1, bool? hasStatus2 = false, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            int count;

            //cn:: imrpove this if u can
            //1. orders with status 1
            //2. orders not with status 1
            //3. orders with status 1 and 2
            //4. orders with status 1 but not 2

            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    if (!documentStatus2.HasValue)
                    {

                        count =  _GetAll(DocumentType.Order)
                            .Count(n => 
                                     (hasStatus1
                                          ? n.DocumentStatusId == documentStatus1
                                          : n.DocumentStatusId != documentStatus1)
                            );
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            count = _GetAll(DocumentType.Order)
                                .Count(n =>  (n.DocumentStatusId == documentStatus1
                                                || n.DocumentStatusId == documentStatus2)
                                );
                        }
                        else if (hasStatus1 && hasStatus2 == false)
                        {
                            count = _GetAll(DocumentType.Order)
                                .Count(n =>  (n.DocumentStatusId != documentStatus1
                                                && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
                else//dates set
                {
                    if (!documentStatus2.HasValue)
                    {
                        count = _GetAll(DocumentType.Order, startDate, endDate)
                            .Count(n => 
                                        (hasStatus1
                                             ? n.DocumentStatusId == documentStatus1
                                             : n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            count = _GetAll(DocumentType.Order, startDate, endDate)
                                .Count(n =>  (n.DocumentStatusId == documentStatus1
                                                || n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            count = _GetAll(DocumentType.Order, startDate,endDate)
                                .Count(n =>  (n.DocumentStatusId != documentStatus1
                                                && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
            }
            else //orderType set
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    if (!documentStatus2.HasValue)
                    {
                        count = _GetAll(DocumentType.Order)
                            .Count(n =>  n.OrderOrderTypeId == orderType
                                        &&
                                        (hasStatus1
                                             ? n.DocumentStatusId == documentStatus1
                                             : n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            count = _GetAll(DocumentType.Order)
                                .Count(n =>  n.OrderOrderTypeId == orderType
                                            && (n.DocumentStatusId == documentStatus1
                                                || n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            count = _GetAll(DocumentType.Order)
                                .Count(n =>  n.OrderOrderTypeId == orderType
                                            && (n.DocumentStatusId != documentStatus1
                                                && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
                else//dates set
                {
                    if (!documentStatus2.HasValue)
                    {
                        count = _GetAll(DocumentType.Order,startDate, endDate)
                            .Count(n =>  n.OrderOrderTypeId == orderType
                                        && (hasStatus1
                                                ? n.DocumentStatusId == documentStatus1
                                                : n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");
                        if (hasStatus1 && hasStatus2 == true)
                        {
                            count = _GetAll(DocumentType.Order, startDate   , endDate)
                                .Count(n =>  n.OrderOrderTypeId == orderType
                                            && (n.DocumentStatusId == documentStatus1
                                                || n.DocumentStatusId == documentStatus2)
                                            );
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            count = _GetAll(DocumentType.Order, startDate,endDate)
                                .Count(n =>  n.OrderOrderTypeId == orderType
                                            && (n.DocumentStatusId != documentStatus1
                                                && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
            }

            return count;
        }

        public int GetCountByDocumentStatus(int documentStatus = -1, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            int count;
            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    count = _GetAll(DocumentType.Order)
                                .Count(n =>  n.DocumentStatusId == documentStatus);
                }
                else
                {
                    count = _GetAll(DocumentType.Order, startDate, endDate)
                                .Count(n =>  n.DocumentStatusId == documentStatus);
                }
            }
            else
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    count = _GetAll(DocumentType.Order)
                                .Count(n =>  n.OrderOrderTypeId == orderType
                                            && n.DocumentStatusId == documentStatus);
                }
                else
                {
                    count = _GetAll(DocumentType.Order, startDate,endDate)
                                .Count(n =>  n.OrderOrderTypeId == orderType
                                            && n.DocumentStatusId == documentStatus);
                }
            }

            return count;
        }

        public OrderLineItem GetLineItemById(Guid lineItemId)
        {
            OrderLineItem lineItem = MapOrderLineItem(_ctx.tblLineItems.FirstOrDefault(n => n.id == lineItemId));
            return lineItem;
        }

        public bool ChangeStatus(Guid documentId, DocumentStatus status)
        {
            Order order = GetById(documentId);
            if (order == null)
                return false;
            order.Status = status;
            Save(order);
            return true;
        }

        public void SaveLineItem(OrderLineItem lineItem, Guid orderId)
        {
            tblLineItems ll = null;
            if (_ctx.tblLineItems.Any(p => p.id == lineItem.Id))
                ll = _ctx.tblLineItems.First(p => p.id == lineItem.Id);
            else
            {
                ll = new tblLineItems();
                ll.id = lineItem.Id;
                _ctx.tblLineItems.AddObject(ll);
            }

            ll.ProductID = lineItem.Product.Id;
            ll.DocumentID = orderId;
            ll.Description = lineItem.Description;
            ll.Quantity = lineItem.Qty;
            ll.LineItemSequenceNo = lineItem.LineItemSequenceNo;
            ll.Value = lineItem.Value;
            ll.Vat = lineItem.LineItemVatValue;
            ll.OrderLineItemType = (int)lineItem.LineItemType;
            ll.ProductDiscount = lineItem.ProductDiscount;
            ll.DiscountLineItemTypeId = (int)lineItem.DiscountType;
            ll.Description = lineItem.Description;
            _ctx.SaveChanges();
        }

        public void DeleteLineItem(OrderLineItem oli)
        {
            tblLineItems lineItem = _ctx.tblLineItems.FirstOrDefault(n => n.id == oli.Id);
            if (lineItem != null)
            {
                _ctx.tblLineItems.DeleteObject(lineItem);
                _ctx.SaveChanges();
            }
        }

        public int OrdersPendingApprovalCount()
        {
            int count = _GetAll(DocumentType.Order).Count(
                n =>  n.OrderOrderTypeId == (int)OrderType.OutletToDistributor
                     && n.DocumentStatusId == (int)DocumentStatus.Confirmed
                );
            return count;
        }

        public int OrdersPendingDispatchCount()
        {
            var items = _GetAll(DocumentType.Order).Where(
                n =>  n.OrderOrderTypeId == (int)OrderType.OutletToDistributor
                     && n.DocumentStatusId == (int)DocumentStatus.OrderPendingDispatch
                ).ToList();
            int count = items.Count(n => OutletFitsCriteria(n.OrderIssuedOnBehalfOfCC.Value));
            return count;
        }

        public int ApprovedPurchaseOrdersCount()
        {
            int count =
                _GetAll(DocumentType.Order).Count(
                    n => n.OrderOrderTypeId == (int)OrderType.DistributorToProducer &&
                    n.DocumentStatusId == (int)DocumentStatus.Approved);
            return count;
        }

        bool OutletFitsCriteria(Guid outletId)
        {
            tblCostCentre outlet = IssuedOnBehalfOf_Outlet(outletId);
            if (outlet == null)
                return false;
            bool fits = true;
            fits = outlet.IM_Status == (int)EntityStatus.Active;
            fits = outlet.tblRoutes.IM_Status == (int)EntityStatus.Active;
            return fits;
        }

        tblCostCentre IssuedOnBehalfOf_Outlet(Guid id)
        {
            return _ctx.tblCostCentre.FirstOrDefault(c => c.Id == id);
        }

        List<Order> SearchByOrderTypeAndDocumentStatus(OrderType orderType, DocumentStatus docStatus, string searchText, bool ofThisStatus = true, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            searchText = searchText.ToLower();
            IQueryable<tblDocument> data;
         
            if (ofThisStatus)
            {
                if (startDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order)
                        .Where(n =>n.OrderOrderTypeId == (int)orderType
                               && n.DocumentStatusId == (int)docStatus);
                else
                {
                    data = _GetAll(DocumentType.Order, startDate,endDate)
                        .Where(n =>
                                n.OrderOrderTypeId == (int)orderType
                               && n.DocumentStatusId == (int)docStatus);
                }
            }
            else
            {
                if (startDate.Equals(new DateTime()))
                    data = _GetAll(DocumentType.Order)
                       .Where(n =>
                               n.OrderOrderTypeId == (int)orderType
                              && n.DocumentStatusId != (int)docStatus);
                else
                {
                    data = _GetAll(DocumentType.Order, startDate,endDate)
                        .Where(n =>
                                n.OrderOrderTypeId == (int)orderType
                               && n.DocumentStatusId != (int)docStatus);
                }
            }
            
            data = data.Where(n => n.DocumentReference.ToLower().Contains(searchText));

            return data.ToList().Select(Map).ToList();
        }

        List<Order> SearchByDocumentStatusPagenated(int skip, int take, string searchText, int documentStatus, int orderType = 0, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            IQueryable<tblDocument> data;
           
            searchText = searchText.ToLower();
           

            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    data = _GetAll(DocumentType.Order);
                }
                else
                {
                    data = _GetAll(DocumentType.Order, startDate,endDate);
                }
            }
            else
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    data = _GetAll(DocumentType.Order)
                        .Where(n =>n.OrderOrderTypeId == orderType);
                }
                else
                {
                    data = _GetAll(DocumentType.Order, startDate,endDate)
                        .Where(n => n.OrderOrderTypeId == orderType );
                }
            }
            data = data.Where(n => n.DocumentStatusId == documentStatus);
            data = data.Where(n => n.DocumentReference.ToLower().Contains(searchText));
            data = data.OrderByDescending(n => n.DocumentDateIssued).Skip((skip - 1) * take).Take(take);

            return data.ToList().Select(Map).Select(n => n as Order).ToList();
        }

        List<Order> SearchByDocumentStatusPagenated(int skip, int take, string searchText,
                                                      int documentStatus1, bool hasStatus1 = true,
                                                      int? documentStatus2 = -1, bool? hasStatus2 = false,
                                                      int orderType = 0,
                                                      DateTime startDate = new DateTime(),
                                                      DateTime endDate = new DateTime())
        {
            IQueryable<tblDocument> data;
            searchText = searchText.ToLower();
           
            //cn:: imrpove this if u can
            //1. orders with status 1
            //2. orders not with status 1
            //3. orders with status 1 and 2
            //4. orders with status 1 but not 2

            if (orderType == 0)
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    if (!documentStatus2.HasValue)
                    {
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>(hasStatus1? n.DocumentStatusId == documentStatus1
                                            : n.DocumentStatusId != documentStatus1));
                        }
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>(n.DocumentStatusId == documentStatus1|| n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 != true)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>(n.DocumentStatusId != documentStatus1 && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }
                    }
                }
                else//dates set
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order, startDate,endDate)
                            .Where(n =>
                                    hasStatus1
                                       ? n.DocumentStatusId == documentStatus1
                                       : n.DocumentStatusId != documentStatus1);
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");
                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order, startDate,endDate)
                                .Where(n =>
                                      (n.DocumentStatusId == documentStatus1
                                           || n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order, startDate,endDate)
                                .Where(n =>
                                      (n.DocumentStatusId != documentStatus1
                                           && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
            }
            else //orderType set
            {
                if (startDate.Equals(new DateTime()) || endDate.Equals(new DateTime()))
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order)
                            .Where(n => n.OrderOrderTypeId == orderType &&
                                   (hasStatus1? n.DocumentStatusId == documentStatus1: n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order)
                                .Where(n =>
                                   n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId == documentStatus1 || n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order)
                                       .Where(n => n.OrderOrderTypeId == orderType && (n.DocumentStatusId != documentStatus1 && n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }

                    }
                }
                else//dates set
                {
                    if (!documentStatus2.HasValue)
                    {
                        data = _GetAll(DocumentType.Order, startDate,endDate)
                            .Where(n =>
                                  n.OrderOrderTypeId == orderType
                                   && (hasStatus1 ? n.DocumentStatusId == documentStatus1: n.DocumentStatusId != documentStatus1));
                    }
                    else
                    {
                        if (!hasStatus2.HasValue)
                            throw new Exception("Variable hasStatus2 must be assigned a value");

                        if (hasStatus1 && hasStatus2 == true)
                        {
                            data = _GetAll(DocumentType.Order, startDate,endDate)
                                .Where(n =>
                                  n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId == documentStatus1
                                           || n.DocumentStatusId == documentStatus2));
                        }
                        else if (!hasStatus1 && hasStatus2 == false)
                        {
                            data = _GetAll(DocumentType.Order, startDate,endDate)
                                .Where(n =>
                                  n.OrderOrderTypeId == orderType
                                       && (n.DocumentStatusId != documentStatus1
                                           || n.DocumentStatusId != documentStatus2));
                        }
                        else
                        {
                            throw new Exception("Use method taking one status param.");
                        }
                    }
                }
            }

            data = data.Where(n => n.DocumentReference.ToLower().Contains(searchText));

            data = data.OrderByDescending(n => n.DocumentDateIssued)
                                       .Skip((skip - 1) * take).Take(take);
            return data.ToList().Select(Map).ToList();
        }

        int GetOrdersCount(DateTime startDate = new DateTime(), DateTime endDate = new DateTime(), int orderType = 0)
        {
            int count;
            
            if (orderType > 0)
            {
                if (startDate.Equals(new DateTime()))
                    count = _GetAll(DocumentType.Order)
                        .Count(n =>n.OrderOrderTypeId == orderType);
                else
                {
                    count = _GetAll(DocumentType.Order, startDate,endDate)
                        .Count(n => n.OrderOrderTypeId == orderType);
                }
            }
            else
            {
                if (startDate.Equals(new DateTime()))
                    count = _GetAll(DocumentType.Order)
                        .Count();
                else
                {
                    count = _GetAll(DocumentType.Order, startDate,endDate)
                        .Count(n =>
                               n.DocumentTypeId == (int)DocumentType.Order);
                }
            }

            return count;
        }
        public List<Order> GetByDocumentStatus(DocumentStatus status)
        {
            var data = _GetAll(DocumentType.Order)
                .Where(n => n.DocumentStatusId == (int)status )
                .ToList()
                .Select(Map)
                .ToList();

            return data;
        }

        public QueryResult<Order> Query(QueryOrders query)
        {
            IEnumerable<tblDocument> tblDocument;
            tblDocument = _GetAll(DocumentType.Order).AsQueryable();
            tblDocument = tblDocument.Where(k => k.DocumentStatusId == (int)query.DocumentStatus);

            tblDocument = tblDocument.Where(k => k.DocumentDateIssued >= query.StartDate && k.DocumentDateIssued <= query.EndDate);
            tblDocument = tblDocument.Where(k => k.OrderOrderTypeId == (int)query.OrderType);

            var queryResult = new QueryResult<Order>();
           
            if (query.Distributr != null && query.Distributr != Guid.Empty)
            {
                tblDocument = tblDocument.Where(k => k.DocumentIssuerCostCentreId == query.Distributr);
            }
            queryResult.Count = tblDocument.Count();

            tblDocument = tblDocument.OrderBy(l => l.DocumentDateIssued);


            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var outletIds =
                    _ctx.tblCostCentre.Where(s => s.CostCentreType == 5 && s.Name.Contains(query.Name))
                        .Select(s => s.Id);
                tblDocument = tblDocument.Where(l =>l.OrderIssuedOnBehalfOfCC != null && outletIds.Contains( l.OrderIssuedOnBehalfOfCC.Value));
            }

            if (query.Skip.HasValue && query.Take.HasValue)
            {

                tblDocument = tblDocument.Skip(query.Skip.Value)
                    .Take(query.Take.Value);
            }
            var entities = tblDocument.ToList().Select(Map);
               
            queryResult.Data = entities.ToList();
            
            return queryResult;
        }
    }
}
