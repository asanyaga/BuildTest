using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.OutletEntities;

namespace Distributr.Core.Factory.Master.Impl.OutletFactory
{
    public class OutletCategoryFactory: IOutletCategoryFactory
    {
        public OutletCategory CreateOutletCategory(string name)
        {
            if (name == null || name.Trim() == "")
                throw new ArgumentException("Name cannot be null or blank");
            OutletCategory outletcategory = new OutletCategory(0)
            {
                Name = name
            };
            return outletcategory;
        }
    }
}
