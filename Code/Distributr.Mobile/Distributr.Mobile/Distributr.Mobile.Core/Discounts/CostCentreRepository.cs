using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core.Discounts
{
    public class CostCentreRepository : BaseRepository<CostCentre>, ICostCentreRepository
    {
        private readonly IOutletRepository outletRepository;

        public CostCentreRepository(Database database, IOutletRepository outletRepository) : base(database)
        {
            this.outletRepository = outletRepository;
        }

        public CostCentre GetById(Guid id, bool includeDeactivated = false)
        {
            return outletRepository.GetById(id, includeDeactivated);
        }

        // 
        // Currently not used on Mobile
        //
        public List<CostCentre> GetByRegionId(Guid regionId, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public CostCentre GetByCode(string code, CostCentreType costcentretype, bool showInActive = false)
        {
            throw new NotImplementedException();
        }

        public void SaveMapping(CostCentreMapping map)
        {
            throw new NotImplementedException();
        }

        public QueryResult<CostCentre> Query(QueryBase q)
        {
            throw new NotImplementedException();
        }
    }
}
