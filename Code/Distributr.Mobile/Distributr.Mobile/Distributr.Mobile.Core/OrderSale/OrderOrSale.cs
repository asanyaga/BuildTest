using System;
using System.Linq;
using Distributr.Mobile.OrderSale;

namespace Distributr.Mobile.Core.OrderSale
{
    public class OrderOrSale
    {
        public static string AllOrderAndSales
        {
            get
            {
                return
                    @"SELECT 
                        OrderOrSale.OrderReference AS OrderSaleReference, OrderOrSale.ProcessingStatus AS Status, OrderOrSale.DateCreated, OrderOrSale.MasterId AS OrderSaleId, Outlet.Name AS OutletName
                     FROM 
                        OrderOrSale
                     INNER JOIN
                        Outlet
                     ON
                        OrderOrSale.OutletMasterId = Outlet.MasterId
                     ORDER BY
                        OrderOrSale.DateCreated DESC"; 
            }
        }

        private static string OrdersWithStatusWithinDateRange(object statuses, DateTime startDate, DateTime endDate)
        {
            return string.Format(
                @"SELECT 
                        OrderOrSale.OrderReference AS OrderSaleReference, OrderOrSale.ProcessingStatus AS Status, OrderOrSale.DateCreated, OrderOrSale.MasterId AS OrderSaleId, Outlet.Name AS OutletName
                     FROM 
                        OrderOrSale
                     INNER JOIN
                        Outlet
                     ON
                        OrderOrSale.OutletMasterId = Outlet.MasterId
                     WHERE
                        OrderOrSale.ProcessingStatus IN ({0}) 
                     AND 
                        OrderOrSale.DateCreated > {1} AND OrderOrSale.DateCreated < {2}
                     ORDER BY
                        OrderOrSale.DateCreated DESC", statuses, startDate.Ticks, endDate.Ticks); 
        }

        public static string OrdersPendingDeliveryForOutlet(Guid outletId)
        {
            return string.Format(
                @"SELECT 
                        OrderOrSale.OrderReference AS OrderSaleReference, OrderOrSale.ProcessingStatus AS Status, OrderOrSale.DateCreated, OrderOrSale.MasterId AS OrderSaleId, Outlet.Name AS OutletName
                     FROM 
                        OrderOrSale
                     INNER JOIN
                        Outlet
                     ON
                        OrderOrSale.OutletMasterId = Outlet.MasterId
                     WHERE
                        OrderOrSale.OutletMasterId = '{0}'
                     AND 
                        OrderOrSale.ProcessingStatus = {1}
                     ORDER BY
                        OrderOrSale.DateCreated DESC", outletId, (int)ProcessingStatus.Deliverable);
        }

        public static string OrdersWithStatusWithinDateRange(OrderFilter filter)
        {
            var joined = string.Join(",", filter.SelectedProcessingStatuses.Select(s => (int) s));

            return OrdersWithStatusWithinDateRange(joined, filter.StartDate, filter.EndDate);
        }

        public static string OrdersPendingDelivery
        {
            get
            {
                var status = (int)ProcessingStatus.Deliverable;
                return OrdersWithStatusWithinDateRange(status, new DateTime(), DateTime.Now);
            }
        }

        public string StatusText
        {
            get { return Enum.GetName(typeof(ProcessingStatus), Status); }
        }

        public Guid OrderSaleId { get; set; }
        public string OrderSaleReference { get; set; }
        public ProcessingStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public string OutletName { get; set; }
    }
}