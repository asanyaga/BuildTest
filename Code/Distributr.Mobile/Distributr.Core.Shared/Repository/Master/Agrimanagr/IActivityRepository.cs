using System;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.Agrimanagr
{
    public interface IActivityRepository 
    {
        QueryActivityResult Query(QueryBase query);
        ActivityDocument GetById(Guid id);
    }
}