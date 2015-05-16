using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

using Distributr.WSAPI.Lib.Integrations;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices
{
    public interface IMasterDataExportService
    {
        Task<IEnumerable<ImportEntity>> DownloadMasterdata(MasterDataCollective entity, List<string> searchTexts = null);
    }
}
