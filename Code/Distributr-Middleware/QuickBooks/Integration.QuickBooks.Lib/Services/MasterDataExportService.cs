using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

using Distributr.WSAPI.Lib.Integrations;
using Distributr_Middleware.WPF.Lib.MiddlewareServices.Impl;

namespace Integration.QuickBooks.Lib.Services
{
    public class QBMasterDataExportService : MasterDataExportService
    {
        public override async Task<IEnumerable<ImportEntity>> DownloadMasterdata(MasterDataCollective entity, List<string> searchTexts = null)
        {
            string entityname = "";
          switch (entity)
            {
                case MasterDataCollective.SaleProduct:
                    entityname = MasterDataCollective.SaleProduct.ToString();
                    break;
                case MasterDataCollective.Outlet:
                    entityname = MasterDataCollective.Outlet.ToString();
                    break;
            }
          var dto = await GetMasterDataAsync(entityname,searchTexts);
            return dto;
        }

        async Task<List<ImportEntity>> GetMasterDataAsync(string entityName, List<string> searchTexts)
        {
            try
            {
                if (searchTexts == null || !searchTexts.Any()) return null;
                var result = new List<ImportEntity>();
                int page = 1;
                int pagesize = 100;
                DateTime lastSync = DateTime.Now.AddYears(-40);
                string middleurl = string.Format("api/Integrations/ExportMasterData");
                var httpClient = MiddlewareHttpClient;
                bool hasNextPage = true;
                string lookUpText = string.Join(",", searchTexts);
               
                while (hasNextPage)
                {
                    string url = string.Format("{0}?EntityName={1}&SearchText={2}&page={3}&pagesize={4}&syncTimeStamp={5}",
                        httpClient.BaseAddress + middleurl, entityName,lookUpText, page, pagesize, lastSync);
                    var response = await httpClient.GetAsync(url);

                    MasterdataExportResponse info = await response.Content.ReadAsAsync<MasterdataExportResponse>();
                    if (info.MasterData !=null && info.MasterData.Any())
                    {
                        result.AddRange(info.MasterData);
                        page = info.Skip;
                        hasNextPage = info.HasNextPage;
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
