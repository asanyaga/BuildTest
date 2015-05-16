using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
   public class ViewOrderPOSViewModel : OrderBaseViewModel
    {
       public ViewOrderPOSViewModel()
       {
           AddProductCommand = new RelayCommand(AddProduct);
           NewSaleCommand =new RelayCommand(DoASale);
           ViewPOSPageLoadedCommand = new RelayCommand(SetUp);

       }

       private void DoASale()
       {
           const string uri = "/views/Order_Pos/AddPOS.xaml";
           NavigateCommand.Execute(uri);
       }
       private void ClearViewModel()
       {
           TotalGross = 0;
           OrderReferenceNo = "";
           SelectedOutlet = null;
           SelectedRoute = null;
           TotalGross = 0m;
           TotalNet = 0m;
           ReturnableValue = 0m;
           AmountPaid = 0m;
           Status = "";
           Note = string.Empty;
           TotalProductDoscount = 0m;
           SaleDiscount = 0m;
           PaymentInfoList.Clear();
           LineItem.Clear();
           SelectedSalesman = null;

       }
       
       private void SetUp()
       {
           ClearViewModel();
           MainOrder mainorder = null;
          if (OrderId != Guid.Empty)
           {
               using (var c = NestedContainer)
               {
                   mainorder = Using<IMainOrderRepository>(c).GetById(OrderId);
                   var pricingService = Using<IDiscountProWorkflow>(c);

                   if (mainorder != null)
                   {
                       OrderId = mainorder.Id;
                       if (mainorder.DocumentIssuerCostCentre is Distributor)
                       {
                        SelectedSalesman = mainorder.DocumentRecipientCostCentre as DistributorSalesman;
                       }
                       else
                       {
                         SelectedSalesman = mainorder.DocumentIssuerCostCentre as DistributorSalesman;
                       }
                      // TotalGross = pricingService.GetTotalGross(mainorder.TotalGross);
                       TotalGross = (mainorder.ItemSummary.Sum(x =>(x.Qty * x.Value).GetTruncatedValue()+(x.Qty*x.VatValue).GetTruncatedValue())).GetTotalGross();
                       OrderReferenceNo = mainorder.DocumentReference;
                       SelectedOutlet = mainorder.IssuedOnBehalfOf as Outlet;
                       SelectedRoute = SelectedOutlet.Route;
                       SaleValue = TotalGross;
                       ReturnableValue = 0m;
                       DateRequired = mainorder.EndDate;
                       Status = mainorder.Status.ToString();
                       TotalProductDoscount = mainorder.TotalDiscount;
                       SaleDiscount = mainorder.SaleDiscount;
                       Note = mainorder.Note;
                       ShipToAddress = mainorder.ShipToAddress;
                       TotalVat = mainorder.TotalVat;
                       //TotalNet = pricingService.GetTotalGross(mainorder.TotalGross-mainorder.SaleDiscount);
                       TotalNet = (TotalGross - mainorder.SaleDiscount).GetTotalGross();
                       if (mainorder.ItemSummary.Any())
                           AddOrderLineItems(mainorder.ItemSummary);
                       GetReceipts(mainorder.Id);
                       GetOrderPaymemtInfo(mainorder);
                   }
                   
               }
           }
          
       }
       
      
       public RelayCommand NewSaleCommand { get; set; }
       public RelayCommand ViewPOSPageLoadedCommand { get; set; }

      
       protected override void Cancel()
        {
           string url = "/views/Order_Pos/ListPOSSales.xaml";
            NavigateCommand.Execute(url);
            
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
            const string uri = "/views/Order_Pos/AddPOS.xaml";
            NavigateCommand.Execute(uri);
        }

        protected override void EditProduct(MainOrderLineItem selectedItem)
        {
            /* GO: TODO: Do we want to edit a sale on view page?
            const string uri = "/views/Order_Pos/AddPOS.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = selectedItem.ProductId });
            NavigateCommand.Execute(uri);
             * */
        }

        protected override void DeleteEditProduct(MainOrderLineItem obj)
        {
            throw new NotImplementedException();
        }

        protected override void GetCurrentDocumentRef()
        {
            throw new NotImplementedException();
        }
      

       #region properties
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

                var oldValue = _selectedReceipt;
                _selectedReceipt = value;
                RaisePropertyChanged(SelectedReceiptPropertyName);
            }
        }

        public const string ReturnableValuePropertyName = "ReturnableValue";
        private decimal _returnableValue = 0;

        public decimal ReturnableValue
        {
            get { return _returnableValue; }

            set
            {
                if (_returnableValue == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalNetPropertyName);
                _returnableValue = value;
                RaisePropertyChanged(ReturnableValuePropertyName);
            }
        }

        public const string IsConfirmedPropertyName = "IsConfirmed";
        private bool _isConfirmed;

        public bool IsConfirmed
        {
            get { return _isConfirmed; }

            set
            {
                if (_isConfirmed == value)
                {
                    return;
                }

                var oldValue = _isConfirmed;
                _isConfirmed = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IsConfirmedPropertyName);
            }
        }

        public const string SaleValuePropertyName = "SaleValue";
        private decimal _saleValue;

        public decimal SaleValue
        {
            get { return _saleValue; }

            set
            {
                if (_saleValue == value)
                {
                    return;
                }

                var oldValue = _saleValue;
                _saleValue = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SaleValuePropertyName);
            }
        }
        #endregion

       
    }
}
