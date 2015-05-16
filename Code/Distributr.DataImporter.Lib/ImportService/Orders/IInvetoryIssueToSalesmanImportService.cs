using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.DataImporter.Lib.ImportEntity;

namespace Distributr.DataImporter.Lib.ImportService.Orders
{
    public interface IInvetoryIssueToSalesmanImportService
    {
       Task<IEnumerable<ImportInvetoryIssueToSalesman>> ImportAsync(string[] files);
       Task<List<string>> IssueInventoryAsync(Dictionary<string, IEnumerable<ImportInvetoryIssueToSalesman>> stockLines);
       Task DampExported(List<string> files);
    }
}
