using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;

namespace Distributr.Mobile.Routes
{
    public interface IRoutesRepository
    {
        Dictionary<string, Route> GetAllRoutes();
    }
}