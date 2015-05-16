using System;
using System.Collections.Generic;
using Integration.Cussons.WPF.Lib.ImportService;
using Integration.Cussons.WPF.Lib.MasterDataImportService;

namespace Integration.Cussons.WPF.Lib.Utils
{
  public interface IImportValidationPopUp
    {
      void ShowPopUp(List<ImportValidationResultInfo> resultInfos);
    }

    public interface IAdjustInventoryWindow
    {
      decimal ShowAdjustDialog();
    }
}
