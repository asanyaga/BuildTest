using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WSAPI.Lib.Integrations.MasterData
{
    public interface IImportMasterDataService
    {
        Task<MasterDataImportResponse> ValidateAsync(IEnumerable<ImportEntity> imports);
        Task<MasterDataImportResponse> ValidateAndSaveAsync(IEnumerable<ImportEntity> imports);
    }

    public abstract class MasterDataImportServiceBase
    {
        protected List<ImportValidationResultInfo> validationResultInfos;
        protected MasterDataImportServiceBase()
        {
            validationResultInfos=new List<ImportValidationResultInfo>();
        }

        protected tblCostCentre GetDistributr(string distributrCode)
        {
            using (var ctx = new CokeDataContext(Con))
            {
                tblCostCentre costCentre = null;
                if (!string.IsNullOrEmpty(distributrCode))
                    costCentre = ctx
                        .tblCostCentre.FirstOrDefault(
                            p => p.Cost_Centre_Code != null &&
                            p.Cost_Centre_Code.ToLower() == distributrCode.ToLower() && p.CostCentreType == (int)CostCentreType.Distributor);

                if (costCentre == null && ctx.tblCostCentre.Count(p => p.CostCentreType == (int)CostCentreType.Distributor) == 1)
                    costCentre = ctx.tblCostCentre.FirstOrDefault(p => p.CostCentreType == (int)CostCentreType.Distributor);
                return costCentre;
            }
        }
        protected DateTime GetDatetime(string date,bool addOneYear=false)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                if (!string.IsNullOrEmpty(date))
                {
                    dateTime = Convert.ToDateTime(date);

                }
                if (addOneYear)
                    return dateTime.AddMonths(12);

                return dateTime;
            }
            catch
            {
                if(addOneYear)
                return DateTime.Now.AddMonths(12);
                return DateTime.Now;
            }

        }
        protected decimal GetDecimal(string rate)
        {
            try
            {
                if (string.IsNullOrEmpty(rate)) return 0m;
                return Convert.ToDecimal(rate);
            }
            catch
            {
                return 0m;
            }
        }
      protected  string SetFieldValue(string[] dataRow, int index)
        {
            index = index - 1;
            return (dataRow.Length-1 < index || string.IsNullOrEmpty(dataRow[index])) ? "" : dataRow[index];
        }
      protected tblProduct GetProduct(string itemName)
      {
          using (var ctx = new CokeDataContext(Con))
          {
              tblProduct pitem = null;
              if (!string.IsNullOrEmpty(itemName))
                  pitem = ctx.tblProduct.FirstOrDefault(p => p.ProductCode != null && p.ProductCode.ToLower() == itemName.ToLower());
              return pitem;
          }
      }
      protected tblPricingTier GetPricingTier(string itemName)
      {
          using (var ctx = new CokeDataContext(Con))
          {
              tblPricingTier pitem = null;
              if (!string.IsNullOrEmpty(itemName))
                  pitem =
                      ctx.tblPricingTier.FirstOrDefault(
                          p =>
                          p.Name.ToLower() == itemName.ToLower() ||
                          p.Code != null && p.Code.ToLower() == itemName.ToLower());
              return pitem;
          }
      }
      protected string Con
      {
          get { return ConfigurationManager.AppSettings["cokeconnectionstring"]; }
      }
    }
}
