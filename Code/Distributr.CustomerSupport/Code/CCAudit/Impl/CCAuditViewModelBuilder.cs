using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.MongoDB.Repository;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.CustomerSupport.Code.CCAudit.Impl
{
    public class CCAuditViewModelBuilder : ICCAuditViewModelBuilder
    {
        private ICostCentreRefRepository _costCentreRefRepository;
        private ICostCentreRepository _costCentreRepository;
        private ICCAuditRepository _ccAuditRepository;

        public CCAuditViewModelBuilder(ICostCentreRefRepository costCentreRefRepository, ICostCentreRepository costCentreRepository, ICCAuditRepository ccAuditRepository)
        {
            _costCentreRefRepository = costCentreRefRepository;
            _costCentreRepository = costCentreRepository;
            _ccAuditRepository = ccAuditRepository;
        }


        public CCAuditSummaryViewModel GetSummary(int dayOfYear, int year)
        {
            var costCentres = _costCentreRefRepository.GetDistributorAndSalesmenAll();
            var summary = _ccAuditRepository.GetDailySummary(dayOfYear, year);

            var jsummary = from c in costCentres
                           join s in summary on c.Id equals s.CostCentreId into ccsummary
                           from cp in ccsummary.DefaultIfEmpty()
                           select new CCAuditSummaryViewModel.CCSummary
                                      {
                                          CCId = c.Id.ToString(),
                                          CCName = c.Name,
                                          CCType = c.CostCentreType.ToString(),
                                          NoHits = cp == null ? 0 : cp.NoHits
                                      };
            return new CCAuditSummaryViewModel
                       {
                           Items = jsummary
                           .OrderBy(n => n.CCType)
                           .ThenBy(n => n.CCName)
                           .ToList()
                           ,
                           Date = new DateTime(year, 1,1).AddDays(dayOfYear -1).ToString("dd-MMM-yyyy")
                       };

        }

        public CCAuditHitSummaryViewModel GetCostCentreHitSummary(int dayofYear, int year, Guid costCentreId, string selectedAction)
        {
            var costCentre = _costCentreRepository.GetById(costCentreId);
            var items = _ccAuditRepository.GetByCC(costCentreId, dayofYear, year);
            var dt = new DateTime(year, 1, 1).AddDays(dayofYear - 1);
            return new CCAuditHitSummaryViewModel(dt, costCentre, items,selectedAction);
        }
    }
}