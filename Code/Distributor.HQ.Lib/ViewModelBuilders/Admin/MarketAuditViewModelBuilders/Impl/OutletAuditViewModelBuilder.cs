using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Domain.Master.MarketAudit;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders.Impl
{
   public class OutletAuditViewModelBuilder:IOutletAuditViewModelBuilder 
    {
       IOutletAuditRepository _outletAuditRepository;
       public OutletAuditViewModelBuilder(IOutletAuditRepository outletAuditRepository)
       {
           _outletAuditRepository = outletAuditRepository;
       }
       OutletAuditViewModel Map(OutletAudit outletAudit) { return new OutletAuditViewModel { Id = outletAudit.Id, Question = outletAudit.Question, IsActive = outletAudit._Status == EntityStatus.Active ? true : false }; }
      
        public IList<OutletAuditViewModel> GetAll(bool inactive = false)
        {
            var outletAudit = _outletAuditRepository.GetAll(inactive);
            return outletAudit.Select(s => Map(s)).ToList();
        }

        public List<OutletAuditViewModel> Search(string srchParam, bool inactive = false)
        {
            var outletAudit = _outletAuditRepository.GetAll().Where(s => (s.Question == srchParam));
            return outletAudit.Select(s => Map(s )).ToList();
        }

        public OutletAuditViewModel Get(Guid Id)
        {
            OutletAudit outletAudit = _outletAuditRepository.GetById(Id);
            
            return Map(outletAudit );
        }

        public void Save(OutletAuditViewModel outletAuditViewModel)
        {
            OutletAudit outletAudit = new OutletAudit(outletAuditViewModel.Id) {Question=outletAuditViewModel.Question  };
            _outletAuditRepository.Save(outletAudit);
        }

        public void SetInactive(Guid id)
        {
            OutletAudit outletAudit = _outletAuditRepository.GetById(id);
            _outletAuditRepository.SetInactive(outletAudit );
        }
    }
}
