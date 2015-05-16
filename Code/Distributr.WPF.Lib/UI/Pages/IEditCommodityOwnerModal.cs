using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntity;

namespace Distributr.WPF.Lib.UI.Pages
{
    public interface IEditCommodityOwnerModal
    {
        bool AddCommodityOwner(CommodityOwner commodityOwnerToEdit, out CommodityOwner commodityOwnerReturned);
    }
}
