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
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.Purchasing
{
    public partial class EditPurchaseOrder : PageBase
    {
        private EditPurchaseOrderViewModel _vm;
        POLineItemModal _newLineItemModal = null;

        public EditPurchaseOrder()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(EditPurchaseOrder_Loaded);
        }

        void EditPurchaseOrder_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as EditPurchaseOrderViewModel;
            dtDateRequired.DisplayDateStart = DateTime.Now;
            LabelControls();
            Load();
        }

        private void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            Gridcolproductname.Header = messageResolver.GetText("sl.po.form.grid.col.productname");
            Gridcolquantity.Header = messageResolver.GetText("sl.po.form.grid.col.quantity");
            Gridcolunitprice.Header = messageResolver.GetText("sl.po.form.grid.col.unitprice");
            Gridcolvat.Header = messageResolver.GetText("sl.po.form.grid.col.vat");
            Gridcoltotalprice.Header = messageResolver.GetText("sl.po.form.grid.col.totalprice");
            textBlockOrderId.Content = messageResolver.GetText("sl.po.form.orderId");
            LabelOrderDate.Content = messageResolver.GetText("sl.po.form.orderdate");
            textBlockStatus.Content = messageResolver.GetText("sl.po.form.status");
            textBlockTotalNet.Content = messageResolver.GetText("sl.po.form.totalnet");
            textBlockTotalVat.Content = messageResolver.GetText("sl.po.form.totalvat");
            textBlockTotalGross.Content = messageResolver.GetText("sl.po.form.totalgross");

        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ButtonCancelName == "Cancel")
            {
                if (_vm.ConfirmNavigatingAway)
                    if (MessageBox.Show(
                        "Are you sure you want to move away from this page without completing the Purchase order?" +
                        "Unsaved changes will be lost",
                        "Distributr: Confirm Navigating Away", MessageBoxButton.YesNo) ==
                        MessageBoxResult.No)
                        e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        private void Load()
        {
            _vm = DataContext as EditPurchaseOrderViewModel;
            _vm.RunClearAndSetup();
            dtDateRequired.IsEnabled = true;
            DatePickerOrderDate.IsEnabled = false;
            string _orderId = NavigationService.Source.OriginalString.ParseQueryString("orderid");
            if (!string.IsNullOrEmpty(_orderId))
            {
                string orderId = _orderId;
                _vm.OrderIdLookup = new Guid(orderId);
                dtDateRequired.IsEnabled = false;
                DatePickerOrderDate.IsEnabled = false;
            }
            _vm.Load.Execute(null);

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _newLineItemModal = new POLineItemModal();
            _newLineItemModal.Closed += new EventHandler(modal_Closed);
            _newLineItemModal.cmbProducts.IsEnabled = true;
            _newLineItemModal.btnAddProduct.IsEnabled = true;
            POLineItemViewModel vmLineItem = _newLineItemModal.DataContext as POLineItemViewModel;
            vmLineItem.ModalTitle = "Add Product to Purchase Order: Ref No. " + _vm.OrderId;
            vmLineItem.ClearAndSetup.Execute(null);
            vmLineItem.IsNew = true;
            vmLineItem.LineItemType = LineItemType.Unit;
            vmLineItem.IsAdd = true;
            vmLineItem.IsEnabled = true;

            _newLineItemModal.ShowDialog();
        }

        void modal_Closed(object sender, EventArgs e)
        {
            POLineItemViewModel vmItem = _newLineItemModal.DataContext as POLineItemViewModel;
            bool result = _newLineItemModal.DialogResult.Value;
            if (result)
            {
                if (vmItem.IsNew)
                {
                    _vm.UpdateOrAddLineItemFromPoductSummary(vmItem.ProductAddSummaries, vmItem.IsAdd);
                    vmItem.MultipleProduct.Clear();
                    vmItem.ProductAddSummaries.Clear();
                }
                else
                {
                    //can only update qry???
                    _vm.UpdateLineItem(vmItem.SequenceNo, vmItem.Qty);
                }
            }
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            string[] tag = hl.Tag.ToString().Split(',');
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString());
            _newLineItemModal = new POLineItemModal();
            _newLineItemModal.Closed += new EventHandler(modal_Closed);
            _newLineItemModal.cmbProducts.IsEnabled = false;
            _newLineItemModal.btnAddProduct.IsEnabled = false;

            LineItemType lit = LineItemType.Unit;///int.Parse(tag[1].ToString());
            var lineItemList = _vm.LineItems.Where(n => n.ProductId == ParentProductid);
            var lineItem = lineItemList.First(p => p.ProductId == ParentProductid);
            POLineItemViewModel vmLineItem = _newLineItemModal.DataContext as POLineItemViewModel;
            vmLineItem.ModalTitle = "Edit Product of Purchase Order: Ref No. " + _vm.OrderId;
            vmLineItem.ClearAndSetup.Execute(null);

            vmLineItem.LoadForEdit(lineItem.ProductId, lineItem.UnitPrice,
                lineItem.LineItemVatValue, lineItem.TotalPrice,
                lineItem.VatAmount, lineItem.SequenceNo, lineItemList.Min(m => m.Qty));
            vmLineItem.LineItemType = lit;

            _newLineItemModal.ShowDialog();

        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {

            Hyperlink hl = sender as Hyperlink;
            string[] tag = hl.Tag.ToString().Split(',');
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString()); //int.Parse(tag[0].ToString());
            LineItemType lit = LineItemType.Unit; //(LineItemType) int.Parse(tag[1].ToString());
            _vm.RemoveLineItem(ParentProductid, lit);

        }

        private void dtDateRequired_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
