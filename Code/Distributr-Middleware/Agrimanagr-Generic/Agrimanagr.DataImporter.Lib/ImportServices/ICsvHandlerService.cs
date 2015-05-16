using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportEntities;

namespace Agrimanagr.DataImporter.Lib.ImportServices
{
   public interface ICsvHandlerService
    {
       Task<IEnumerable<MasterImportEntity>> ReadFromCsVFileAsync(string filePath);
    }

}
