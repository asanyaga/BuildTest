using System.Windows.Controls;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Views.ReceiptDocuments
{
    /// <summary>
    /// Interaction logic for GenericReceiptContent.xaml
    /// </summary>
    public partial class GenericReceiptContent : UserControl
    {
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public GenericReceiptContent()
        {
            InitializeComponent();
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
