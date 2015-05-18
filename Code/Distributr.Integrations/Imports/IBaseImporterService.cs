using System.Collections.Generic;
using Distributr.Import.Entities;

namespace Distributr.Integrations.Imports
{
    public interface IBaseImportService<T> where T : class
    {
        ImportResponse Save(List<T> imports);
        ImportResponse Delete(List<string> deletedCodes);
    }

}
