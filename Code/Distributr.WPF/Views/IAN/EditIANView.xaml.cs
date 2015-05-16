using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.IAN
{
    public partial class EditIANView : PageBase
    {
        IANLineItemModal modal;
        EditIANViewModel _vm = null;
        private IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public EditIANView()
        {
            InitializeComponent();
            _vm = this.DataContext as EditIANViewModel;
            LabelControls();
            Loaded += new RoutedEventHandler(EditIANView_Loaded);
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to navigate away from this page?\n Unsaved changes will be lost",
                        "Distributr: Inventory Adjustment", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        void EditIANView_Loaded(object sender, RoutedEventArgs e)
        {
            string navParam = PresentationUtility.GetLastTokenFromUri(NavigationService.CurrentSource);
            if (!string.IsNullOrEmpty(navParam) && navParam.Equals("StockTake"))
            {
                _vm.AdjustInventory = false;
                lbltitle.Content = messageResolver.GetText("sl.inventory.stocktake.title");
            }
            else
            {
                _vm.AdjustInventory = true;
                lbltitle.Content = messageResolver.GetText("sl.inventory.adjust.title");
            }
        }

        void LabelControls()
        {
            Product.Header = messageResolver.GetText("sl.inventory.adjust.grid.col.product");
            Actual.Header = messageResolver.GetText("sl.inventory.adjust.grid.col.actual");
            Expected.Header = messageResolver.GetText("sl.inventory.adjust.grid.col.expected");
            Variance.Header = messageResolver.GetText("sl.inventory.adjust.grid.col.variance");
            Reason.Header = messageResolver.GetText("sl.inventory.adjust.grid.col.reason");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult isConfirmed = MessageBox.Show("Are sure you want to Cancel ",
                                                           "Inventory Adjustment", MessageBoxButton.OKCancel);

            if (isConfirmed == MessageBoxResult.OK)
            {

                _vm.CancelCommand.Execute(null);
                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri("/views/Reports/InventoryAdjustmentsReport.xaml", UriKind.Relative));
            }
        }

        private void AddLineItem_Click(object sender, RoutedEventArgs e)
        {
            modal = new IANLineItemModal();
            IANLineItemViewModel vmItem = modal.DataContext as IANLineItemViewModel;
            vmItem.ClearAndSetup.Execute(null);
            vmItem.RadioEdit = true;
            vmItem.StockTake = !_vm.AdjustInventory;
            if (_vm.AdjustInventory && !_vm.CanAdjustInventory())
            {
                MessageBox.Show("You dont have permission to Adjustment Inventory", "Inventory Adjustment");
                return;
            }
            modal.Closed += new EventHandler(modal_Closed);
            modal.ShowDialog();
        }

        void modal_Closed(object sender, EventArgs e)
        {
            
            IANLineItemViewModel vmItem = modal.DataContext as IANLineItemViewModel;
            bool result = modal.DialogResult.Value;
            if (result)
            {
              
                _vm.AddLineItem(vmItem.SelectedProduct.ProductId,
                    vmItem.SelectedProduct.ProductDesc,
                    vmItem.Expected,
                    vmItem.Actual, vmItem.Reason,vmItem.LineItemType
                    );
                _vm.sAddLineItem(vmItem.SelectedProduct.ProductId,
                  vmItem.SelectedProduct.ProductDesc,
                  vmItem.Expected,
                  vmItem.Actual, vmItem.Reason, vmItem.IsEdit
                  , vmItem.LineItemType);

            }
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            modal = new IANLineItemModal();
            modal.Closed += new EventHandler(modal_Closed);
            Hyperlink hl = sender as Hyperlink;
            IANLineItemViewModel vmItem = modal.DataContext as IANLineItemViewModel;
            vmItem.ClearAndSetup.Execute(null);

            Guid product = (Guid)hl.Tag;
            var lineItem = _vm.LineItems.First(n => n.Id == product && n.IsEditable);
            vmItem.LoadForEdit(
                lineItem.Id,
                lineItem.ActualQty,
                lineItem.ExpectedQty,
                lineItem.Reason, lineItem.Variance,
                lineItem.LineItemType
                );
            modal.ShowDialog();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            Guid product = (Guid)hl.Tag;
            var dialogRes = MessageBox.Show("Are you sure you want to delete item?","Distributr Warning",MessageBoxButton.YesNo);
            if(dialogRes==MessageBoxResult.Yes)
                _vm.RemoveProduct(product);

        }

    }
}
