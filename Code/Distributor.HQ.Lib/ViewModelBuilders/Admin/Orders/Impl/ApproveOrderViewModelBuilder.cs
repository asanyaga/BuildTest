using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl
{
    public class ApproveOrderViewModelBuilder : IApproveOrderViewModelBuilder
    {
        
        private IProductRepository _productRepository;
        private IVATClassRepository _vatClassRepository;
        private ProductPackagingSummaryViewBuilder _summarizeProduct;
        private IMainOrderRepository _orderMainRepository;
        private IMainOrderFactory _mainOrderFactory;
        private IPurchaseOrderWorkflow _purchaseOrderWorkflow;

        
        public ApproveOrderViewModelBuilder( IProductRepository productRepository, IVATClassRepository vatClassRepository, ProductPackagingSummaryViewBuilder summarizeProduct, IMainOrderRepository orderMainRepository, IMainOrderFactory mainOrderFactory, IPurchaseOrderWorkflow purchaseOrderWorkflow)
        {
            
            _productRepository = productRepository;
            _vatClassRepository = vatClassRepository;
            _summarizeProduct = summarizeProduct;
            _orderMainRepository = orderMainRepository;
            _mainOrderFactory = mainOrderFactory;
            _purchaseOrderWorkflow = purchaseOrderWorkflow;
        }

        public ApproveOrderViewModel Get(Guid orderId)
        {
            MainOrder o = _orderMainRepository.GetById(orderId) as MainOrder;

            var vm = new ApproveOrderViewModel
            {
                DocumentId = o.Id.ToString(),
                DocumentReference = o.DocumentReference,
                DocumentIssuerCostCentre = o.DocumentIssuerCostCentre.Name,
                DocumentRecipientCostCentre = o.DocumentRecipientCostCentre.Name,
                DocumentIssuerUser = o.DocumentIssuerUser.Username,
                DocumentStatus = o.Status.ToString(),
                DocumentDateIssued = o.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                DateRequired = o.DateRequired.ToString("dd-MMM-yyyy"),
                TotalNet = o.TotalNet,
                TotalVat = o.TotalVat,
                OrderTotal = o.TotalGross,
                //LineItems = o.LineItems.Where(n=>n.LineItemType==OrderLineItemType.PostConfirmation).Select(n => Map(n, o.Id)).ToList(),
                CanEdit = o.Status == DocumentStatus.Confirmed
            };
            if (HttpContext.Current.Session["PurchaseOrderLineItemList"] == null)
            {
                _summarizeProduct.ClearBuffer();
                List<SubOrderLineItem> list = o.PendingApprovalLineItems.ToList();
              
                foreach (SubOrderLineItem x in list)
                {
                    if (x.Product is SaleProduct || x.Product is ConsolidatedProduct)
                        _summarizeProduct.AddProduct(x.Product.Id, x.Qty, false, false, true);
                }
                List<PackagingSummary> summaryList = _summarizeProduct.GetProductSummary();
                HttpContext.Current.Session["PurchaseOrderLineItemList"] = summaryList;
            }
            else
            {
                List<PackagingSummary> summaryList = HttpContext.Current.Session["PurchaseOrderLineItemList"] as List<PackagingSummary>;

            }

            vm = GetSummaryList(vm);
            return vm;
        }

        public ApproveOrderViewModel Find(Guid orderId)
        {
            var order = _orderMainRepository.GetById(orderId);
            var vm = new ApproveOrderViewModel
                {
                    DocumentId = order.Id.ToString(),
                    DocumentReference = order.DocumentReference,
                    DocumentIssuerCostCentre = order.DocumentIssuerCostCentre.Name,
                    DocumentRecipientCostCentre = order.DocumentRecipientCostCentre.Name,
                    DocumentIssuerUser = order.DocumentIssuerUser.Username,
                    DocumentStatus = order.Status.ToString(),
                    DocumentDateIssued = order.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                    DateRequired = order.DateRequired.ToString("dd-MMM-yyyy"),
                    TotalNet = order.TotalNet,
                    TotalVat = order.TotalVat,
                    OrderTotal = order.TotalGross,
                };

                foreach (var item in order.ItemSummary)
                {
                    var lineItem = new ApproveOrderViewModel.ApproveOrderLineItemViewModel()
                        {
                            Qty = item.Qty,
                            VatValue = 0,
                            ProductId = item.Product.Id,
                            ProductDesc = item.Product.Description,
                            ProductType = item.Product.GetType().ToString(),
                            LineTotal = item.TotalNet,
                            Value = item.Product.ExFactoryPrice,
                            TotalNet = item.Product.ExFactoryPrice * item.Qty,
                        };

                    vm.LineItems.Add(lineItem);
                }
            return vm;
        }

        private ApproveOrderViewModel GetSummaryList(ApproveOrderViewModel vm)
        {
            List<PackagingSummary> summaryList = _summarizeProduct.GetProductSummary();
            List<PackagingSummary> sumaryReturnable = summaryList.Where(s => s.Product is ReturnableProduct).ToList();

            sumaryReturnable = _summarizeProduct.GetMixedPackContainers(sumaryReturnable);
            foreach (PackagingSummary item in summaryList)
            {
                UpdateOrAddLineItem(vm.LineItems, item.Product, item.Quantity, item.IsEditable, item.ParentProductId, false);
            }
            foreach (PackagingSummary item in sumaryReturnable)
            {

                UpdateOrAddLineItem(vm.LineItems, item.Product, item.Quantity, false, item.ParentProductId, true);
            }
            vm.TotalNet = vm.LineItems.Sum(t => t.TotalNet);
           vm.TotalVat = vm.LineItems.Sum(t => t.TotalVat);
            vm.OrderTotal = vm.LineItems.Sum(t => t.LineTotal);

            return vm;
        }
       
        private void UpdateOrAddLineItem(List<ApproveOrderViewModel.ApproveOrderLineItemViewModel> LineItems, Product product, decimal quantity, bool isEditable, Guid parentProductId, bool isEdit)
        {
            decimal UnitPrice = PriceCalc(product);
            decimal UnitVat = 0;// VatCalc(product);

            decimal net = UnitPrice * quantity;
            //decimal vat = quantity * UnitVat;//cn
            decimal VatAmount = quantity * UnitVat;
            decimal TotalPrice = net + VatAmount;


            ApproveOrderViewModel.ApproveOrderLineItemViewModel li;
            if (LineItems.Any(p => p.ProductId == product.Id) && isEdit == false)
            {

                li = LineItems.First(p => p.ProductId == product.Id);
                li.Qty = li.Qty + quantity;
                li.VatValue = UnitVat;
                li.LineTotal = li.TotalNet + TotalPrice;



            }
            else if (LineItems.Any(p => p.ProductId == product.Id) && isEdit == true)
            {
                li = LineItems.First(p => p.ProductId == product.Id);
                li.Qty = quantity;
               
                li.LineTotal = TotalPrice;
            }
            else
            {
                li = new ApproveOrderViewModel.ApproveOrderLineItemViewModel();
                LineItems.Add(li);
                li.Qty = quantity;
               
                li.LineTotal = TotalPrice;
                li.ProductType = product.GetType().ToString().Split('.').Last();
            }


            li.ProductId = product.Id;
            li.ProductDesc = product.Description;
            li.Value = UnitPrice;
            li.TotalNet = net;


            li.VatValue = UnitVat;




        }

        decimal PriceCalc(Product product)
        {
            decimal UnitPrice = 0m;


            if (product is ConsolidatedProduct)
                try
                {
                    UnitPrice = product.ExFactoryPrice;//((ConsolidatedProduct)product).TotalExFactoryValue(tier);
                }
                catch
                {
                    UnitPrice = 0m;
                }
            else
                try
                {
                    UnitPrice = product.ExFactoryPrice;// product.TotalExFactoryValue(tier);
                }
                catch
                {
                    UnitPrice = 0m;
                }
            return UnitPrice;
        }

        decimal VatCalc(Product product)
        {
            decimal vat = 0m;
            if (product.VATClass != null && (product is SaleProduct || product is ConsolidatedProduct))
            {
                vat = product.VATClass.CurrentRate;
            }
            return vat;
        }

        private ApproveOrderViewModel.ApproveOrderLineItemViewModel Map(OrderLineItem lineItem, Guid documentId)
        {
            return new ApproveOrderViewModel.ApproveOrderLineItemViewModel
            {
                DocumentId = documentId.ToString(),
                LineItemId = lineItem.Id.ToString(),
                ProductId = lineItem.Product.Id,
                ProductDesc = lineItem.Product.Description,
                Qty = lineItem.Qty,
                Value = lineItem.Value,
                TotalNet = (lineItem.Qty * lineItem.Value),
                VatValue = lineItem.LineItemVatValue,
                LineTotal = lineItem.LineItemTotal,
                ProductType = lineItem.Product.GetType().ToString().Split('.').Last(),
            };
        }
        
        public OrderEditLineItemViewModel GetLineItem(Guid productId)
        {
            OrderEditLineItemViewModel orderEditLineItem = new OrderEditLineItemViewModel();
            if (HttpContext.Current.Session["PurchaseOrderLineItemList"] != null)
            {
                //List<PackagingSummary> summaryList = HttpContext.Current.Session["PurchaseOrderLineItemList"] as List<PackagingSummary>;
                List<PackagingSummary> summaryList = _summarizeProduct.GetProductSummary();
                var lineItem = summaryList.FirstOrDefault(n => n.Product.Id == productId);
                if (lineItem != null)
                {

                    orderEditLineItem.ProductId = lineItem.Product.Id;


                    orderEditLineItem.ProductDesc = lineItem.Product.Description;
                    orderEditLineItem.Qty = lineItem.Quantity.ToString();

                   
                }
            }
            return orderEditLineItem;

        }

        public OrderAddLineItemViewModel GetAddLineItem(Guid orderId)
        {
            Dictionary<Guid, string> dictProducts = _productRepository.GetAll().ToList().Where(n => n is SaleProduct || n is ConsolidatedProduct).OrderBy(n => n.Description).ToDictionary(n => n.Id,
                                                                                            n => n.Description);
            MainOrder o = _orderMainRepository.GetById(orderId) as MainOrder;
            return new OrderAddLineItemViewModel
            {
                Qty = "1",
                DocumentId = orderId.ToString(),
                DocumentRef = o.DocumentReference,
                ProductLookup = new SelectList(dictProducts, "Key", "Value")
            };
        }
        
        public void AddUpdateLineItems(Guid productId, decimal qty, bool isNew, bool isBulk)
        {

            if (isBulk)
            {
                decimal q = qty;
                qty = _summarizeProduct.GetProductQuantityInBulk(productId) * q;
            }

            _summarizeProduct.AddProduct(productId, qty, false, !isNew, true);

        }
        
        public void RemoveLineItem(Guid productId)
        {
            _summarizeProduct.RemoveProduct(productId);
        }

        public void Approve(Guid documentId)
        {
           // Order o = _orderRepository.GetById(documentId) as Order;
            MainOrder o = _orderMainRepository.GetById(documentId);
            o.ChangeccId(Guid.Empty);
         //   VATClass vatClass = _vatClassRepository.GetAll().First();

           // Producer producer = _producerRepository.GetProducer();
            if (HttpContext.Current.Session["PurchaseOrderLineItemList"] != null)
            {
                List<PackagingSummary> summaryList = _summarizeProduct.GetProductSummary();
                if (summaryList.Count == 0)
                {
                    o.Reject();

                }
                else
                {

                    foreach (PackagingSummary item in summaryList)
                    {
                        SubOrderLineItem ol = o.PendingApprovalLineItems.FirstOrDefault(p => p.Product.Id == item.Product.Id);
                        if (ol != null)
                        {
                            if (ol.Qty != item.Quantity)
                            {
                                ol.Qty = item.Quantity;
                                o.EditLineItem(ol);
                            }
                        }
                        else if (ol == null)
                        {
                            SubOrderLineItem aol = _mainOrderFactory.CreateLineItem(item.Product.Id, item.Quantity,
                                                                                  item.Product.ExFactoryPrice, "item",
                                                                                   item.Product.VATClass.CurrentRate);
                            o.EditLineItem(aol);
                        }

                    }
                    foreach (SubOrderLineItem oline in o.PendingApprovalLineItems)
                    {
                        PackagingSummary summaryitem = summaryList.FirstOrDefault(p => p.Product.Id == oline.Product.Id);

                        if (summaryitem == null)
                        {
                            o.RemoveLineItem(oline);
                        }
                    }
                     foreach (SubOrderLineItem item in o.PendingApprovalLineItems)
                     {
                         o.ApproveLineItem(item);
                     }
                    o.Approve();
                }

                _purchaseOrderWorkflow.Submit(o);
            }
            HttpContext.Current.Session["PurchaseOrderLineItemList"] = null;
            _summarizeProduct.ClearBuffer();
        }

        public void Reject(Guid documentId, string rejectReason)
        {
            MainOrder o = _orderMainRepository.GetById(documentId);
            o.ChangeccId(Guid.Empty);
            o.Reject();
            _purchaseOrderWorkflow.Submit(o);

        }
    }
}
