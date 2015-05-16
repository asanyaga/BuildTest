using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Exports
{
    public interface IMasterDataExportService
    {
        MasterdataExportResponse GetResponse(ThirdPartyMasterDataQuery query);
    }
}
