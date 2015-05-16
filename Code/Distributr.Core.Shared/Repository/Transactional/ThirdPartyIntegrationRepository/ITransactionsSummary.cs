using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.MasterDataDTO.DataContracts;

namespace Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public interface ITransactionsSummary
    {
        object GetFarmerSummary(Guid farmerId);
        decimal GetFarmerCummulativeWeightDelivered(Guid farmerId);

        List<FarmerSummaryDTO> GetFarmerCummulativeWeightDeliveredByCode(string farmerCode);
        decimal GetFarmerCummulativeWeightDelivered(Guid farmerId, DateTime startDate, DateTime endDate);
    }
}
