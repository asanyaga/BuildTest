using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.UI.Pages
{
   public interface IDistributrMessageBox
   {
       DistributrMessageBoxResult ShowBox(List<DistributrMessageBoxButton> items, string text, string messageBoxTitle = "Distributr Message Box");
   }

    public class DistributrMessageBoxResult
    {
        public DistributrMessageBoxButton Button { get; set; }
        public string Url { get; set; }
    }

    public enum DistributrMessageBoxButton
    {
        None = 0,
        Home=1,

        //order
        SalesmanOrderNew=2,
        SalesmanOrderSummary=3,
        SalesmanOrderApprove=4,
        SalesmanDispatch=5,
        Cancel = 6,
        SalesmanBackOrderAndApprove=7,
        SalesmanProcessBackOrder=8,
        SalesmanLossAndApprove = 9,

        //Purchase order
        PurchaseOrderNew = 10,
        PurchaseOrderSummary = 11,

        //POS
        NewSale = 12,
        PosSaleSummary = 13,

        SalemanOrderViewDispatched = 14,
         //Purchase order
        StockistPurchaseOrderNew = 15,
        StockistPurchaseOrderSummary = 16,
        StockistPurchaseOrderApproval = 17,

        //Agrim
        
    }

    public class DistributrMessageBoxItem
    {
        public DistributrMessageBoxButton Button { set; get; }
        public string Url { set; get; }
        public string ButtonToolTip { set; get; }
        public string ButtonText { set; get; }
       
    }

    public  class CustomMessageBoxItems
    {
        private List<DistributrMessageBoxItem> item;
        public CustomMessageBoxItems()
        {
           item= Items();
        }

        public  DistributrMessageBoxItem  MessageBoxButtonItem(DistributrMessageBoxButton b)
        {
           var btn= item.FirstOrDefault(s => s.Button == b);
           if (btn==null)
           {
               btn = new DistributrMessageBoxItem
                         {
                             Button = DistributrMessageBoxButton.None,
                             ButtonText = "None",
                             Url = @"\Views\HomeViews\Home.xaml",
                             ButtonToolTip = "None"
                         };
           }
            return btn;
        }

        private  List<DistributrMessageBoxItem> Items()
        {
            List<DistributrMessageBoxItem> item = new List<DistributrMessageBoxItem>();
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.Home, ButtonText = "Home", Url = @"\Views\HomeViews\Home.xaml", ButtonToolTip = "Home" });
            //order
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanOrderNew, ButtonText = "New Order", Url = @"\Views\Orders\CreateOrder.xaml",ButtonToolTip="Home" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanOrderSummary, ButtonText = "Order Summary", Url = @"\Views\Orders\SalesManOrdersListing.xaml?PendingApprovalTab", ButtonToolTip = "Order summary" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanOrderApprove, ButtonText = "Approve Order", Url = @"\Views\Orders\SalesManOrdersListing.xaml?PendingApprovalTab", ButtonToolTip = "Approve orders" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanDispatch, ButtonText = "Dispatch Order", Url = @"\Views\Orders\OrderDispatch.xaml", ButtonToolTip = "Dispatch orders" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanBackOrderAndApprove, ButtonText = "Approve & Backorder", Url = "", ButtonToolTip = "Create Backorder and Approve" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.Cancel, ButtonText = "Cancel", Url = @"\Views\HomeViews\Home.xaml", ButtonToolTip = "Cancel" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanProcessBackOrder, ButtonText = "Process BackOrder", Url = @"\Views\Orders\SalesManOrdersListing.xaml?BackOrdersTab", ButtonToolTip = "Process BackOrder" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalesmanLossAndApprove, ButtonText = "Approve & Losssale", Url = "", ButtonToolTip = "Approve & Create Losssale" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.SalemanOrderViewDispatched, ButtonText = "View Dispatched Orders", Url = @"\Views\Orders\SalesManOrdersListing.xaml?DispatchedTab", ButtonToolTip = "View Dispatched Orders" });
            
            //Purchase
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.PurchaseOrderNew, ButtonText = "New Purchase Order", Url = @"\Views\Orders_Purchase\CreatePurchaseOrder.xaml", ButtonToolTip = "New Purchase Order" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.PurchaseOrderSummary, ButtonText = "Purchase Order Summary", Url = @"\Views\Orders_Purchase\PurchaseOrderListing.xaml?PendingApprovalTab", ButtonToolTip = "Purchase Order Summary" });
           //POS
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.NewSale, ButtonText = "New Sale", Url = @"\Views\Order_Pos\AddPOS.xaml", ButtonToolTip = "Do a New Sale" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.PosSaleSummary, ButtonText = "Sales Summary", Url = @"\Views\Order_Pos\ListPOSSales.xaml", ButtonToolTip = "POS sales Summary" });
            //Stockist Purchase
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.StockistPurchaseOrderNew, ButtonText = "New P.O.", Url = @"\Views\Orders_Stockist\CreateStockistOrder.xaml", ButtonToolTip = "New Stockist Purchase Order" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.StockistPurchaseOrderSummary, ButtonText = "P.O. Summary", Url = @"\Views\Orders_Stockist\StockistPurchaseOrderListing.xaml?PendingApprovalTab", ButtonToolTip = "Stockist Purchase Order Summary" });
            item.Add(new DistributrMessageBoxItem { Button = DistributrMessageBoxButton.StockistPurchaseOrderApproval, ButtonText = "P.O. Approval", Url = @"\Views\Orders_Stockist\StockistPurchaseOrderListing.xaml?PendingApprovalTab", ButtonToolTip = "Stockist Purchase Order Approval" });
          
            return item;

        }
    }
}
