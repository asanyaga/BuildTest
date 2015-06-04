using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility;

namespace Distributr.Core.Repository.Map
{
  public  interface IMapCordinateRepository
  {
      List<MapCoordinate> GetOutletMap(Guid distrubutorId, Guid? routeId);
  }
}
