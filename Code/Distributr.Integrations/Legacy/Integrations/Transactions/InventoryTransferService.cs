namespace Distributr.Integrations.Legacy.Integrations.Transactions
{
   public interface IInventoryTransferService
   {
       IntegrationResponse Process(InventoryTransferDTO inventoryTransferDto);
   }
}
