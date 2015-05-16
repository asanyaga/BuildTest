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
using Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views
{
    public partial class ReceiptDocument : Page
    {
       private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
       
        public ReceiptDocument()
        {
            InitializeComponent();
           
            LabelControls();
        }

        void LabelControls()
        {
            PrintButton.Content = _messageResolver.GetText("sl.receipt.print");
            btnBack.Content = _messageResolver.GetText("sl.receipt.back");
            lblCompanyName.Content = _messageResolver.GetText("sl.receipt.companyname");
            lblAddress.Content = _messageResolver.GetText("sl.receipt.address");
            lblPhysicalAddress.Content = _messageResolver.GetText("sl.receipt.physicaladdress");
            lblTelNo.Content = _messageResolver.GetText("sl.receipt.telno");
            lblFaxNo.Content = _messageResolver.GetText("sl.receipt.faxno");
            lblCell.Content = _messageResolver.GetText("sl.receipt.cell");
            lblVatNo.Content = _messageResolver.GetText("sl.receipt.vatno");
            lblPin.Content = _messageResolver.GetText("sl.receipt.pinno");
            lblEmail.Content = _messageResolver.GetText("sl.receipt.email");
            lblWebsite.Content = _messageResolver.GetText("sl.receipt.website");
            lblOfficialReceipt.Content = _messageResolver.GetText("sl.receipt.officialreceipt");
            lblReceiptNo.Content = _messageResolver.GetText("sl.receipt.receiptno");
            lblInvoiceNo.Content = _messageResolver.GetText("sl.receipt.invoicerefno");
            lblReceiptDate.Content = _messageResolver.GetText("sl.receipt.receiptdate");
            lblInvoiceDate.Content = _messageResolver.GetText("sl.receipt.invoicedate");
            txtWithThanks.Text = _messageResolver.GetText("sl.receipt.withthanks");
            lblBeingReceiptFor.Text = _messageResolver.GetText("sl.receipt.beingreceiptfor");
            lblTotalGross.Content = _messageResolver.GetText("sl.receipt.totalgross");
            lblServedBy.Content = _messageResolver.GetText("sl.receipt.servedby");
            lblWithThanksRegards.Content = _messageResolver.GetText("sl.receipt.withthanksandregards");
            lblSignature.Content = _messageResolver.GetText("sl.receipt.signature");
            lblSignedDate.Content = _messageResolver.GetText("sl.receipt.date");

            //grid
            colPatmentType.Header = _messageResolver.GetText("sl.receipt.payments.grid.col.paymentmode");
            colPaymentTypeRef.Header = _messageResolver.GetText("sl.receipt.payments.grid.col.reference");
            //colDescriptiom.Header = _messageResolver.GetText("sl.receipt.payments.grid.col.description");
            colValue.Header = _messageResolver.GetText("sl.receipt.payments.grid.col.amount");

        }


        
    }
}
