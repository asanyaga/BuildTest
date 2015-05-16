using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.DataImporter.Lib.ImportService;

namespace Distributr.DataImporter.Lib.ViewModel
{
   public interface IImportValidationPopUp
   {
       void ShowPopUp(List<ImportValidationResultInfo> resultInfos);
   }

    public interface IShowWorkingFolderPopUp
    {
        void ShowPopUp();
    }
}
