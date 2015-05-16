using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders_Stockist
{
    public class StockistPurchaseOrderApprovalFormViewModel : OrderBaseViewModel
    {
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand ApproveCommand { get; set; }
        public RelayCommand RejectCommand { get; set; }

        public StockistPurchaseOrderApprovalFormViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            ApproveCommand = new RelayCommand(Approve);
            RejectCommand = new RelayCommand(Reject);
        }

        private void Reject()
        {
            
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to reject this order : " + OrderReferenceNo, "Stockist Order Form", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;
            RefreshLineItemsAvailablity();
            using (var c = NestedContainer)
            {
                Config config = Using<IConfigService>(c).Load();
                MainOrder order = Using<IMainOrderRepository>(c).GetById(OrderId);
                var orderWorkflow = Using<IOrderWorkflow>(c);
                
                order.Reject();
                orderWorkflow.Submit(order, config);
                var action = new List<DistributrMessageBoxButton>
                                 {
                                     DistributrMessageBoxButton.StockistPurchaseOrderNew,
                                     DistributrMessageBoxButton.StockistPurchaseOrderSummary
                                 };

                var result = Using<IDistributrMessageBox>(c).ShowBox(action,
                                                                     string.Format("Stockist Purchase Order Rejected  . Click on one of the button to navigate to module of your choice "));
                NavigateCommand.Execute(result.Url);
              
            }
        }

        public void SetOrderToApprove(ApproveStockistPurchaseOrderMessage obj)
        {
            OrderId = obj.Id;
        }

        private string messageItems()
        {
            var message = "";
            
            if (LineItem.Any(l=>l.Quantity > l.Available))
            {
                foreach (var mainOrderLineItem in LineItem.Where(p=>p.Quantity > p.Available))
                {
                    int available = (int)mainOrderLineItem.Available;
                    message += "\n" + mainOrderLineItem.ProductName + "\t Available:  " + available;
                }
            }
            return message;
        }

        private void Approve()
        {
            
            using (var c = NestedContainer)
            {
                MainOrder order = Using<IMainOrderRepository>(c).GetById(OrderId);
                Config config = Using<IConfigService>(c).Load();
                Guid costCentreApplicationid = config.CostCentreApplicationId;
                order.ChangeccId(costCentreApplicationid);
                var orderWorkflow = Using<IStockistPurchaseOrderWorkflow>(c);
                if (LineItem.Any(s => s.ApprovableQuantity!=s.Quantity ))
                {
                  
                    //MessageBox.Show("Order cannot be Approved.\n\t there isnt Inventory to satisfy any item in the following order", "Order form , OrderRef " + OrderReferenceNo);
                    MessageBox.Show("Order cannot be Approved.\t" + messageItems(), "Order form , OrderRef " + OrderReferenceNo);
                    return;
                }
               
                else
                {
                   
                    
                    bool takeTolostSale = true;
                    order = UpdadateMainOrderIfEdited(order);
                    foreach (var line in order.PendingApprovalLineItems)
                    {
                        var existing = LineItem.FirstOrDefault(g => g.LineItemType == line.LineItemType && g.ProductId == line.Product.Id);
                        if (line.Qty == existing.ApprovableQuantity)
                            order.ApproveLineItem(line);
                        else
                            order.ApproveLineItem(line, existing.ApprovableQuantity, takeTolostSale);
                    }
                }
                order.Approve();
                orderWorkflow.Submit(order,config);
                var action = new List<DistributrMessageBoxButton>
                                 {
                                     DistributrMessageBoxButton.StockistPurchaseOrderNew,
                                     DistributrMessageBoxButton.StockistPurchaseOrderSummary
                                 };

                var result = Using<IDistributrMessageBox>(c).ShowBox(action,
                                                                     string.Format("Stockist Purchase Order Approved  and Inventory transfered successfully. Click on one of the button to navigate to module of your choice "));
                NavigateCommand.Execute(result.Url);
                IntializeViewModel();
            }

        }
        private MainOrder UpdadateMainOrderIfEdited(MainOrder order)
        {
            using (var c = NestedContainer)
            {
                var factory = Using<IMainOrderFactory>(c);
                //update and add line item
                foreach (var item in LineItem)
                {
                    SubOrderLineItem existing =
                        order.PendingApprovalLineItems.FirstOrDefault(
                            s => s.Product.Id == item.ProductId && s.LineItemType == item.LineItemType);
                    if (existing != null && existing.Qty != item.Quantity)
                    {
                        existing.Qty = item.Quantity;
                        order.EditLineItem(existing);
                    }
                    else if (existing == null)
                    {
                        if (item.UnitDiscount > 0)
                        {
                            existing = factory.CreateDiscountedLineItem(item.ProductId, item.Quantity,
                                                                              item.UnitPrice, item.ProductType,
                                                                              item.UnitVAT, item.UnitDiscount);
                        }
                        else
                        {
                            existing = factory.CreateLineItem(item.ProductId, item.Quantity,
                                                                                    item.UnitPrice, item.ProductType,
                                                                                    item.UnitVAT);
                        }
                        order.EditLineItem(existing);
                    }
                }
                //remove line item

                foreach (var item in order.PendingApprovalLineItems)
                {
                    var existing = LineItem.FirstOrDefault(g => g.LineItemType == item.LineItemType && g.ProductId == item.Product.Id);
                    if (existing == null)
                    {
                        order.RemoveLineItem(item);
                    }
                }
            }
            return order;
        }
        private void RefreshViewModel()
        {
            CanChange = true;
            TotalNet = 0;
            TotalProductDoscount = 0;
            TotalVat = 0;
            TotalGross = 0;
            SaleDiscount = 0;
            DateRequired = DateTime.Now;
            OrderReferenceNo = "";
        }
        private void IntializeViewModel()
        {
            RefreshViewModel();
            _lineItem.Clear();
            LineItem.Clear();

            GetCurrentDocumentRef();

        }
        private void Load()
        {

            IntializeViewModel();
            CanChange = false;
            _lineItem.Clear();
            _lineItemTemp.Clear();

            using (var c = NestedContainer)
            {

                MainOrder o = Using<IMainOrderRepository>(c).GetById(OrderId);
               // IsEditable = o.IsEditable();
                if (!o.IsApprovable())
                {
                    MessageBox.Show("Stockist Purchase Order has no item to be approved", "Stockist Purchase Order approval");
                    NavigateCommand.Execute(@"\Views\Orders_Stockist\StockistPurchaseOrderListing.xaml");
                    return;
                }
                if (o.DocumentIssuerCostCentre is Distributor)
                {
                    SelectedSalesman = o.DocumentRecipientCostCentre as DistributorSalesman;
                }
                else
                {
                    SelectedSalesman = o.DocumentIssuerCostCentre as DistributorSalesman;
                }
                Outlet outlet = o.IssuedOnBehalfOf as Outlet;
                if (outlet != null)
                {
                    SelectedRoute = outlet.Route;
                    SelectedOutlet = outlet;
                }
                OrderReferenceNo = o.DocumentReference;
                SaleDiscount = o.SaleDiscount;
                DateRequired = o.DateRequired;
                Status = o.OrderStatus.ToString();
                TotalNet = o.TotalNet;
                TotalVat = o.TotalVat;
                TotalProductDoscount = o.TotalDiscount;
                TotalGross = o.TotalGross;
                ShipToAddress = o.ShipToAddress;
                Note = o.Note;

                foreach (var item in o.PendingApprovalLineItems.OrderBy(s => s.LineItemType).ThenByDescending(s => s.Description))
                {
                    if (item.Product is SaleProduct)
                    {
                        ProductPopUpItem ppi = new ProductPopUpItem();
                        ppi.Product = item.Product;
                        ppi.Quantity = item.Qty;
                        _lineItem.Add(ppi);
                    }
                    MainOrderLineItem mitem = new MainOrderLineItem();
                    mitem.UnitPrice = item.Value;
                    mitem.UnitVAT = item.LineItemVatValue;
                    mitem.TotalAmount = item.LineItemTotal;
                    mitem.TotalNet = item.TotalNetPrice;
                    mitem.TotalVAT = item.LineItemVatTotal;
                    mitem.GrossAmount = item.LineItemTotal;
                    mitem.UnitDiscount = item.ProductDiscount;
                    mitem.TotalProductDiscount = item.TotalDiscount;
                    mitem.ProductName = item.Product.Description;
                    mitem.ProductId = item.Product.Id;
                    mitem.Quantity = item.Qty;
                    mitem.ProductType = item.LineItemType.ToString();
                    mitem.CanChange = o.IsEditable() && item.Product is SaleProduct && item.LineItemType == MainOrderLineItemType.Sale;
                    mitem.LineItemType = item.LineItemType;
                    mitem.Product = item.Product;
                    _lineItemTemp.Add(mitem);


                }

                RefreshLineItemsAvailablity();

            }

        }
        private void RefreshLineItemsAvailablity()
        {
            LineItem.Clear();
            int seqeunceNo = 1;
            using (var c = NestedContainer)
            {
                var inventoryRepository = Using<IInventoryRepository>(c);
                var configService = Using<IConfigService>(c);
                var config = configService.Load();
                foreach (var item in _lineItemTemp)
                {
                    Product product = item.Product;
                    decimal alreadyUsed = LineItem.Where(p => p.ProductId == item.ProductId).Sum(s => s.Quantity);
                    decimal availableQuantity = 0;
                    decimal balance = 0;
                    var inveAvailable = inventoryRepository.GetByProductIdAndWarehouseId(item.ProductId, config.CostCentreId);
                    balance = inveAvailable != null ? inveAvailable.Balance : 0;
                    if (product is SaleProduct)
                    {
                        decimal current = balance - alreadyUsed;
                        SaleProduct sp = product as SaleProduct;
                        if (sp.ReturnableProduct != null)
                        {
                            decimal balanceRetunable = 0;
                            decimal alreadyUsedReturnable = LineItem.Where(p => p.ProductId == sp.ReturnableProduct.Id).Sum(s => s.ApprovableQuantity);
                            var retuinveAvailable = inventoryRepository.GetByProductIdAndWarehouseId(sp.ReturnableProduct.Id, config.CostCentreId);
                            balanceRetunable = retuinveAvailable != null ? retuinveAvailable.Balance : 0;
                            decimal currentRetunable = balanceRetunable - alreadyUsedReturnable;
                            if (currentRetunable < current)
                                current = currentRetunable;

                        }
                        availableQuantity = current > 0 ? current : 0;



                    }
                    else if (product is ReturnableProduct)
                    {
                        decimal current = balance - alreadyUsed;
                        availableQuantity = current > 0 ? current : 0;
                    }
                    item.ApprovableQuantity = availableQuantity >= item.Quantity ? item.Quantity : availableQuantity;
                    item.SequenceNo = seqeunceNo;
                    item.Available = availableQuantity;
                    item.BackOrder = availableQuantity < item.Quantity ? item.Quantity - availableQuantity : 0;
                    LineItem.Add(item);
                    seqeunceNo++;

                }

            }
            RefreshReturnableLineItemsAvailablity();
        }

        private void RefreshReturnableLineItemsAvailablity()
        {
            foreach (var item in _lineItemTemp)
            {
                item.BackOrder = item.ApprovableQuantity < item.Quantity ? item.Quantity - item.ApprovableQuantity : 0;
                if (item.Product is SaleProduct)
                    continue;
                var returnableItem = _lineItemTemp
                    .Where(s => s.Product is SaleProduct && (s.Product as SaleProduct).ReturnableProduct != null &&
                        (s.Product as SaleProduct).ReturnableProduct.Id == item.ProductId).ToList();

                if (returnableItem.Count == 0)
                    continue;

                item.ApprovableQuantity = returnableItem.Sum(p => p.ApprovableQuantity);
                item.BackOrder = item.ApprovableQuantity < item.Quantity ? item.Quantity - item.ApprovableQuantity : 0;
            }
        }
        protected override void Cancel()
        {
           MessageBoxResult isCancel = MessageBox.Show(
                "Are you sure you want to cancel this purchase order process?\n without approving  ",
                "Order Form", MessageBoxButton.OKCancel);
            if (isCancel == MessageBoxResult.OK)
            {
                var uri = "/views/Orders_Stockist/StockistPurchaseOrderListing.xaml";
                NavigateCommand.Execute(uri);
            }
              
        
        }

        protected override void Confirm()
        {
            throw new NotImplementedException();
        }

        protected override void SaveAndContinue()
        {
            throw new NotImplementedException();
        }

        protected override void AddProduct()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IOrderProductPage>(container).GetProduct(null, OrderType.DistributorToProducer);
                foreach (var popItem in selected)
                {
                    AddUpdateLineItem(popItem, false);
                }
            }

            CalculateSummary();
        }

        protected override void EditProduct(MainOrderLineItem selectedItem)
        {
            using (var container = NestedContainer)
            {

                var selected = Using<IOrderProductPage>(container).EditProduct(null, selectedItem.ProductId, selectedItem.Quantity, OrderType.DistributorToProducer);
                foreach (var popItem in selected)
                {
                    AddUpdateLineItem(popItem, true);
                }
            }
            CanChange = false;
            CalculateSummary();
        }

        protected override void DeleteEditProduct(MainOrderLineItem lineItem)
        {
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete the lineitem", "Purchase Order Form", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;
            DeleteLineItem(lineItem.ProductId);
            CalculateSummary();
        }
        private void CalculateSummary()
        {
            LineItem.Clear();
            _lineItemTemp.Clear();
            using (var container = NestedContainer)
            {
                var summaryService = Using<IProductPackagingSummaryService>(container);
                var pricingService = Using<IDiscountProWorkflow>(container);
                foreach (var item in _lineItem)
                {
                    summaryService.AddProduct(item.Product.Id, item.Quantity);
                }
                List<PackagingSummary> summary = summaryService.GetProductFinalSummary();
                foreach (var s in summary)
                {
                    LineItemPricingInfo info = pricingService.GetPurchaseLineItemPricing(s);
                   
                    _lineItemTemp.Add(new MainOrderLineItem
                    {
                        UnitPrice = info.UnitPrice,
                        UnitVAT = info.VatValue,
                        TotalAmount = info.TotalPrice,
                        TotalNet = info.TotalNetPrice,
                        TotalVAT = info.TotalVatAmount,
                        GrossAmount = info.TotalPrice,
                        UnitDiscount = info.ProductDiscount,
                        TotalProductDiscount = info.TotalProductDiscount,
                        ProductName = s.Product.Description,
                        ProductId = s.Product.Id,
                        Quantity = s.Quantity,
                        ProductType = s.Product is SaleProduct ? "Sale" : "Returnable",
                        CanChange = s.Product is SaleProduct,
                        Product = s.Product
                    });


                }
                _lineItemTemp = _lineItemTemp.OrderBy(n => n.LineItemType).ThenByDescending(n => n.ProductType).ToList();
                CalculateFinalSummary();
                RefreshLineItemsAvailablity();
            }
        }
        private void CalculateFinalSummary()
        {
            TotalGross = _lineItemTemp.Sum(s => s.GrossAmount);
            TotalProductDoscount = _lineItemTemp.Sum(s => s.UnitDiscount * s.Quantity);
            TotalVat = _lineItemTemp.Sum(s => s.TotalVAT);
            TotalNet = _lineItemTemp.Sum(s => s.TotalNet);
        }
        protected override void GetCurrentDocumentRef()
        {
            //throw new NotImplementedException();
        }
    }
}