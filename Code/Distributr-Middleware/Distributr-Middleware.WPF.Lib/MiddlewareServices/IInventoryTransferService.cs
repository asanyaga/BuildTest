using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.Integrations;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices
{
 
    public interface IInventoryTransferService
    {
        Task<IntegrationResponse> UploadInventory(InventoryTransferDTO file);
        List<ImportItemModel> Import(string directoryPath);
        Task<List<string>> GetAcknowledgements(DateTime date);
    }

    public class ImportItemModel
    {
        public string SalesmanCode { get; set; }
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string DocumentRef { get; set; }
    }
}
