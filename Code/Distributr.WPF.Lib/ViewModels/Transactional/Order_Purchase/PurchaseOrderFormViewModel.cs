using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Distributr.Core;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Purchase
{
    public class PurchaseOrderFormViewModel : OrderBaseViewModel
    {
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand SupplierDropDownOpenCommand { get; set; }
        
        public PurchaseOrderFormViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            SupplierDropDownOpenCommand = new RelayCommand(SupplierDropDownOpen);
        }

        private void SupplierDropDownOpen()
        {
            if (LineItem.Any())
                return;
            using (var container = NestedContainer)
            {
                SelectedSupplier = Using<IItemsLookUp>(container).SelectSupplier() ?? new Supplier(Guid.Empty){Name = "---Select Supplier"};
                GetCurrentDocumentRef();
            }
        }

        private void Load()
        {
            IntializeViewModel();
            if (IsUnConfirmed)
            {
                IsUnConfirmed = false;
                LoadPurchaseOrdersToContinue();
                GetCurrentDocumentRef();
                CalculateSummary();
            }
        }

        private void LoadPurchaseOrdersToContinue()
        {
            LoadToContinue();
            SelectedSupplier = _lineItem.FirstOrDefault().Product.Brand.Supplier;
        }

        private void IntializeViewModel()
        {
            RefreshViewModel();
            _lineItem.Clear();
            LineItem.Clear();

            GetCurrentDocumentRef();

        }

        protected override void GetCurrentDocumentRef()
        {
            using (var c = NestedContainer)
            {
                IConfigService _configService = Using<IConfigService>(c);
                Guid costCentreId = _configService.Load().CostCentreId;

                ICostCentreRepository _costCentreService = Using<ICostCentreRepository>(c);
                CostCentre cc = _costCentreService.GetById(costCentreId);
                OrderReferenceNo = Using<IGetDocumentReference>(c).GetDocReference("PO", cc.Id, Guid.Empty);
            }
        }

        private void RefreshViewModel()
        {
            SelectedSupplier = new Supplier(Guid.Empty){Name = "-----Select Supplier-----"};
            CanChange = true;
            TotalNet = 0;
            TotalProductDoscount = 0;
            TotalVat = 0;
            TotalGross = 0;
            SaleDiscount = 0;
            DateRequired = DateTime.Now;
           OrderReferenceNo = "";
        }

        protected override void Cancel()
        {
            MessageBoxResult isCancel = MessageBox.Show(
                "Are you sure you want to cancel this purchase order process?\n Unsaved changes will be lost ", 
                "Order Form", MessageBoxButton.OKCancel);
            if (isCancel == MessageBoxResult.OK)
                IntializeViewModel();
        }

        protected override void Confirm()
        {
            if (!ValidateLineItem()) return;
            GetCurrentDocumentRef();
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to confirm purchase order : " + OrderReferenceNo, "Purchase Order Form", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;

            using (var c = NestedContainer)
            {
                var configService = Using<IConfigService>(c);
                Config config = configService.Load();
                Guid costCentreApplicationid = config.CostCentreApplicationId;
                CostCentre dcc = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                User user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                IPurchaseOrderWorkflow orderWorkflow = Using<IPurchaseOrderWorkflow>(c);
                IMainOrderFactory mainOrderFactory = Using<IMainOrderFactory>(c);
                MainOrder order = mainOrderFactory.Create(dcc, costCentreApplicationid, dcc, user,
                                                          dcc, OrderType.DistributorToProducer,
                                                          this.OrderReferenceNo,
                                                          Guid.Empty,"emptyshiping", DateRequired,0,Note);
                foreach (var item in LineItem)
                {

                    if (item.LineItemType == MainOrderLineItemType.Sale)
                    {
                        SubOrderLineItem lineItem = mainOrderFactory.CreateLineItem(item.ProductId, item.Quantity,
                                                                                    item.UnitPrice, item.ProductType,
                                                                                    item.UnitVAT);
                        order.AddLineItem(lineItem);
                    }
                    else if (item.LineItemType == MainOrderLineItemType.Discount)
                    {
                        SubOrderLineItem lineItem = mainOrderFactory.CreateFOCLineItem(item.ProductId, item.Quantity,
                                                                                       item.ProductType,
                                                                                       item.DiscountType);
                        order.AddLineItem(lineItem);

                    }
                }
                order.Confirm();
                orderWorkflow.Submit(order);
                Using<IOrderSaveAndContinueService>(c).MarkAsConfirmed(OrderId);
                var action = new List<DistributrMessageBoxButton>
                                 {
                                     DistributrMessageBoxButton.PurchaseOrderNew,
                                     DistributrMessageBoxButton.PurchaseOrderSummary
                                 };

                var result = Using<IDistributrMessageBox>(c).ShowBox(action, 
                    string.Format("Purchase Order saved and submited successfully. Click on one of the button to navigate to module of your choice "));
                NavigateCommand.Execute(result.Url);
                IntializeViewModel();
            }
        }

        protected override void SaveAndContinue()
        {
            if (!LineItem.Any())
            {
                MessageBox.Show("Make sure you have atleast one item");
                return;

            }
            using (var c = NestedContainer)
            {
                var saveAndContinue = new OrderSaveAndContinueLater();
                saveAndContinue.OrderType = OrderType.DistributorToProducer;
                saveAndContinue.Id = OrderId != Guid.Empty ? OrderId : Guid.NewGuid();
               
                saveAndContinue.Required = DateRequired;
                foreach (var item in LineItem)
                {
                    var i = new OrderSaveAndContinueLaterItem();
                    i.DiscountType = item.DiscountType;
                    i.LineItemType = item.LineItemType;
                    i.ProductId = item.ProductId;
                    i.ProductType = item.ProductType;
                    i.Quantity = item.Quantity;
                    i.UnitDiscount = item.UnitDiscount;
                    i.UnitPrice = item.UnitPrice;
                    i.UnitVat = item.UnitVAT;
                    saveAndContinue.LineItem.Add(i);

                }

                Using<IOrderSaveAndContinueService>(c).Save(saveAndContinue);
                MessageBox.Show("Purchase Order saved successfully ");
                var uri = "/views/orders_purchase/PurchaseOrderListing.xaml";
                NavigateCommand.Execute(uri);


            }
        }

        private bool ValidateLineItem()
        {
            string msg = string.Empty;
            if (LineItem.Count < 1)
                msg += "\t Lineitem  are required !\n";
            if (msg != string.Empty)
            {
                MessageBox.Show("Provide \n" + msg, "Order");
                return false;
            }
            return true;
        }

        protected override void AddProduct()
        {
            if (SelectedSupplier == null || SelectedSupplier.Id == Guid.Empty)
            {
                MessageBox.Show("Please Select Supplier");
                return;
            }
           
            using (var container = NestedContainer)
            {
                var selected = Using<IOrderProductPage>(container).GetProductBySupplier(null, OrderType.DistributorToProducer,SelectedSupplier);
                foreach (var popItem in selected)
                {
                    AddUpdateLineItem(popItem,false);
                }
            }
           
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
                                              CanChange = s.Product is SaleProduct
                                          });


                }
                _lineItemTemp = _lineItemTemp.OrderBy(n => n.LineItemType).ThenByDescending(n => n.ProductType).ToList();
                CalculateFinalSummary();
                RefreshLineItems();
            }
        }

        private void RefreshLineItems()
        {
            int seqeunceNo = 1;
            foreach (var item in _lineItemTemp)
            {
                item.SequenceNo = seqeunceNo;
                LineItem.Add(item);
                seqeunceNo++;
            }
        }

        private void CalculateFinalSummary()
        {
            TotalGross = _lineItemTemp.Sum(s => s.GrossAmount);
            TotalProductDoscount = _lineItemTemp.Sum(s => s.UnitDiscount * s.Quantity);
            TotalVat = _lineItemTemp.Sum(s => s.TotalVAT);
            TotalNet = _lineItemTemp.Sum(s => s.TotalNet);
        }

        protected override void EditProduct(MainOrderLineItem selectedItem)
        {
            using (var container = NestedContainer)
            {
                
                var selected = Using<IOrderProductPage>(container).EditProduct(null, selectedItem.ProductId, selectedItem.Quantity, OrderType.DistributorToProducer);
                foreach (var popItem in selected)
                {
                    AddUpdateLineItem(popItem,true);
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
         public void Continue(PurchaseOrderContinueMessage obj)
         {
             OrderId = obj.Id;
             IsUnConfirmed = obj.IsUnConfirmed;
         }

         /// <summary>
         /// The <see cref="SelectedSupplier" /> property's name.
         /// </summary>
         public const string SelectedSupplierPropertyName = "SelectedSupplier";

         private Supplier  _supplier = null;

         /// <summary>
         /// Sets and gets the SelectedSupplier property.
         /// Changes to that property's value raise the PropertyChanged event. 
         /// </summary>
         public Supplier SelectedSupplier
         {
             get
             {
                 return _supplier;
             }

             set
             {
                 if (_supplier == value)
                 {
                     return;
                 }

                 RaisePropertyChanging(SelectedSupplierPropertyName);
                 _supplier = value;
                 RaisePropertyChanged(SelectedSupplierPropertyName);
             }
         }
        
    }
}
