using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility.MasterData;
using Distributr.WebApi.ApiControllers;
using Distributr.WSAPI.Lib.Services.Sync;
using log4net;

namespace Distributr.WebApi.Common.ApiControllers
{
    [RoutePrefix("api/downloadmasterdata")]
    public class DownloadMasterDataController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("DownloadMasterDataController");
        private IMasterDataZipperService _dataZipperService;

        public DownloadMasterDataController( IMasterDataZipperService dataZipperService)
        {
            _dataZipperService = dataZipperService;
           
        }

        [HttpGet]
        [Route("GetZipped/{costCentreApplicationId}")]
        public HttpResponseMessage GetZipped(Guid costCentreApplicationId)
        {

            var path = _dataZipperService.CreateCsvAndZip(new QueryMasterData { ApplicationId = costCentreApplicationId ,Skip = 0,Take = 500,});
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "MasterDataCSVFiles.zip"
            };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }
    }
}
