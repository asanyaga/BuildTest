namespace Distributr.WPF.Lib.Services.UI
{
    public class UserRights
    {
        //If can Add > can Edit > can View > can Delete
        //If (can Edit > can Add > can Delete) mutually inclusive

        public bool IsAdministrator { get; set; }
        public bool IsFinanceHandler { get; set; }
        public bool IsInventoryHandler { get; set; }

        #region MasterData
        public bool CanViewAdminMenu { get; set; }

        //outlets, routes, salesman route assigment, 
        public bool CanViewMasterData { get; set; }
        public bool CanAddmasterData { get; set; }
        public bool CanModifyMasterData { get; set; }

        //Routes
        public bool CanViewRoutes { get; set; }
        public bool CanManageRoutes { get; set; }

        //Outlets
        public bool CanViewOutlets { get; set; }
        public bool CanManageOutlet { get; set; }

        //Users
        public bool CanViewUsers { get; set; }
        public bool CanManageUsers { get; set; }

        //Salesmanroute
        public bool CanViewSalesmanRoutes { get; set; }
        public bool CanManageSalesmanRoutes { get; set; }

        //Contacts
        public bool CanViewContacts { get; set; }
        public bool CanManageContacts { get; set; }

        #endregion

        #region POS
        public bool CanViewPOSSales { get; set; }
        public bool CanManagePOSSales { get; set; }
        #endregion

        #region Orders
        public bool CanViewOrders { get; set; }
        public bool CanCreateOrder { get; set; }
        public bool CanEditOrder { get; set; }
        public bool CanApproveOrders { get; set; }
        public bool CanDispatchOrder { get; set; }

        #endregion

        #region PurchaseOrder
        public bool CanViewPurchaseOrders { get; set; }
        public bool CanCreatPurchaseOrders { get; set; }
        #endregion

        #region Inventory
        public bool CanViewInventory { get; set; }
        public bool CanAdjustInventory { get; set; }
        public bool CanIssueInventory { get; set; }
        public bool CanReceiveInventory { get; set; }
        public bool CanReceiveReturnables { get; set; }
        public bool CanReturnInventory { get; set; }
        public bool CanDispatchProducts { get; set; }
        public bool CanReconcileGenericReturnables { get; set; }
        public bool CanViewReturnsList { get; set; }
        public bool CanDoStockTake { get; set; }

        #endregion

        #region Payments
        public bool CanViewOutstandingPayments { get; set; }
        public bool CanIssueCreditNote { get; set; }
        public bool CanReceivePayments { get; set; }

        #endregion

        #region Reports
        public bool CanViewReports { get; set; }
        public bool CanViewOrdersReports { get; set; }
        public bool CanViewFinancialReports { get; set; }
        public bool CanViewInventoryLevels { get; set; }
        public bool CanViewInventoryIssues { get; set; }
        public bool CanViewInventoryAdjustmentReports { get; set; }
        public bool CanViewAuditLog { get; set; }

        #endregion

        #region Sync
        public bool CanViewSyncMenu { get; set; }
        public bool CanViewSettings { get; set; }
        #endregion


        #region Stockist
        public bool CanCreateStockistOrder { get; set; }
        public bool CanViewStockistOrder { get; set; }
        #endregion

        #region Banking
        public bool CanViewUnderbankinglist { get; set; }
        #endregion



        #region OutletVisitDays
        public bool CanCreateOutletVisitDays { get; set; }

        #endregion

        #region OutletPriority
        public bool CanViewOutletPriority { get; set; }

        #endregion

         #region OutletTargets
        public bool CanViewOutletTargets { get; set; }

        #endregion

        #region SalesManTarget
        public bool CanViewSalesManTarget { get; set; }

        #endregion
        
        #region Agrimanagr
        public bool CanViewAdmin{ get; set; }
        public bool CanViewWarehouse { get; set; } 
        public bool CanViewActivities { get; set; }
        public bool CanViewCommodity { get; set; }
        #endregion


    }
}
