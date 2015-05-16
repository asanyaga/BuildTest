using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;

namespace Distributr.DataImporter.Lib.Experimental.Sync
{
   public interface ISyncProductPricing
   {
       long UpdateLocalDb();
   }

   public class SyncProductPricing : SyncBase, ISyncProductPricing
    {
       public long UpdateLocalDb()
       {
           Stopwatch watch = new Stopwatch();
           watch.Start();
           using (var hqDb = HqConnection)
           {
               var distributr =
                   hqDb.Query<tblCostCentre>(
                       "SELECT *FROM [dbo].[tblCostCentre] where [Cost_Centre_Code]=@costcentreCode",
                       new {costcentreCode = 87878}).FirstOrDefault();
               var distributrId = Guid.Empty;
               if (distributr != null)
                   distributrId = distributr.Id;

               var pricingQuery = @"SELECT *
                                  FROM dbo.tblPricing 
                                  WHERE Tier in (SELECT distinct SpecialPricingTierId  from tblCostCentre outlet where outlet.CostCentreType = 5 
                                  AND outlet.ParentCostCentreId=@parentCostCentreId
                                  UNION 
                                  SELECT distinct Tier_Id from tblCostCentre outlet where outlet.CostCentreType = 5 AND outlet.ParentCostCentreId=@parentCostCentreId)";

               var pricingItemQuery = @"SELECT * FROM dbo.tblPricingItem";

               var pricings = hqDb.Query<tblPricing>(pricingQuery, new {parentCostCentreId = distributrId}).ToList();
                var pricingIds = pricings.Select(n => n.id).ToList();
               var pricingItems =
                   hqDb.Query<tblPricingItem>(pricingItemQuery).Where(n => pricingIds.Contains(n.PricingId)).ToList();

                  DropPricings();
                 

                 
                  const string pricingInsertQuery = @"INSERT INTO [dbo].[tblPricing]
                                   ([id]
                                   ,[ProductRef]
                                   ,[Tier]
                                   ,[IM_DateCreated]
                                   ,[IM_DateLastUpdated]
                                   ,[IM_Status])
                             VALUES
                                   (@id,@ProductRef,@Tier,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)";

                  const string pricingItemInserQuery = @"INSERT INTO [dbo].[tblPricingItem]
                                       ([id]
                                       ,[PricingId]
                                       ,[Exfactory]
                                       ,[SellingPrice]
                                       ,[EffecitiveDate]
                                       ,[IM_DateCreated]
                                       ,[IM_DateLastUpdated]
                                       ,[IM_Status])
                                        VALUES 
                                       (@id,@PricingId,@Exfactory,@SellingPrice,@EffecitiveDate,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)";

                  using (var localDb = DistributrLocalConnection)
                  {
                      int row = -1;
                      for (int index = 0; index < pricings.Count; index++)
                      {
                          row = localDb.Execute(pricingInsertQuery, Map(pricings[index]));
                      }
                      if (row > 0)
                      {
                          for (int index = 0; index < pricingItems.Count; index++)
                          {
                              localDb.Execute(pricingItemInserQuery, MapItem(pricingItems[index]));
                          }
                          watch.Stop();
                      }
                  }
           }
           return watch.ElapsedMilliseconds;
       }

       tblPricing Map(tblPricing item)
        {
            var discountGroup = new tblPricing()
            {
                id = item.id,
                ProductRef = item.ProductRef,
                Tier = item.Tier,
                IM_DateCreated =item.IM_DateCreated,
                IM_DateLastUpdated = DateTime.Now,
                IM_Status = item.IM_Status,
               
               
            };
            return discountGroup;
        }
       tblPricingItem MapItem(tblPricingItem n)
       {
           return new tblPricingItem()
                      {
                          IM_DateLastUpdated = DateTime.Now,
                          IM_Status = (int) EntityStatus.Active,
                          IM_DateCreated = n.IM_DateCreated,
                          id = n.id,
                          EffecitiveDate = n.EffecitiveDate,
                          Exfactory = n.Exfactory,
                          PricingId = n.PricingId,
                          SellingPrice = n.SellingPrice
                      };
       }

       bool DropPricings()
       {
           using (var cn = DistributrLocalConnection)
           {
               int result = -1;
               result = cn.Execute("DELETE FROM  tblPricingItem");
               if (result != -1)
                   return cn.Execute("DELETE FROM  tblPricing") != -1;

               return false;
           }
       }
    }
}
