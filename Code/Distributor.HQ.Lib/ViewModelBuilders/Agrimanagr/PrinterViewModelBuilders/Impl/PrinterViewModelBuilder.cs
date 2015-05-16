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
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.PrinterViewModelBuilders.Impl
{
    public class PrinterViewModelBuilder : IPrinterViewModelBuilder
    {
        private IEquipmentRepository _equipmentRepository;
        private IHubRepository _hubRepository;

        public PrinterViewModelBuilder(IEquipmentRepository equipmentRepository, IHubRepository hubRepository)
        {
            _equipmentRepository = equipmentRepository;
            _hubRepository = hubRepository;
        }


        #region Implementation of IPrinterViewModelBuilder

        public IList<PrinterViewModel> GetAll(bool inactive = false)
        {
            return _equipmentRepository.GetAll(inactive).Where(p => p.EquipmentType == EquipmentType.Printer).Select(
                    p => Map((Printer)p)).ToList();
           
        }
        
        PrinterViewModel Map(Printer printer)
        {
            PrinterViewModel vm = new PrinterViewModel();
            vm.Id = printer.Id;
            vm.Code = printer.Code;
            vm.EquipmentNumber = printer.EquipmentNumber;
            vm.Name = printer.Name;
            vm.Make = printer.Make;
            vm.Model = printer.Model;
            vm.EquipmentType = (int)printer.EquipmentType;
            vm.Description = printer.Description;
            vm.CostCentre = printer.CostCentre.Id;
            vm.IsActive = (int)printer._Status;

            return vm;
        }
        
        public List<PrinterViewModel> SearchPrinters(string srchParam, bool inactive = false)
        {
            var printerModels =
                _equipmentRepository.GetAll(inactive).OfType<Printer>().Where(
                    p => (p.Name.ToLower().StartsWith(srchParam.ToLower())));

            return printerModels.Select(p => Map(p)).ToList();

        }

        public PrinterViewModel Get(Guid id)
        {
            Printer printer = (Printer) _equipmentRepository.GetById(id);

            if (printer == null) return null;

            return Map(printer);
        }

        public void Save(PrinterViewModel printerViewModel)
        {
            Printer printer = new Printer(printerViewModel.Id);
            printer.Name = printerViewModel.Name;
            printer.Code = printerViewModel.Code;
            printer.EquipmentNumber = printerViewModel.EquipmentNumber;
            printer.Make = printerViewModel.Make;
            printer.Model = printerViewModel.Model;
            printer.EquipmentType = (EquipmentType)printerViewModel.EquipmentType;
            printer.Description = printerViewModel.Description;
            printer.CostCentre = (Hub)_hubRepository.GetById(printerViewModel.CostCentre);
            printer._Status = EntityStatus.Active;

            _equipmentRepository.Save(printer);
        }

        public void SetInactive(Guid Id)
        {
            Equipment equipment = _equipmentRepository.GetById(Id);
            _equipmentRepository.SetInactive(equipment);

        }

        public void SetActive(Guid Id)
        {
            Equipment equipment = _equipmentRepository.GetById(Id);
            _equipmentRepository.SetActive(equipment);
        }

        public void SetAsDeleted(Guid Id)
        {
            Equipment equipment = _equipmentRepository.GetById(Id);
            _equipmentRepository.SetAsDeleted(equipment);
        }

        public Dictionary<int, string> EquipmentTypes()
        {
            return Enum.GetValues(typeof (EquipmentType)).Cast<EquipmentType>().
                Where(p => (int)p == (int)EquipmentType.Printer).ToDictionary(t => (int)t, t => t.ToString());
        }

        public Dictionary<Guid, string> CostCentres()
        {
            return _hubRepository.GetAll().OrderBy(c => c.Name).Where(n => n.CostCentreType == CostCentreType.Hub)
                .Select(h => new {h.Id, h.Name}).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public QueryResult<PrinterViewModel> Query(QueryEquipment query)
        {
            var queryResults = _equipmentRepository.Query(query);

            var results = new QueryResult<PrinterViewModel>();
            results.Data = queryResults.Data.OfType<Printer>().Select(Map).ToList();
            results.Count = queryResults.Count;

            return results;
        }

        public IList<PrinterViewModel> QueryList(List<Printer> list)
        {
            return list.Select(Map).ToList();
        }
    }
        #endregion
}
