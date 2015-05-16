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
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.ITN
{
    public partial class EditITNView : PageBase
    {
        ITNLineItemModal _modal;
        EditITNViewModel _vm;
        bool isInititalized = false;

        public EditITNView()
        {
            isInititalized = false;
            InitializeComponent();
            isInititalized = true;
            Loaded += new RoutedEventHandler(EditITNView_Loaded);
            LabelControls();
        }

        void EditITNView_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as EditITNViewModel;
            cmbSalesmen.IsEnabled = true;
            cmdSave.Visibility = Visibility.Collapsed;
            _vm.ClearViewModel();
            _vm.LoadSalesMenCommand.Execute(null);
        }

        void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            lblSalesman.Content = messageResolver.GetText("sl.issueInventory.salesman_lbl");
            labelpagetitle.Content = messageResolver.GetText("sl.issueInventory.titlepage"); ;
            lblSalesman.Content = messageResolver.GetText("sl.issueInventory.salesman"); ;
            colproduct.Header = messageResolver.GetText("sl.issueInventory.edit.grid.col.product");
            colissuequantity.Header = messageResolver.GetText("sl.issueInventory.edit.grid.col.issuequantity");
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
                if (
                    MessageBox.Show(
                        "Are you sure you want to move away from this page without completing the inventory transfer?"
                        + Environment.NewLine +
                        "Unsaved changes will be lost",
                        "Distributr: Confirm Navigating Away", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                    e.Cancel = true;
            base.OnNavigatingFrom(sender, e);

        }

        private void cmdAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbSalesmen.SelectedItem != null)
                {
                    if (cmbSalesmen.SelectedIndex != 0)
                    {
                        _modal = new ITNLineItemModal();
                        _modal.Closed += new EventHandler(modal_Closed);
                        _modal.Title = "Add Product to Issue";
                        //_modal.cmbProduct.IsEnabled = true;
                        ITNLineItemViewModel vmItem = _modal.DataContext as ITNLineItemViewModel;
                        vmItem.ClearAndSetup.Execute(null);
                        vmItem.SalesmanId = _vm.SelectedSaleMan.CostCentre;
                        _modal.ShowDialog();
                    }
                    else
                        MessageBox.Show("Please Select a valid sales man");
                }
                else
                    MessageBox.Show("Please select a sales man");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void modal_Closed(object sender, EventArgs e)
        {
            ITNLineItemViewModel vmItem = _modal.DataContext as ITNLineItemViewModel;
            bool result = _modal.DialogResult.Value;
            if (result)
            {
               
                _vm.Reload(vmItem.GetProducts(), vmItem.IsEdit, vmItem.ProductSerialNumbersList.ToList());
                TranfersGridView.ItemsSource = _vm.LineItems;
                cmbSalesmen.IsEnabled = _vm.LineItems.Count <= 0;
            }
            else
                _vm.IsEdit = false;
        }

        private void cmbSalesmen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInititalized)
                return;
            
            _vm.ClearBuffer();
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            _modal = new ITNLineItemModal();
            _modal.Closed += new EventHandler(modal_Closed);
            _modal.Title = "Edit Product";
          //  _modal.cmbProduct.IsEnabled = false;
            Hyperlink hl = sender as Hyperlink;
            ITNLineItemViewModel vmItem = _modal.DataContext as ITNLineItemViewModel;
            Guid product = (Guid)hl.Tag;
            var lineItem = _vm.LineItems.First(n => n.ProductId == product);
            vmItem.ClearAndSetup.Execute(null);
            vmItem.LoadForEdit(lineItem.ProductId, lineItem.Qty, _vm.GetProductSerials(lineItem.ProductId));
            _vm.IsEdit = true;
            _modal.ShowDialog();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _vm.RemoveLineItem(Guid.Parse(((Hyperlink)sender).Tag.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbSalesmen_DropDownOpened(object sender, EventArgs e)
        {
            cmbSalesmen.IsDropDownOpen = false;
           ComboPopUp popup = new ComboPopUp();
           _vm.SelectedSaleMan = (User)popup.ShowDlg(sender);
        }
    }
}
