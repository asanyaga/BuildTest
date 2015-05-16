using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Reporting;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Reporting.WinForms;
using StructureMap;


namespace Distributr.WPF.UI.Views.Orders
{
    /// <summary>
    /// Interaction logic for SalesManOrdersListing.xaml
    /// </summary>
    public partial class SalesManOrdersListing : Page
    {
        private SalesmanOrderListingViewModel _vm;
       public SalesManOrdersListing()
        {
            InitializeComponent();
           LocalizeControls();
           _vm = DataContext as SalesmanOrderListingViewModel;
          
        }

       private void LocalizeControls()
       {
           var messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

           lblSearchText.Content = messageResolver.GetText("sl.orderSummary.searchby_lbl");
           btnClear.Content = messageResolver.GetText("sl.orderSummary.clear_btn");
           btnAddItem.Content = messageResolver.GetText("sl.orderSummary.addOrder_btn");

           //Pending Approval Tab
           PendingApprovalTab.Header = messageResolver.GetText("sl.orderSummary.pendingApproval.tab");
           coldgPendingApprovalDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.orderRef");
           coldgPendingApprovalDateRequired.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.dateRequired");
           coldgPendingApprovalSalesman.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.salesman");
           coldgPendingApprovalStatus.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.status");
           coldgPendingApprovalTotalNet.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.netAmount");
           coldgPendingApprovalTotalVat.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.totalVat");
           coldgPendingApprovalTotalGross.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.grossAmount");
           coldgPendingApprovalTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.amountPaid");
           coldgPendingApprovalTotalDue.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.outstandingAmount");

           //Pending Dispatch Tab
           PendingDispatchTab.Header = messageResolver.GetText("sl.orderSummary.pendingDispatch.tab");
           coldgPendingDispatchDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.orderRef");
           coldgPendingDispatchDate.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.dateRequired");
           coldgPendingDispatchSalesman.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.salesman");
           coldgPendingDispatchStatus.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.status");
           coldgPendingDispatchTotalNet.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.netAmount");
           coldgPendingDispatchTotalVat.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.totalVat");
           coldgPendingDispatchTotalGross.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.grossAmount");
           coldgPendingDispatchTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.amountPaid");
           coldgPendingDispatchTotalDue.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.outstandingAmount");

           //Pending Dispatched Tab
           DispatchedTab.Header = messageResolver.GetText("sl.orderSummary.dispatched.tab");
           coldgDispatchedOrdersDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.orderRef");
           coldgDispatchedOrdersDate.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.dateRequired");
           coldgDispatchedOrdersSalesman.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.salesman");
           coldgDispatchedOrdersStatus.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.status");
           coldgDispatchedOrdersTotalNet.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.netAmount");
           coldgDispatchedOrdersTotalVat.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.totalVat");
           coldgDispatchedOrdersTotalGross.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.grossAmount");
           coldgDispatchedOrdersTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.amountPaid");
           coldgDispatchedOrdersTotalDue.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.outstandingAmount");

           //Pending Incomplete Tab
           IncompleteTab.Header = messageResolver.GetText("sl.orderSummary.incomplete.tab");
           coldgIncompleteDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.orderRef");
           coldgIncompleteDate.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.dateRequired");
           coldgIncompleteSalesman.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.salesman");
           coldgIncompleteStatus.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.status");
           coldgIncompleteTotalNet.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.netAmount");
           coldgIncompleteTotalVat.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.totalVat");
           coldgIncompleteTotalGross.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.grossAmount");
           coldgIncompleteTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.amountPaid");
           coldgIncompleteTotalDue.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.outstandingAmount");

           //Pending Delivered Tab
           DeliveredTab.Header = messageResolver.GetText("sl.orderSummary.delivered.tab");
           coldgDeliveredDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.orderRef");
           coldgDeliveredDate.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.dateRequired");
           coldgDeliveredSalesman.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.salesman");
           coldgDeliveredStatus.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.status");
           coldgDeliveredTotalNet.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.netAmount");
           coldgDeliveredTotalVat.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.totalVat");
           coldgDeliveredTotalGross.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.grossAmount");
           coldgDeliveredTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.amountPaid");
           coldgDeliveredTotalDue.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.outstandingAmount");

           //Pending Outstanding Payments Tab
           OutstandingPaymentsTab.Header = messageResolver.GetText("sl.orderSummary.outstandingPayment.tab");
           coldgPartiallyPaidDeliveriesDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.orderRef");
           coldgPartiallyPaidDeliveriesDate.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.dateRequired");
           coldgPartiallyPaidDeliveriesSalesman.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.salesman");
           coldgPartiallyPaidDeliveriesStatus.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.status");
           coldgPartiallyPaidDeliveriesTotalNet.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.netAmount");
           coldgPartiallyPaidDeliveriesTotalVat.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.totalVat");
           coldgPartiallyPaidDeliveriesTotalGross.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.grossAmount");
           coldgPartiallyPaidDeliveriesTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.amountPaid");
           coldgPartiallyPaidDeliveriesTotalDue.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.outstandingAmount");

           //Pending Fully Paid Tab
           FullyPaidDeliveriesTab.Header = messageResolver.GetText("sl.orderSummary.fullyPaid.tab");
           coldgFullyPaidDeliveriesDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.orderRef");
           coldgFullyPaidDeliveriesDate.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.dateRequired");
           coldgFullyPaidDeliveriesSalesman.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.salesman");
           coldgFullyPaidDeliveriesStatus.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.status");
           coldgFullyPaidDeliveriesTotalNet.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.netAmount");
           coldgFullyPaidDeliveriesTotalVat.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.totalVat");
           coldgFullyPaidDeliveriesTotalGross.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.grossAmount");
           coldgFullyPaidDeliveriesTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.amountPaid");
           coldgFullyPaidDeliveriesTotalDue.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.outstandingAmount");

           //Pending Back Orders Tab
           BackOrdersTab.Header = messageResolver.GetText("sl.orderSummary.backOrders.tab");
           coldgBackOrdersDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.orderRef");
           coldgBackOrdersDate.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.dateRequired");
           coldgBackOrdersSalesman.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.salesman");
           coldgBackOrdersStatus.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.status");
           coldgBackOrdersTotalNet.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.netAmount");
           coldgBackOrdersTotalVat.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.totalVat");
           coldgBackOrdersTotalGross.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.grossAmount");
           coldgBackOrdersTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.amountPaid");
           coldgBackOrdersTotalDue.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.outstandingAmount");

           //Pending Lost Sales Tab
           LostSalesTab.Header = messageResolver.GetText("sl.orderSummary.lostSales.tab");
           coldgLostSalesDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.orderRef");
           coldgLostSalesDate.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.dateRequired");
           coldgLostSalesSalesman.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.salesman");
           coldgLostSalesStatus.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.status");
           coldgLostSalesTotalNet.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.netAmount");
           coldgLostSalesTotalVat.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.totalVat");
           coldgLostSalesTotalGross.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.grossAmount");
           coldgLostSalesTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.amountPaid");
           coldgLostSalesTotalDue.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.outstandingAmount");

           //Pending Rejected Tab
           RejectedOrdersTab.Header = messageResolver.GetText("sl.orderSummary.rejected.tab");
           coldgRejectedOrdersDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.orderRef");
           coldgRejectedOrdersDate.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.dateRequired");
           coldgRejectedOrdersSalesman.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.salesman");
           coldgRejectedOrdersStatus.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.status");
           coldgRejectedOrdersTotalNet.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.netAmount");
           coldgRejectedOrdersTotalVat.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.totalVat");
           coldgRejectedOrdersTotalGross.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.grossAmount");
           coldgRejectedOrdersTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.amountPaid");
           coldgRejectedOrdersTotalDue.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.outstandingAmount");
       }

