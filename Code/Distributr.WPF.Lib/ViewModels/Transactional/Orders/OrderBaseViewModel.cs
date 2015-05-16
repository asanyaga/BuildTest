using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.MainPage;
using Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    
    public abstract class OrderBaseViewModel  : DistributrViewModelBase
    {
       
     
        public ObservableCollection<PaymentInfoItem> PaymentInfoItems { get; set; }
        public RelayCommand<PaymentInfoItem> RemovePaymentLineItemCommand { get; set; }
        protected List<ProductPopUpItem> _lineItem;
        public ObservableCollection<MainOrderLineItem> LineItem { get; set; }
        protected List<MainOrderLineItem> _lineItemTemp;
        public RelayCommand LoadSalesmanCommand { get; set; }
        public RelayCommand SetupCommand { get; set; }
        public RelayCommand SalesmanChangedCommand { get; set; }
        public RelayCommand RouteChangedCommand { get; set; }
        public RelayCommand OutletChangedCommand { get; set; }
        public RelayCommand SalesmanDropDownOpenedCommand { get; set; }
        public RelayCommand RouteDropDownOpenedCommand { get; set; }
        public RelayCommand OutletDropDownOpenedCommand { get; set; }
        public RelayCommand ShipDropDownOpenedCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand<MainOrderLineItem> EditProductCommand { get; set; }
        public RelayCommand<MainOrderLineItem> DeleteProductCommand { get; set; }
        public RelayCommand<Receipt> ViewReceiptCommand { get; set; }
        public RelayCommand<Receipt> ViewPrintableReceiptCommand { get; set; }
        public RelayCommand ViewInvoiceCommand { get; set; }
        public RelayCommand ViewPrintableInvoiceCommand { get; set; }
        public ObservableCollection<Receipt> InvoiceReceipts { get; set; }
        public RelayCommand<PaymentInfoItem> ConfirmPaymentCommand { get; set; }
        public RelayCommand ReceivePaymentsCommand { get; set; }
        public List<PaymentInfo> PaymentInfoList { get; set; }
        public RelayCommand SaveAndContinueCommand { get; set; }
       
        //Paginated items
        internal IPagenatedList<Outlet> PaginatedOutlets { get; set; }
        internal IPagenatedList<Route> PaginatedRoutes { get; set; }
     

        protected OrderBaseViewModel()
        {
         
           SetupCommand = new RelayCommand(Setup);
           SalesmanChangedCommand=new RelayCommand(SalesmanChanged);
            RouteChangedCommand = new RelayCommand(RouteChanged);
            OutletChangedCommand=new RelayCommand(OutletChanged);
            SalesmanDropDownOpenedCommand = new RelayCommand(SalesmanDropDownOpened);
            RouteDropDownOpenedCommand = new RelayCommand(RouteDropDownOpened);
            OutletDropDownOpenedCommand = new RelayCommand(OutletDropDownOpened);
            ShipDropDownOpenedCommand = new RelayCommand(ShipDropDownOpened);
            AddProductCommand = new RelayCommand(AddProduct);
            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
            _lineItem = new List<ProductPopUpItem>();
            LineItem = new ObservableCollection<MainOrderLineItem>();
            _lineItemTemp = new List<MainOrderLineItem>();
            EditProductCommand = new RelayCommand<MainOrderLineItem>(EditProduct);
            DeleteProductCommand = new RelayCommand<MainOrderLineItem>(DeleteEditProduct);
            ViewReceiptCommand = new RelayCommand<Receipt>(ViewReceipt);
            ViewPrintableReceiptCommand = new RelayCommand<Receipt>(ViewPrintableReceipt);
            ViewInvoiceCommand = new RelayCommand(ViewInvoice);
            ViewPrintableInvoiceCommand = new RelayCommand(ViewPrintableInvoice);
            InvoiceReceipts = new ObservableCollection<Receipt>();
           
            PaymentInfoList = new List<PaymentInfo>();
            PaymentInfoItems = new ObservableCollection<PaymentInfoItem>();
            ConfirmPaymentCommand = new RelayCommand<PaymentInfoItem>(ConfirmPayment);
            RemovePaymentLineItemCommand = new RelayCommand<PaymentInfoItem>(RemovePaymentInfo);
            ReceivePaymentsCommand = new RelayCommand(ReceivePayment);
            SaveAndContinueCommand = new RelayCommand(SaveAndContinue);
            SetupCommand.Execute(null);
            ConfigureFiscalPrinter();
            IsUnConfirmed = false;
        }

       
        #region Methods

        private FiscalPrinterUtility _printerUtility=null;
        public FiscalPrinterUtility PrinterUtility { get { return _printerUtility; } }
       public void ConfigureFiscalPrinter()
        {

            int port = 2;
            int portSpeed = 115200;
            using (var c = NestedContainer)
            {
                var printerRepo = Using<IGeneralSettingRepository>(c);
                var printerEnabled = printerRepo.GetByKey(GeneralSettingKey.FiscalPrinterEnabled);
                if (printerEnabled != null && !string.IsNullOrEmpty(printerEnabled.SettingValue))
                {

                    bool enabled = Boolean.Parse(printerEnabled.SettingValue);
                    _printerUtility = null;
                    if (!enabled) return;

                    var portSetting = printerRepo.GetByKey(GeneralSettingKey.FiscalPrinterPort);
                    if (portSetting != null && !string.IsNullOrEmpty(portSetting.SettingValue))
                    {
                        port = Convert.ToInt32(portSetting.SettingValue);
                    }
                    var portSpeedSetting = printerRepo.GetByKey(GeneralSettingKey.FiscalPrinterPortSpeed);
                    if (portSpeedSetting != null && !string.IsNullOrEmpty(portSpeedSetting.SettingValue))
                    {
                        portSpeed = Convert.ToInt32(portSpeedSetting.SettingValue);
                    }
                    _printerUtility = new FiscalPrinterUtility(port, portSpeed) { IsEnabled = enabled };
                }
            }

        }
        protected abstract void Cancel();


        protected abstract void Confirm();
        protected abstract void SaveAndContinue();

        protected abstract void AddProduct();
        protected abstract void EditProduct(MainOrderLineItem obj);
        protected abstract void DeleteEditProduct(MainOrderLineItem obj);
        protected abstract void GetCurrentDocumentRef();

        public void GetId(ViewModelMessage obj)
        {
            OrderId = obj.Id;
           
        }
        protected virtual void ViewInvoice()
        {
            const string uri = "/views/invoicedocument/invoicedocument.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id =OrderId });
            NavigateCommand.Execute(uri);
        }
        protected void LoadToContinue()
        {
            using (var c = NestedContainer)
            {
                var o = Using<IOrderSaveAndContinueService>(c).GetById(OrderId);
                var costCentreRepository = Using<ICostCentreRepository>(c);

                SelectedSalesman = costCentreRepository.GetById(o.SalesmanId) as DistributorSalesman;

                Outlet outlet = costCentreRepository.GetById(o.OutletId) as Outlet;
                if (outlet != null)
                {
                    SelectedRoute = outlet.Route;
                    SelectedOutlet = outlet;
                }
                OrderReferenceNo = "";
                SaleDiscount = o.SaleDiscount;
                DateRequired = DateTime.Now;
                Status = "New";
                TotalNet = 0;
                TotalVat = 0;
                TotalProductDoscount = 0;
                TotalGross = 0;
                if (outlet != null)
                {
                    var shipping = outlet.ShipToAddresses.FirstOrDefault(s => s.Id == o.ShipToAddressId);
                    SelectedShipAddress = shipping ?? DefaultShipTo;
                }
                Note = "";

                foreach (
                    var item in
                        o.LineItem.OrderBy(s => s.LineItemType)
                    )
                {
                    var product = Using<IProductRepository>(c).GetById(item.ProductId);
                    if (product is SaleProduct)
                    {
                        ProductPopUpItem ppi = new ProductPopUpItem();
                        ppi.Product = product;
                        ppi.Quantity = item.Quantity;
                        _lineItem.Add(ppi);
                    }
                    MainOrderLineItem mitem = new MainOrderLineItem();
                    mitem.UnitPrice = item.UnitPrice;
                    mitem.UnitVAT = item.UnitVat;
                    mitem.TotalAmount = 0;
                    mitem.TotalNet = 0;
                    mitem.TotalVAT = 0;
                    mitem.GrossAmount = 0;
                    mitem.UnitDiscount = item.UnitDiscount;
                    mitem.TotalProductDiscount = 0;
                    mitem.ProductName = product.Description;
                    mitem.ProductId = product.Id;
                    mitem.Quantity = item.Quantity;
                    mitem.ProductType = item.LineItemType.ToString();
                    mitem.CanChange = product is SaleProduct &&
                                      item.LineItemType == MainOrderLineItemType.Sale;
                    mitem.LineItemType = item.LineItemType;
                    mitem.Product = product;
                    _lineItemTemp.Add(mitem);
                }
              
            }
        }
        protected virtual void ViewReceipt(Receipt receipt)
        {
            receipt = receipt ?? SelectedReceipt;
            if (receipt==null || receipt.Id == Guid.Empty)
                MessageBox.Show("Select a receipt to view.", "Distributr: View Receipt", MessageBoxButton.OK);
            else
            {
                const string uri = "/views/receiptdocuments/receiptdocument.xaml";
                Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = receipt.Id });
                NavigateCommand.Execute(uri);

            }
        }
        protected void GetOrderPaymemtInfo(MainOrder order)
        {
            PaymentInfoItems.Clear();
            var payments = order.GetPayments;
            if (payments.Any())
            {
                int i = 0;
                foreach (var paymentInfo in payments)
                {
                    i++;
                    PaymentInfoItems.Add(new PaymentInfoItem()
                    {
                        Id = paymentInfo.Id,
                        Amount = paymentInfo.Amount,
                        ConfirmedAmount = paymentInfo.ConfirmedAmount,
                        IsConfirmed = paymentInfo.IsConfirmed,
                        PaymentTypeDisplayer = paymentInfo.PaymentModeUsed.ToString(),
                        BankInfo = string.Format("{0}-{1}", paymentInfo.Bank, paymentInfo.BankBranch),
                        SequenceNo = i,
                        ShowConfirmHyperlink = false
                    });

                }
                AmountPaid = PaymentInfoItems.Sum(p => p.ConfirmedAmount);
                OutstandingAmount = (PaymentInfoItems.Sum(p => p.Amount) - AmountPaid);
            }

        }
        protected void GetReceipts(Guid orderId)
        {
            if (InvoiceReceipts.Any())
                InvoiceReceipts.Clear();
            using (var c = NestedContainer)
            {
                var invoice = Using<IInvoiceRepository>(c).GetInvoiceByOrderId(orderId);
                if (invoice != null)
                {
                    var receipts = Using<IReceiptRepository>(c).GetReceipts(invoice.Id);
                    receipts.ToList().ForEach(InvoiceReceipts.Add);
                }
            }
        }
        protected  void AddUpdateLineItem(ProductPopUpItem popupItem,bool isEdit)
        {
            if (_lineItem.Any(s => s.Product.Id == popupItem.Product.Id))
            {
                var item = _lineItem.First(s => s.Product.Id == popupItem.Product.Id);
                item.Quantity =isEdit? popupItem.Quantity:popupItem.Quantity+item.Quantity;
                item.IsFreeOfCharge = popupItem.IsFreeOfCharge;
            }
            else
            {
                var line = new ProductPopUpItem
                               {
                                   Quantity = popupItem.Quantity,
                                   Product = popupItem.Product,
                                   IsFreeOfCharge = popupItem.IsFreeOfCharge
                               };
                _lineItem.Add(line);
            }


        }

        protected void AddOrderLineItems(List<MainOrder.LineItemSummary> lineItems)
        {
            if (LineItem.Any())
                LineItem.Clear();
            using (var c = NestedContainer)
            {
                var pricingService = Using<IDiscountProWorkflow>(c);

                foreach (var item in lineItems)
                {
                    var lineitem = new MainOrderLineItem
                    {
                        ProductId = item.Product.Id,
                        ProductName = item.Product.Description,
                        ProductType = item.LineItemType.ToString(),
                        GrossAmount = item.TotalGross.GetTruncatedValue(),
                        TotalAmount = item.TotalNet,
                        TotalNet = item.TotalNet,
                        TotalVAT = item.TotalVat,
                        UnitVAT = item.VatValue,
                        Quantity = item.Qty,
                        UnitPrice = item.Value,
                        UnitDiscount = item.ProductDiscount,
                        ApprovableQuantity = item.ApprovedQuantity,
                        BackOrder = item.BackOrderQuantity,
                        LossSaleQuantity = item.LostSaleQuantity,
                        TotalProductDiscount = item.Product.ProductDiscounts.Sum(p => p.CurrentDiscountRate(item.Qty))

                    };
                    LineItem.Add(lineitem);
                }
            }

        }
        protected void DeleteLineItem(Guid productId)
        {
            if (_lineItem.Any(s => s.Product.Id == productId))
            {
                var item = _lineItem.First(s => s.Product.Id == productId );
                _lineItem.Remove(item);
            }
            


        }

        protected string FormatShipToAddress(ShipToAddress selectedShipAddress)
        {
            if (selectedShipAddress == null || selectedShipAddress.Name.Contains("--Select Shipto address--"))
            {
                return string.Empty;
            }
            else
            {
                return string.Format("[" + selectedShipAddress.Name + "]" + "[" + selectedShipAddress.Code + "]" + "[" + selectedShipAddress.PhysicalAddress + "]");
            }
        }
        protected void HandleFiscalPrinter(Guid orderid)
        {

            Application.Current.Dispatcher.InvokeAsync(() =>
                                                           {
                                                               using (var c = NestedContainer)
                                                               {
                                                                   ConfigureFiscalPrinter();
                                                                   if (PrinterUtility != null && PrinterUtility.IsEnabled)
                                                                   {
                                                                       var approvedOrder = Using<IMainOrderRepository>(c).GetById(orderid);
                                                                       if (approvedOrder != null)
                                                                       {
                                                                           #region Working Printing
                                                                           //if (approvedOrder.PaidAmount > 0)
                                                                           //{
                                                                           //    PrinterUtility.PrinterOrderReceipt(approvedOrder);
                                                                           //}
                                                                           //else
                                                                           //{
                                                                           //    PrinterUtility.PrintNonFiscalOrderReceipt(approvedOrder);
                                                                           //}
                                                                           #endregion
                                                                           PrinterUtility.PrintReceipt(approvedOrder);
                                                                       }
                                                                           
                                                                   }
                                                               }
                                                           });

        }

        private void OutletDropDownOpened()
        {
            if(SelectedRoute==null||SelectedRoute.Id==Guid.Empty)
            {
                MessageBox.Show("Select route first")
                    ;return;
            }
            using (var container = NestedContainer)
            {
                SelectedOutlet = Using<IItemsLookUp>(container).SelectOutletByRoute(SelectedRoute.Id);
                GetCurrentDocumentRef();
            }
        }
        private void ShipDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                if(SelectedOutlet==null || SelectedOutlet.Id==Guid.Empty)
                {
                    MessageBox.Show("Choose outlet first");
                    return;
                }
                var shiptoId = Using<IItemsLookUp>(container).SelectOutletShipToAddress(SelectedOutlet.Id);
                var tblship =
                    Using<CokeDataContext>(container).tblShipToAddress.FirstOrDefault(p => p.Id == shiptoId);
                if (tblship == null)
                {
                    SelectedShipAddress = DefaultShipTo;
                    return;
                }

                SelectedShipAddress = new ShipToAddress(tblship.Id)
                                          {
                                              Code = tblship.Code,
                                              Name = tblship.Name,
                                              Description = tblship.Description,
                                              Latitude = tblship.Latitude.HasValue ? tblship.Latitude.Value : 0m,
                                              Longitude = tblship.Longitude.HasValue ? tblship.Longitude.Value : 0m,
                                              PhysicalAddress = tblship.PhysicalAddress,
                                              PostalAddress = tblship.PostalAddress,
                                              _DateCreated = tblship.IM_DateCreated,
                                              _DateLastUpdated = tblship.IM_DateLastUpdated
                                          };

            }
        }

        private void RouteDropDownOpened()
        {
            if(SelectedSalesman==null ||SelectedSalesman.Id==Guid.Empty)
            {
                MessageBox.Show("Select Salesman first");return;
            }
            using (var container = NestedContainer)
            {
                SelectedRoute = Using<IItemsLookUp>(container).SelectRoute(SelectedSalesman.Id)??DefaultRoute;
                GetCurrentDocumentRef();
            }
        }

        private void SalesmanDropDownOpened()
        {
            
            using (var container = NestedContainer)
            {
                SelectedSalesman = Using<IItemsLookUp>(container).SelectDistributrSalesman() as DistributorSalesman ??
                                   DefaultSalesman;
                GetCurrentDocumentRef();
            }

        }

        private void SalesmanChanged()
        {
            SelectedRoute = DefaultRoute;
        }
        private void RouteChanged()
        {
            SelectedOutlet = DefaultOutlet;
        }

        private void OutletChanged()
        {
            SelectedShipAddress = DefaultShipTo;
        }

        private void Setup()
        {
            PaymentInfoItems.Clear();
            PaymentInfoList.Clear();
            SelectedReceipt = new Receipt(Guid.Empty) {DocumentReference = "--Select Receipt--"};
            SelectedRoute = DefaultRoute;
            SelectedSalesman = DefaultSalesman;
            SelectedOutlet = DefaultOutlet;
            SelectedShipAddress = DefaultShipTo;
        }

        protected DistributorSalesman DefaultSalesman
        {
            get
            {
                return
                    new DistributorSalesman(Guid.Empty) { Name = "Select Salesman--" };
            }
        }
        protected Route DefaultRoute
        {
            get
            {
                return
                    new Route(Guid.Empty) { Name = "Select Route--" };
            }
        }
        protected Outlet DefaultOutlet
        {
            get
            {
                return
                    new Outlet(Guid.Empty) { Name = "Select Outlet--" };
            }
        }
        protected ShipToAddress DefaultShipTo
        {
            get
            {
                return
                    new ShipToAddress(Guid.Empty) { Name = "Select ShipTo--" };
            }
        }

        

        private void ViewPrintableInvoice()
        {
            using (var c = NestedContainer)
            {
                Using<IPrintableDocumentViewer>(c).ViewDocument(OrderId, DocumentType.Invoice);
            }
        }

        private void ViewPrintableReceipt(Receipt receipt)
        {
            using (var c = NestedContainer)
            {
                if (receipt == null || receipt.Id == Guid.Empty)
                    MessageBox.Show("Select a receipt to view.", "Distributr: View Receipt", MessageBoxButton.OK);
                else
                    Using<IPrintableDocumentViewer>(c).ViewDocument(receipt.Id, DocumentType.Receipt);
            }
        }
        #region PAYMENTS
        protected virtual void ConfirmPayment(PaymentInfoItem item)
        {
            if (item.IsConfirmed)
            {
                MessageBox.Show("This payment has already been confirmed.", "Distributr: Payment Module", MessageBoxButton.OK);
                return;
            }
            try
            {
                using (var c = NestedContainer)
                {
                    PaymentInfo info = PaymentInfoList.FirstOrDefault(p => p.Id == item.Id);
                    if (info != null)
                        info.IsConfirmed =
                            Using<IPaymentUtils>(c).ConfirmPayment(info);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error confirming payment\ndetails" + ex.Message);
            }

        }


        protected virtual void RemovePaymentInfo(PaymentInfoItem item)
        {
            if (item != null)
            {
                var modeUsed = (PaymentMode) Enum.Parse(typeof (PaymentMode), item.PaymentTypeDisplayer);
               
                var removedItems = PaymentInfoList.Where(p => p.PaymentModeUsed == modeUsed).ToList();
                if (removedItems.Any())
                {
                    if (modeUsed != PaymentMode.Credit)
                        AmountPaid -= removedItems.Sum(n=>n.ConfirmedAmount);
                    if (AmountPaid < 0) AmountPaid = 0;
                    foreach (var removedItem in removedItems)
                    {
                        PaymentInfoList.Remove(removedItem);
                    }
                }
                PaymentInfoItems.Remove(item);
                
            }

        }


        protected virtual void ReceivePayment()
        {
            if (TotalNet<0)
            {
                MessageBox.Show( /*"Gross amount should be greater than zero"*/
                GetLocalText("sl.pos.receivepayments.messagebox.grossamountiszero")
               , "!" + GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
               , MessageBoxButton.OK);
                return;
            }
            if (AmountPaid >= TotalNet)
            {
                MessageBox.Show("The payment is already confirmed,remove payments first");
                return;
            }
            ChoosePaymentMode(TotalNet - AmountPaid);
        }

        protected bool ValidatePayment()
        {
            bool isValid = true;
            if (TotalNet>0 && (AmountPaid<=0 ||PaymentInfoList.Count(p=>p.PaymentModeUsed !=PaymentMode.Credit) <= 0))
            {
                MessageBoxResult isResult = MessageBox.Show( /*"Are you sure you want to complete Orders without receiving payment."*/
                     GetLocalText("sl.createOrder.completesale.messagebox.notReceivedPayment")
                    , "! " + GetLocalText("sl.createOrder.completesale.messagebox.caption") /*"Distributr:Orders"
*/                    , MessageBoxButton.OKCancel);
                if (isResult == MessageBoxResult.Cancel)
                {
                    isValid = false;
                }
            }
            return isValid;
        }


        protected bool ValidatePosPayment()
        {
            bool isValid = true;
            if (TotalNet > 0 && (AmountPaid <= 0 || PaymentInfoList.Count(p => p.PaymentModeUsed != PaymentMode.Credit) <= 0))
            {
                MessageBoxResult isResult = MessageBox.Show(/*"Are you sure you want to complete Sales without receiving payment."*/
                   GetLocalText("sl.pos.completesale.messagebox.notReceivedPayment")
                    , "! " + GetLocalText("sl.pos.messagebox.caption")/*"Distributr:Pos"*/
                    , MessageBoxButton.OKCancel);
                if (isResult == MessageBoxResult.Cancel)
                {
                    isValid = false;
                }
            }
            return isValid;
        }


        private void ChoosePaymentMode(decimal amountToPay)
        {
            using (var container = NestedContainer)
            {
                var payInfo = Using<IPaymentPopup>(container).GetPayments(amountToPay, OrderReferenceNo);
                if (payInfo.Any())
                {
                    PaymentInfoItems.Clear();
                    var credits = payInfo.Where(p => p.PaymentModeUsed == PaymentMode.Credit).ToList();

                    PaymentInfoList.AddRange(payInfo.Except(credits));
                    var cheques = PaymentInfoList.Where(p => p.PaymentModeUsed == PaymentMode.Cheque).ToList();
                    var cash = PaymentInfoList.Where(p => p.PaymentModeUsed == PaymentMode.Cash).ToList();
                    var mMoney = PaymentInfoList.Where(p => p.PaymentModeUsed == PaymentMode.MMoney).ToList();
                    
                    PaymentInfoItem cashInfo = null;
                    PaymentInfoItem chequeInfo = null;
                    PaymentInfoItem mMoneyInfo = null;
                    if(cash.Any())
                    {
                        cashInfo = new PaymentInfoItem()
                        {
                            ConfirmedAmount = cash.Sum(n => n.ConfirmedAmount),
                            Amount = cash.Sum(n => n.Amount),
                            IsConfirmed = true,
                            IsNotCredit = true,
                            PaymentTypeDisplayer = cash.First().PaymentModeUsed.ToString(),
                            ShowConfirmHyperlink = false,
                            Id = Guid.NewGuid()
                        };
                        PaymentInfoItems.Add(cashInfo);
                    }
                   
                    if(cheques.Any())
                    {
                        chequeInfo = new PaymentInfoItem()
                        {
                            ConfirmedAmount = cheques.Sum(n => n.ConfirmedAmount),
                            Amount = cheques.Sum(n => n.Amount),
                            IsConfirmed = true,
                            PaymentTypeDisplayer = cheques.First().PaymentModeUsed.ToString(),
                            ShowConfirmHyperlink = false,
                            Id = Guid.NewGuid(),
                            IsNotCredit = true
                        };
                        PaymentInfoItems.Add(chequeInfo);
                    }

                    if (mMoney.Any())
                    {
                        mMoneyInfo = new PaymentInfoItem()
                        {
                            ConfirmedAmount = mMoney.Sum(n => n.ConfirmedAmount),
                            Amount = mMoney.Sum(n => n.Amount),
                            IsConfirmed = true,
                            PaymentTypeDisplayer = mMoney.First().PaymentModeUsed.ToString(),
                            ShowConfirmHyperlink = false,
                            Id = Guid.NewGuid(),
                            IsNotCredit = true
                        };
                        PaymentInfoItems.Add(mMoneyInfo);
                    }
                
                  AmountPaid += payInfo.Where(n => n.PaymentModeUsed != PaymentMode.Credit).Sum(p => p.ConfirmedAmount);
                }
                

            }

        }
        #endregion

        #endregion

        #region properties

        
        public const string IsUnConfirmedPropertyName = "IsUnConfirmed";
        private bool _IsUnConfirmed = true;
        public bool IsUnConfirmed
        {
            get
            {
                return _IsUnConfirmed;
            }

            set
            {
                if (_IsUnConfirmed == value)
                {
                    return;
                }

                RaisePropertyChanging(IsUnConfirmedPropertyName);
                _IsUnConfirmed = value;
                RaisePropertyChanged(IsUnConfirmedPropertyName);
            }
        }
       
        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }
              var  oldValue = _orderId;
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName, oldValue, value, true);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Order";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string OrderReferenceNoPropertyName = "OrderReferenceNo";
        private string _orderReference = "OrderRef";
        public string OrderReferenceNo
        {
            get
            {
                return _orderReference;
            }

            set
            {
                if (_orderReference == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderReferenceNoPropertyName);
                _orderReference = value;
                RaisePropertyChanged(OrderReferenceNoPropertyName);
            }
        }
        public const string SelectedReceiptPropertyName = "SelectedReceipt";
        private Receipt _selectedReceipt = null;
        public Receipt SelectedReceipt
        {
            get
            {
                return _selectedReceipt;
            }

            set
            {
                if (_selectedReceipt == value)
                {
                    return;
                }
                _selectedReceipt = value;
                RaisePropertyChanged(SelectedReceiptPropertyName);
            }
        }


        public const string DateRequiredPropertyName = "DateRequired";
        private DateTime _dateRequired = DateTime.Now;
        public DateTime DateRequired
        {
            get
            {
                return _dateRequired;
            }

            set
            {
                if (_dateRequired == value)
                {
                    return;
                }

                RaisePropertyChanging(DateRequiredPropertyName);
                _dateRequired = value;
                RaisePropertyChanged(DateRequiredPropertyName);
            }
        }

        public const string OrderDatePropertyName = "OrderDate";
        private DateTime _orderDate = DateTime.Now;
        public DateTime OrderDate
        {
            get
            {
                return _orderDate;
            }

            set
            {
                if (_orderDate == value)
                {
                    return;
                }

                RaisePropertyChanging(OrderDatePropertyName);
                _orderDate = value;
                RaisePropertyChanged(OrderDatePropertyName);
            }
        }

        public const string DistributorSalesmanPropertyName = "SelectedSalesman";
        private DistributorSalesman _selectedSalesman = null;
        public DistributorSalesman SelectedSalesman
        {
            get
            {
                return _selectedSalesman;
            }

            set
            {
                if (_selectedSalesman == value)
                {
                    return;
                }

                RaisePropertyChanging(DistributorSalesmanPropertyName);
                _selectedSalesman = value;
                RaisePropertyChanged(DistributorSalesmanPropertyName);
            }
        }


        
        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _route = null;
        public Route SelectedRoute
        {
            get
            {
                return _route;
            }

            set
            {
                if (_route == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedRoutePropertyName);
                _route = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }


        public const string NotePropertyName = "Note";
        private string _note = null;
       public string Note
        {
            get
            {
                return _note;
            }

            set
            {
                if (_note == value)
                {
                    return;
                }

                RaisePropertyChanging(NotePropertyName);
                _note = value;
                RaisePropertyChanged(NotePropertyName);
            }
        }
       
        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _outlet = null;
        [Required(ErrorMessage="Select Salesman from the dropdown list")]
        public Outlet SelectedOutlet
        {
            get
            {
                return _outlet;
            }

            set
            {
                if (_outlet == value)
                {
                    return;
                }
                
                RaisePropertyChanging(SelectedOutletPropertyName);
                _outlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }
        public const string SelectedShipAddressPropertyName = "SelectedShipAddress";
        private ShipToAddress _shipToAddress = null;
        public ShipToAddress SelectedShipAddress
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

                RaisePropertyChanging(SelectedShipAddressPropertyName);
                _shipToAddress = value;
                RaisePropertyChanged(SelectedShipAddressPropertyName);
            }
        }

        
        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet = 0;
        public decimal TotalNet
        {
            get
            {
                return _totalNet;
            }

            set
            {
                if (_totalNet == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalNetPropertyName);
                _totalNet = value;
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }

        
        public const string TotalProductDoscountPropertyName = "TotalProductDoscount";
        private decimal _totalProductDiscount = 0;
        public decimal TotalProductDoscount
        {
            get
            {
                return _totalProductDiscount;
            }

            set
            {
                if (_totalProductDiscount == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalProductDoscountPropertyName);
                _totalProductDiscount = value;
                RaisePropertyChanged(TotalProductDoscountPropertyName);
            }
        }

        public const string AmountPaidPropertyName = "AmountPaid";
        private decimal _amountPaid = 0m;
        public decimal AmountPaid
        {
            get
            {
                return _amountPaid;
            }

            set
            {
                if (_amountPaid == value)
                {
                    return;
                }
                _amountPaid = value;
                RaisePropertyChanged(AmountPaidPropertyName);
            }
        }
       
        public const string TotalVatPropertyName = "TotalVat";
        private decimal _totalVat = 0;
        public decimal TotalVat
        {
            get
            {
                return _totalVat;
            }

            set
            {
                if (_totalVat == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalVatPropertyName);
                _totalVat = value;
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }


        
        public const string SaleDiscountPropertyName = "SaleDiscount";
        private decimal _saleDiscount = 0;      
        public decimal SaleDiscount
        {
            get
            {
                return _saleDiscount;
            }

            set
            {
                if (_saleDiscount == value)
                {
                    return;
                }

                RaisePropertyChanging(SaleDiscountPropertyName);
                _saleDiscount = value;
                RaisePropertyChanged(SaleDiscountPropertyName);
            }
        }

        
        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _totalGross = 0;
        public decimal TotalGross
        {
            get
            {
                return _totalGross;
            }

            set
            {
                if (_totalGross == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalGrossPropertyName);
                _totalGross = value;
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }


        
        public const string StatusPropertyName = "Status";
        private string _status = "New";
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                RaisePropertyChanging(StatusPropertyName);
                _status = value;
                RaisePropertyChanged(StatusPropertyName);
            }
        }
        public const string ShipToAddressPropertyName = "ShipToAddress";
        private string _shippingAdress = "";
        public string ShipToAddress
        {
            get
            {
                return _shippingAdress;
            }

            set
            {
                if (_shippingAdress == value)
                {
                    return;
                }

                RaisePropertyChanging(ShipToAddressPropertyName);
                _shippingAdress = value;
                RaisePropertyChanged(ShipToAddressPropertyName);
            }
        }

        
        public const string CanChangePropertyName = "CanChange";
        private bool _canchage = true;
        public bool CanChange
        {
            get
            {
                return _canchage;
            }

            set
            {
                if (_canchage == value)
                {
                    return;
                }

                RaisePropertyChanging(CanChangePropertyName);
                _canchage = value;
                RaisePropertyChanged(CanChangePropertyName);
            }
        }

        public const string OutstandingAmountPropertyName = "OutstandingAmount";
        private decimal _outstandingAmount = 0m;
        public decimal OutstandingAmount
        {
            get
            {
                return _outstandingAmount;
            }

            set
            {
                if (_outstandingAmount == value)
                {
                    return;
                }

                _outstandingAmount = value;
                RaisePropertyChanged(OutstandingAmountPropertyName);
            }
        }

        public const string CanApproveOrderPropertyName = "CanApproveOrder";
        private bool _canApproveOrder = false;
        public bool CanApproveOrder
        {
            get
            {
                return _canApproveOrder;
            }

            set
            {
                if (_canApproveOrder == value)
                {
                    return;
                }

                var oldValue = _canApproveOrder;
                _canApproveOrder = value;
                RaisePropertyChanged(CanApproveOrderPropertyName);
            }
        }
        #endregion




   
    }
}