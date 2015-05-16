using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.DataImporter.Lib.ExportEntities;

namespace Distributr.DataImporter.Lib.ImportService.Orders
{
  public  interface IOrderExportService
  {
      void ExportOrders();
      
  }
}
