using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core
{
    public class CostCentreRepository : RepositoryBase<CostCentre>, ICostCentreRepository
    {
        private readonly Database db;

        public CostCentreRepository(Database db) : base(db)
        {
            this.db = db;
        }

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
    }
}