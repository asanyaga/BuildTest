using System;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Products
{
    public interface IInventoryRepository
    {
        void AdjustInventoryForSale(Order order);
        void AdjustInventoryForProduct(Guid productId, decimal quantity);
        int GetBalanceForProduct(Guid productId);
    }

    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private readonly Database database;

        public InventoryRepository(Database database) : base(database)
        {
            this.database = database;
        }

        public void AdjustInventoryForSale(Order order)
        {
            foreach (var lineItem in order.LineItems)
            {
                var inventoryItem = database.Get<Inventory>(i => i.ProductMasterID == lineItem.ProductMasterId);
                inventoryItem.Balance -= lineItem.Quantity;
                Save(inventoryItem);
            }
        }

        public void AdjustInventoryForProduct(Guid productId, decimal quantity)
        {
            var count = database.Execute("UPDATE Inventory SET Balance = Balance + ? WHERE ProductMasterID = ?", quantity, productId);
        }

        public int GetBalanceForProduct(Guid productId)
        {
            var count = database.Query<Balance>("SELECT Balance AS Count FROM Inventory WHERE ProductMasterID = ?", productId);
            return count.Count > 0 ? count[0].Count : 0;
        }
    }

    public class Balance
    {
        public int Count { get; set; }
    }
}
