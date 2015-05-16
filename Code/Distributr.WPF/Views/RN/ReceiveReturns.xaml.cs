using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.RN;
using Distributr.WPF.Views.RN;
using StructureMap;

namespace Distributr.WPF.UI.Views.RN
{
    public partial class ReceiveReturns : Page
    {
        ListReturnsViewModel vm;
        ReturnItemModal modal = null;
        ListReturnsViewModel.ListRNLineItemsViewModel item = null;
        public ReceiveReturns()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ReceiveReturns_Loaded);
        }

        void ReceiveReturns_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                vm = DataContext as ListReturnsViewModel;
                string _returnsId = NavigationService.Source.OriginalString.ParseQueryString("ReturnsId");
                if (!string.IsNullOrEmpty(_returnsId))
                {
                    string ProductId = _returnsId;
                    vm.ReturnNoteId = new Guid(ProductId);
                    vm.LoadReturnCommand.Execute(null);
                }
                LocalizeLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LocalizeLabels()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            labelSalesman.Content = messageResolver.GetText("sl.returns.receive.salesman");
            lblDateRequired.Content = messageResolver.GetText("sl.returns.receive.returndate");
            colreturntype.Header = messageResolver.GetText("sl.returns.receive.grid.col.returntype");
            colitem.Header = messageResolver.GetText("sl.returns.receive.grid.col.item");
            colexpected.Header = messageResolver.GetText("sl.returns.receive.grid.col.expected");
            colactual.Header = messageResolver.GetText("sl.returns.receive.grid.col.actual");
            cmdSave.Content = messageResolver.GetText("sl.returns.receive.btn.save");
            cmdComplete.Content = messageResolver.GetText("sl.returns.receive.btn.confirm");
            cmdCancel.Content = messageResolver.GetText("sl.returns.receive.btn.cancel");
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            item = (ListReturnsViewModel.ListRNLineItemsViewModel)ReturnsDataGrid.SelectedItem;
            modal = new ReturnItemModal();
            ReturnItemViewModel viewModel = modal.DataContext as ReturnItemViewModel;
            List<ListProductSerialItem> serials = vm.SerialItems.Where(n => n.ProductId == item.ProductId).ToList();
            viewModel.LoadData(item, serials);
            modal.Closed += new EventHandler(modal_Closed);
            modal.CenterWindowOnScreen();
            modal.ShowDialog();
        }

        void modal_Closed(object sender, EventArgs e)
        {
            bool result = modal.DialogResult.Value;
            if (result)
            {
                ReturnItemViewModel viewModel = modal.DataContext as ReturnItemViewModel;
                vm.EditActual(viewModel);
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save?", "Salesman Returns", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                vm.RunSaveCommand.Execute(null);
                MessageBox.Show("Return saved successfully");
                NavigationService.Navigate(new Uri("/views/rn/RetunsList.xaml", UriKind.Relative));
            }
        }

        private void cmdComplete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will close the return to further editing \n Proceed?", "Salesman Returns", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                vm.RunConfirmCommand.Execute(null);
              
                NavigationService.Navigate(new Uri("/views/rn/RetunsList.xaml", UriKind.Relative));
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Salesman Returns", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/views/rn/RetunsList.xaml", UriKind.Relative));
            }
        }

    }
}
