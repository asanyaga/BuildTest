using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
  public  class TransporterViewModelBuilder:ITransporterViewModelBuilder
    {
      ITransporterRepository _transporterRepository;
      ICostCentreFactory _costCentreFactory;
      ICostCentreRepository _costCentreRepository;
      private IHubRepository _hubRepository;
      public TransporterViewModelBuilder(ITransporterRepository transporterRepository, ICostCentreFactory costCentreFactory, ICostCentreRepository costCentreRepository,IHubRepository hubRepository)
      {
          _transporterRepository = transporterRepository;
          _costCentreFactory = costCentreFactory;
          _costCentreRepository = costCentreRepository;
          _hubRepository = hubRepository;
      }
        public IList<TransporterViewModel> GetAll(bool inactive = false)
        {
            var transporter = _transporterRepository.GetAll(inactive).OfType<Transporter>();
            return transporter.Select(n => new TransporterViewModel
                {
                 Id=n.Id,
                 Code = n.CostCentreCode,
                 Name=n.Name,
                 DriverName=n.DriverName,
                 VehicleRegistrationNo=n.VehicleRegistrationNo,
                 isActive = n._Status == EntityStatus.Active ? true : false,
                 CostCentreType = (int)n.CostCentreType,
                 ParentCostCentre = n.ParentCostCentre.Id
                  
                }
                ).ToList();
        }

        public TransporterViewModel Get(Guid Id)
        {
            Transporter trans = (Transporter)_transporterRepository.GetById(Id);
         
                
            return Map(trans);
        }
        TransporterViewModel Map(Transporter transporter)
        {
            return new TransporterViewModel 
            {
             Id=transporter.Id,
             Code = transporter.CostCentreCode,
             Name=transporter.Name,
             DriverName=transporter.DriverName,
             VehicleRegistrationNo=transporter.VehicleRegistrationNo,
             isActive = transporter._Status == EntityStatus.Active ? true : false,
             CostCentreType= (int)transporter.CostCentreType,
             ParentCostCentre = transporter.ParentCostCentre.Id
            };
        }
        public void Save(TransporterViewModel transporter)
        {
            /*Guid parnt = transporter.ParentCostCentre;
            CostCentre parent = _costCentreRepository.GetById(parnt);*/

            Transporter transporterCC = new Transporter(transporter.Id);

            /*if (transporter.Id == Guid.Empty)
            {
                transporter.Id = Guid.NewGuid();
                //CostCentre transporterCC = _costCentreFactory.CreateCostCentre(CostCentreType.Transporter,parent);
                transporterCC = _costCentreFactory.CreateCostCentre(transporterCC.Id, CostCentreType.Transporter, _costCentreRepository.GetById(parnt))
                 as Transporter;
            }
            else
            {
                transporterCC = _costCentreRepository.GetById(transporter.Id) as Transporter;
            }*/
            Guid Id = transporter.Id;
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
            transporterCC.Id = Id;
            transporterCC.Name = transporter.Name;
            transporterCC.CostCentreType = (CostCentreType)transporter.CostCentreType;
            transporterCC.DriverName= transporter.DriverName;
            transporterCC.VehicleRegistrationNo = transporter.VehicleRegistrationNo;
            transporterCC.ParentCostCentre = new CostCentreRef { Id = transporter.ParentCostCentre };
            _transporterRepository.Save(transporterCC);
        }

        public void SetInactive(Guid id)
        {
            Transporter trans =(Transporter) _transporterRepository.GetById(id);
            _transporterRepository.SetInactive(trans);
        }


        public Dictionary<Guid, string> CostCentre()
        {
            //return _transporterRepository.GetAll().OfType<Distributr.Core.Domain.Master.CostCentreEntities.Distributor>()
            //    .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
            return _costCentreRepository.GetAll()

                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

      public void SetAsDeleted(Guid Id)
      {
          Transporter transporter = (Transporter)_transporterRepository.GetById(Id);
          _transporterRepository.SetAsDeleted(transporter);
      }

      public void SetActive(Guid Id)
      {
          Transporter transporter = (Transporter)_transporterRepository.GetById(Id);
          _transporterRepository.SetActive(transporter);
      }
      public Dictionary<Guid, string> ParentCostCentre()
      {
          return _hubRepository.GetAll().OrderBy(n => n.Name).Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
      }

      public Dictionary<int, string> CostCentreTypes()
      {
          var dict = Enum.GetValues(typeof(CostCentreType))
               .Cast<CostCentreType>()
               .Where(n => (int)n == (int)CostCentreType.Hub)
               .ToDictionary(t => (int)t, t => t.ToString());
          return dict;
      }
    }
}
