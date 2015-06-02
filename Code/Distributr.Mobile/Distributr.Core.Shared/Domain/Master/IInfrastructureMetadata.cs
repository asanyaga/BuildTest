using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master
{
    public interface IInfrastructureMetadata
    {
        DateTime _DateCreated { get; }

        DateTime _DateLastUpdated { get; }

        EntityStatus _Status { get; }
    }
}
