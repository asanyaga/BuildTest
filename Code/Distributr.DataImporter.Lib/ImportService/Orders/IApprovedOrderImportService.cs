using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.Orders
{
    public interface IApprovedOrderImportService
    {
        IEnumerable<ApprovedOrderImport> Import(string path);
        List<string> Approve(IEnumerable<ApprovedOrderImport> importOrders);
    }
}
