using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrimanagr.DataImporter.Lib.ImportServices;

namespace Agrimanagr.DataImporter.Lib.Utils
{
   public interface IImportValidationPopUp
    {
       void ShowPopUp(List<ImportValidationResultInfo> resultInfos);
    }
}
