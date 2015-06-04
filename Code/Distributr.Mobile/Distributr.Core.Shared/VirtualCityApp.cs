using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core
{
    public enum VirtualCityApp { Ditributr = 1, Agrimanagr = 2 }
    public class OrderSaveAndContinueLater
    {
        public OrderSaveAndContinueLater()
        {
            LineItem = new List<OrderSaveAndContinueLaterItem>();
        }
        public OrderType OrderType { get; set; }
        public Guid SalesmanId { get; set; }
        public string Salesman { get; set; }
        public Guid RouteId { get; set; }
        public Guid OutletId { get; set; }
        public string Outlet { get; set; }
        public Guid ShipToAddressId { get; set; }
        public Guid Id { get; set; }
        public decimal SaleDiscount { get; set; }
        public DateTime Required { get; set; }

        public List<OrderSaveAndContinueLaterItem> LineItem { get; set; }
    }

    public class OrderSaveAndContinueLaterItem
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitVat { get; set; }
        public decimal UnitDiscount { get; set; }
        public string ProductType { get; set; }
        public MainOrderLineItemType LineItemType { get; set; }
        public DiscountType DiscountType { get; set; }


    }
}
