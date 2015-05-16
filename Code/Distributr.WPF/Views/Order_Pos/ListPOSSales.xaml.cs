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
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using StructureMap;

namespace Distributr.WPF.UI.Views.Order_Pos
{
    /// <summary>
    /// Interaction logic for ListPOSSales.xaml
    /// </summary>
    public partial class ListPOSSales : Page
    {
        public ListPOSSales()
        {
            InitializeComponent();
            LocalizeControls();
        }

        private void LocalizeControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            CompleteSalesTabItem.Header = messageResolver.GetText("sl.listPosSales.completeSalesTab");
            IncompleteTabItem.Header = messageResolver.GetText("sl.listPosSales.incompleteTab");
            OutstandingTabItem.Header = messageResolver.GetText("sl.listPosSales.outstandingTab");

            //summary
            lblSearchBy.Content = messageResolver.GetText("sl.listPosSales.searchBy_lbl");
            cmdClear.Content = messageResolver.GetText("sl.listPosSales.clear_btn");
            btnAddItem.Content = messageResolver.GetText("sl.listPosSales.addNewSale_btn");

            //Complete Sales Tab
            colDgSalesSaleRef.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.saleNumber");
            colDgSalesDate.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.saleDate");
            colDgSalesStatus.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.status");
            colDgSalesDiscount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.discount");
            coldgSalesAmountPaid.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.amountPaid");
            coldgSalesAmountDue.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.outstandingAmount");

            colDgSalesNetAmount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.netAmt");
            colDgSalesVatAmount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.vatAmt");
            colDgSalesGrossTotal.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.grossTotal");

            //Incomplete Sales Tab
            coldgIncompleteSaleRef.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.saleNumber");
            coldgIncompleteDate.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.saleDate");
            coldgIncompleteStatus.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.status");
            coldgIncompleteNetAmount.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.netAmt");
            coldgIncompleteVatAmount.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.vatAmt");
            colDgIncompleteDiscount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.discount");
            coldgIncompleteTotalGross.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.grossTotal");
            coldgIncompleteAmountPaid.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.amountPaid");
            coldgIncompleteAmountDue.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.outstandingAmount");

            //Outstanding Payments Tab
            //coldgoutStandingPaymentsSalesRef.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.saleNumber");
            coldgoutStandingPaymentsDate.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.saleDate");
            coldgoutStandingDiscount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.discount");
            coldgoutStandingPaymentsStatus.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.status");
            coldgoutStandingPaymentsNetAmount.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.netAmt");
            coldgoutStandingPaymentsVatAmount.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.vatAmt");
            coldgoutStandingPaymentsGrossAmount.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.grossTotal");
            coldgoutStandingPaymentsAmountPaid.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.amountPaid");
            coldgoutStandingPaymentsAmountDue.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.outstandingAmount");
        }
    }
}
