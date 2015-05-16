using System.Collections.Generic;
using Distributr.Core.Domain.PG;

namespace Distributr.Core.Repository.PG
{
    public interface IPgRepositoryHelper
    {
        List<ExportClientMember> GetClientMembers();
    }



}