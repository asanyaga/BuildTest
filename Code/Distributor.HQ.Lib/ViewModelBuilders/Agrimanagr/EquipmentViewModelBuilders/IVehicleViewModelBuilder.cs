using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders
{
   public interface IVehicleViewModelBuilder
    {
        IList<VehicleViewModel> GetAll(bool inactive = false);
        List<VehicleViewModel> SearchVehicles(string srchParam, bool inactive = false);
        VehicleViewModel Get(Guid id);
        void Save(VehicleViewModel VehicleViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<Guid, string> Hubs();

       QueryResult<VehicleViewModel> Query(QueryEquipment query);
       IList<VehicleViewModel> QueryList(List<Vehicle> data);
    }
}
