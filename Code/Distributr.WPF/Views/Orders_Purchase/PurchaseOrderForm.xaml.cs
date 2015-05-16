using System.Windows.Controls;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.WPF.UI.Views.Orders_Purchase
{
    /// <summary>
    /// Interaction logic for PurchaseOrderForm.xaml
    /// </summary>
    public partial class PurchaseOrderForm : Page
    {
        
        public PurchaseOrderForm()
        {
            InitializeComponent();
            LocalizeControls();
        }

        private void LocalizeControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            Gridcolproductname.Header = messageResolver.GetText("sl.po.form.grid.col.productname");
            Gridcolquantity.Header = messageResolver.GetText("sl.po.form.grid.col.quantity");
            Gridcolunitprice.Header = messageResolver.GetText("sl.po.form.grid.col.unitprice");
            Gridcolunitvat.Header = messageResolver.GetText("sl.po.form.grid.col.vat");
            Gridcoltotalamt.Header = messageResolver.GetText("sl.po.form.grid.col.totalprice");
            textBlockOrderId.Content = messageResolver.GetText("sl.po.form.orderId");
            LabelOrderDate.Content = messageResolver.GetText("sl.po.form.orderdate");
            textBlockStatus.Content = messageResolver.GetText("sl.po.form.status");
            textBlockTotalNet.Content = messageResolver.GetText("sl.po.form.totalnet");
            textBlockTotalVat.Content = messageResolver.GetText("sl.po.form.totalvat");
            textBlockTotalGross.Content = messageResolver.GetText("sl.po.form.totalgross");
        }



    }
}
