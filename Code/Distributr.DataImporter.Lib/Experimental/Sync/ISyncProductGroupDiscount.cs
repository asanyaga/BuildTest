using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Documents;
using Dapper;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.DataImporter.Lib.Experimental.Sync
{
    interface ISyncProductGroupDiscount
    {
        long UpdateLocal();
    }

    public class SyncProductGroupDiscount : SyncBase, ISyncProductGroupDiscount
    {
        public long UpdateLocal()
        {
            //1.get distributr
            //2.get outlets with discountgroups
            //3.get productdiscountgroups in the selected discount groups
            //4.get productdiscountgroup items for selected group
            try
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

                    var discountGroupIds =
                        hqDb.Query(
                            "SELECT DISTINCT [Outlet_DiscountGroupId] FROM [dbo].[tblCostCentre] WHERE [ParentCostCentreId]=@distributorId AND [CostCentreType]=5 AND [Outlet_DiscountGroupId] IS NOT NULL",
                            new { distributorId = distributrId }).Select(n => n.Outlet_DiscountGroupId).ToList();

                    using (var localDb = DistributrLocalConnection)
                    {
                        const string insertDPSql =
                            @"INSERT INTO [dbo].[tblProductDiscountGroup](id,DiscountGroup,IM_DateCreated,IM_DateLastUpdated,IM_Status)
                                     values (@id,@DiscountGroup,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)";

                        const string insertDPItemQuery = @"INSERT INTO [dbo].[tblProductDiscountGroupItem]
                                                    (   [id]
                                                       ,[ProductDiscountGroup]
                                                       ,[ProductRef]
                                                       ,[DiscountRate]
                                                       ,[EffectiveDate]
                                                       ,[IM_DateCreated]
                                                       ,[IM_DateLastUpdated]
                                                       ,[IM_Status]
                                                       ,[EndDate])
                                VALUES (@id,
                                         @ProductDiscountGroup,
                                        @ProductRef,
                                        @DiscountRate,
                                        @EffectiveDate,@IM_DateCreated,@IM_DateLastUpdated,
                                       @IM_Status,@EndDate)";

                       
                        var sql = @"SELECT *FROM  [dbo].[tblProductDiscountGroup]  WHERE [IM_Status]=1
                                     SELECT *FROM  [dbo].[tblProductDiscountGroupItem]";

                        var productGroupDiscounts = new List<tblProductDiscountGroup>();
                        var productGroupDiscountItems = new List<tblProductDiscountGroupItem>();
                        using (var multi = hqDb.QueryMultiple(sql))
                        {
                            productGroupDiscounts =
                                multi.Read<tblProductDiscountGroup>().Where(
                                    n => discountGroupIds.Contains(n.DiscountGroup)).ToList();
                            productGroupDiscountItems = multi.Read<tblProductDiscountGroupItem>().ToList();

                        }
                        var productGroupDiscountsIds = productGroupDiscounts.Select(n => n.id).ToList();
                     
                        productGroupDiscountItems =
                            productGroupDiscountItems.Where(
                                n => productGroupDiscountsIds.Contains(n.ProductDiscountGroup)).ToList();
                           

                        localDb.Execute(@"DELETE FROM [dbo].[tblProductDiscountGroupItem]");
                        localDb.Execute(@"DELETE FROM [dbo].[tblProductDiscountGroup]");
                        int row = -1;
                        for (int index = 0; index < productGroupDiscounts.Count; index++)
                        {
                            row = localDb.Execute(insertDPSql, Map(productGroupDiscounts[index]));
                        }
                        if(row >0)
                        {
                            for (int index = 0; index < productGroupDiscountItems.Count; index++)
                            {
                               localDb.Execute(insertDPItemQuery, MapItem(productGroupDiscountItems[index]));
                            }
                        }
                        
                        watch.Stop();
                    }
                }

                return watch.Elapsed.Minutes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        tblProductDiscountGroup Map(tblProductDiscountGroup item)
        {
            return new tblProductDiscountGroup()
            {
                id = item.id,
                DiscountGroup = item.DiscountGroup,
                IM_DateCreated = item.IM_DateCreated,
                IM_DateLastUpdated = DateTime.Now,
                IM_Status =item.IM_Status
            };
           
        }
        tblProductDiscountGroupItem MapItem(tblProductDiscountGroupItem n)
        {
            return new tblProductDiscountGroupItem()
                       {
                           IM_DateLastUpdated = DateTime.Now,
                           IM_Status = n.IM_Status,
                           IM_DateCreated = n.IM_DateCreated,
                           EffectiveDate = n.EffectiveDate,
                           EndDate = n.EndDate,
                           id = n.id,
                           DiscountRate = n.DiscountRate,
                           ProductDiscountGroup =n.ProductDiscountGroup,
                           ProductRef = n.ProductRef
                       };

        }
       
    }
}
