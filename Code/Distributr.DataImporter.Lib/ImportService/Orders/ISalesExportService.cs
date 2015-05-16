using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.DataImporter.Lib.ImportService.Orders
{
  public  interface ISalesExportService
    {
      void ExportSales();
      void ExportPayments();
    }
}
