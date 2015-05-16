using System;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders.OrderProduct
{
    public class ViewOrderViewModel : OrderBaseViewModel
    {
      
       public ViewOrderViewModel()
       {
           EditOrderCommand = new RelayCommand(EditOrder);
           ApproveOrderCommand = new RelayCommand(ApproveOrder);
           ViewOrderPageLoadedCommand = new RelayCommand(LoadOrder);
           CancelCommand = new RelayCommand(Cancel);
          
           
       }

       

        #region Methods

       private void LoadOrder()
       {
           MainOrder mainorder = null;
            SetUp();

            if (OrderId != Guid.Empty)
            {
                using (var c = NestedContainer)
                {
                    mainorder = Using<IMainOrderRepository>(c).GetById(OrderId);
                    var pricingService = Using<IDiscountProWorkflow>(c);
                    try
                    {


                        if (mainorder != null)
                        {
                            OrderId = mainorder.Id;
                            if (mainorder.DocumentIssuerCostCentre is Distributor)
                            {
                               // SalesmanLookUp.Add(mainorder.DocumentRecipientCostCentre as DistributorSalesman);
                                SelectedSalesman = mainorder.DocumentRecipientCostCentre as DistributorSalesman;
                            }
                            else
                            {
                              //  SalesmanLookUp.Add(mainorder.DocumentIssuerCostCentre as DistributorSalesman);
                                SelectedSalesman = mainorder.DocumentIssuerCostCentre as DistributorSalesman;
                            }
                            TotalGross =mainorder.TotalGross.GetTotalGross();
                            OrderReferenceNo = mainorder.DocumentReference;
                            SelectedOutlet = mainorder.IssuedOnBehalfOf as Outlet;

                            SelectedRoute = SelectedOutlet !=null? SelectedOutlet.Route:null;
                            //TotalGross = pricingService.GetTotalGross(mainorder.TotalGross);
                            TotalGross =(mainorder.ItemSummary.Sum(x => x.TotalGross.GetTruncatedValue())).GetTotalGross();
                            TotalNet = mainorder.TotalNet;
                            Note = mainorder.Note;
                            DateRequired = mainorder.EndDate;
                            Status = mainorder.OrderStatus.ToString();
                            TotalProductDoscount = mainorder.TotalDiscount;
                            SaleDiscount = mainorder.SaleDiscount;
                            TotalVat = mainorder.TotalVat;
                            ShipToAddress = mainorder.ShipToAddress;
                            if (mainorder.ItemSummary.Any())
                                AddOrderLineItems(mainorder.ItemSummary);

                            GetReceipts(mainorder.Id);

                            IsApprovedOrder = !mainorder.IsEditable();
                            ShowViewInvoiceReceipt = IsApprovedOrder;
                            GetOrderPaymemtInfo(mainorder);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fatal Error,cannot load order details..see error logs for details");
                        
                    }
                }
            }
       }
       
      
        private void ApproveOrder()
        {
            var uri = "/views/orders/OrderApproval.xaml";
            Messenger.Default.Send(OrderId);
            NavigateCommand.Execute(uri);
        }

        private void EditOrder()
        {
            throw new NotImplementedException();
        }

        private void SetUp()
       {
           ClearViewModel();
         try
           {
               using (var container = NestedContainer)
               {
                   var configService = Using<IConfigService>(container);
                   CanApproveOrder = configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
                   CanChange = configService.ViewModelParameters.CurrentUserRights.CanEditOrder;

                   SelectedReceipt = new Receipt(Guid.Empty) { DocumentReference = "--Select Receipt--" };
               }
           }catch(Exception e)
           {
               throw e;
           }

       }
        protected override void Cancel()
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Distributr Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var uri = "/views/orders/SalesManOrdersListing.xaml";
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
            throw new NotImplementedException();
        }

        protected override void EditProduct(MainOrderLineItem product)
        {
            
        }

        protected override void DeleteEditProduct(MainOrderLineItem obj)
        {
            throw new NotImplementedException();
        }


        protected override void GetCurrentDocumentRef()
        {
            throw new NotImplementedException();
        }
       private void ClearViewModel()
       {
           LineItem.Clear();
           InvoiceReceipts.Clear();
           Note = string.Empty;
           PaymentInfoList.Clear();
           AmountPaid = 0m;
           OutstandingAmount = 0m;

       }
       #endregion

       #region Properites

       public RelayCommand EditOrderCommand { get; set; }
       public RelayCommand ViewOrderPageLoadedCommand { get; set; }
       public RelayCommand ApproveOrderCommand { get; set; }

       public const string IsApprovedOrderPropertyName = "IsApprovedOrder";
       private bool _isApprovedOrder;

       public bool IsApprovedOrder
       {
           get { return _isApprovedOrder; }

           set
           {
               if (_isApprovedOrder == value)
               {
                   return;
               }
               _isApprovedOrder = value;
               RaisePropertyChanged(IsApprovedOrderPropertyName);
           }
       }
       public const string ShowViewInvoiceReceiptPropertyName = "ShowViewInvoiceReceipt";
       private bool _showViewInvoiceReceipt;

       public bool ShowViewInvoiceReceipt
       {
           get { return _showViewInvoiceReceipt; }

           set
           {
               if (_showViewInvoiceReceipt == value)
               {
                   return;
               }
               _showViewInvoiceReceipt = value;
               RaisePropertyChanged(ShowViewInvoiceReceiptPropertyName);
           }
       }



        #endregion
    }
}
