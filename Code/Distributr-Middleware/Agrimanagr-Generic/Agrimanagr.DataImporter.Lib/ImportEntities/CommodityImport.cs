using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrimanagr.DataImporter.Lib.ImportEntities
{
    public abstract class ImportItemBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CommodityTypeImport : ImportItemBase
    {

    }

    public class CommodityImport : ImportItemBase
    {
        public string CommodityTypeCode { get; set; }

    }

    public class CommodityOwnerTypeImport : ImportItemBase
    {
    }

    public class CommoditySupplierImport : ImportItemBase
    {
        public int CommoditySupplierType { get; set; }
        public DateTime JoinDate { get; set; }
        public string AccountNo { get; set; }
        public string PinNo { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
    }

}
