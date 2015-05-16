using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.Service;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Commands.DocumentCommands.Orders;
//using Distributr.Reports;
using Distributr.WSAPI.Lib.Services.Bus;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.Paging;
using StructureMap;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl
{
    public class ListOrdersViewModelBuilder : IListOrdersViewModelBuilder
    {
        IDistributorRepository _distributorRepository;
        ICostCentreRepository _costCentreRepository;
        IDocumentFactory _documentFactory;
        IUserRepository _userRepository;
        IProductRepository _productRepository;
        IBusPublisher _busPublisher;
        IProducerRepository _producerRepository;
        private ProductPackagingSummaryViewBuilder _summarizeProduct;
        public List<OrderViewModel> Unproc = new List<OrderViewModel>();
        private IDiscountProcessorService _discountProcessorService;
        private IOrderRepository _orderRepository;
        private IMainOrderRepository _mainOrderRepository;

        // IOrderRepository orderRepository,
        public ListOrdersViewModelBuilder(IDistributorRepository distributorRepository, ICostCentreRepository costCentreRepository, IDocumentFactory documentFactory, IUserRepository userRepository, IProductRepository productRepository, IBusPublisher busPublisher, IProducerRepository producerRepository, ProductPackagingSummaryViewBuilder summarizeProduct, IDiscountProcessorService discountProcessorService, IOrderRepository orderRepository, IMainOrderRepository mainOrderRepository)
        {
            _distributorRepository = distributorRepository;
            _costCentreRepository = costCentreRepository;
            _documentFactory = documentFactory;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _busPublisher = busPublisher;
            _producerRepository = producerRepository;
            _summarizeProduct = summarizeProduct;
            _discountProcessorService = discountProcessorService;
            _orderRepository = orderRepository;
            _mainOrderRepository = mainOrderRepository;
        }

        public ListOrdersViewModelBuilder()
        {

        }

        private OrderViewModel Map(Order n)
        {



            OrderViewModel ovm = new OrderViewModel
            {
                id = n.Id,
                documentReference = n.DocumentReference,
                gross = n.TotalGross,
                net = n.TotalNet,
                vat = n.TotalVat,
                status = n.Status,
                outletCode = n.DocumentIssuerCostCentre.CostCentreCode,
                orderDate = n.DocumentDateIssued,
                isseuedOnBehalf = n.IssuedOnBehalfOf == null ? null : _costCentreRepository.GetById(n.IssuedOnBehalfOf.Id).Name,

            };
            foreach (var it in n.LineItems)
            {
                //OrderViewModel.OrderLineItemViewModel oovm = new OrderViewModel.OrderLineItemViewModel 
                ovm.ProductType = it.GetType().ToString().Split('.').Last();
                ovm.productId = it.Product == null ? Guid.Empty : it.Product.Id;
                ovm.quantity = it.Qty;
                ovm.productCode = it.Product == null ? "" : it.Product.ProductCode;


            }

            return ovm;
        }

        public List<OrderViewModel> GetByDistDate(Guid distributor, string startDate, string endDate)
        {
            HttpContext.Current.Session["PurchaseOrderLineItemList"] = null;
            var list = _orderRepository.GetAll().Where(n => (n.OrderType == OrderType.DistributorToProducer) && (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.DocumentIssuerCostCentre.Id == distributor)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public List<OrderViewModel> GetSalesByDistDate(Guid distributor, string startDate, string endDate)
        {
            var list = _orderRepository.GetAll().Where(n => (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.DocumentIssuerCostCentre.Id == distributor) && (n.OrderType == OrderType.DistributorPOS)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public List<OrderViewModel> GetAllSales(string startDate, string endDate)
        {
            var list = _orderRepository.GetAll().Where(n => (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.OrderType == OrderType.DistributorPOS)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public List<OrderViewModel> GetAllOrdersByDate(string startDate, string endDate)
        {
            HttpContext.Current.Session["PurchaseOrderLineItemList"] = null;
            //var list = _documentRepository.GetAll().OfType<Order>().ToList();
            var list = _orderRepository.GetAll().Where(n => (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.OrderType == OrderType.DistributorToProducer)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public void SaveOrder(OrderViewModel ovm)
        {
            ValidationResultInfo vri = ovm.BasicValidation();
            if (ovm.distributorCode != null || ovm.salesManCode != null || ovm.outletCode != null)
            {
                CostCentre distributor = _costCentreRepository.GetAll().FirstOrDefault(n => n.CostCentreCode == ovm.distributorCode);
                if (distributor == null)
                    throw new DomainValidationException(vri, "Distributor Not Found\nPlease Check Distributor Code");
                CostCentre outlet = _costCentreRepository.GetAll().FirstOrDefault(n => n.CostCentreCode == ovm.outletCode);
                if (outlet == null)
                    throw new DomainValidationException(vri, "Outlet Not Found\nPlease Check Outlet Code");
                CostCentre salesman = _costCentreRepository.GetAll().FirstOrDefault(n => n.CostCentreCode == ovm.salesManCode) as DistributorSalesman;
                if (salesman == null)

                    throw new DomainValidationException(vri, "Salesman not found\nPlease Check DistributorSalesman Code");

                User docIssuerUser = _userRepository.GetAll().FirstOrDefault(n => n.CostCentre == salesman.Id);
                Product product = _productRepository.GetAll().FirstOrDefault(n => n.ProductCode == ovm.productCode);
                if (product == null)
                    throw new DomainValidationException(vri, "Product not found");
                Order orderCC = _documentFactory.CreateDocument(Guid.NewGuid(), DocumentType.Order, salesman, distributor, docIssuerUser, Guid.NewGuid().ToString()) as Order;

            }
        }

        public void AddOrderLineItem(Guid productId, int lineItemSequence, int qty, decimal vatValue, decimal value, int lineItemType)
        {

        }

        public void ProcessOrder(List<OrderViewModel> orderViewModelList)
        {
            ValidationResultInfo vri = orderViewModelList.BasicValidation();
            foreach (OrderViewModel ovm in orderViewModelList)
            {
                Order order = new Order(Guid.NewGuid());
                // Order ord = _documentRepository.GetAll().OfType<Order>().FirstOrDefault(n => n.DocumentReference == ovm.documentReference);
                // if (ord == null)
                if (IfExistsOrderReference(ovm.documentReference) == false)
                {
                    if (ovm.distributorCode != null || ovm.salesManCode != null || ovm.outletCode != null)
                    {
                        var costCentreList = _costCentreRepository.GetAll().Where(p => p.CostCentreCode != null).ToList();
                        Distributor distributor = costCentreList.Where(n => n.CostCentreCode.ToLower() == ovm.distributorCode.ToString().ToLower()).ToList().FirstOrDefault() as Distributor;
                        if (distributor == null)
                            throw new DomainValidationException(vri, "Distributor Not Found\nPlease Check Distributor Code");
                        Outlet outlet = costCentreList.Where(n => n.CostCentreCode == ovm.outletCode).FirstOrDefault() as Outlet;//.FirstOrDefault (n => n.CostCentreCode.ToLower() == ovm.outletCode.ToLower());
                        if (outlet == null)
                            throw new DomainValidationException(vri, "Outlet Not Found\nPlease Check Outlet Code");
                        ProductPricingTier pricingTier = (outlet as Outlet).OutletProductPricingTier;// _productPricingTierRepository.GetAll().FirstOrDefault(s=>s.);
                        VATClass vatClass = (outlet as Outlet).VatClass;// _vatClassRepository.GetAll().First();

                        DistributorSalesman salesman = costCentreList.Where(n => n.CostCentreCode == ovm.salesManCode).FirstOrDefault() as DistributorSalesman;
                        if (salesman == null)
                            throw new DomainValidationException(vri, "Salesman not found\nPlease Check DistributorSalesman Code");
                        User docIssuerUser = _userRepository.GetAll().FirstOrDefault(n => n.CostCentre == salesman.Id);
                        if (docIssuerUser == null)
                            throw new DomainValidationException(vri, "User not found\nPlease Check DistributorSalesman Code");

                        // order = new Order(Guid.NewGuid(),true,ovm.documentReference,salesman,DateTime.Now,outlet,0,docIssuerUser,DateTime.Now,outlet,DocumentStatus.Confirmed,OrderType.OutletToDistributor);// _documentFactory.CreateDocument(Guid.NewGuid(), DocumentType.Order, outlet, distributor, docIssuerUser, Guid.NewGuid().ToString()) as Order;
                        order = new Order(Guid.NewGuid(), true, ovm.documentReference, outlet, DateTime.Now, salesman, Guid.Empty, docIssuerUser, DateTime.Now, distributor, DocumentStatus.Confirmed, OrderType.OutletToDistributor, 0);
                        foreach (OrderViewModel.OrderLineItemViewModel orderLineItemsViewModel in ovm.orderViewModelLineItemVm)
                        {
                            _summarizeProduct.ClearBuffer();
                            Product product = _productRepository.GetAll().FirstOrDefault(n => n.ProductCode.ToLower() == orderLineItemsViewModel.productCode.ToLower());
                            if (product == null)
                                throw new DomainValidationException(vri, "Product not found");
                            SummarizeProduct(product.Id, orderLineItemsViewModel.quantity, true, false);
                            List<PackagingSummary> summary = _summarizeProduct.GetProductSummary();
                            foreach (PackagingSummary item in summary)
                            {
                                var unitpricediscount = _discountProcessorService.GetUnitPrice(item.Product.Id, outlet.Id);
                                var vat = _discountProcessorService.GetVATRate(item.Product, outlet);
                                order.AddLineItem(new OrderLineItem(Guid.NewGuid())
                                {
                                    Product = _productRepository.GetById(item.Product.Id),
                                    //Description = ovm.Description,

                                    LineItemSequenceNo = 1,
                                    Qty = item.Quantity,// orderLineItemsViewModel.quantity,
                                    LineItemVatValue = vat * unitpricediscount.UnitPrice,//vatClass != null ? vatClass.CurrentRate : 0,// orderLineItemsViewModel.LineItemVatTotal,
                                    Value = unitpricediscount.UnitPrice,//pricingTier!=null?item.Product.ProductPrice(pricingTier):0,// orderLineItemsViewModel.LineItemTotal,
                                    ProductDiscount = unitpricediscount.Discount,
                                    LineItemType = (OrderLineItemType)Enum.Parse(typeof(OrderLineItemType), orderLineItemsViewModel.LineItemType)

                                });
                            }

                        }
                        //var orderitems = order.LineItems;
                        //foreach (var item in orderitems)
                        //{
                        //    List<ProductAsDiscount> productAsDiscounts = _discountProcessorService.GetFOCCertainProduct(item.Product.Id, item.Qty);
                        //    if(productAsDiscounts!=null&& productAsDiscounts.Count>0)
                        //    {
                        //        var discount = productAsDiscounts.FirstOrDefault();
                        //       order._SetLineItems( AddLineItem(order.LineItems, discount.ProductId, discount.Quantity, 0, 0, 0, OrderLineItemType.Discount, discount.DiscountType));
                        //    }
                        //}
                        //decimal amount = order.LineItems.Sum(p => (p.Qty*(p.Value + p.LineItemVatValue)));
                        //ProductAsDiscount productAsDiscount = _discountProcessorService.GetFOCCertainValue(amount);
                        // order._SetLineItems( AddLineItem(order.LineItems, productAsDiscount.ProductId, productAsDiscount.Quantity, 0, 0, 0, OrderLineItemType.Discount, productAsDiscount.DiscountType));
                        SubmitOrder(order);
                        _summarizeProduct.ClearBuffer();
                    }
                }
                else
                {

                    Unproc.Add(ovm);

                }
            }


        }

        private List<OrderLineItem> AddLineItem(List<OrderLineItem> items, Guid productId, decimal qty, decimal vat, decimal unitprice, decimal productDiscout, OrderLineItemType type, DiscountType discointType)
        {
            items.Add(new OrderLineItem(Guid.NewGuid())
            {
                Product = _productRepository.GetById(productId),
                LineItemSequenceNo = 1,
                Qty = qty,// orderLineItemsViewModel.quantity,
                LineItemVatValue = vat,//vatClass != null ? vatClass.CurrentRate : 0,// orderLineItemsViewModel.LineItemVatTotal,
                Value = unitprice,//pricingTier!=null?item.Product.ProductPrice(pricingTier):0,// orderLineItemsViewModel.LineItemTotal,
                ProductDiscount = productDiscout,
                LineItemType = type,
                DiscountType = discointType,
            });
            return items;

        }

        public void SubmitOrder(Order order)
        {
            //send commands
            Producer producer = _producerRepository.GetProducer();
            var coc = new CreateOrderCommand(
                Guid.NewGuid(),
                order.Id,
               order.DocumentIssuerUser.Id,
               producer.Id,
               0,
               Guid.Empty,
               order.DocumentReference,
               order.DocumentDateIssued,
               order.DateRequired,
               order.IssuedOnBehalfOf.Id,
               order.DocumentIssuerCostCentre.Id,
               order.DocumentRecipientCostCentre.Id,
               order.DocumentIssuerUser.Id,
               (int)OrderType.OutletToDistributor,
               order.Note,
               order.SaleDiscount
             );
            coc.SendDateTime = DateTime.Now;
            //coc.CommandCreatedDateTime = DateTime.Now;
            _busPublisher.WrapAndPublish(coc, CommandType.CreateOrder);


            foreach (var item in order.LineItems)
            {
                var ali = new AddOrderLineItemCommand(Guid.NewGuid(),
                    order.Id,
                    order.DocumentIssuerUser.Id,
                order.DocumentIssuerCostCentre.Id, 0,
                Guid.Empty,
                item.LineItemSequenceNo,
                item.Value,
                item.Product.Id,
                item.Qty, item.LineItemVatValue,
                item.ProductDiscount, item.Description, (int)item.LineItemType, (int)item.DiscountType);
                ali.SendDateTime = DateTime.Now;

                _busPublisher.WrapAndPublish(ali, CommandType.AddOrderLineItem);

            }

            var co = new ConfirmOrderCommand(Guid.NewGuid(), order.Id,
                order.DocumentIssuerUser.Id,
                order.DocumentIssuerCostCentre.Id, 0, Guid.Empty, order.DocumentParentId
              );
            co.SendDateTime = DateTime.Now;
            _busPublisher.WrapAndPublish(co, CommandType.ConfirmOrder);

        }

        public QueryResult<OrderViewModel> Query(QueryOrders query)
        {
            var orders = _orderRepository.Query(query);

            var result = new QueryResult<OrderViewModel>();
            if (!orders.Data.Any())
                return new QueryResult<OrderViewModel>();
            result.Data = orders.Data.Select(Map).ToList();
            result.Count = orders.Count;

            return result;
        }

        public List<OrderViewModel> GetPendingOrders(Guid distributor, string startDate, string endDate)
        {
            var list = _orderRepository.GetAll().Where(n => (n.OrderType == OrderType.DistributorToProducer) && (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.Status == DocumentStatus.Confirmed)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public List<OrderViewModel> GetPendingDeliveries(Guid distributor, string startDate, string endDate)
        {
            var list = _orderRepository.GetAll().Where(n => (n.OrderType == OrderType.DistributorToProducer) && (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.Status == DocumentStatus.OrderPendingDispatch)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public List<OrderViewModel> GetCountPendingOrders()
        {
            var list2 = _orderRepository.GetAll().Where(n => (n.Status == DocumentStatus.Confirmed)).ToList();
            // int pendingOrdersCount = list.Count();
            return list2.Select(n => Map(n)).ToList();// null;// pendingOrdersCount;
        }

        public int GetCount()
        {
            int qq = _orderRepository.GetAll().Where(n => (n.Status == DocumentStatus.Confirmed) && (n.OrderType == OrderType.DistributorToProducer)).ToList().Count();

            return qq;
        }

        public List<OrderViewModel> SearchOrders(string orderRef)
        {
            var list = _orderRepository.GetAll().Where(n => (n.DocumentReference.ToString().ToLower() == orderRef.ToLower()) || (n.OrderType.ToString().ToLower() == orderRef.ToLower()) && (n.OrderType == OrderType.DistributorToProducer)).ToList();
            return list.Select(n => Map(n)).ToList();
        }

        public OrderViewModel GetOrdersSkipAndTake(int CurrentPage, int PageSize, bool inactive = false)
        {
            DateTime sdate= DateTime.Now.AddDays(-30);
            DateTime edate= DateTime.Now;
            var orders = _mainOrderRepository.PagedPurchaseDocumentList(CurrentPage,PageSize,sdate,edate,DocumentStatus.Confirmed);

            

            OrderViewModel orderVM = new OrderViewModel
            {
                TotalPages = orders.PageCount,
                Items = orders
                .Select(n => MapMainOrderSummary(n)).ToList()
            };
            return orderVM;
        }

        public Dictionary<Guid, string> GetDistributor()
        {
            return _distributorRepository.GetAll().OfType<Distributr.Core.Domain.Master.CostCentreEntities.Distributor>()
                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

        public IList<OrderViewModel> GetAllList()
        {//.Where(n => n.IssuedOnBehalfOf.CostCentreType == CostCentreType.Distributor)
            HttpContext.Current.Session["PurchaseOrderLineItemList"] = null;
            var list = _orderRepository.GetAll().Where(n => (n.OrderType == OrderType.DistributorToProducer) && (n.DocumentDateIssued.ToShortDateString() == DateTime.Now.ToShortDateString()));//&&(n.DocumentDateIssued.ToShortDateString()==DateTime.Now.ToShortDateString())).ToList();

            return list
                .Select(n => Map(n)
                ).ToList();

        }

        public OrderViewModel GetByDist(Guid distributor, int CurrentPage, int PageSize, bool inactive = false)
        {
            var orders = new List<Order>();
            if (distributor != Guid.Empty)
            {
                orders = _orderRepository.GetAll().Where(n => (n.OrderType == OrderType.DistributorToProducer) && (n.DocumentIssuerCostCentre.Id == distributor)).ToList();
            }
            else
            {
                orders = _orderRepository.GetAll().Where(n => (n.OrderType == OrderType.DistributorToProducer)).ToList();
            }

            OrderViewModel orderVm = new OrderViewModel
            {
                TotalPages = (int)Math.Ceiling((double)_orderRepository.GetCount() / (double)PageSize),
                Items = orders.Skip((CurrentPage - 1) * PageSize).Take(PageSize).Select(MapSkipAndTake).ToList()
            };

            return orderVm;
        }


        public OrderViewModel GetAllPendingOrders(int CurrentPage, int PageSize)
        {
            DateTime sdate= DateTime.Now.AddDays(-90);
            DateTime edate= DateTime.Now;
            var orders = _mainOrderRepository.PagedPurchaseDocumentList(CurrentPage, PageSize, sdate, edate,DocumentStatus.Confirmed);
            OrderViewModel orderVM = new OrderViewModel
            {

                TotalPages = orders.PageCount,
                PageSize = orders.PageSize,
                Items = orders
               .Select(n => MapMainOrderSummary(n))
               .OrderByDescending(d => d.orderDate.Year)
               .ThenBy(d => d.orderDate.Month)
               .ThenBy(d => d.orderDate.Day)
               .ThenBy(d => d.orderDate.TimeOfDay)
               .ToList()

            };
            return orderVM;
        }

        public OrderViewModel GetAllClosedPurchaseOrders(int CurrentPage, int PageSize)
        {

            DateTime sdate = DateTime.Now.AddDays(-90);
            DateTime edate = DateTime.Now;
            var orders = _mainOrderRepository.PagedPurchaseDocumentList(CurrentPage, PageSize, sdate, edate, DocumentStatus.Closed);
            OrderViewModel orderVM = new OrderViewModel
            {

                TotalPages = orders.PageCount,
                Items = orders
               .Select(n => MapMainOrderSummary(n))
               .OrderByDescending(d => d.orderDate.Year)
               .ThenBy(d => d.orderDate.Month)
               .ThenBy(d => d.orderDate.Day)
               .ThenBy(d => d.orderDate.TimeOfDay)
               .ToList()

            };
            return orderVM;
        }

        public OrderViewModel GetAllApprovedOrders(int CurrentPage, int PageSize)
        {
            DateTime sdate = DateTime.Now.AddDays(-30);
            DateTime edate = DateTime.Now;
            var orders = _mainOrderRepository.PagedPurchaseDocumentList(CurrentPage, PageSize, sdate, edate, DocumentStatus.Approved);
            OrderViewModel orderVM = new OrderViewModel
            {

                TotalPages = orders.PageCount,
                Items = orders
               .Select(n => MapMainOrderSummary(n))
               .OrderByDescending(d => d.orderDate.Year)
               .ThenBy(d => d.orderDate.Month)
               .ThenBy(d => d.orderDate.Day)
               .ThenBy(d => d.orderDate.TimeOfDay)
               .ToList()

            };
            return orderVM;
        }

        public OrderViewModel SearchPOrders(string searchText, int CurrentPage, int PageSize)
        {
             DateTime sdate= DateTime.Now.AddDays(-30);
            DateTime edate= DateTime.Now.AddDays(1);
            var orders = _mainOrderRepository.PagedPurchaseDocumentList(CurrentPage, PageSize, sdate, edate,DocumentStatus.Confirmed, null,
                                                                        searchText);
            OrderViewModel orderVM = new OrderViewModel
            {
                //TotalPages = (int)Math.Ceiling((double)_documentRepository.GetCount() / (double)PageSize),
                TotalPages =orders.PageCount,
                Items =orders.Select(n=>MapMainOrderSummary(n)).ToList()
            };
            return orderVM;
        }

        public OrderViewModel FilterOrdersByDate(string startDate, string endDate, int CurrentPage, int PageSize)
        {
            endDate = endDate + " 23:59:59";
            OrderViewModel orderVM = new OrderViewModel
            {
                //TotalPages = (int)Math.Ceiling((double)_documentRepository.GetCount() / (double)PageSize),
                TotalPages = (int)Math.Ceiling((double)_orderRepository.GetAll().Where(n => (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.OrderType == OrderType.DistributorToProducer)).Count() / (double)PageSize),
                Items = _orderRepository.GetAll().Where(n => (n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate)) && (n.OrderType == OrderType.DistributorToProducer)).Skip((CurrentPage - 1) * PageSize).Take(PageSize)
                .Select(n => MapSkipAndTake(n))
                .ToList()
            };
            return orderVM;
        }

        public OrderViewModel FilterOrdersByDateDistributor(Guid distributor, string startDate, string endDate, int CurrentPage, int PageSize)
        {
            endDate = endDate + " 23:59:59";
            var items =
                _mainOrderRepository.PagedPurchaseDocumentList(CurrentPage, PageSize, DateTime.Parse(startDate),
                                                               DateTime.Parse(endDate),DocumentStatus.Confirmed, distributor);
                //.Where(n =>
                //     (n.OrderType == OrderType.DistributorToProducer && 
                //     n.DocumentDateIssued >= DateTime.Parse(startDate) && n.DocumentDateIssued <= DateTime.Parse(endDate))
                //    && (distributor == Guid.Empty 
                //        || n.DocumentIssuerCostCentre.Id == distributor 
                //        || (n.DocumentIssuerCostCentre.ParentCostCentre != null 
                //            && n.DocumentIssuerCostCentre.ParentCostCentre.Id == distributor)));
            OrderViewModel orderVM = new OrderViewModel
            {
                TotalPages =items.PageCount,
                Items = items.Select(MapMainOrderSummary).ToList()
            };
            return orderVM;
        }
        OrderViewModel.OrderViewModelItem MapMainOrderSummary(MainOrderSummary order)
        {
            OrderViewModel.OrderViewModelItem orderViewModelItem = new OrderViewModel.OrderViewModelItem();
            orderViewModelItem.documentReference = order.OrderReference;
            orderViewModelItem.gross = order.GrossAmount;
            orderViewModelItem.net = order.NetAmount;
            orderViewModelItem.vat = order.TotalVat;
            orderViewModelItem.status = order.Status;

            //orderViewModelItem.orderDate = ;
            orderViewModelItem.isseuedOnBehalf = order.Salesman;
            orderViewModelItem.id = order.OrderId;
            return orderViewModelItem;
        }
        OrderViewModel.OrderViewModelItem MapSkipAndTake(Order order)
        {
            OrderViewModel.OrderViewModelItem orderViewModelItem = new OrderViewModel.OrderViewModelItem();
            orderViewModelItem.documentReference = order.DocumentReference;
            orderViewModelItem.gross = order.TotalGross;
            orderViewModelItem.net = order.TotalNet;
            orderViewModelItem.vat = order.TotalVat;
            orderViewModelItem.status = order.Status;

            orderViewModelItem.orderDate = order.DocumentDateIssued;
            orderViewModelItem.isseuedOnBehalf = order.IssuedOnBehalfOf == null
                                                     ? null
                                                     : _costCentreRepository.GetById(order.IssuedOnBehalfOf.Id).Name;
            orderViewModelItem.id = order.Id;
            return orderViewModelItem;
        }

        void SummarizeProduct(Guid productId, decimal qty, bool isNew, bool isBulk)
        {

            if (isBulk)
            {
                qty = _summarizeProduct.GetProductQuantityInBulk(productId);
            }

            _summarizeProduct.AddProduct(productId, qty, false, !isNew, true);

        }

        public List<OrderViewModel> GetUnImported()
        {
            List<OrderViewModel> UnImported = Unproc;
            return UnImported;

        }

        bool IfExistsOrderReference(string documentReference)
        {
            throw new Exception("Need to resolve this - ReportConnection - AJM");
            //using (var _ctx = new CokeDataContext(ReportConnection.connectionString))
            //{
            //    var existing = (from doc in _ctx.tblDocument.Where(n => n.DocumentReference == documentReference)
            //                    select doc).FirstOrDefault();

            //    if (existing == null)
            //        return false;
            //    return true;
            //}
        }
    }
}
