using System;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync
{
    public interface IMasterDataZipperService
    {
        string CreateCsvAndZip(QueryMasterData myQuery);
    }
}