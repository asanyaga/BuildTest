using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Distributr.Core;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Reporting.WinForms;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{

    public class OrderFormViewModel : OrderBaseViewModel
    {
        public RelayCommand LoadCommand { get; set; }
        public OrderFormViewModel()
        {
            LoadCommand = new RelayCommand(Load);
        }
        public void Continue(OrderContinueMessage obj)
        {
            OrderId = obj.Id;
            IsUnConfirmed = obj.IsUnConfirmed;
        }
        private void Load()
        {
            IntializeViewModel();
            if (IsUnConfirmed)
            {
                IsUnConfirmed = false;
                LoadToContinue();
                  GetCurrentDocumentRef();
                CalculateSummary();
            }
        }

     

        private void IntializeViewModel()
        {
           
            SelectedSalesman = DefaultSalesman;
            SelectedRoute = DefaultRoute;
            SelectedOutlet = DefaultOutlet;
            SelectedShipAddress = DefaultShipTo;
            RefreshViewModel();
            _lineItem.Clear();
            LineItem.Clear();
            _lineItemTemp.Clear();
            GetCurrentDocumentRef();
            AmountPaid = 0m;
            PaymentInfoList.Clear();
            PaymentInfoItems.Clear();
            
        }
        
        protected override void GetCurrentDocumentRef()
        {
            using (var c = NestedContainer)
            {
                
                if (SelectedOutlet != null && SelectedOutlet.Id!=Guid.Empty)
                {
                   OrderReferenceNo = Using<IGetDocumentReference>(c).GetDocReference("SO", SelectedSalesman.Id,
                                                                                       SelectedOutlet.Id);
                }else
                {
                    OrderReferenceNo = "New Order Ref";
                }
            }
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
            Note = string.Empty;
            AmountPaid = 0m;
            PaymentInfoList.Clear();
            PaymentInfoItems.Clear();
        }

        protected override void Cancel()
        {
           
            MessageBoxResult isCancel =
                MessageBox.Show("Are you sure you want to cancel this order process?\n Unsaved changes will be lost ","Order Form",MessageBoxButton.OKCancel);
            if (isCancel == MessageBoxResult.OK)
                IntializeViewModel();
        }

        protected override void Confirm()
        {
            if (!ValidateBeforeAddProduct()) return;
            if (!ValidateLineItem()) return;
            if(DateRequired<DateTime.Today)
            {
                MessageBox.Show("Please verify when this order is required");
                return;
            }
            if(!ValidatePayment())return;
            GetCurrentDocumentRef();
            MessageBoxResult confirm =MessageBox.Show("Are you sure you want to confirm order : "+OrderReferenceNo, "Order Form", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;

            using (var c = NestedContainer)
            {
                var configService = Using<IConfigService>(c);
                Config config = configService.Load();
                Guid costCentreApplicationid = config.CostCentreApplicationId;
                CostCentre dcc = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                User user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                IOrderWorkflow orderWorkflow = Using<IOrderWorkflow>(c);
                IMainOrderFactory mainOrderFactory = Using<IMainOrderFactory>(c);
                string shipto = FormatShipToAddress(SelectedShipAddress );
                MainOrder order = mainOrderFactory.Create(dcc, costCentreApplicationid, SelectedSalesman, user,
                                                          SelectedOutlet, OrderType.OutletToDistributor,
                                                          this.OrderReferenceNo,
                                                          Guid.Empty, shipto, DateRequired, SaleDiscount,Note);
                order.SaleDiscount = SaleDiscount;
                foreach (var item in LineItem)
                {

                    if (item.LineItemType == MainOrderLineItemType.Sale)
                    {
                        SubOrderLineItem lineItem = null;
                        if(item.UnitDiscount>0)
                        {
                            lineItem = mainOrderFactory.CreateDiscountedLineItem(item.ProductId, item.Quantity,
                                                                              item.UnitPrice, item.ProductType,
                                                                              item.UnitVAT, item.UnitDiscount);
                        }else
                        {
                          
                            lineItem = mainOrderFactory.CreateLineItem(item.ProductId, item.Quantity,
                                                                                    item.UnitPrice, item.ProductType,
                                                                                    item.UnitVAT);
                        }
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
                foreach (var paymentInfo in PaymentInfoList.Where(p => p.PaymentModeUsed != PaymentMode.Credit))
                {
                    order.AddOrderPaymentInfoLineItem(paymentInfo);
                }
                orderWorkflow.Submit(order,config);
                Using<IOrderSaveAndContinueService>(c).MarkAsConfirmed(OrderId);
                var action = new List<DistributrMessageBoxButton>
                                 {
                                     DistributrMessageBoxButton.SalesmanOrderNew,
                                     DistributrMessageBoxButton.SalesmanOrderApprove,
                                     DistributrMessageBoxButton.SalesmanOrderSummary
                                 };

                var result = Using<IDistributrMessageBox>(c).ShowBox(action, string.Format("Order {0} on behalf of {1} Saved and submited successfully,Click on one of the button to navigate to module of your choice ",order.DocumentReference,SelectedSalesman.Name));
                NavigateCommand.Execute(result.Url);

                IntializeViewModel();
            }
        }

        protected override void SaveAndContinue()
        {
            if(!LineItem.Any())
            {
                MessageBox.Show("Make sure you have atleast one item");
                return;
                
            }
            using (var c = NestedContainer)
            {
               
                var saveAndContinue= new OrderSaveAndContinueLater();
                saveAndContinue.OrderType=OrderType.OutletToDistributor;
                saveAndContinue.Id =OrderId!=Guid.Empty?OrderId:Guid.NewGuid();
                saveAndContinue.OutletId = SelectedOutlet.Id;
                saveAndContinue.Outlet = SelectedOutlet.Name;
                saveAndContinue.RouteId = SelectedRoute.Id;
                saveAndContinue.SalesmanId = SelectedSalesman.Id;
                saveAndContinue.Salesman = SelectedSalesman.Name;
                if (SelectedShipAddress != null) saveAndContinue.ShipToAddressId = SelectedShipAddress.Id;
                saveAndContinue.Required = DateRequired;
                foreach (var item in LineItem)
                {
                    var i= new OrderSaveAndContinueLaterItem();
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
                MessageBox.Show("Order saved successfully ");
                 var uri = "/views/orders/SalesManOrdersListing.xaml";
                 NavigateCommand.Execute(uri);


            }
        }

        private bool ValidateLineItem()
        {
            string msg = string.Empty;
            if (LineItem.Count <1)
                msg += "\t Lineitem  are required !\n";
            if (msg != string.Empty)
            {
                MessageBox.Show("Provide \n" + msg,"Saleman Order");
                return false;
            }
            return true;
        }

        protected override void AddProduct()
        {
            if (!ValidateBeforeAddProduct()) return;
            using (var container = NestedContainer)
            {
                var selected = Using<IOrderProductPage>(container).GetProduct(SelectedOutlet, OrderType.OutletToDistributor);
                foreach (var popItem in selected)
                {
                    AddUpdateLineItem(popItem,false);
                }
            }
            CanChange = false;
            CalculateSummary();
        }
        protected override void EditProduct(MainOrderLineItem seleectedItem)
        {
            if (!ValidateBeforeAddProduct()) return;
            using (var container = NestedContainer)
            {
                var selected = Using<IOrderProductPage>(container).EditProduct(SelectedOutlet, seleectedItem.ProductId, seleectedItem.Quantity, OrderType.OutletToDistributor);
                foreach (var popItem in selected)
                {
                    AddUpdateLineItem(popItem,true);
                }
            }
            CanChange = false;
            CalculateSummary();
        }
        protected override void DeleteEditProduct(MainOrderLineItem obj)
        {
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete the lineitem", "Order Form", MessageBoxButton.OKCancel);
            if (confirm != MessageBoxResult.OK) return;
            DeleteLineItem(obj.ProductId);
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
                    MainOrderLineItem mitem = new MainOrderLineItem();
                    LineItemPricingInfo info = pricingService.GetLineItemPricing(s, SelectedOutlet.Id);
                    mitem.UnitPrice = info.UnitPrice;
                    mitem.UnitVAT = info.VatValue;
                    mitem.TotalAmount = info.TotalPrice;
                    mitem.TotalNet = info.TotalNetPrice;
                    mitem.TotalVAT = info.TotalVatAmount;
                    mitem.GrossAmount = info.TotalPrice;
                    mitem.UnitDiscount = info.ProductDiscount;
                    mitem.TotalProductDiscount = info.TotalProductDiscount;
                    mitem.ProductName = s.Product.Description;
                    mitem.ProductId = s.Product.Id;
                    mitem.Quantity = s.Quantity;
                    mitem.ProductType = s.Product is SaleProduct ? "Sale" : "Returnable";
                    if (_lineItem.Any(p => p.Product.Id == s.Product.Id && p.IsFreeOfCharge))
                    {
                        mitem.ProductType = "Sale(Free of Charge)";
                    }
                    mitem.CanChange = s.Product is SaleProduct;
                    mitem.Product = s.Product;
                    _lineItemTemp.Add(mitem);


                }
                CalculateFinalSummary();
                RefreshLineItems();
            }

        }

        private void RefreshLineItems()
        {    int seqeunceNo =1;
            LineItem.Clear();
            foreach (var item in _lineItemTemp)
            {
                item.SequenceNo = seqeunceNo;
                LineItem.Add(item);
                seqeunceNo++;
            }
            
        }

        public virtual void CalculateFinalSummary()
        {
            using (var container = NestedContainer)
            {
               

                IDiscountHelper _discountHelper = Using<IDiscountHelper>(container);
                var pricingService = Using<IDiscountProWorkflow>(container);
                _lineItemTemp = _discountHelper.Calculate(_lineItemTemp, SelectedOutlet.Id);

                SaleDiscount = _discountHelper.CalculateSaleDiscount(_lineItemTemp.Sum(s => s.GrossAmount),
                                                                     SelectedOutlet.Id);
                TotalGross = _lineItemTemp.Sum(s => s.GrossAmount).GetTotalGross();
                TotalProductDoscount = _lineItemTemp.Sum(s => s.UnitDiscount*s.Quantity);
                TotalVat = _lineItemTemp.Sum(s => s.TotalVAT);

                _lineItemTemp.ForEach(x => x.TotalNet = x.TotalNet.GetTruncatedValue());
                _lineItemTemp.ForEach(x => x.TotalVAT = x.TotalVAT.GetTruncatedValue());

                TotalNet = ((_lineItemTemp.Sum(s => s.TotalNet) + TotalVat) - SaleDiscount).GetTotalGross();

                
               // pricingService.GetTotalGross())-SaleDiscount);
                
            }
        }

        

        private bool ValidateBeforeAddProduct()
        {
            string msg = string.Empty;
            if (SelectedSalesman == null || SelectedSalesman.Id == Guid.Empty)
                msg += "\tSalesman is required !\n";
            if (SelectedRoute == null || SelectedRoute.Id == Guid.Empty)
                msg += "\tRoute is required !\n";
            if (SelectedOutlet == null || SelectedOutlet.Id == Guid.Empty)
                msg += "\tOutlet is required !\n";
            if (msg != string.Empty)
            {
                MessageBox.Show("Provide \n" + msg, "Saleman Order");
                return false;
            }
            return true;
        }

       
    }
}