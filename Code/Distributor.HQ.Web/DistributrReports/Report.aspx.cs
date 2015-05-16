using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Agrimanagr.HQ.Models;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Microsoft.Reporting.WebForms;
using StructureMap;

namespace Distributr.HQ.Web.DistributrReports
{
    public partial class Report : System.Web.UI.Page
    {
        string reportNames;
        string reportPath;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                reportNames = Request.QueryString["reportName"];
                switch (reportNames)
                {
                        //SALES REPORTS
                    //case "RegionalSales":
                    //    reportPath = "/DistributrSalesReports/Sales By Country";
                    //    break;
                    case "SalesByBrand":
                        reportPath = "/DistributrReports/Sales By Brand";
                        break;

                    case "SaleByDistributor":
                        //reportPath = "/DistributrReports/sale by Distributor";
                        reportPath = "/DistributrReports/SalesSummary_PerDistributor";

                        break;

                    case "SalesByPackaging":
                      //  reportPath = "/DistributrReports/Sales By Packaging";
                        reportPath = "/DistributrReports/SaleSummary_PerDistributor_PerPackaging";
                        break;

                    case "SalesSummary":
                        reportPath = "/DistributrReports/SaleSummary";
                        break;

                    case "SalesByProductType":
                        reportPath = "/DistributrReports/SalesByProductType";
                        break;

                    case "SalesSummaryByDate":
                        reportPath = "/DistributrReports/Sales summary by date";
                        break;
                    case "SalesSummaryPerSalesman":
                        reportPath = "/DistributrReports/SalesSummary_Per_Salesman";
                        break;
                    case "RegionalSales":
                        reportPath = "/DistributrReports/Sales By Country";
                        break;

                    case "CloseOfDay":
                        reportPath = "/DistributrReports/closeOfDay";
                        break;

                    case "Losses":
                        reportPath = "/DistributrReports/losses";
                        break;
                    case "SalesBySalesman":
                        //reportPath = "/DistributrReports/Sales By Brant per DistributorSalesman";
                        reportPath = "/DistributrReports/Sales_By_Salesman";
                        break;
                    case "SalesByProduct":
                        reportPath = "/DistributrReports/Sales By Product";
                        break;
                    case "SalesByRoute":
                        reportPath = "/DistributrReports/Sales_By_Route";
                        break;
                    case "SalesByOutlet":
                        reportPath = "/DistributrReports/Sales By Region Per Distributor Routes Outlets";
                        break;
                    case "ZeroSales":
                        reportPath = "/DistributrReports/ZeroSalesOutlets";
                        break;


                        //INVENTORY STOCK REPORTS

                    case "stockSummary":
                        reportPath = "/DistributrReports/StockSummary_PerBrand";
                        break;
                    case "stockByBrand":
                           reportPath = "/DistributrReports/StockByBrand";
                        break;
                    case "stockByDistributor":
                        reportPath = "/DistributrReports/StockByDistributor";
                        break;
                    case "stockAtDistributor":
                        reportPath = "/DistributrReports/Inventory_StockAtDistributor";
                        break;
                    case "stockBySalesman":
                        reportPath = "/DistributrReports/StockBySalesman_Summary";
                        break;
                    case "StockByProductType":
                        reportPath = "/DistributrReports/StockByProductType";
                        break;
                    case "stockBySubBrand":
                        reportPath = "/DistributrReports/StockBySubBrand";
                        break;
                    case "stockByPackaging":
                        reportPath = "/DistributrReports/StockByPackaging";
                        break;
                    case "stockReturns":
                        reportPath = "/DistributrReports/Inventory_StockReturns";
                        break;
                    case "stockTake":
                        reportPath = "/DistributrReports/Inventory_StockTake";
                        break;
                    case "stockMovement":
                        reportPath = "/DistributrReports/Inventory_StockMovementByDistributor";
                        break;
                    case "productCatalog":
                        reportPath = "/DistributrReports/Inventory_ProductCatalog";
                        break;

                        //PURCHASES REPORTS

                    case "purchaseByDistributor":
                        reportPath = "/DistributrReports/PurchaseByDistributor";
                        break;
                    case "purchaseByBrand":
                        reportPath = "/DistributrReports/PurchaseByBrand";
                        break;
                    case "purchaseByPackaging":
                        reportPath = "/DistributrReports/PurchasesByPackaging";
                        break;

                        //OUTLET REPORTS

                    case "OutletsByDistributor":
                        reportPath = "/DistributrReports/OutletsByDistrbutor";
                        break;
                    case "OutletsByCategory":
                        reportPath = "/DistributrReports/OutletsByCategory";
                        break;
                    case "OutletsByTier":
                        reportPath = "/DistributrReports/OutletByTier";
                        break;
                    case "OutletsSummary":
                        reportPath = "/DistributrReports/OutletsSummary";
                        break;
                    case "OutletsByType":
                        reportPath = "/DistributrReports/OutletsByType";
                        break;
                    case "DeactivatedOutlets":
                        reportPath = "/DistributrReports/DeactivatedOutlets";
                        break;

                        //ORDERS REPORTS

                    case "OrdersByDistributor":
                        reportPath = "/DistributrReports/OrdersByDitsributor";
                        break;
                    
                    case "OrdersSummary":
                        reportPath = "/DistributrReports/OrderSummaryByDistributor";
                        break;
                    case "OrdersByBrand":
                        reportPath = "/DistributrReports/OrderByBrand";
                        break;

                    case "OrdersByProductType":
                        reportPath = "/DistributrReports/OrdersSummaryByProductType";
                        break;

                    case "OrdersBySubBrand":
                        reportPath = "/DistributrReports/OrdersBySubBrand";
                        break;
                    case "OrdersByProductPackaging":
                        reportPath = "/DistributrReports/OrdersByProductPackaging";
                        break;
                    case "OrdersByProduct":
                        reportPath = "/DistributrReports/OrdersByProduct";
                        break;
                    case "OrderDateException":
                        //reportPath = "/DistributrReports/OrderDateException"; 
                        reportPath = "/DistributrReports/OderByDate";
                        break;



                       //RECONCILIATION REPORTS



                      //  ROUTE REPORT

                    case "RouteByDistributor":
                        reportPath = "/DistributrReports/RouteByDistributor";
                        break;


                     // INVENTORY RECEIPT REPORTS

                    case "InventoryReceiptByBrand":
                        reportPath = "/DistributrReports/InventoryReceipt_ByBrand";
                        break;

                    case "InventoryReceiptByDistributor":
                        reportPath = "/DistributrReports/InventoryReceipt_ByDistributor";
                        break;

                    case "InventoryReceiptByPackaging":
                        reportPath = "/DistributrReports/InventoryReceipt_ByPackaging";
                        break;

                    case "InventoryReceiptBySubBrand":
                        reportPath = "/DistributrReports/InventoryReceipt_BySubBrand";
                        break;
                        //DELIVERIES REPORTS
                    case "DeliveriesSummary":
                        reportPath = "/DistributrReports/DeliveriesSummaryPerDistributor";
                        break;

                        // DISCOUNT REPORTS
                    case "TotalDiscountsByDistributor":
                        reportPath = "/DistributrReports/Discounts_TotalDiscountsByDistributor";
                        break;
                    case "ProductDiscountSummary":
                        reportPath = "/DistributrReports/Discounts_ProductDiscounts";
                        break;
                    case "PromotionDiscountByDistributor":
                        reportPath = "/DistributrReports/Discounts_PromotionDiscountByDistributor";
                        break;

                    case "PromotionDiscountByProduct":
                        reportPath = "/DistributrReports/Discounts_PromotionDiscountByProduct";
                        break;

                    case "CertainValueCertainProduct":
                        reportPath = "/DistributrReports/Discounts_CertainValueCertainProductDiscountByProduct";
                        break;

                    case "FreeOfChargeByProduct":
                        reportPath = "/DistributrReports/Discounts_FreeOfChargeByProduct";
                        break;
                    case "FreeOfChargeByDistributor":
                        reportPath = "/DistributrReports/Discounts_FreeOfChargeByDistributor";
                        break;
                    case "SVDSummary":
                        reportPath = "/DistributrReports/SVD_PerCountry";
                        break;
                    case "SVDPerOutlet":
                        reportPath = "/DistributrReports/SVD_PerOutlet";
                        break;
                        //PAYEMENT REPORTS
                    case "PaymentSummary":
                        reportPath = "/DistributrReports/PaymentSummary_PerDistributor";
                        break;
                    case "PaymentSummaryPerSalesman":
                        reportPath = "/DistributrReports/PaymentSummary_Per_Salesman";
                        break;
                    case "Dashboard":
                        reportPath = "/DistributrReports/DB_Home";
                        break;
                     //RECONCILIATION
                    //case "outstandingPayments":
                    //    reportPath = "DistributrReports/Reconciliation_OustandingPayment";
                    //    break;
                    case "dailySaleSummary":
                        reportPath = "/DistributrReports/Daily_SalesSummary";
                        break;
                    case "salesAndGrossProfit":
                        reportPath = "/DistributrReports/Reconciliation_SalesAndGrossProfit";
                        break;
                    case "salesmanMovements":
                        reportPath = "/DistributrReports/Reconcilliation_SalesmanMovements";
                        break;
                    case "lostOrdersByBrand":
                        reportPath = "/DistributrReports/Reconciliation_LostOrdersByBrand";
                        break;
                    case "lostOrdersSummary":
                        reportPath = "/DistributrReports/Reconciliation_LostOrderSummary";
                        break;
                    case "backOrdersByBrand":
                        reportPath = "/DistributrReports/Reconciliation_BackOrders_ByBrand";
                        break;
                    case "backOrdersSummary":
                        reportPath = "/DistributrReports/Reconciliation_BackOrderSummary_ByRoute";
                        break;
                    case "SalesValueSummary":
                        reportPath = "/DistributrReports/Reconciliation_SalesValueSummary";
                        break;
                    case "SalesQuantitySummary":
                        reportPath = "/DistributrReports/Reconciliation_SalesQuantitySummary";
                        break;
                    case "BrandRanking":
                        reportPath = "/DistributrReports/Reconciliation_BrandRanking";
                        break;
                    case "BrandPerformance":
                        reportPath = "/DistributrReports/Reconciliation_BrandPerformance";
                        break;
                    case "BrandSales":
                        reportPath = "/DistributrReports/Reconciliation_BrandSalesReconciliation";
                        break;
                    case "outstandingPayments":
                        reportPath = "/DistributrReports/Reconciliation_OustandingPayment";
                        break;

                    case "CashSales":
                        reportPath = "/DistributrReports/Reconciliation_Cash_Sales_Reconciliation";
                        break;
                    case "SalesDateException":
                        reportPath = "/DistributrReports/SalesDateException";
                        break;

                    case "OutletSummaryByProduct":
                        reportPath = "/DistributrReports/OutletSummary_ByProduct";
                        break;
                        //PERFORMANCE REPORTS
                    case "performanceByRegion":
                        reportPath = "/DistributrReports/Performance_RegionPerformance";
                        break;
                    case "performanceByDistributor":
                        reportPath = "/DistributrReports/Performance_DistributorPerformance";
                        break;
                    case "performanceByRoute":
                        reportPath = "/DistributrReports/Performance_RoutePerformance";
                        break;
                    case "performanceBySalesman":
                        reportPath = "/DistributrReports/Performance_SalesmanPerformance";
                        break;
                    case "performanceByOutlet":
                        reportPath = "/DistributrReports/Performance_OutletPerformance";
                        break;
                        //TARGET REPORTS
                    case "countryTargets":
                        reportPath = "/DistributrReports/Targets_CountryTargets";
                        break;
                    case "regionTargets":
                        reportPath = "/DistributrReports/Targets_RegionTargets";
                        break;
                    case "distributorTargets":
                        reportPath = "/DistributrReports/Targets_DistributorTargets";
                        break;
                    case "salesmanTargets":
                        reportPath = "/DistributrReports/Targets_SalesmanTargets";
                        break;
                    case "outletTargets":
                        reportPath = "/DistributrReports/Targets_OutletTargets";
                        break;
                    //CALL PROTOCOL REPORT
                    case "cpDailySales":
                        reportPath = "/DistributrReports/CallProtocol_DailySalesCallReport";
                        break;
                    case "cpSalesCall":
                        reportPath = "/DistributrReports/CallProtocol_SalesCallReport";
                        break;
                    case "cpStockTake":
                        reportPath = "/DistributrReports/CallProtocol_DailyStockTakeRpt";
                        break;
                    case "cpNonProductive":
                        reportPath = "/DistributrReports/CallProtocol_NonProductiveCalls";
                        break;
                    case "cpNoActivity":
                        reportPath = "/DistributrReports/CallProtocol_NoActivityCalls";
                        break;
                    //AUDIT REPORT
                    case "aAuditTrail":
                        reportPath = "/DistributrReports/Audit_AuditTrail";
                        break;
                    case "aStockMovement":
                        reportPath = "/DistributrReports/Audit_StockMovement";
                        break;
                    case "aStockMovementPerProduct":
                        reportPath = "/DistributrReports/Audit_StockMovement_PerProduct";
                        break;
                    //CRATES SALES
                    case "crateSaleSummary":
                        reportPath = "/DistributrReports/CrateSales_PerCountry";
                        break;
                    //CRATES ORDERS
                    case "crateOrderSummary":
                        reportPath = "/DistributrReports/CrateOrders_PerCountry";
                        break;
                    //CRATES DELIVERIES
                    case "crateDeliverySummary":
                        reportPath = "/DistributrReports/CrateDeliveries_PerCountry";
                        break;
                    //CRATES INVENTORY
                    case "crateInventoryByDistributor":
                        reportPath = "/DistributrReports/CrateInventory_ByDistributor";
                        break;
                    case "crateInventoryAtDistributor":
                        reportPath = "/DistributrReports/CrateInventory_AtDistributor";
                        break;
                    case "crateInventoryBySalesman":
                        reportPath = "/DistributrReports/CrateInventory_BySalesman";
                        break;  
                   //STOCKIST REPORT
                    case "stockistStock":
                        reportPath = "/DistributrReports/Stockist_StockByStockist";
                        break;
                    case "stockistSalesmanStock":
                        reportPath = "/DistributrReports/Stockist_StockByStockistSalesman";
                        break;
                    case "stockistSales":
                        reportPath = "/DistributrReports/Stockist_SalesByStockist";
                        break;
                    case "stockistSalesmanSales":
                        reportPath = "/DistributrReports/Stockist_SalesByStockistSalesman";
                        break;
                    default:
                        reportPath = "";
                        break;
                }
                string server = "";
                string folder = "";
                var settings = ObjectFactory.GetInstance<ISettingsRepository>();
                var reportServer = settings.GetByKey(SettingsKeys.ReportServerUrl);
                var reportFolder = settings.GetByKey(SettingsKeys.ReportServerFolder);
                if (reportFolder != null)
                {
                    folder = "/" + reportFolder.Value;
                }
                if (reportServer != null) server = reportServer.Value;

                ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(server);
                ReportViewer1.ServerReport.ReportPath = folder + reportPath;
                ReportViewer1.ServerReport.ReportServerCredentials = new DistributrReportServerCredentials();
                ReportViewer1.ServerReport.Refresh();


            }

        }
    }
}