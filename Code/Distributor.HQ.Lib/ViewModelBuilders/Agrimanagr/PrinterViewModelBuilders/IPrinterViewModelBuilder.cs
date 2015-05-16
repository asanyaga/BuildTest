using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.PrinterViewModelBuilders
{
    public interface IPrinterViewModelBuilder
    {
        IList<PrinterViewModel> GetAll(bool inactive = false);
        List<PrinterViewModel> SearchPrinters(string srchParam, bool inactive = false);
        PrinterViewModel Get(Guid id);
        void Save(PrinterViewModel printerViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<int, string> EquipmentTypes();
        Dictionary<Guid, string> CostCentres();

        QueryResult<PrinterViewModel> Query(QueryEquipment query);
        IList<PrinterViewModel> QueryList(List<Printer> list);
    }
}
