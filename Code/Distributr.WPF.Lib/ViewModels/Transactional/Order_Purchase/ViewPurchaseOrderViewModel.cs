using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Purchase
{
    public class ViewPurchaseOrderViewModel : OrderBaseViewModel
    {
        public ViewPurchaseOrderViewModel()
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

                    if (mainorder != null)
                    {
                        OrderId = mainorder.Id;
                        TotalGross = mainorder.TotalGross;
                        OrderReferenceNo = mainorder.DocumentReference;
                        TotalGross = mainorder.TotalGross;
                        TotalNet = mainorder.TotalNet;
                        Note = mainorder.Note;
                        
                        DateRequired = mainorder.EndDate;
                        Status = mainorder.Status.ToString();
                        TotalProductDoscount = mainorder.TotalDiscount;
                        SaleDiscount = mainorder.SaleDiscount;
                        TotalVat = mainorder.TotalVat;
                        if (mainorder.ItemSummary.Any())
                            AddOrderLineItems(mainorder.ItemSummary);
                    }
                }
            }
        }

        private void SetUp()
        {
            ClearViewModel();
            try
            {
                using (var container = NestedContainer)
                {
                    var configService = Using<IConfigService>(container);
                    //CanApproveOrder = configService.ViewModelParameters.CurrentUserRights.CanApproveOrders;
                    CanChange = configService.ViewModelParameters.CurrentUserRights.CanEditOrder;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private void EditOrder()
        {
            throw new NotImplementedException();
        }

        private void ApproveOrder()
        {
            throw new NotImplementedException();
        }



        protected override void Cancel()
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Distributr Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var uri = "/Views/Orders_Purchase/PurchaseOrderListing.xaml";
                NavigateCommand.Execute(uri);
            }
        }

        #region not required overridden methods
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

        protected override void EditProduct(MainOrderLineItem productId)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteEditProduct(MainOrderLineItem obj)
        {
            throw new NotImplementedException();
        }
        
        protected override void GetCurrentDocumentRef()
        {
            throw new NotImplementedException();
        }
        #endregion

        private new void ClearViewModel()
        {
            LineItem.Clear();
          
        }
       #endregion

       #region Properites
       public RelayCommand EditOrderCommand { get; set; }
       public RelayCommand ViewOrderPageLoadedCommand { get; set; }
       public RelayCommand ApproveOrderCommand { get; set; }
       #endregion
    }
}
