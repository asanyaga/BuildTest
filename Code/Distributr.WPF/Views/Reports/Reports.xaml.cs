using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Navigation;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Reports;
using Distributr.WPF.UI.Views.DocumentReports;
using Microsoft.Reporting.WinForms;
using StructureMap;

namespace Distributr.WPF.UI.Views.Reports
{
    public partial class Reports : PageBase
    {
        ReportsViewModel _vm = null;
        public Reports()
        {
            InitializeComponent();
            _vm = DataContext as ReportsViewModel;
            LabelControls();
            _vm.Setup();
        }
        void LabelControls()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

            tiSales.Header = messageResolver.GetText("sl.reportsMenu.salesReports_tab");
            tiOrders.Header = messageResolver.GetText("sl.reportsMenu.ordersReports_tab");
            tiInventory.Header = messageResolver.GetText("sl.reportsMenu.inventoryReports_tab");
            cmdGenerateReport.Content = messageResolver.GetText("sl.reportsMenu.viewReport_btn");
        }

        private void cmdGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            _vm.ReportUrl = null;
            if (tcReportMenu.SelectedItem == tiSales)
            {
                if (rsSalesSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SaleSummary";
                    AddSalesTabWithReportView("Sales Summary");
                }
                if (rsSalesSummaryPerSalesman.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SalesSummary_Per_Salesman";
                    AddSalesTabWithReportView("Sales Summary Per Salesman");
                }
                if (rsSalesSummaryByDate.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales summary by date";
                    AddSalesTabWithReportView("Sales Summary by Date");
                }
                if (rsSalesByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SalesSummary_PerDistributor";
                    AddSalesTabWithReportView("Sales by Distributor");
                }
                if (rsSalesByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales By Brand";
                    AddSalesTabWithReportView("Sales by Brand");
                }
                if (rsSalesByPackaging.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SaleSummary_PerDistributor_PerPackaging";
                    AddSalesTabWithReportView("Sales by Packaging");
                }
                if (rsRegionalSales.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales by Country";
                    AddSalesTabWithReportView("Sales by Country");
                }
                if (rsCloseOfDay.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/closeOfDay";
                    AddSalesTabWithReportView("Close of Day");
                }
                if (rsLosses.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/losses";
                    AddSalesTabWithReportView("Losses");
                }
                if (rsSalesBySalesman.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales By Brant per DistributorSalesman";
                    AddSalesTabWithReportView("Sale by Salesman");
                }
                if (rsSalesByProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales By Product";
                    AddSalesTabWithReportView("Sale by Product");
                }
                if (rsSalesByProductType.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SalesByProductType";
                    AddSalesTabWithReportView("Sale By ProductType");
                }
                if (rsSalesByRoute.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales By Region Per Distributor Route";
                    AddSalesTabWithReportView("Sale by Route");
                }
                if (rsSalesByOutlet.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Sales By Region Per Distributor Routes Outlets";
                    AddSalesTabWithReportView("Sale by Outlet");
                }
                if (rsZeroSales.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/ZeroSalesOutlets";
                    AddTabWithReportView("Zero Sales");
                }
            }
            else if (tcReportMenu.SelectedItem == tiOrders)
            {
                //NEW ORDERS FROM RS
                if (rsOrdersSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrderSummaryByDistributor";
                    AddTabWithReportView("Orders Summary");
                }
                if (rsOrdersByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrderByBrand";
                    AddTabWithReportView("Orders By Brand");
                }
                if (rsOrdersByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrdersByDitsributor";
                    AddTabWithReportView("Orders By Distributor");
                }
                if (rsOrdersByProductType.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrdersSummaryByProductType";
                    AddTabWithReportView("Orders By Product Type");
                }
                if (rsOrdersByProductPackaging.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrdersByProductPackaging";
                    AddTabWithReportView("Orders By Product Packaging");
                }
                if (rsOrdersByProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrdersByProduct";
                    AddTabWithReportView("Orders By Product ");
                }
                if (rsOrdersByDateException.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OrderDateException";
                    AddTabWithReportView("Orders By Date Exception ");
                }


            }
            else if(tcReportMenu.SelectedItem == tiReconciliation)
            {
                if (rsSalesAndGrossProfit.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_SalesAndGrossProfit";
                    AddTabWithReportView("Sales & Gross Profit ");
                }
                if (rsSalesmanMovement.IsChecked==true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconcilliation_SalesmanMovements";
                    AddTabWithReportView("Salesman Movements ");
                }
                if (rsLostOrdersByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_LostOrdersByBrand";
                    AddTabWithReportView("Lost Orders by Brand ");
                }
                if (rsLostOrdersSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_LostOrderSummary";
                    AddTabWithReportView("Lost Orders Summary");
                }
                if (rsBackOrdersByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_BackOrders_ByBrand";
                    AddTabWithReportView("Back Orders by Brand");
                }
                if (rsBackOrdersSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_BackOrderSummary_ByRoute";
                    AddTabWithReportView("Back Orders Summary");
                }
                if (rsSalesQuantitySummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_SalesQuantitySummary";
                    AddTabWithReportView("Sales Quantity Summary");
                }
                if (rsSalesValueSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_SalesValueSummary";
                    AddTabWithReportView("Sales Value Summary");
                }
                if (rsOutstandingPayment.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_OustandingPayment";
                    AddTabWithReportView("Sales Value Summary");
                } 
                if (rsCashSalesReconciliation.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_Cash_Sales_Reconciliation";
                    AddTabWithReportView("Cash Sales Reconciliation");
                }
                if (rsBrandSalesReconciliation.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_BrandSalesReconciliation";
                    AddTabWithReportView("Brand Sales Reconciliation");
                }

                if (rsBrandPerformance.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Reconciliation_BrandPerformance";
                    AddTabWithReportView("Brand Performance");
                }
                if (rsOutletsSummaryByProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OutletSummary_ByProduct";
                    AddTabWithReportView("Outlets Summary by Product");
                }

            }
            else if (tcReportMenu.SelectedItem == tiDeliveries)
            {
                if (rsDeliveriesByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/DeliveriesSummaryPerDistributor";
                    AddTabWithReportView("Deliveries Summary");
                }  
            }
            else if (tcReportMenu.SelectedItem == tiInventory)
            {
                //NEW INVENTORY FROM RS
                if (rsStockSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockSummary_PerBrand";
                    AddTabWithReportView("Stock Summary");
                }
                if (rsStockByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockByBrand";
                    AddTabWithReportView("Stock By Brand");
                }
                if (rsStockByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockByDistributor";
                    AddTabWithReportView("Stock By Distributor");
                }
                if (rsStockAtDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Inventory_StockAtDistributor";
                    AddTabWithReportView("Stock at Distributor");
                }
                if (rsStockBySalesman.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockBySalesman_Summary";
                    AddTabWithReportView("Stock By Salesman");
                }
                if (rsStockByProductType.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockByProductType";
                    AddTabWithReportView("Stock By Product Type");
                }
                if (rsStockBySubBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockBySubBrand";
                    AddTabWithReportView("Stock By SubBrand");
                }
                if (rsStockByPackaging.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/StockByPackaging";
                    AddTabWithReportView("Stock By Packaging");
                }
                if (rsStockReturns.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Inventory_StockReturns";
                    AddTabWithReportView("Stock Returns");
                }
                if (rsStockTake.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Inventory_StockTake";
                    AddTabWithReportView("Stock Take");
                }
                if (rsStockMovement.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Inventory_StockMovementByDistributor";
                    AddTabWithReportView("Stock Movement");
                }
                if (rsProductCatalog.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Inventory_ProductCatalog";
                    AddTabWithReportView("Product Catalog");
                }


            }
            else if (tcReportMenu.SelectedItem == tiPurchases)
            {
                if (rsPurchasesByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/PurchaseByDistributor";
                    AddTabWithReportView("Purchase By Distributor");
                }
                if (rsPurchasesByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/PurchaseByBrand";
                    AddTabWithReportView("Purchase By Brand");
                }
                if (rsPurchasesByPackaging.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/PurchasesByPackaging";
                    AddTabWithReportView("Purchase By Packaging");
                }

            }
            else if (tcReportMenu.SelectedItem == tiExceptionReports)
            {
                if (rsSalesDateExReport.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SalesDateException";
                    AddTabWithReportView("Sale Date Exception");
                }
            }
            else if (tcReportMenu.SelectedItem == tiPaymentSummaryReports)
            {
                if (rSPaymentSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/PaymentSummary_PerDistributor";
                    AddTabWithReportView("Payment Summary");
                }
                if (rSPaymentSummaryPerSalesman.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/PaymentSummary_Per_Salesman";
                    AddTabWithReportView("Payment Summary Per Salesman");
                }

            }
            else if (tcReportMenu.SelectedItem == tiOutlets)
            {
                //NEW OUTLETS
                if (rsOutletsByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OutletsByDistrbutor";
                    AddTabWithReportView("Outlets By Distributor");
                }
                if (rsOutletsCategory.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OutletsByCategory";
                    AddTabWithReportView("Outlets Category");
                }
                if (rsOutletByTier.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OutletByTier";
                    AddTabWithReportView("Outlets By Tier");
                }
                if (rsDistributorOutlets.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OutletsSummary";
                    AddTabWithReportView("Distributor Outlets Summary");
                }
                if (rsOutletsByType.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/OutletsByType";
                    AddTabWithReportView("Outlets By Type");
                }


            }
            else if (tcReportMenu.SelectedItem == tiTargets)
            {
                if (rsDistributorTargets.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Targets_DistributorTargets";
                    AddTabWithReportView("Distributor Targets");
                }
                if (rsSalesmanTargets.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Targets_SalesmanTargets";
                    AddTabWithReportView("Salesman Targets");
                }
                if (rsOutletTargets.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Targets_OutletTargets";
                    AddTabWithReportView("Outlet Targets");
                }
            }
            else if (tcReportMenu.SelectedItem == tiRoutesByDistributor)
            {
                if (rsRoutesByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/RouteByDistributor";
                    AddTabWithReportView("Routes by Distributor");
                }
            }
            else if (tcReportMenu.SelectedItem == tiInventoryReceipt)
            {
                if (rsInventoryReceiptByBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/InventoryReceipt_ByBrand";
                    AddTabWithReportView("Inventory Receipt by Brand");
                }
                if (rsInventoryReceiptByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/InventoryReceipt_ByDistributor";
                    AddTabWithReportView("Inventory Receipt by Distributor");
                }
                if (rsInventoryReceiptByPackaging.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/InventoryReceipt_ByPackaging";
                    AddTabWithReportView("Inventory Receipt by Packaging");
                }
                if (rsInventoryReceiptBySubBrand.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/InventoryReceipt_BySubBrand";
                    AddTabWithReportView("Inventory Receipt by SubBrand");
                }
            }
                // CALL PROTOCOL RPTS
            else if (tcReportMenu.SelectedItem == tiCall)
            {
                if (rsSalesCall.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CallProtocol_SalesCallReport";
                    AddTabWithReportView("Sales Call Report");
                }
            }


