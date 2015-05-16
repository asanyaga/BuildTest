using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Transactional.DocumentEntities;

using Distributr.WSAPI.Lib.Integrations;

namespace SAPUtilityLib.Proxies
{
    public  interface ISapWebProxy
    {
       TransactionExportResponse GetNextOrder(OrderType orderType, DocumentStatus status);
       ResponseBool MarkOrderAsExported(string orderExternalRef);
    }
}
