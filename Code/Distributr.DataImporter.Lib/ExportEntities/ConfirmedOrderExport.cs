using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ExportEntities
{
    //OrderReference,OrderDate,SalesmanCode,OutletCode,ShiptoAddressCode,ProductCode,ApprovedQuantity,DistributrCode
   public class ConfirmedOrderExport
    {
       public string OrderReference { get; set; }
       
        public string OrderDate { get; set; }
       
        public string SalesmanCode { get; set; }
       
        public string OutletCode { get; set; }
        public string ShiptoAddressCode { get; set; }
       
        public string ProductCode { get; set; }
       
        public decimal ApprovableQuantity { get; set; }
       
        public string DistributrCode { get; set; }
    }
}
