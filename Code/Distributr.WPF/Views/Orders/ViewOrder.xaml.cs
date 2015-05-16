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
using Distributr.WPF.Lib.ViewModels.Transactional.Orders.OrderProduct;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.Orders
{
    /// <summary>
    /// Interaction logic for ViewOrder.xaml
    /// </summary>
    public partial class ViewOrder : UserControl
    {
        public ViewOrder()
        {
            InitializeComponent();
            LocalizeControls();
        }

        private void LocalizeControls()
        {
            var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            lblOrderId.Content = messageResolver.GetText("sl.createOrder.saleid_lbl");
            lblDateRequired.Content = messageResolver.GetText("sl.createOrder.date_lbl");
            lblSalesman.Content = messageResolver.GetText("sl.createOrder.salesman_lbl");
            lblRoute.Content = messageResolver.GetText("sl.createOrder.route_lbl");
            lblOutlet.Content = messageResolver.GetText("sl.createOrder.outlet_lbl");
            lblTotalNet.Content = messageResolver.GetText("sl.createOrder.totalNet_ExclVat_lbl");
            lblTotaProductDiscount.Content = messageResolver.GetText("sl.createOrder.totalProductDiscount_lbl");
            lblTotalVat.Content = messageResolver.GetText("sl.createOrder.totalVat_lbl");
            lblSaleDiscount.Content = messageResolver.GetText("sl.createOrder.saleDiscount_lbl");
            lblTotalGross.Content = messageResolver.GetText("sl.createOrder.totalGross_lbl");
            lblStatus.Content = messageResolver.GetText("sl.createOrder.status_lbl");
            
            //lblAmountPaid.Content = messageResolver.GetText("sl.createOrder.status_lbl");
            //lblAmountOutstanding.Content = messageResolver.GetText("sl.createOrder.status_lbl");
            
            //btnCancel.Content = messageResolver.GetText("sl.createOrder.addProduct_btn");
            //btnViewInvoice.Content = messageResolver.GetText("sl.createOrder.saveAndContinueLater_btn");
            //btnViewReceipt.Content = messageResolver.GetText("sl.createOrder.completeSale_btn");
            //btnProcess.Content = messageResolver.GetText("sl.createOrder.cancelOrder");//cn: Cancel Order

            colProduct.Header = messageResolver.GetText("sl.createOrder.grid.col.product");
            colQty.Header = messageResolver.GetText("sl.createOrder.grid.col.qty");
            //colApproved.Header = messageResolver.GetText("sl.createOrder.grid.col.qty");
            //colBackOrder.Header = messageResolver.GetText("sl.createOrder.grid.col.qty");
            //colLoss.Header = messageResolver.GetText("sl.createOrder.grid.col.qty");
            colUnitPrice.Header = messageResolver.GetText("sl.createOrder.grid.col.unitprice");
            colUnitDisc.Header = messageResolver.GetText("sl.createOrder.grid.col.unitdiscount");
            colTotalAmt.Header = "Amount (Excl VAT)"; //messageResolver.GetText("sl.createOrder.grid.col.totalamount");
            colUnitVat.Header = messageResolver.GetText("sl.createOrder.grid.col.unitvat");
            colTotalVat.Header = messageResolver.GetText("sl.createOrder.grid.col.totalvat");
            colGrossAmt.Header = messageResolver.GetText("sl.createOrder.grid.col.grossamount");
            colProductType.Header = messageResolver.GetText("sl.createOrder.grid.col.producttype");
        }
    }
}
