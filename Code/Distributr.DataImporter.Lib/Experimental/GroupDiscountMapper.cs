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

namespace Distributr.DataImporter.Lib.Experimental
{
    public interface IGroupDiscountMapper : IDataMapper<DiscountGroup>
    {
        DiscountGroup FindByCode(string p);
        void Insert(tblDiscountGroup item);
    }
    public class GroupDiscountMapper : AbstractDataMapper<DiscountGroup>, IGroupDiscountMapper
    {
        protected override string TableName
        {
            get { return "tblDiscountGroup"; }
        }

        public override DiscountGroup Map(dynamic result)
        {
            var dg = new DiscountGroup(result.id)
            {
                Name = result.Name,
                Code = result.Code
            };
            dg._SetDateCreated(result.IM_DateCreated);
            dg._SetDateLastUpdated(result.IM_DateLastUpdated);
            dg._SetStatus((EntityStatus)result.IM_Status);

            return dg;
        }

        public DiscountGroup FindById(Guid id)
        {
            return FindSingle("SELECT * FROM tblDiscountGroup WHERE ID=@ID", new { ID = id });
        }
         public void Insert(DiscountGroup item)
         {
             using (IDbConnection cn = HqConnection)
             {

                 cn.Execute(@"INSERT INTO [dbo].[tblDiscountGroup]
                                                               ([id]
                                                               ,[Code]
                                                               ,[Name]
                                                               ,[IM_DateCreated]
                                                               ,[IM_DateLastUpdated]
                                                               ,[IM_Status]) values (@id,@Code,@Name,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)",
                            new tblDiscountGroup()
                                {
                                    id = item.Id,
                                    Code = item.Code,
                                    IM_DateLastUpdated = DateTime.Now,
                                    IM_DateCreated = DateTime.Now,
                                    IM_Status = (int) EntityStatus.Active
                                });

             }
         }

        public void Insert(tblDiscountGroup item)
        {
            using (IDbConnection cn = HqConnection)
            {

               cn.Execute(@"INSERT INTO [dbo].[tblDiscountGroup]
                                                               ([id]
                                                               ,[Code]
                                                               ,[Name]
                                                               ,[IM_DateCreated]
                                                               ,[IM_DateLastUpdated]
                                                               ,[IM_Status]) values (@id,@Code,@Name,@IM_DateCreated,@IM_DateLastUpdated,@IM_Status)",item);

               

            }
        }

        public void Update(DiscountGroup item)
        {
            throw new NotImplementedException();
        }

        public DiscountGroup FindByCode(string p)
        {
            return FindSingle("SELECT * FROM tblDiscountGroup WHERE code=@code", new { code = p });
        }
    }
}
