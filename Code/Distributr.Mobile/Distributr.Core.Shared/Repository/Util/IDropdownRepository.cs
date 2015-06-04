using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility;

namespace Distributr.Core.Repository.Util
{
   public interface IDropdownRepository
   {
      TranferResponse<CustomSelectListItem> GetDistributors(int? skip,int? take,string search="");
      TranferResponse<CustomSelectListItem> GetSaleProduct(int? skip, int? take, string search = "");
   }
}
