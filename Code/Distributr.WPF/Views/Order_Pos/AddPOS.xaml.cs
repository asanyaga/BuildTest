using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using StructureMap;

namespace Distributr.WPF.UI.Views.Order_Pos
{
    /// <summary>
    /// Interaction logic for AddEditPOS.xaml
    /// </summary>
    public partial class AddEditPOS : Page
    {
         public AddEditPOS()
        {
            InitializeComponent();
            LocalizeControls();
        }

         private void LocalizeControls()
         {
              IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

             lblSalesId.Content = messageResolver.GetText("sl.createSale.saleid_lbl");
             lblDateRequired.Content = messageResolver.GetText("sl.createSale.date_lbl");
             lblSalesman.Content = messageResolver.GetText("sl.createSale.salesman_lbl");
             lblRoute.Content = messageResolver.GetText("sl.createSale.route_lbl");
             lblOutlet.Content = messageResolver.GetText("sl.createSale.outlet_lbl");
             lblStatus.Content = messageResolver.GetText("sl.createSale.status_lbl");
             lblTotalDiscount.Content = messageResolver.GetText("sl.createSale.totalProductDiscount_lbl");
             lblTotalNetAmnt.Content = messageResolver.GetText("sl.createSale.totalNet_lbl");
             lblTotalVat.Content = messageResolver.GetText("sl.createSale.totalVat_lbl");
             lblSaleValue.Content = messageResolver.GetText("sl.createSale.saleValue_lbl");
             lblReturnablesValue.Content = messageResolver.GetText("sl.createSale.returnablesValue_lbl");
             lblSaleDiscount.Content = messageResolver.GetText("sl.createSale.saleDiscount_lbl");
             lblTotalGross.Content = messageResolver.GetText("sl.createSale.totalGross_lbl");
             lblAmountPaid.Content = messageResolver.GetText("sl.createSale.amountPaid_lbl");

             //cn: buttons
             btnAddLineItem.Content = messageResolver.GetText("sl.createSale.addProduct_btn");
             btnConfirmOrder.Content = messageResolver.GetText("sl.createSale.completeSale_btn");
             btnReceiveReturnables.Content = messageResolver.GetText("sl.createSale.receiveReturnables_btn");
             //btnCancel.Content = messageResolver.GetText("sl.pos.cancel_btn");
             btnReceivePayments.Content = messageResolver.GetText("sl.createSale.receivePayments_btn");
             btnSave.Content = messageResolver.GetText("sl.createSale.saveAndContinueLater_btn");

             //payment grid
             var item = PaymentsDataGrid.Columns.GetByName("colPaymentType");
             colPaymentType.Header = messageResolver.GetText("sl.pos.payment.grid.col.paymenttype");
             colAmount.Header = messageResolver.GetText("sl.pos.payment.grid.col.amount");
             colConfirmed.Header = messageResolver.GetText("sl.pos.payment.grid.col.confirmed");

             //Line Items grid
             colProduct.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.product");
             colQty.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.qty");
             colUnitPrice.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.unitPrice");
             colUnitVat.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.unitVat");
             colUnitDisc.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.unitDisc");
             colUnitVat.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.unitVat");
             //colGrossAmt.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.grossAmt");
             colTotal.Header = "Amount (Excl VAT)";//messageResolver.GetText("sl.pos.lineitems.grid.col.total");
             colProductType.Header = messageResolver.GetText("sl.pos.lineitems.grid.col.producttype");
         }
    }
}
