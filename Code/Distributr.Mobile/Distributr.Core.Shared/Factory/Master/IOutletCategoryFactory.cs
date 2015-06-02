using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.OutletEntities;

namespace Distributr.Core.Factory.Master
{
    //NOT REQUIRED TAKE OUT
    public interface IOutletCategoryFactory
    {
        OutletCategory CreateOutletCategory(string name);
    }
}