            else if (tcReportMenu.SelectedItem == tiDiscounts)
            {
                //NEW DISCOUNTS
                if (rsTotalDiscountsByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_TotalDiscountsByDistributor";
                    AddTabWithReportView("Total Discounts By Distributor");
                }
                if (rsProductDiscount.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_ProductDiscounts";
                    AddTabWithReportView("Product Discount");
                }
                if (rsPromoDiscountsByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_PromotionDiscountByDistributor";
                    AddTabWithReportView("Promotion Discount By Distributor");
                }
                if (rsPromoDiscountsByProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_PromotionDiscountByProduct";
                    AddTabWithReportView("Promotion Discount By Product");
                }
                if (rsCVCP.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_CertainValueCertainProductDiscountByProduct";
                    AddTabWithReportView("Certain Value Certain Product Discount");
                }
                if (rsFOCByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_FreeOfChargeByDistributor";
                    AddTabWithReportView("Free of Charge Discount by Distributor");
                }
                if (rsFOCByProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Discounts_FreeOfChargeByProduct";
                    AddTabWithReportView("Free of Charge Discount by Product");
                }
                if (rsSVDSummary.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SVD_PerDistributor";
                    AddTabWithReportView("Sale Value Discount Summary");
                }
                if (rsSVDperOutlet.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/SVD_PerOutlet";
                    AddTabWithReportView("Sale Value Discount per Outlet");
                }
            }
            else if (tcReportMenu.SelectedItem == tiAudit)
            {
                if (rsAuditTrail.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Audit_AuditTrail";
                    AddTabWithReportView("Audit Trail");
                }
                //if (rsAuditStockMovement.IsChecked == true)
                //{
                //    _vm.ReportUrl = "/DistributrReports/Audit_StockMovement";
                //    AddTabWithReportView("Stock Movement Summary");
                //}
                if (rsAuditStockMovementPerProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Audit_StockMovement_PerProduct";
                    AddTabWithReportView("Stock Movement Per Product");
                }
                if (rsAuditSalesmanStockMovementPerProduct.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Audit_Salesman_StockMovement_PerProduct";
                    AddTabWithReportView("Salesman Stock Movement Per Product");
                }

            }
            else if (tcReportMenu.SelectedItem == tiCrates)
            {
                if (rsCrateSales.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CrateSales_PerCountry";
                    AddTabWithReportView("Crate Sales Summary");
                }
                if (rsCrateOrders.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CrateOrders_PerCountry";
                    AddTabWithReportView("Crate Orders Summary");
                }
                if (rsCrateDeliveries.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CrateDeliveries_PerCountry";
                    AddTabWithReportView("Crate Deliveries Summary");
                }
                if (rsCrateInvByDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CrateInventory_ByDistributor";
                    AddTabWithReportView("Crate Inventory By Distributor");
                }
                if (rsCrateInvAtDistributor.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CrateInventory_AtDistributor";
                    AddTabWithReportView("Crate Inventory At Distributor");
                }
                if (rsCrateInvBySalesman.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/CrateInventory_BySalesman";
                    AddTabWithReportView("Crate Inventory By Salesman");
                }
            }
            else if(tcReportMenu.SelectedItem == tiStockist)
            {
                if (rsStockistStock.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Stockist_StockByStockist";
                    AddTabWithReportView("Stock By Stockist");
                }
                if (rsStockistSalesmanStock.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Stockist_StockByStockistSalesman";
                    AddTabWithReportView("Stock By Stockist Salesman");
                }
                if (rsStockistSales.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Stockist_SalesByStockist";
                    AddTabWithReportView("Sales By Stockist");
                }
                if (rsStockistSalesmanSales.IsChecked == true)
                {
                    _vm.ReportUrl = "/DistributrReports/Stockist_SalesByStockistSalesman";
                    AddTabWithReportView("Sales By Stockist Salesman");
                }

            }

        }

        void AddTab(string title,bool isReportServer = false)
        {

            TabItem tab = new TabItem();
            tab.Header = title;
            Style style = this.FindResource("ClosableTabItemStyle") as Style;
            tab.Style = style;
            WebBrowser browser = new WebBrowser();
            var uri = isReportServer == true ? _vm.getReportServerUri() : _vm.getReportUri();
            browser.Navigate(uri);
            browser.ObjectForScripting = new ScriptingHelper();
            tab.Content = browser;
            int i = tcReports.Items.Add(tab);
            tcReports.SelectedIndex = i;
        }
        void AddTabWithReportView(string title, bool isReportServer = false)
        {
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            var reportViewer = new ReportViewer();
            reportViewer.ProcessingMode=ProcessingMode.Remote;
            string url = "";
            string password = "";
            string username = "";
            string folder = "";
            using(var c = ObjectFactory.Container.GetNestedContainer())
            {
                var settingrepo = c.GetInstance<IGeneralSettingRepository>();
                GeneralSetting passsetting = settingrepo.GetByKey(GeneralSettingKey.ReportPassword);
                if (passsetting != null) password = VCEncryption.DecryptString(passsetting.SettingValue);
                GeneralSetting usersetting = settingrepo.GetByKey(GeneralSettingKey.ReportUsername);
                if (usersetting != null) username = usersetting.SettingValue;
                GeneralSetting reportUrl = settingrepo.GetByKey(GeneralSettingKey.ReportUrl);
                if (reportUrl != null) url = reportUrl.SettingValue;
                GeneralSetting reportfolder = settingrepo.GetByKey(GeneralSettingKey.ReportFolder);
                if (reportfolder != null)
                {
                    folder = "/" + reportfolder.SettingValue;
                }
            }
            string distributorID = _vm.DistributorID.ToString();//.ToUpper();
            //string distributorID = "167480A6-DC20-44B9-A1B6-0B523914B5CE";
            var param = new ReportParameter();
            param = new ReportParameter("distributorId",distributorID,false);

            reportViewer.ServerReport.ReportPath =folder +  _vm.GetReportPath();// "/DistributrReports/Losses";
            reportViewer.ServerReport.ReportServerUrl = new Uri(url);
            reportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new NetworkCredential(username, password);
           // reportViewer.ShowParameterPrompts = false;
           // param.Visible = false;
            reportViewer.ServerReport.SetParameters(new[]{param});
            reportViewer.Drillthrough += DrillthroughEventHandler;
            reportViewer.ServerReport.Refresh();
            windowsFormsHost.Child = reportViewer;


            TabItem tab = new TabItem();
            tab.Header = title;
            Style style = this.FindResource("ClosableTabItemStyle") as Style;
            tab.Style = style;

            var uri = isReportServer == true ? _vm.getReportServerUri() : _vm.getReportUri();

            tab.Content = windowsFormsHost;
            int i = tcReports.Items.Add(tab);
            tcReports.SelectedIndex = i;
            reportViewer.RefreshReport();
        }
        void AddSalesTabWithReportView(string title, bool isReportServer = false)
        {
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            var reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Remote;
            string url = "";
            string password = "";
            string username = "";
            string folder = "";
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var settingrepo = c.GetInstance<IGeneralSettingRepository>();
                GeneralSetting passsetting = settingrepo.GetByKey(GeneralSettingKey.ReportPassword);
                if (passsetting != null) password = VCEncryption.DecryptString(passsetting.SettingValue);
                GeneralSetting usersetting = settingrepo.GetByKey(GeneralSettingKey.ReportUsername);
                if (usersetting != null) username = usersetting.SettingValue;
                GeneralSetting reportUrl = settingrepo.GetByKey(GeneralSettingKey.ReportUrl);
                if (reportUrl != null) url = reportUrl.SettingValue;
                GeneralSetting reportfolder = settingrepo.GetByKey(GeneralSettingKey.ReportFolder);
                if (reportfolder != null)
                {
                    folder = "/" + reportfolder.SettingValue;

                }

            }
            string distributorID = _vm.DistributorID.ToString();//.ToUpper();
            //string distributorID = "167480A6-DC20-44B9-A1B6-0B523914B5CE";
            var param = new ReportParameter();
            param = new ReportParameter("distributorID", distributorID, false);
            reportViewer.ServerReport.ReportPath = folder + _vm.GetReportPath();// "/DistributrReports/Losses";
       
            reportViewer.ServerReport.ReportServerUrl = new Uri(url);
            reportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new NetworkCredential(username, password);
            // reportViewer.ShowParameterPrompts = false;
            // param.Visible = false;
           reportViewer.ServerReport.SetParameters(new ReportParameter[] { param });

           reportViewer.Drillthrough += SalesDrillthroughEventHandler;
            reportViewer.ServerReport.Refresh();
            windowsFormsHost.Child = reportViewer;


            TabItem tab = new TabItem();
            tab.Header = title;
            Style style = this.FindResource("ClosableTabItemStyle") as Style;
            tab.Style = style;

            var uri = isReportServer == true ? _vm.getReportServerUri() : _vm.getReportUri();

            tab.Content = windowsFormsHost;
            int i = tcReports.Items.Add(tab);
            tcReports.SelectedIndex = i;
            reportViewer.RefreshReport();
        }
        void DrillthroughEventHandler(object sender, DrillthroughEventArgs e)
        {
            var report = e.Report;
            string distributorID = _vm.DistributorID.ToString();
            //string distributorID = "167480A6-DC20-44B9-A1B6-0B523914B5CE";
            var param = new ReportParameter();
            param = new ReportParameter("distributorId", distributorID, false); 

            try
            {
                report.SetParameters(new ReportParameter[] { param });
            }
            catch (Exception ex)
            {
            }

            report.Refresh();

        }
        void SalesDrillthroughEventHandler(object sender, DrillthroughEventArgs e)
        {
            var report = e.Report;
            string distributorID = _vm.DistributorID.ToString();
            //string distributorID = "167480A6-DC20-44B9-A1B6-0B523914B5CE";
            var param = new ReportParameter();
            param = new ReportParameter("distributorID", distributorID, false);

            try
            {
                report.SetParameters(new ReportParameter[] { param });
            }
            catch (Exception ex)
            {
            }

            report.Refresh();

        }
        [ComVisible(true)]
        public class ScriptingHelper
        {
            public void ShowMessage(string message)
            {
                MessageBox.Show(message);
            }
        }
    }
}
