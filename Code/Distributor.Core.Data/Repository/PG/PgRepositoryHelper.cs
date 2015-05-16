using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.PG;
using Distributr.Core.Repository.PG;

namespace Distributr.Core.Data.Repository.PG
{
   public class PgRepositoryHelper : IPgRepositoryHelper
    {
       CokeDataContext _ctx;

       public PgRepositoryHelper(CokeDataContext ctx)
       {
           _ctx = ctx;
       }

       public List<ExportClientMember> GetClientMembers()
       {
           return _ctx.tblCostCentre
               .Where(
                   s =>
                   s.CostCentreType == (int) CostCentreType.Outlet ||
                   s.CostCentreType == (int) CostCentreType.CommoditySupplier).ToList()
               .Select(
                   s =>
                   new ExportClientMember
                       {
                           Name = s.Name,
                           ExternalId = s.Id.ToString(),
                           Code = s.Cost_Centre_Code,
                           MemberType = (int) s.CostCentreType
                       })
               .ToList();
       }
    }
}
