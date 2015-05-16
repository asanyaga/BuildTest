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
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
namespace Distributr.WPF.UI.Views.CN
{
    public partial class AddCreditNote : Page
    {
        private AddCreditNoteModel vm;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public AddCreditNote()
        {
            InitializeComponent();
            Loaded += AddCreditNote_Loaded;
            LabelControls();
        }

        void AddCreditNote_Loaded(object sender, RoutedEventArgs e)
        {
            vm = this.DataContext as AddCreditNoteModel;
            Guid InvoiceNo = new Guid(PresentationUtility.ParseQueryString(NavigationService.CurrentSource, "InvoiceNo"));
            vm.Setup(InvoiceNo);
        }

        void LabelControls()
        {
            lblInvoiceRef.Content = _messageResolver.GetText("sl.creditnote.addcn.invoiceref");
            lblInvoiceNo.Content = _messageResolver.GetText("sl.creditnote.addcn.invoiceno");
            lblInvoiceAmount.Content = _messageResolver.GetText("sl.creditnote.addcn.invoiceamount");
            lblPrevCNAmount.Content = _messageResolver.GetText("sl.creditnote.addcn.prevcnamount");
            lblCNAmount.Content = _messageResolver.GetText("sl.creditnote.addcn.cnamount");
            btAdd.Content = _messageResolver.GetText("sl.creditnote.addcn.addproduct");
            btConfirm.Content = _messageResolver.GetText("sl.creditnote.addcn.confirm");
            btCancel.Content = _messageResolver.GetText("sl.creditnote.addcn.cancel");

            //grid
            colProduct.Header = _messageResolver.GetText("sl.creditnote.addcn.grid.col.header.product");
            colQuantity.Header = _messageResolver.GetText("sl.creditnote.addcn.grid.col.header.qty");
            colUnitPrice.Header = _messageResolver.GetText("sl.creditnote.addcn.grid.col.header.unitprice");
            colTotalPrice.Header = _messageResolver.GetText("sl.creditnote.addcn.grid.col.header.totalprice");
            colReason.Header = _messageResolver.GetText("sl.creditnote.addcn.grid.col.header.reason");
        }

        private AddCreditNoteLineItemModal modal = null;
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            modal = new AddCreditNoteLineItemModal();
            AddCreditNoteLineViewModel vmLineItem = modal.DataContext as AddCreditNoteLineViewModel;
            vmLineItem.Setup(vm.InvoiceId);
            modal.Closed += (s, modale) =>
            {
                if (modal.DialogResult == true)
                {
                    vm.AddLineItem(vmLineItem);
                }
            };
            modal.ShowDialog();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            string tag = hl.Tag.ToString();
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString()); //int.Parse(tag[0].ToString());
            vm.RemoveLineItem(ParentProductid);
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            string tag = hl.Tag.ToString();
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString()); //int.Parse(tag[0].ToString());
            var load = vm.LoadForEdit(ParentProductid);
            modal = new AddCreditNoteLineItemModal();
           
            AddCreditNoteLineViewModel vmLineItem = modal.DataContext as AddCreditNoteLineViewModel;
            vmLineItem.Setup(vm.InvoiceId);
            vmLineItem.Reason = load.Reason;
            vmLineItem.QuantityRequired = load.Quantity;
            vmLineItem.ProductLookUp = vmLineItem.ProductLookUpList.FirstOrDefault(p => p.ProductId == load.ProductId);
            modal.Closed += (s, modale) =>
            {
                if (modal.DialogResult == true)
                {
                    vm.AddLineItem(vmLineItem);
                }
            };
            modal.ShowDialog();
        }

       
    }
}
