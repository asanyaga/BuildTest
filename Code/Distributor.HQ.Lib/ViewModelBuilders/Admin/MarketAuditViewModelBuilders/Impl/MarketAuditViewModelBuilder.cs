using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders.Impl
{
   public class MarketAuditViewModelBuilder:IMarketAuditViewModelBuilder 
    {
       IMarketAuditRepository _marketAuditRepository;
       public MarketAuditViewModelBuilder(IMarketAuditRepository marketAuditRepository)
       {
           _marketAuditRepository = marketAuditRepository;
       }
       MarketAuditViewModel Map(MarketAudit marketAudit)
       {
           return new MarketAuditViewModel { Id = marketAudit.Id, Question = marketAudit.Question, StartDate = marketAudit.StartDate, EndDate = marketAudit.EndDate, IsActive = marketAudit._Status == EntityStatus.Active ? true : false };
       
       }
        public IList<MarketAuditViewModel> GetAll(bool inactive = false)
        {
            var MarketAudit = _marketAuditRepository.GetAll(inactive);
            return MarketAudit.Select(n => Map(n)).ToList();
        }

        public List<MarketAuditViewModel> Search(string srchParam, bool inactive = false)
        {
            var marketAudit=_marketAuditRepository.GetAll().Where(w=>(w.Question==srchParam ));
            return marketAudit.Select(s => Map(s)).ToList();
        }

        public MarketAuditViewModel Get(Guid Id)
        {
            MarketAudit marketAudit = _marketAuditRepository.GetById(Id);
            return Map(marketAudit );
        }

        public void Save(MarketAuditViewModel marketAuditViewModel)
        {
            MarketAudit marketAudit = new MarketAudit(marketAuditViewModel.Id) 
            {
                Question=marketAuditViewModel.Question,
                StartDate=marketAuditViewModel.StartDate,
                EndDate=marketAuditViewModel.EndDate  
            };
            _marketAuditRepository.Save(marketAudit );
        }

        public void SetInactive(Guid id)
        {
            MarketAudit marketAudit = _marketAuditRepository.GetById(id);
            _marketAuditRepository.SetInactive(marketAudit );
        }

       public QueryResult<MarketAuditViewModel> Query(QueryStandard q)
       {
           var queryResult = _marketAuditRepository.Query(q);

           var result = new QueryResult<MarketAuditViewModel>();

           result.Data = queryResult.Data.Select(Map).ToList();
           result.Count = queryResult.Count;

           return result;
       }
    }
}
