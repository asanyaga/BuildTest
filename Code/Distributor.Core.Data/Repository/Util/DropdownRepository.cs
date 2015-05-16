using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Util;
using Distributr.Core.Utility;

namespace Distributr.Core.Data.Repository.Util
{
    public class DropdownRepository : IDropdownRepository
    {
        CokeDataContext _ctx;

        public DropdownRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public TranferResponse<CustomSelectListItem> GetDistributors(int? skip, int? take, string search = "")
        {
            var result = new  TranferResponse<CustomSelectListItem>();
            IQueryable<tblCostCentre> query = _ctx.tblCostCentre.Where(s => s.CostCentreType == (int)CostCentreType.Distributor).AsQueryable();
           
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Name.Contains(search) ||
                        s.Cost_Centre_Code.Contains(search)
                       );
            }
            query = query.OrderBy(s => s.Name);
            result.RecordCount = query.Count();
            if (skip.HasValue & take.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }


            result.Data = query.ToList().Select(s => new CustomSelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();
            return result;
        }

        public TranferResponse<CustomSelectListItem> GetSaleProduct(int? skip, int? take, string search = "")
        {
            var result = new TranferResponse<CustomSelectListItem>();
            IQueryable<tblProduct> query = _ctx.tblProduct.Where(s => s.DomainTypeId == (int)DomainProductType.Sale && s.IM_Status==(int)EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Description.Contains(search) ||
                        s.ProductCode.Contains(search)
                       );
            }
            query = query.OrderBy(s => s.Description);
            result.RecordCount = query.Count();
            if (skip.HasValue & take.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }


            result.Data = query.ToList().Select(s => new CustomSelectListItem { Text = s.Description, Value = s.id.ToString() }).ToList();
            return result;
        }
    }
}
