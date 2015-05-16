using System;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Views.InvoiceDocument
{
    public partial class InvoiceDocument : Page
    {
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public InvoiceDocument()
        {
            try
            {
                InitializeComponent();
                LabelControls();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void LabelControls()
        {
            btnViewReceipt.Content = _messageResolver.GetText("sl.invoice.viewreceipt");
            PrintButton.Content = _messageResolver.GetText("sl.invoice.print");
            btnBack.Content = _messageResolver.GetText("sl.invoice.back");
            lblCompanyName.Content = _messageResolver.GetText("sl.invoice.companyname");
            lblAddress.Content = _messageResolver.GetText("sl.invoice.address");
            lblPhysicalAddress.Content = _messageResolver.GetText("sl.invoice.physicaladd");
            lblTelNo.Content = _messageResolver.GetText("sl.invoice.telno");
            lblFaxNo.Content = _messageResolver.GetText("sl.invoice.faxno");
            lblCell.Content = _messageResolver.GetText("sl.invoice.cell");
            lblVatNo.Content = _messageResolver.GetText("sl.invoice.vatno");
            lblPinNo.Content = _messageResolver.GetText("sl.invoice.pinno");
            lblEmail.Content = _messageResolver.GetText("sl.invoice.email");
            lblWebsite.Content = _messageResolver.GetText("sl.invoice.website");
            lblSaleInvoice.Content = _messageResolver.GetText("sl.invoice.saleinvoice");
            lblInvoiceNo.Content = _messageResolver.GetText("sl.invoice.invoiceno");
            lblInvoiceDate.Content = _messageResolver.GetText("sl.invoice.invoicedate");
            txtWithThanks.Text = _messageResolver.GetText("sl.invoice.withthanks");
            lblTotalNet.Content = _messageResolver.GetText("sl.invoice.totalnet");
            lblTotalVAT.Content = _messageResolver.GetText("sl.invoice.totalvat");
            lblSaleDiscount.Content = _messageResolver.GetText("sl.invoice.salediscount");
            lblTotalGross.Content = _messageResolver.GetText("sl.invoice.totalgross");
            txtInvoiceDeductions.Text = _messageResolver.GetText("sl.invoice.deductions");
            lblTotalDeductions.Content = _messageResolver.GetText("sl.invoice.totaldeductions");
            lblSubInvoiceBalance.Content = _messageResolver.GetText("sl.invoice.invoicesubbalance");
            txtPaymentInfo.Text = _messageResolver.GetText("sl.invoice.paymentinfo");
            lblTotalAmountPaid.Content = _messageResolver.GetText("sl.invoice.totalamountpaid");
            lblInvoiceBalance.Content = _messageResolver.GetText("sl.invoice.invoicebalance");
            lblWithThanks.Content = _messageResolver.GetText("sl.invoice.withthanksandregards");
            lblCreditTerms.Content = _messageResolver.GetText("sl.invoice.creditterms");
            lblCheqPayableTo.Content = _messageResolver.GetText("sl.invoice.chequepayableto");
            lblPreparedBy.Content = _messageResolver.GetText("sl.invoice.preparedby");
            lblPreparedDate.Content = _messageResolver.GetText("sl.invoice.preparedby.date");
            lblPreparedSignature.Content = _messageResolver.GetText("sl.invoice.preparedby.signature");
            lblApprovedBy.Content = _messageResolver.GetText("sl.invoice.approvedby");
            lblApprovedDate.Content = _messageResolver.GetText("sl.invoice.approvedby.date");
            lblApproverSignature.Content = _messageResolver.GetText("sl.invoice.approvedby.signature");

            //grid gdInvoiceLineItems
            colInvDescription.Header = _messageResolver.GetText("sl.invoice.invoicelineitems.grid.col.description");
            colInvQty.Header = _messageResolver.GetText("sl.invoice.invoicelineitems.grid.col.qty");
            colInvUnitPrice.Header = _messageResolver.GetText("sl.invoice.invoicelineitems.grid.col.unitprice");
            colInvDisc.Header = _messageResolver.GetText("sl.invoice.invoicelineitems.grid.col.productdiscount");
            colInvAmount.Header = _messageResolver.GetText("sl.invoice.invoicelineitems.grid.col.amount");

            //grid dgCreditNoteLineItems
            colCNCreditNoteRef.Header = _messageResolver.GetText("sl.invoice.cn.grid.col.cnref");
            colCNDescription.Header = _messageResolver.GetText("sl.invoice.cn.grid.col.itemdesc");
            colCNQty.Header = _messageResolver.GetText("sl.invoice.cn.grid.col.qty");
            colCNUnitPrice.Header = _messageResolver.GetText("sl.invoice.cn.grid.col.unitprice");
            colCNAmount.Header = _messageResolver.GetText("sl.invoice.cn.grid.col.amount");

            //grid dgReceipts
            colRecReceiptReference.Header = _messageResolver.GetText("sl.invoice.receipt.grid.col.reference");
            colRecReceiptDate.Header = _messageResolver.GetText("sl.invoice.receipt.grid.col.date");
            colRecAmount.Header = _messageResolver.GetText("sl.invoice.receipt.grid.col.paid");

        }
    }
}
