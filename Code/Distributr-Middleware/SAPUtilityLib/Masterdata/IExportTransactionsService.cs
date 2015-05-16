using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.Utils;
using SAPbobsCOM;

namespace SAPUtilityLib.Masterdata
{
   public interface IExportTransactionsService
   {
       Task<bool> ExportToSap(OrderType orderType);
     
   }
   public interface IOrderExportTransactionsService
   {
       Task<SyncBasicResponse> ExportToSap(OrderType orderType,DocumentStatus status);

   }
    
}
