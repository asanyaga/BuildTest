using System;
using System.Collections.Generic;
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
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.DispatchProducts
{
    public partial class DispatchProducts : PageBase
    {
        private DispatchProductsViewModel _vm;
        private DispatchProductsLineItemModal lineItemModal;
        bool _confirmExist = true;
        public DispatchProducts()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DispatchProducts_Loaded);
        }

        void DispatchProducts_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DispatchProductsViewModel;
            _vm.ClearAndSetUp();
            Localizelabels();
        }

        private void Localizelabels()
        {
            IMessageSourceAccessor messageResolver =ObjectFactory.GetInstance<IMessageSourceAccessor>();
            lbltitile.Content = messageResolver.GetText("sl.inventory.dispatch.title");
            colProduct.Header = messageResolver.GetText("sl.inventory.dispatch.grid.col.product");
            colReason.Header = messageResolver.GetText("sl.inventory.dispatch.grid.col.reason");
            colQty.Header = messageResolver.GetText("sl.inventory.dispatch.grid.col.quantity");
            colProductType.Header = messageResolver.GetText("sl.inventory.dispatch.grid.col.producttype");
            colOtherReason.Header = messageResolver.GetText("sl.inventory.dispatch.grid.col.otherreason");

        }

        private int seqId = 0;
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            lineItemModal = new DispatchProductsLineItemModal();
            lineItemModal.Closing += lineItemModal_Closing;
            Hyperlink hl = sender as Hyperlink;
            DPLineItemViewModel vmItem = lineItemModal.DataContext as DPLineItemViewModel;
            seqId = (int)hl.Tag;

            var lineItem = _vm.LineItems.First(n => n.SequenceId == seqId);

            vmItem.ClearAndSetUp();
            vmItem.LoadForEdit(
                lineItem.ProductId,
                lineItem.Reason,
                lineItem.OtherReason,
                lineItem.Qty
                );
            lineItemModal.ShowDialog();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            seqId = (int)hl.Tag;
            _vm.RemoveProduct(seqId);

            dgLineItems.ItemsSource = null;
            dgLineItems.ItemsSource = _vm.LineItems;
        }

        private void btnAddLineItem_Click(object sender, RoutedEventArgs e)
        {
            lineItemModal = new DispatchProductsLineItemModal();
            lineItemModal.Closing += lineItemModal_Closing;
            DPLineItemViewModel vm = lineItemModal.DataContext as DPLineItemViewModel;
            vm.ClearAndSetUp();
            lineItemModal.ShowDialog();
        }

        void lineItemModal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DPLineItemViewModel vmItem = lineItemModal.DataContext as DPLineItemViewModel;
            bool result = lineItemModal.DialogResult.Value;
            if (result)
            {
                _vm.AddorUpdateLineItem(vmItem.SelectedProduct.Id,
                  vmItem.SelectedProduct.Description,
                  vmItem.SelectedReason.Reason,
                  vmItem.OtherReason,
                  vmItem.Qty,
                  vmItem.IsEdit,
                  seqId
                  );
            }
            seqId = 0;
            dgLineItems.ItemsSource = null;
            dgLineItems.ItemsSource = _vm.LineItems;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _confirmExist = false;
                if(!_vm.LineItems.Any())
                {
                    MessageBox.Show("No product to confirm");
                    return;
                }
                _vm.Confirm();
                _vm.Cleanup();
                MessageBox.Show("Products dispatched successfully.",
                                "Distributr: Dispatch Products", MessageBoxButton.OK);
                NavigationService.Navigate(new Uri("/views/Reports/InventoryListing.xaml", UriKind.Relative));
            }
            catch
            {
                MessageBox.Show("An error occurred while confirming");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Reports/InventoryListing.xaml", UriKind.Relative));
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (!_confirmExist) return;
            if (
                MessageBox.Show("Are you sure you want to move away from this page without completing the dispatch products proccess?" +
                                "Current work will be lost",
                                "Distributr: Confirm Navigating Away", MessageBoxButton.OKCancel) ==
                MessageBoxResult.OK)
            {
                _vm.Cleanup();
            }
            else
                e.Cancel = true;
            base.OnNavigatingFrom(sender, e);
        }
    }
}
