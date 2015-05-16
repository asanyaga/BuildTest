using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public interface IUpdateLocalDBService
    {
        void UpdateLocalDB(ResponseMasterDataInfo responseMasterDataInfo);
        void UpdateInventoryDB(ResponseMasterDataInfo responseMasterDataInfo);
        void UpdatePaymentDB(ResponseMasterDataInfo responseMasterDataInfo);
        void UnderBankingDB(ResponseMasterDataInfo responseMasterDataInfo);

    }
}
