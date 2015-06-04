using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.UserEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum UserRole
    {
       // None = 0,
        Administrator=1,
        FinanceHandler=2,
        InventoryHandler=3,
        
        //Master Data
        RoleViewMasterData = 10,
        RoleAddMasterData = 11,
        RoleModifyMasterData = 12,

        //Usermanagement
        RoleViewUser = 20,
        RoleAddUser = 21,
        RoleModifyUser=22,

        //CostcentreManagement
        RoleViewCostCentre=30,
        RoleAddCostCentre=31,
        RoleModifyCostcentre=32,

        //Outlet
        RoleViewOutlet = 50,
        RoleAddOutlet = 51,
        RoleModifyOutlet = 52,

        //Orders
        RoleCreateOrder = 40,
        RoleApproveOrder = 41,
        RoleDispatchOrder = 42,
        RoleEditOrder = 43,
        RoleViewOrder = 44,

        //Inventory
        RoleViewInventory = 60,
        RoleAdjustInventory = 61,
        RoleIssueInventory = 62,
        RoleReceiveInventory = 68,
        RoleReceiveReturnables = 69,
        RoleDispatchProducts = 70,
        RoleReturnInventory = 71,
        RoleReconcileGenericReturnables = 72,
        RoleViewReturnsList = 73,
   


        //POS
        RoleViewPOS= 63,
        RoleCreatePOSSale = 64,

        //Purchase Order
        RoleViewPurchaseOrder = 65,
        RoleCreatePurchaseOrder = 66,

        //Payments
        RoleViewOutstandingPayments = 74,
        RoleIssueCreditNote = 75,
       


        //Reports
        RoleViewReportsMenu = 76,
        RoleViewOrdersReports = 77,
        RoleViewFinancialReports = 78,
        RoleViewInventoryLevels = 79,
        RoleViewInventoryIssues = 80,
        RoleViewInventoryAdjustmentReports = 81,
        RoleViewAuditLog = 82,


        //Sync
        RoleViewSyncMenu = 83,
        RoleViewSettings = 84,

        //SalesmanRoutes
        RoleViewSalesmanRoute = 85,
        RoleCreateSalesmanRoute = 86,

        //Routes
        RoleViewRoutes = 87,
        RoleCreateRoutes = 88,

        //Contacts
        RoleViewContacts = 89,
        RoleCreateContacts = 90,

        RoleStockTake = 91,
       //StockistOrder
       RoleCreateStockistOrder = 92,
       RoleViewStockistOrder = 93,

        RoleReceivePayments = 94,

       //Banking
        RoleViewUnderbankinglist = 95,

       //Outlet Visit Days
        RoleCreateOutletVisitDays = 96,
       
       // Outlet Priority

       RoleViewOutletPriority = 97,

        //OutletTargets

       RoleViewOutletTargets =98,

        //SalesManTarget
       RoleViewSalesManTarget =99,
       RoleImportSalesmanInventory = 100


     }
}
