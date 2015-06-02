using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories
{
    public interface IDisbursementNoteRepository  : IDocumentRepository<DisbursementNote> , IDocumentRepositorySaveable<DisbursementNote>
    {
    }
}
