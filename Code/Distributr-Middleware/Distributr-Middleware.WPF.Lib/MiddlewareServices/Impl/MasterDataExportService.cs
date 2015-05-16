using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.WSAPI.Lib.Integrations;

namespace Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl
{
    /// <summary>
    /// Downloads  master data from distributr ready for export  into external ERP
    /// </summary>
    public class MasterDataExportService : MiddleWareBase, IMasterDataExportService
    {

        public virtual Task<IEnumerable<ImportEntity>> DownloadMasterdata(MasterDataCollective entity, List<string> searchTexts = null)
        {
           throw new NotImplementedException();
        }

      
    }
}
