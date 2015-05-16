using System.Windows.Controls;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Views.Orders_Stockist
{
    /// <summary>
    /// Interaction logic for CreatePurchaseOrder.xaml
    /// </summary>
    public partial class ApproveStockistOrder : Page
    {
        public ApproveStockistOrder()
        {
            InitializeComponent();
            LocalizeControls();
        }

        private void LocalizeControls()
        {
            var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            colProduct.Header = messageResolver.GetText("sl.po.form.grid.col.productname");
            colQty.Header = messageResolver.GetText("sl.po.form.grid.col.quantity");
            colUnitPrice.Header = messageResolver.GetText("sl.po.form.grid.col.unitprice");
            colUnitVat.Header = messageResolver.GetText("sl.po.form.grid.col.vat");
            colTotalAmt.Header = messageResolver.GetText("sl.po.form.grid.col.totalprice");
            lblOrderId.Content = messageResolver.GetText("sl.po.form.orderId");
            //lblDateRequired.Content = messageResolver.GetText("sl.po.form.orderdate");
            lblStatus.Content = messageResolver.GetText("sl.po.form.status");
            lblTotalNet.Content = messageResolver.GetText("sl.po.form.totalnet");
            lblTotalVat.Content = messageResolver.GetText("sl.po.form.totalvat");
            lblTotalGross.Content = messageResolver.GetText("sl.po.form.totalgross");
        }
    }
}
