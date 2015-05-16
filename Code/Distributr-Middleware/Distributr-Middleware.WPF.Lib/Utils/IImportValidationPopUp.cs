using System.Collections.Generic;

using Distributr.WSAPI.Lib.Integrations;

namespace Distributr_Middleware.WPF.Lib.Utils
{
  public interface IImportValidationPopUp
    {
      void ShowPopUp(List<StringifiedValidationResult> resultInfos);
    }

    
}
