using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
    public interface ICostCentreRepository : IRepositoryMaster<CostCentre>
    {
        List<CostCentre> GetByRegionId(Guid regionId, bool includeDeactivated = false);
        CostCentre GetByCode(string code,CostCentreType costcentretype,bool showInActive=false);
        void SaveMapping(CostCentreMapping map);

        QueryResult<CostCentre> Query(QueryBase q);
       
        
    }

    public interface ICostCentreRefRepository
    {
        List<CostCentreDetailRef> GetDistributorAndSalesmenAll();
    }
}
