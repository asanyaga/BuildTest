using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.DataImporter.Lib.Experimental
{
    public interface IPricingMapper : IDataMapper<ProductPricing>
   {
       bool DropPricings();
       bool Insert(List<ProductPricing> pricings);
       tblProduct GetProduct(string code);

   }

    public class PricingMapper: AbstractDataMapper<ProductPricing>,IPricingMapper
    {
        protected override string TableName
        {
            get { return "tblPricing"; }
        }

        public override ProductPricing Map(dynamic result)
        {
            var pricing = result as tblPricing;
            if (pricing != null)
                return pricing.Map();
            return null;
        }

        public ProductPricing FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Insert(ProductPricing item)
        {
            throw new NotImplementedException();
        }
        
        public void Update(ProductPricing item)
        {
            try
            {
                using (var cn = HqConnection)
                {
                    string updatePricing =
                        @"UPDATE [dbo].[tblPricing] SET [ProductRef]=@ProductRef,[Tier]=@Tier,[IM_DateLastUpdated]=@IM_DateLastUpdated WHERE [id]=@id";
                    string updatePricingItem = @"UPDATE [dbo].[tblPricingItem] SET [Exfactory]=@Exfactory,[SellingPrice]=@SellingPrice,[IM_DateLastUpdated]=@IM_DateLastUpdated,[EffecitiveDate]=@EffecitiveDate WHERE [PricingId]=@PricingId";

                   int res= cn.Execute(updatePricing, Map(item));
                    if(res>0)
                        cn.Execute(updatePricingItem, MapItem(item));
                }
               
            }
            catch (Exception exception)
            {
               
            }
        }

        public bool DropPricings()
        {
            using (IDbConnection cn = HqConnection)
            {
                int result=-1;
                result = cn.Execute("DELETE FROM  tblPricingItem");
                if(result !=-1)
                 return cn.Execute("DELETE FROM  tblPricing") !=-1;

                return false;
            }
        }
       
        public bool Insert(List<ProductPricing> pricings)
        {
            bool completed = false;
            using (IDbConnection cn = HqConnection)
            {
                foreach (var pricing in pricings)
                {
                    const string pricingQuery = @"INSERT INTO [dbo].[tblPricing]
                                   ([id]
                                   ,[ProductRef]
                                   ,[Tier]
                                   ,[IM_DateCreated]
                                   ,[IM_DateLastUpdated]
                                   ,[IM_Status])
                             VALUES
                                   (@id,@ProductRef,@Tier,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)";

                    int success = cn.Execute(pricingQuery, Map(pricing));
                    if(success>=1)
                    {
                        const string pricingItemQuery = @"INSERT INTO [dbo].[tblPricingItem]
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

                        completed = cn.Execute(pricingItemQuery, MapItem(pricing)) !=0;
                    }
                }
                return completed;
            }
        }

        tblPricing Map(ProductPricing item)
        {
            var discountGroup = new tblPricing()
            {
                id = item.Id,
                ProductRef = item.ProductRef.ProductId,
                Tier = item.Tier.Id,
                IM_DateCreated = DateTime.Now,
                IM_DateLastUpdated = DateTime.Now,
                IM_Status = (int)EntityStatus.Active,
               
               
            };
            return discountGroup;
        }
        List<tblPricingItem> MapItem(ProductPricing pricing)
        {
            return pricing.ProductPricingItems.Select(n => new tblPricingItem()
            {
                IM_DateLastUpdated = DateTime.Now,
                IM_Status = (int)EntityStatus.Active,
                IM_DateCreated = DateTime.Now,
                id = n.Id,
                EffecitiveDate = n.EffectiveDate,
                Exfactory = n.ExFactoryRate,
                PricingId = pricing.Id,
                SellingPrice = n.SellingPrice
            }).ToList();
            
        }
    }
}
