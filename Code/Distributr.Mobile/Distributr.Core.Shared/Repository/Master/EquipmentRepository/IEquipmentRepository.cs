using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.EquipmentRepository
{
    public interface IEquipmentRepository : IRepositoryMaster<Equipment>
    {
        QueryResult<Equipment> Query(QueryEquipment query);
    }
    public interface IContainerTypeRepository : IRepositoryMaster<ContainerType>
    {
        QueryResult<ContainerType> Query(QueryStandard query);
    }
}
