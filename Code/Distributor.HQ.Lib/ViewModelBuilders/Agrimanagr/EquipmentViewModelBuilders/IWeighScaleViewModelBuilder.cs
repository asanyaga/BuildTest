using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders
{
    public interface IWeighScaleViewModelBuilder
    {
        IList<WeighScaleViewModel> GetAll(bool inactive = false);
        List<WeighScaleViewModel> SearchWeighScales(string srchParam, bool inactive = false);
        WeighScaleViewModel Get(Guid id);
        void Save(WeighScaleViewModel weighScaleViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        Dictionary<int, string> EquipmentTypes();
        Dictionary<Guid, string> CostCentres();

        QueryResult<WeighScaleViewModel> Query(QueryEquipment query);
        IList<WeighScaleViewModel> QueryList(List<WeighScale> list);
    }
}