       private void view_click(object sender, RoutedEventArgs e)
       {
           //var reportViewer = new ReportViewer();
          
           //Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.ApprovedOrdersReport);
           //reportViewer.LocalReport.DataSources.Clear();
           //reportViewer.ProcessingMode = ProcessingMode.Local;
           //reportViewer.LocalReport.EnableHyperlinks = true;
           //using (var c = ObjectFactory.Container.GetNestedContainer())
           //{
           //    var orderReposiory = c.GetInstance<IMainOrderRepository>();
           //    var startDatetime = startdate.Value;
           //    var endDateTime = enddate.Value;
           //    var salesmanId = _vm.SelectedSalesman != null ? _vm.SelectedSalesman.Id : Guid.Empty;
           //    var data = orderReposiory.GetApproveOrderAndDateProcessedList(startDatetime.Value, endDateTime.Value,
           //                                                                  salesmanId).ToList()
           //        .OrderByDescending(p => p.DateProcessed);
           //        //.ThenByDescending(n => n.OrderReference)
           //        //.ThenByDescending(n => n.ExternalRefNo);

           //    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSetApprovedOrder",data));
           //}
          
           //reportViewer.LocalReport.LoadReportDefinition(stream);

           //reportViewer.LocalReport.Refresh();
           //reportViewer.RefreshReport();

           //windowsFormsHost.Child = reportViewer;
       }
   
    }
}
