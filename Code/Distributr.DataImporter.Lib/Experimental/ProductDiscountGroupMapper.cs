using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.WSAPI.Lib.DTOModels.MasterDataDTO.Product;

namespace Distributr.DataImporter.Lib.Experimental
{
    public interface IProductDiscountGroupMapper : IDataMapper<ProductGroupDiscount>
    {
        bool DropProductDiscountGroups();
        tblProduct GetProduct(string code);
        ProductGroupDiscountDTO GetProductGroupDiscount(Guid discountgroupId, Guid productId);
        int TestUpdate(List<tblProductDiscountGroupItem> upDateObjects);
        int TestUpdate(StringBuilder query);
        string GetQuery(Guid discountgroupId, Guid productId);
        void Insert(List<ProductGroupDiscount> groupDiscounts);
        void Update(tblProductDiscountGroupItem item);

    }

    public class ProductDiscountGroupMapper : AbstractDataMapper<ProductGroupDiscount>, IProductDiscountGroupMapper
    {
        private IGroupDiscountMapper _groupDiscountMapper;

        public ProductDiscountGroupMapper(IGroupDiscountMapper groupDiscountMapper)
        {
            _groupDiscountMapper = groupDiscountMapper;
        }

        protected override string TableName
        {
            get { return "tblProductDiscountGroup"; }
        }

        public override ProductGroupDiscount Map(dynamic result)
        {
            var item = new ProductGroupDiscount(result.id)
            {
                GroupDiscount = _groupDiscountMapper.FindById(result.DiscountGroup),
             
            };

            return item;
        }
        public bool DropProductDiscountGroups()
        {
            using (IDbConnection cn = HqConnection)
            {
                var result = cn.Execute("DELETE FROM [dbo].[tblProductDiscountGroupItem]");
                if (result != -1)
                    return cn.Execute("DELETE FROM  [dbo].[tblProductDiscountGroup]") != -1;

                return false;
            }
        }
      

        public ProductGroupDiscount FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Insert(ProductGroupDiscount item)
        {
            throw new NotImplementedException();
        }
        public void Insert(List<ProductGroupDiscount> groupDiscounts)
        {
            using (IDbConnection cn = HqConnection)
            {
                foreach (var p in groupDiscounts)
                {
                    const string sql =
                        @"INSERT INTO [dbo].[tblProductDiscountGroup](id,DiscountGroup,IM_DateCreated,IM_DateLastUpdated,IM_Status)
                                     values (@id,@DiscountGroup,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)";


                    int success = cn.Execute(sql, Map(p));

                    if (success != 0)
                    {
                        foreach (var groupItem in MapItem(p))
                        {
                            const string query = @"INSERT INTO [dbo].[tblProductDiscountGroupItem]
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
                            cn.Execute(query, groupItem);

                        }

                    }
                }
            }
        }
        public void Update(tblProductDiscountGroupItem item)
        {
            try
            {
                using (var cn = HqConnection)
                {
                   var query= string.Format(@"DELETE FROM [dbo].[tblProductDiscountGroupItem] WHERE [ProductDiscountGroup]='{0}';
                                                INSERT INTO [dbo].[tblProductDiscountGroupItem]
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
                                                                                       @IM_Status,@EndDate)",item.ProductDiscountGroup);
                   
                    cn.Execute(query, item);

                }

            }
            catch (Exception exception)
            {

            }
        }
        public void Update(ProductGroupDiscount item)
        {
            try
            {
                using (var cn = HqConnection)
                {
                    string updateProductGroupDiscountQuery =
                        @"UPDATE [dbo].[tblProductDiscountGroup] SET [DiscountGroup]=@DiscountGroup,[IM_DateLastUpdated]=@IM_DateLastUpdated WHERE [id]=@id";
                    string updateProductGroupDiscountItem = @"UPDATE [dbo].[tblProductDiscountGroupItem] SET [ProductRef]=@ProductRef,[IM_DateLastUpdated]=@IM_DateLastUpdated,[EffectiveDate]=@EffectiveDate,[DiscountRate]=@DiscountRate WHERE [ProductDiscountGroup]= @ProductDiscountGroup";

                    int res = cn.Execute(updateProductGroupDiscountQuery, Map(item));
                    if (res > 0)
                        cn.Execute(updateProductGroupDiscountItem, MapItem(item));
                }

            }
            catch (Exception exception)
            {

            }
        }

