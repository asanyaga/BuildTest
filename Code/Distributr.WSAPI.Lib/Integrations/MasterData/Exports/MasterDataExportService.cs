using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Integrations.MasterData.Exports
{
    public class MasterDataExportService :MasterDataImportServiceBase, IMasterDataExportService
    {
     
        public MasterdataExportResponse GetResponse(ThirdPartyMasterDataQuery query)
        {
            var response = new MasterdataExportResponse();
            bool hasNext = false;
            IEnumerable<ImportEntity> masterData = new List<ImportEntity>();
            try
            {
                switch (query.MasterCollective)
                {
                    case MasterDataCollective.Outlet:
                        masterData = GetOutlets(query, out hasNext);
                        break;
                    case MasterDataCollective.SaleProduct:
                        masterData = GetProducts(query, out hasNext);
                        break;
                }
                response.MasterData = masterData;
                response.HasNextPage = hasNext;
            }catch(Exception ex)
            {
                response.Result = "Error";
                response.ResultInfo = ex.Message;
            }
            
            return response;
        }

        #region Products
        IEnumerable<ImportEntity> GetProducts(ThirdPartyMasterDataQuery myQuery, out bool hasNext)
        {
           
            using (var ctx=new CokeDataContext(Con))
            {
                IQueryable<tblProduct> query=null;
                hasNext = false;
                if (myQuery.SearchTextList.Any())
                {
                    List<tblProduct> items = new List<tblProduct>();
                    foreach (var productcode in myQuery.SearchTextList)
                    {
                        var found = ctx.tblProduct.FirstOrDefault(
                            p =>
                            p.ProductCode.Trim().Equals(productcode.Trim(),
                                                        StringComparison.CurrentCultureIgnoreCase));
                        if (found != null)
                            items.Add(found);
                    }
                    if (!items.Any()) return null;
                    return items.Select(Map);
                }
                return null;
            }
           
        }

        private ImportEntity Map(tblProduct item)
        {
            var result = new ImportEntity {MasterDataCollective = MasterDataCollective.SaleProduct.ToString()};
            result.Fields[0] =string.IsNullOrEmpty(item.ProductCode)?"":item.ProductCode.Trim();
            result.Fields[1] = string.IsNullOrEmpty(item.Description) ? "" : item.Description.Trim(); 
            result.Fields[2] = item.ExFactoryPrice.ToString("0.00");
            return result;
        }
       
        
       
        #endregion

        #region Outlets
        IEnumerable<ImportEntity> GetOutlets(ThirdPartyMasterDataQuery myQuery, out bool hasNext)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                IQueryable<tblCostCentre> query = null;
                
                    if (myQuery.SearchTextList.Any())
                    {
                        hasNext = false;
                        List<tblCostCentre> items=new List<tblCostCentre>();
                        foreach (var outletcode in myQuery.SearchTextList)
                        {
                            var found = ctx.tblCostCentre.FirstOrDefault(
                                p =>
                                p.Cost_Centre_Code.Trim().Equals(outletcode.Trim(),
                                                                 StringComparison.CurrentCultureIgnoreCase) ||
                                p.Name.Trim() == outletcode.Trim() && p.CostCentreType == (int)CostCentreType.Outlet);
                            if (found != null)
                                items.Add(found);
                            
                        }
                        if (!items.Any()) return null;
                        return items.Select(Map);

                        
                    }
                
               query= ctx.tblCostCentre.AsQueryable().OrderBy(s => s.IM_DateCreated).Where(s=>s.IM_Status != (int)EntityStatus.Deleted
                    && s.IM_DateLastUpdated > myQuery.From);
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
                hasNext = false;
                return query.Select(Map).ToList();
            }

        }

        private ImportEntity Map(tblCostCentre item)
        {
            var result = new ImportEntity {MasterDataCollective = MasterDataCollective.Outlet.ToString()};
            result.Fields[0] = string.IsNullOrEmpty(item.Cost_Centre_Code)?"":item.Cost_Centre_Code.Trim();
            result.Fields[1] =string.IsNullOrEmpty(item.Name)?"":item.Name.Trim(); 
            result.Fields[2] = item.IM_Status == 1 ? true.ToString() : false.ToString();
            return result;

        }
       
        #endregion
    }
}
