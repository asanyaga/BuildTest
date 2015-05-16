using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;

namespace Distributr.DataImporter.Tests
{
   public class PerformanceAnalyzer
   {
       private CokeDataContext context;

      
       public PerformanceAnalyzer()
       {
           context = new CokeDataContext(ConfigurationManager.AppSettings["cokeconnectionstring"]);
          
       }
       private void GetLineItems(List<Guid> discountGroupIds)
       {
           var values = new StringBuilder();
           values.AppendFormat("{0}", discountGroupIds[0]);
           for (int i = 1; i < discountGroupIds.Count; i++)
               values.AppendFormat(", {0}", discountGroupIds[i]);

           var sql = string.Format(
               "SELECT * FROM [dbo].[tblProductDiscountGroup] WHERE [DiscountGroup] IN ({0})",
               values);
       }
       public long FetchLargeResultSet( )
       {
           var dtos = new List<ProductGroupDiscountDTO>();
           var discountGroups = context.ExecuteStoreQuery<tblProductDiscountGroup>("SELECT *FROM [dbo].[tblProductDiscountGroup])").ToList();

           
           var values = new StringBuilder();
           values.AppendFormat("{0}", discountGroups[0]);
           for (int i = 1; i < discountGroups.Count; i++)
               values.AppendFormat(", {0}", discountGroups[i].id);

           var sql = string.Format(
               "SELECT * FROM [dbo].[tblProductDiscountGroupItem] WHERE [ProductDiscountGroup] IN ({0})",values);
           Stopwatch watch = new Stopwatch();
           watch.Start();
           //var result = context.Set<MyEntity>().SqlQuery(sql).ToList();
           watch.Stop();
           return watch.ElapsedMilliseconds;
       }

       private ProductGroupDiscountDTO Map(tblProductDiscountGroup tbl)
       {
           var dto = new ProductGroupDiscountDTO
           {
               MasterId = tbl.id,
               DateCreated = tbl.IM_DateCreated,
               DateLastUpdated = tbl.IM_DateLastUpdated,
               StatusId = tbl.IM_Status,
               DiscountGroupMasterId = tbl.DiscountGroup,
               GroupDiscountItems = new List<ProductGroupDiscountItemDTO>()
           };
           foreach (var item in tbl.tblProductDiscountGroupItem.Where(n => n.IM_Status == (int)EntityStatus.Active))
           {
               var dtoitem = new ProductGroupDiscountItemDTO
               {
                   MasterId = item.id,
                   DateCreated = item.IM_DateCreated,
                   DateLastUpdated = item.IM_DateLastUpdated,
                   StatusId = item.IM_Status,
                   DiscountRate = item.DiscountRate,
                   EffectiveDate = item.EffectiveDate,
                   EndDate = item.EndDate ?? DateTime.Now,
                   ProductMasterId = item.ProductRef
               };
               dto.GroupDiscountItems.Add(dtoitem);
           }
           return dto;
       }

       public List<ProductGroupDiscountItemDTO> MapItem(tblProductDiscountGroup pdg)
       {
           return
               pdg.tblProductDiscountGroupItem.Where(p => p.IM_Status == (int) EntityStatus.Active).Select(
                   item => new ProductGroupDiscountItemDTO
                               {
                                   MasterId = item.id,
                                   DateCreated = item.IM_DateCreated,
                                   DateLastUpdated = item.IM_DateLastUpdated,
                                   StatusId = item.IM_Status,
                                   DiscountRate = item.DiscountRate,
                                   EffectiveDate = item.EffectiveDate,
                                   EndDate = item.EndDate ?? DateTime.Now,
                                   ProductMasterId = item.ProductRef
                               }).ToList();

       }
   }
   public class ProductGroupDiscountDTO : MasterBaseDTO
   {
       public Guid DiscountGroupMasterId { get; set; }
       public List<ProductGroupDiscountItemDTO> GroupDiscountItems { get; set; }
   }

   public class ProductGroupDiscountItemDTO : MasterBaseDTO
   {
       public Guid ProductMasterId { get; set; }
       public decimal DiscountRate { get; set; }
       public DateTime EffectiveDate { get; set; }
       public DateTime EndDate { get; set; }
   }
   public abstract class MasterBaseDTO
   {
       public Guid MasterId { get; set; }
       public DateTime DateCreated { get; set; }
       public DateTime DateLastUpdated { get; set; }
       public int StatusId { get; set; }
   }
}