        tblProductDiscountGroup Map(ProductGroupDiscount item)
        {
            var discountGroup = new tblProductDiscountGroup()
            {
                id = item.Id,
                DiscountGroup = item.GroupDiscount.Id,
                IM_DateCreated = DateTime.Now,
                IM_DateLastUpdated = DateTime.Now,
                IM_Status = (int)EntityStatus.Active
            };
            return discountGroup;
        }
        IEnumerable<tblProductDiscountGroupItem> MapItem(ProductGroupDiscount discountGroup)
        {
            return discountGroup.GroupDiscountItems.Select(n => new tblProductDiscountGroupItem()
                                                                    {
                                                                        IM_DateLastUpdated = DateTime.Now,
                                                                        IM_Status = (int) EntityStatus.Active,
                                                                        IM_DateCreated = DateTime.Now,
                                                                        EffectiveDate = n.EffectiveDate,
                                                                        EndDate = n.EndDate,
                                                                        id = n.Id,
                                                                        DiscountRate = n.DiscountRate,
                                                                        ProductDiscountGroup = discountGroup.Id,
                                                                        ProductRef = n.Product.ProductId
                                                                    }).ToList();

        }

        public ProductGroupDiscountDTO GetProductGroupDiscount(Guid discountgroupId,Guid productId)
        {
           try
            {
                using (var cn = HqConnection)
                {
                    var item = cn.Query<ProductGroupDiscountDTO>(GetQuery(discountgroupId,productId)).FirstOrDefault();
                    return item;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public string GetQuery(Guid discountgroupId, Guid productId)
        {
            return string.Format(@"SELECT  p.[Id] as MasterId
                              ,p.[DiscountGroup] as DiscountGroupMasterId
                              ,p.[IM_DateCreated] as DateCreated
                              ,p.[IM_DateLastUpdated] as DateLastUpdated
                              ,p.[IM_Status]  as StatusId 
                              ,pItem.id as LineItemId
                              ,pItem.DiscountRate as DiscountRate
                              ,pItem.EffectiveDate as EffectiveDate
                              ,pItem.EndDate as EndDate
                              ,pItem.ProductRef as ProductMasterId
                               FROM   [tblProductDiscountGroup] p
                                           OUTER APPLY (SELECT TOP 1 *
                                            FROM   [tblProductDiscountGroupItem] pItem
                                            WHERE  pItem.ProductDiscountGroup = p.id and pItem.ProductRef='{0}'
                                            ORDER  BY pItem.EffectiveDate desc) pItem
                               WHERE p.DiscountGroup='{1}' and p.IM_Status =1;", productId, discountgroupId);
        }

        public int TestUpdate(List<tblProductDiscountGroupItem> upDateObjects)
        {
            var bulkInsert = new StringBuilder();
            foreach (var item in upDateObjects)
            {
                bulkInsert.AppendFormat(@"UPDATE [tblProductDiscountGroupItem] 
                                SET 
                                DiscountRate = '{0}',
                                EffectiveDate = '{1}',
                                IM_DateLastUpdated ='{2}',
                                EndDate='{3}'   
                          WHERE ProductDiscountGroup='{4}' AND ProductRef='{5}'AND CAST(DiscountRate as numeric(10,2))!='{6}'",
                                        item.DiscountRate, item.EffectiveDate,item.IM_DateLastUpdated, item.EndDate,
                                        item.ProductDiscountGroup, item.ProductRef, item.DiscountRate.ToString("0.00")
                    );
            }
            try
            {
                using (var cn = HqConnection)
                {
                    int affectedRows = cn.Execute(bulkInsert.ToString());
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public int TestUpdate(StringBuilder query)
        {
            try
            {
                using (var cn = HqConnection)
                {
                    int affectedRows = cn.Execute(query.ToString());
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

    }

}
