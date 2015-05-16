using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.ViewModels.Reports
{
    
    public class PaymentReportsViewModel : DistributrViewModelBase
    {
        bool _paymentNotificationCompleted = true;
        private List<PaymentExceptionReportItem> PaymentExceptionReportItemsCached;
        private List<PaymentExceptionReportItem> PaymentExceptionReportItems;
        //BusyWindow isBusyWindow = null;
        public PaymentReportsViewModel()
        {

            PaymentExceptionReportItemsCached = new List<PaymentExceptionReportItem>();
            PaymentExceptionReportItems = new List<PaymentExceptionReportItem>();
            ListPaymentExceptionReportItems = new ObservableCollection<PaymentExceptionReportItem>();
            //Salesmen = new ObservableCollection<ListSalesmanOrdersViewModel.Salesman>();
            Outlets = new ObservableCollection<Outlet>();
            //isBusyWindow = new BusyWindow();
            //isBusyWindow.lblWhatsUp.Content = "Fetching data from server.";
        }

        public ObservableCollection<PaymentExceptionReportItem> ListPaymentExceptionReportItems { get; set; }
      //  public ObservableCollection<ListSalesmanOrdersViewModel.Salesman> Salesmen { get; set; }
        public ObservableCollection<Outlet> Outlets { get; set; }

        public const string StartDatePropertyName = "StartDate";
        private DateTime _dateTime = DateTime.Now.AddMonths(-1);
        public DateTime StartDate
        {
            get
            {
                return _dateTime;
            }

            set
            {
                if (_dateTime == value)
                {
                    return;
                }

                var oldValue = _dateTime;
                _dateTime = value;
                RaisePropertyChanged(StartDatePropertyName);
            }
        }
         
        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                var oldValue = _endDate;
                _endDate = value;
                RaisePropertyChanged(EndDatePropertyName);
            }
        }
         
        public const string PageCountPropertyName = "PageCount";
        private int _pageCount = 1;
        public int PageCount
        {
            get
            {
                return _pageCount;
            }

            set
            {
                if (_pageCount == value)
                {
                    return;
                }

                var oldValue = _pageCount;
                _pageCount = value;
                RaisePropertyChanged(PageCountPropertyName);
            }
        }

        public const string ItemsPerPagePropertyName = "ItemsPerPage";
        private int _itemsPerPage = 10;
        public int ItemsPerPage
        {
            get
            {
                return _itemsPerPage;
            }

            set
            {
                if (_itemsPerPage == value)
                {
                    return;
                }

                var oldValue = _itemsPerPage;
                _itemsPerPage = value;
                RaisePropertyChanged(ItemsPerPagePropertyName);
            }
        }
         
        public const string CurrentPagePropertyName = "CurrentPage";
        private int _currentPage = 1;
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                var oldValue = _currentPage;
                _currentPage = value;
                RaisePropertyChanged(CurrentPagePropertyName);
            }
        }
         
        public const string ItemsCountPropertyName = "ItemsCount";
        private int _itemsCount = 0;
        public int ItemsCount
        {
            get
            {
                return _itemsCount;
            }

            set
            {
                if (_itemsCount == value)
                {
                    return;
                }

                var oldValue = _itemsCount;
                _itemsCount = value;
                RaisePropertyChanged(ItemsCountPropertyName);
            }
        }
         
        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                var oldValue = _searchText;
                _searchText = value;
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }
         
        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private ListSalesmanOrdersViewModel.Salesman _selectedSalesman = null;
        public ListSalesmanOrdersViewModel.Salesman SelectedSalesman
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

                var oldValue = _selectedSalesman;
                _selectedSalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }
         
        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private Outlet _selectedOutlet = null;
        public Outlet SelectedOutlet
        {
            get
            {
                return _selectedOutlet;
            }

            set
            {
                if (_selectedOutlet == value)
                {
                    return;
                }

                var oldValue = _selectedOutlet;
                _selectedOutlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }
         
        public const string LoadingPropertyName = "Loading";
        private bool _loading = true;
        public bool Loading
        {
            get
            {
                return _loading;
            }

            set
            {
                if (_loading == value)
                {
                    return;
                }

                var oldValue = _loading;
                _loading = value;
                RaisePropertyChanged(LoadingPropertyName);
            }
        }

        public void SetUp()
        {
            LoadSalesmen();
            LoadOutlets();
        }

        void ClearViewModel()
        {

        }

        void LoadSalesmen()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Salesmen.Clear();
                var salesman = new ListSalesmanOrdersViewModel.Salesman
                    {
                        Id = Guid.Empty,
                        Username = "--Select a Salesman--"
                    };
                Salesmen.Add(salesman);
                SelectedSalesman = salesman;
                Using<IUserRepository>(c).GetAll().Where(n => n.UserType == UserType.DistributorSalesman).ToList().ForEach(
                    n => Salesmen.Add(new ListSalesmanOrdersViewModel.Salesman {Id = n.Id, Username = n.Username}));
            }
        }

        void LoadOutlets()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Outlets.Clear();
                var outlets = Using<ICostCentreRepository>(c).GetAll().OfType<Outlet>().Where(n => n._Status == EntityStatus.Active);
                outlets = outlets.OrderBy(n => n.Name).ToList();
                var outlet = new Outlet(Guid.Empty) {CostCentreCode = " ", Name = "--Select Outlet--"};
                SelectedOutlet = outlet;
                Outlets.Add(outlet);
                outlets.ToList().ForEach(n => Outlets.Add(n));
            }
        }

        public void Load()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                //if (isBusyWindow != null)
                //    isBusyWindow.Show();
                Loading = true;
                _paymentNotificationCompleted = false;
                string url = Using<IPaymentService>(c).GetPGWSUrl(ClientRequestResponseType.ExceptionReport);
                Uri uri = new Uri(url, UriKind.Absolute);
                WebClient wc = new WebClient();
                var obj = new PaymentReportRequestObj
                    {
                        ServiceProviderId = Using<IConfigService>(c).Load().CostCentreId, //Guid.Empty,
                        StartDate = StartDate,
                        EndDate = EndDate
                    };

                string json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
                wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadPaymentInstReqCompleted);
                wc.UploadStringAsync(uri, "POST", json);
            }
        }

        private void wc_UploadPaymentInstReqCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try{
                if (e.Error == null)
                {
                    if (_paymentNotificationCompleted)
                    {
                        //if (isBusyWindow != null)
                        //    isBusyWindow.OKButton_Click(this, null);
                        return;
                    }

                    string jsonResult = e.Result;

                    var pnrs = new List<PaymentNotificationResponse>();

                    JArray ja = JArray.Parse(jsonResult);
                    if (ja.Count == 0)
                    {
                        MessageBox.Show("There is no data to report for the selected period.");
                        _paymentNotificationCompleted = true;

                        //if (isBusyWindow != null)
                        //    isBusyWindow.OKButton_Click(this, null);
                        return;
                    }
                    var jo = ja.FirstOrDefault();

                    if ((int)(jo["ClientRequestResponseType"]) == 3)
                    {
                        MessageSerializer.CanDeserializeMessage(jsonResult, out pnrs);
                        if ((pnrs != null && pnrs.Count == 0))
                        {
                            return;
                        }
                        else
                        {
                            Map(pnrs);
                            Filter();
                        }
                    }

                    else
                    {
                        ReportPaymentNotificationError();
                        return;
                    }

                    _paymentNotificationCompleted = true;

                    //if (isBusyWindow != null)
                    //    isBusyWindow.OKButton_Click(this, null);
                }
                else
                {
                    ReportPaymentNotificationError();
                }
            }
            catch
            {
                ReportPaymentNotificationError();
            }
            Loading = false;
        }

        private void Map(List<PaymentNotificationResponse> notifications)
        {

            using (StructureMap.IContainer c = NestedContainer)
            {
                var items = new List<PaymentExceptionReportItem>();

                int i = 1;
                foreach (var item in notifications)
                {
                    try
                    {
                        var receipt = Using<IReceiptRepository>(c).GetByLineItemId(new Guid(item.TransactionRefId));
                        if (receipt == null)
                            continue;
                        Invoice invoice = Using<IInvoiceRepository>(c).GetById(receipt.InvoiceId) as Invoice ;
                        var order = Using<IOrderRepository>(c).GetById(invoice.OrderId) as Order;
                        var salesman = order.DocumentIssuerUser;
                        var receiptLineItem =
                            receipt.LineItems.FirstOrDefault(
                                n => n.Id.ToString().ToLower() == item.TransactionRefId.ToLower());
                        var childLineItems = Using<IReceiptRepository>(c).GetChildLineItemsByLineItemId(receiptLineItem.Id);

                        var exItem = new PaymentExceptionReportItem
                            {
                                SequenceId = i,
                                OrderId = order.Id,
                                OrderRef = order.DocumentReference,
                                InvoiceId = invoice.Id,
                                InvoiceRef = invoice.DocumentReference,
                                ReceiptId = receipt.Id,
                                ReceiptRef = receipt.DocumentReference,
                                ReceiptLineItemId = new Guid(item.TransactionRefId),
                                SalesmanId = salesman.Id,
                                Salesman = salesman.Username,
                                OutletId = order.IssuedOnBehalfOf.Id,
                                OuteltName = order.IssuedOnBehalfOf.Name,
                                StatusCode = item.StatusCode,
                                StatusDetail = item.StatusDetail,
                                DateCreated = receipt.DocumentDateIssued,
                                TimeOutDate = item.TimeStamp,
                                LineItemValue = receiptLineItem.Value,
                                ConfirmedAmount = CalcConfirmedAmount(childLineItems),
                                UnConfirmedAmount = Convert.ToDecimal(item.BalanceDue)
                                //CalcUnConfirmedAmount(receiptLineItem, childLineItems),
                            };
                        items.Add(exItem);
                        i++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading info for notification Id " + item.Id + ".\nException Details\n" +
                                        ex.ToString());
                    }
                }
                items.ForEach(PaymentExceptionReportItemsCached.Add);
                ItemsCount = items.Count();
                PageCount = (int) Math.Ceiling(Math.Round(((double) ItemsCount)/ItemsPerPage, 1));
            }
        }

        public void Filter()
        {
            Loading = true;
            if (SearchText.Trim() == "")
            {
                PaymentExceptionReportItems = PaymentExceptionReportItemsCached;
            }
            else
            {
                var searchText = SearchText.ToLower();
                PaymentExceptionReportItems = PaymentExceptionReportItemsCached
                    .Where(n => n.Salesman.ToLower().Contains(searchText)
                    || n.OrderRef.ToLower().Contains(searchText)
                    || n.ReceiptRef.ToLower().Contains(searchText)
                    || n.InvoiceRef.ToLower().Contains(searchText)
                    )
                    .ToList();
            }

            if (SelectedOutlet.Id != Guid.Empty && SelectedSalesman.Id != Guid.Empty)
            {
                PaymentExceptionReportItems = PaymentExceptionReportItems
                    .Where(n => n.OutletId == SelectedOutlet.Id && n.SalesmanId == SelectedSalesman.Id).ToList();
            }
            else if(SelectedOutlet.Id != Guid.Empty && SelectedSalesman.Id == Guid.Empty)
            {
                PaymentExceptionReportItems = PaymentExceptionReportItems
                    .Where(n => n.OutletId == SelectedOutlet.Id).ToList();
            }
            else if (SelectedOutlet.Id == Guid.Empty && SelectedSalesman.Id != Guid.Empty)
            {
                PaymentExceptionReportItems = PaymentExceptionReportItems
                    .Where(n => n.SalesmanId == SelectedSalesman.Id).ToList();
            }

            var items = PaymentExceptionReportItems/*.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage)*/.ToList();
            DumpAndDeploy(items);
            ItemsCount = PaymentExceptionReportItems.Count();
            PageCount = (int)Math.Ceiling(Math.Round(((double)ItemsCount) / ItemsPerPage, 1));
            Loading = false;
        }

        public void SkipAndTake()
        {
            var items = PaymentExceptionReportItems.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            DumpAndDeploy(items);
        }

        void DumpAndDeploy(List<PaymentExceptionReportItem> items)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(new Action(
                                                          () =>
                                                          {
                                                              ListPaymentExceptionReportItems.Clear();
                                                              items.ForEach(ListPaymentExceptionReportItems.Add);
                                                          }
                                                          ));
            
        }

        decimal CalcConfirmedAmount(List<ReceiptLineItem> lineItems)
        {
            var amt = 0m;
            amt = lineItems.Sum(n => n.Value);

            return amt;
        }

        decimal CalcUnConfirmedAmount(ReceiptLineItem lineItem, List<ReceiptLineItem> chilLineItems)
        {
            var confirmed = chilLineItems.Sum(n => n.Value);
            decimal unconfirmed = lineItem.Value - confirmed;
            if(unconfirmed < 0)
                unconfirmed = 0;

            return unconfirmed;
        }

        void ReportPaymentNotificationError()
        {
            if (!_paymentNotificationCompleted)
            {
                MessageBox.Show("Unable to retrieve data from server. Check connetion to the payment gateway.");
                _paymentNotificationCompleted = true;
            }
            //if (isBusyWindow != null)
            //    isBusyWindow.OKButton_Click(this, null);
        }

        public override void Cleanup()
        {

            // Clean own resources if needed
            CurrentPage = 1;
            ItemsCount = 0;
            PageCount = 1;
            PaymentExceptionReportItemsCached.Clear();
            PaymentExceptionReportItems.Clear();
            //Salesmen.Clear();
            //Outlets.Clear();
            base.Cleanup();
        }

        public class PaymentExceptionReportItem
        {
            public int SequenceId { get; set; }
            public Guid OrderId { get; set; }
            public string OrderRef { get; set; }
            public Guid InvoiceId { get; set; }
            public string InvoiceRef { get; set; }
            public Guid ReceiptId { get; set; }
            public string ReceiptRef { get; set; }
            public Guid ReceiptLineItemId { get; set; }
            public decimal LineItemValue { get; set; }
            public decimal ConfirmedAmount { get; set; }
            public decimal UnConfirmedAmount { get; set; }
            public string StatusCode { get; set; }
            public string StatusDetail { get; set; }
            public Guid SalesmanId { get; set; }
            public string Salesman { get; set; }
            public DateTime DateCreated { get; set; }
            public DateTime TimeOutDate { get; set; }
            public Guid OutletId { get; set; }
            public string OuteltName { get; set; }
        }

        public class PaymentReportRequestObj
        {
            public Guid ServiceProviderId { get; set; }//Distributr Id
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}