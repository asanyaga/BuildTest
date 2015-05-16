using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.Utility;

namespace Distributr.Core.Repository.Master
{
    public interface IClientMasterDataTrackerRepository : IRepositoryMaster<ClientMasterDataTracker>
    {
        bool DoesCostCentreApplicationNeedToSync(Guid costCentreApplicationId, VirtualCityApp vcAppId);
        string[] RepositoryList();
        MasterDataEnvelope GetTableContents(Guid costCentreApplicationId, int entityName);
        TestCostCentreEnvelope GetTestCostCentre();
        //MasterDataModelCommand GetUpdatesSinceDateTime(int costCentreId, DateTime dateSince);
        MasterDataEnvelope GetInventory(Guid costCentreApplicationId);
        ClientMasterDataTracker GetByMasterDataAndEntity(Guid costCentreApplicationId, int entityId);

        MasterDataEnvelope GetPayments(Guid costCentreApplicationId);
        void IntializeApplication(Guid costCentreApplicationId);
        List<FarmerCummulative> FamersCummulative(Guid costCentreApplicationId);
    }
}
