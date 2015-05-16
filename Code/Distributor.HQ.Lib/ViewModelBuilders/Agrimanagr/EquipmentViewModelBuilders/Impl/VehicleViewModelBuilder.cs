using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders.Impl
{
    public class VehicleViewModelBuilder : IVehicleViewModelBuilder
   {
       private readonly IVehicleRepository _vehicleRepository;
       private readonly IHubRepository _hubRepository;

   
       public VehicleViewModelBuilder(IVehicleRepository vehicleRepository, IHubRepository hubRepository)
       {
           _vehicleRepository = vehicleRepository;
           _hubRepository = hubRepository;
       }


        public IList<VehicleViewModel> GetAll(bool inactive = false)
        {
            return _vehicleRepository.GetAll(inactive).Select(Map).ToList();
        }

        public List<VehicleViewModel> SearchVehicles(string srchParam, bool inactive = false)
        {
            var items = _vehicleRepository.GetAll(inactive)
                .Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower()))
                ||n.Code.ToLower().StartsWith(srchParam.ToLower())
                ||n.EquipmentNumber.ToLower().StartsWith(srchParam.ToLower()));
            return items.Select(n => Map(n)).ToList();
        }

        public VehicleViewModel Get(Guid id)
        {
            var item = _vehicleRepository.GetById(id);
            if (item == null) return null;
            return Map(item);
        }

        public void Save(VehicleViewModel vehicleViewModel)
        {
            Vehicle vehicle = Construct(vehicleViewModel);
            _vehicleRepository.Save(vehicle);
        }

        public void SetInactive(Guid id)
        {
            var vehicle = _vehicleRepository.GetById(id);
            if(vehicle!=null)
                _vehicleRepository.SetInactive(vehicle);
        }

        public void SetActive(Guid id)
        {
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle != null)
                _vehicleRepository.SetActive(vehicle);
        }

        public void SetAsDeleted(Guid id)
        {
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle != null)
                _vehicleRepository.SetAsDeleted(vehicle);
        }

        public Dictionary<Guid, string> Hubs()
        {
            var hubs = new Dictionary<Guid, string>();
              
            var tmp = _hubRepository.GetAll().OrderBy(n => n.Name)
                .Select(r => new {r.Id, r.Name}).ToList().ToDictionary(d => d.Id, d => d.Name).ToList();
            foreach (var pair in tmp)
            {
                hubs.Add(pair.Key,pair.Value);
            }
          
            return hubs;
        }

        public QueryResult<VehicleViewModel> Query(QueryEquipment query)
        {
            var queryResult = _vehicleRepository.Query(query);

            var result = new QueryResult<VehicleViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<VehicleViewModel> QueryList(List<Vehicle> data)
        {
            return data.Select(Map).ToList();
        }

        #region Utils

        private VehicleViewModel Map(Vehicle vehicle)
        {
            return new VehicleViewModel()
                         {
                             Id = vehicle.Id,
                             Code = vehicle.Code,
                             Name = vehicle.Name,
                             Description = vehicle.Description,
                             HubId = vehicle.CostCentre.Id,
                             Hub = vehicle.CostCentre.Name,
                             Model = vehicle.Model,
                             Make = vehicle.Make,
                             RegistrationNumber = vehicle.EquipmentNumber,
                             Status =vehicle._Status
                         };
        }

        private Vehicle Construct(VehicleViewModel vm)
        {
            if (vm.Id == Guid.Empty)
                vm.Id = Guid.NewGuid();
            return new Vehicle(vm.Id)
                       {
                           Code = vm.Code,
                           Description = vm.Description,
                           CostCentre = _hubRepository.GetById(vm.HubId) as Hub,
                           EquipmentNumber = vm.RegistrationNumber,
                           Make = vm.Make,
                           Model = vm.Model,
                           Name = vm.Name,
                           EquipmentType = EquipmentType.Vehicle,
                           _Status =  vm.Status,
                          
                       };
        }
        #endregion
   }
}
