using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class OrderApprovalViewModel : OrderFormViewModel
    {
        //public RelayCommand LoadCommand { get; set; }
        public RelayCommand ApproveCommand { get; set; }
        public RelayCommand RejectCommand { get; set; }
        public RelayCommand<Guid> ApproveBatchCommand { get; set; }
        public bool ShowMessage = true;
        bool batchApproveSuccess = false;

        public void SetId(Guid  id)
        {
            OrderId = id;
           
        }

        public OrderApprovalViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            ApproveCommand = new RelayCommand(Approve);
            RejectCommand = new RelayCommand(Reject);
            ApproveBatchCommand= new RelayCommand<Guid>(ApproveBatch);
           
        }

        protected override void AddProduct()
        {
            base.AddProduct();
            RefreshLineItemsAvailablity();
        }

        protected void ApproveBatch(Guid itemId)
        {
           
            SetId(itemId);
            LoadCommand.Execute(null);
           ApproveCommand.Execute(null);
        }


        private void Reject()
        {
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to reject this order : " + OrderReferenceNo, "Order Form", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;
             RefreshLineItemsAvailablity();
             using (var c = NestedContainer)
             {
                 MainOrder order = Using<IMainOrderRepository>(c).GetById(OrderId);
                 Config config = Using<IConfigService>(c).Load();
                 var orderWorkflow = Using<IOrderWorkflow>(c);
                 order = UpdadateMainOrderIfEdited(order);
                 order.Reject();
                 orderWorkflow.Submit(order,config);
                 OrderRejectedMessage();
             }
        }
        private void OrderRejectedMessage()
        {
            using (var c = NestedContainer)
            {
                var faction = new List<DistributrMessageBoxButton>
                                  {
                                      DistributrMessageBoxButton.SalesmanDispatch,
                                      DistributrMessageBoxButton.SalesmanProcessBackOrder,
                                      DistributrMessageBoxButton.SalesmanOrderSummary
                                  };
                var fresult = Using<IDistributrMessageBox>(c)
                    .ShowBox(faction,
                             string.Format("Order {0} on behalf of {1} successfully Rejected", OrderReferenceNo,
                                           SelectedSalesman.Name),
                             string.Format("Distributr: Reject order  "));
                NavigateCommand.Execute(fresult.Url);
                IntializeViewModel();
            }
        }
        private void IntializeViewModel()
        {
            RefreshViewModel();
            SelectedSalesman = DefaultSalesman;
            SelectedRoute = DefaultRoute;
            SelectedOutlet = DefaultOutlet;
            _lineItem.Clear();
            LineItem.Clear();
            _lineItemTemp.Clear();

         

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
            ShipToAddress = string.Empty;
            Note = string.Empty;
            batchApproveSuccess = false;
            

        }

        private string messageItems()
        {
            var message = "";

            if (LineItem.Any(l => l.Quantity > l.Available))
            {
                foreach (var mainOrderLineItem in LineItem.Where(p => p.Quantity > p.Available))
                {
                    int available = (int)mainOrderLineItem.Available;
                    message += "\n" + mainOrderLineItem.ProductName + "\t Available:  " + available;
                }
            }
            return message;
        }

        private void Approve()
        {
            batchApproveSuccess = true;
            RefreshLineItemsAvailablity();
           using(var c = NestedContainer)
           {
               MainOrder  order= Using<IMainOrderRepository>(c).GetById(OrderId);
               Config config =Using<IConfigService>(c).Load();
                Guid costCentreApplicationid = config.CostCentreApplicationId;
               order.ChangeccId(costCentreApplicationid);
               var orderWorkflow = Using<IOrderWorkflow>(c);
               if (LineItem.All(s => s.ApprovableQuantity == 0))
               {
                   batchApproveSuccess = false;
                   //MessageBox.Show("Order cannot be Approved.\n\t there isnt Inventory to satisfy any item in the following order", "Order form , OrderRef " + OrderReferenceNo);
                   MessageBox.Show("Order cannot be Approved.\t" + messageItems(), "Order form , OrderRef " + OrderReferenceNo);
                   return;
               }
               else if(LineItem.All(s=>s.ApprovableQuantity==s.Quantity))
               {
                   //Approve all lineitem No Back order
                    order = UpdadateMainOrderIfEdited(order);
                    foreach(var line in order.PendingApprovalLineItems)
                    {
                        order.ApproveLineItem(line);
                    }
               }
               else
               {
                   DistributrMessageBoxButton action = ShowApprovableVsBackorder();
                   if (action == DistributrMessageBoxButton.Cancel)
                   {
                       batchApproveSuccess = false;
                       return;
                   }
                   //Approve and back order
                   bool takeTolostSale = false;//action != DistributrMessageBoxButton.SalesmanBackOrderAndApprove;
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
              HandleFiscalPrinter(order.Id);
               if ( ShowMessage)
               {
                   OrderApprovedMessage();
               }
           }

        }

       
        private void OrderApprovedMessage()
        {
            using (var c = NestedContainer)
            {
                var faction = new List<DistributrMessageBoxButton>
                                  {
                                      DistributrMessageBoxButton.SalesmanDispatch,
                                      DistributrMessageBoxButton.SalesmanProcessBackOrder,
                                      DistributrMessageBoxButton.SalesmanOrderSummary
                                  };
                var fresult = Using<IDistributrMessageBox>(c)
                    .ShowBox(faction,
                             string.Format("Order {0} on behalf of {1} successfully approved", OrderReferenceNo,
                                           SelectedSalesman.Name),
                             string.Format("Distributr: Approve order on behalf of salesman "));
                NavigateCommand.Execute(fresult.Url);
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
                    else if(existing == null)
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
                    var existing =LineItem.FirstOrDefault(g => g.LineItemType == item.LineItemType && g.ProductId == item.Product.Id);
                    if(existing==null)
                    {
                        order.RemoveLineItem(item);
                    }
                }
            }
            return order;
        }
        public override void CalculateFinalSummary()
        {
            base.CalculateFinalSummary();
            RefreshLineItemsAvailablity();
        }

        private DistributrMessageBoxButton ShowApprovableVsBackorder()
        {

            using (var c = NestedContainer)
            {

                var unfulfilable = LineItem.Where(s => s.ApprovableQuantity < s.Quantity);
                string messageitem = unfulfilable.Aggregate("",
                                                            (current, line) =>
                                                            current +
                                                            ("\t " + line.ProductName + ": Required: " + line.Quantity +
                                                             ", Available :" + line.ApprovableQuantity + "\n"));
                var action = new List<DistributrMessageBoxButton>
                                 {
                                     DistributrMessageBoxButton.SalesmanBackOrderAndApprove,
                                     DistributrMessageBoxButton.Cancel
                                 };

                var result = Using<IDistributrMessageBox>(c)
                    .ShowBox(action,
                             string.Format(
                                 "Order cannot be fulfilled.\n\tThe available Inventory cant satisfy the following order item(s)\n\n" +
                                 messageitem
                                 ), "Order form, OrderRef " + OrderReferenceNo);
                return result.Button;
            }
        }

        private void Load()
        {
            
            IntializeViewModel();
            CanChange = false;
            _lineItem.Clear();
            _lineItemTemp.Clear();
           
            using(var c =NestedContainer)
            {

                MainOrder o = Using<IMainOrderRepository>(c).GetById(OrderId);
                var pricingService = Using<IDiscountProWorkflow>(c);
                IsEditable = o.IsEditable();
                if (!o.IsApprovable())
                {
                    MessageBox.Show("Order has no item to be approved", "Order approval");
                    NavigateCommand.Execute(@"\Views\Orders\SalesManOrdersListing.xaml");
                    return;
                }
                if (o.DocumentIssuerCostCentre is Distributor)
                {
                    SelectedSalesman = o.DocumentRecipientCostCentre as DistributorSalesman;
                }else
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
                    //TotalGross = pricingService.GetTotalGross(o.TotalGross);
                    TotalGross =(o.ItemSummary.Sum(x => x.TotalGross.GetTruncatedValue())).GetTotalGross();
                    ShipToAddress = o.ShipToAddress;
                    Note = o.Note;

                    foreach (var item in o.PendingApprovalLineItems.OrderBy(s => s.LineItemType).ThenByDescending(s => s.Description))
                    {
                        if (item.Product is SaleProduct && item.LineItemType == MainOrderLineItemType.Sale)
                        {
                            ProductPopUpItem ppi = new ProductPopUpItem();
                            ppi.Product = item.Product;
                            ppi.Quantity = item.Qty;
                            _lineItem.Add(ppi);
                        }
                        MainOrderLineItem mitem = new MainOrderLineItem();
                        mitem.UnitPrice = item.Value;
                        mitem.UnitVAT = item.LineItemVatValue;
                        mitem.TotalAmount = item.LineItemTotal.GetTruncatedValue();
                        mitem.TotalNet = item.TotalNetPrice;
                        mitem.TotalVAT = item.LineItemVatTotal;
                        mitem.GrossAmount = item.LineItemTotal.GetTruncatedValue();
                        mitem.UnitDiscount = item.ProductDiscount;
                        mitem.TotalProductDiscount = item.TotalDiscount;
                        mitem.ProductName = item.Product.Description;
                        mitem.ProductId = item.Product.Id;
                        mitem.Quantity = item.Qty;
                        mitem.ProductType = item.LineItemType.ToString();
                        mitem.CanChange = o.IsEditable() && item.Product is SaleProduct &&item.LineItemType==MainOrderLineItemType.Sale;
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
                var configService= Using<IConfigService>(c);
                var config=configService.Load();
                foreach (var item in _lineItemTemp)
                {
                    Product product = item.Product;
                    decimal alreadyUsed = LineItem.Where(p => p.ProductId == item.ProductId).Sum(s => s.Quantity);
                    decimal availableQuantity = 0;
                    decimal balance = 0;
                    var inveAvailable = inventoryRepository.GetByProductIdAndWarehouseId(item.ProductId,config.CostCentreId);
                    balance = inveAvailable != null ? inveAvailable.Balance : 0;
                    if (product is SaleProduct)
                    {
                        decimal current = balance - alreadyUsed;
                        SaleProduct sp = product as SaleProduct;
                        if(sp.ReturnableProduct!=null)
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
                    else if (product is ReturnableProduct )
                    {
                        decimal current =balance - alreadyUsed;
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
                item.BackOrder = item.ApprovableQuantity < item.Quantity ? item.Quantity -item.ApprovableQuantity : 0;
                if (item.Product is SaleProduct)
                    continue;
                var returnableItem = _lineItemTemp
                    .Where(s =>s.Product is SaleProduct && (s.Product as SaleProduct).ReturnableProduct != null &&
                        (s.Product as SaleProduct).ReturnableProduct.Id == item.ProductId).ToList();

                if(returnableItem.Count==0)
                    continue;

                item.ApprovableQuantity = returnableItem.Sum(p => p.ApprovableQuantity);
                item.BackOrder = item.ApprovableQuantity < item.Quantity ? item.Quantity - item.ApprovableQuantity : 0;
            }
        }


        protected override void Cancel()
        {
            var uri = @"\Views\Orders\SalesManOrdersListing.xaml";
            NavigateCommand.Execute(uri);
        }

        protected override void Confirm()
        {
            throw new System.NotImplementedException();
        }

        protected override void GetCurrentDocumentRef()
        {

           
        }


        public const string ShipToAddressPropertyName = "ShipToAddress";
        private string  _shipToAddress ="";
        public string ShipToAddress
        {
            get
            {
                return _shipToAddress;
            }

            set
            {
                if (_shipToAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(ShipToAddressPropertyName);
                _shipToAddress = value;
                RaisePropertyChanged(ShipToAddressPropertyName);
            }
        }
        public const string IsEditablePropertyName = "IsEditable";
        private bool _iseditable = true;
        public bool IsEditable
        {
            get
            {
                return _iseditable;
            }

            set
            {
                if (_iseditable == value)
                {
                    return;
                }

                RaisePropertyChanging(IsEditablePropertyName);
                _iseditable = value;
                RaisePropertyChanged(IsEditablePropertyName);
            }
        }
       
       
    }
}