using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Map;
using Distributr.Core.Utility;

namespace Distributr.Core.Data.Repository.Map
{
    public class MapCordinateRepository : IMapCordinateRepository
    {
        CokeDataContext _ctx;

        public MapCordinateRepository(CokeDataContext ctx)
        {
            _ctx = ctx;
        }

        public List<MapCoordinate> GetOutletMap(Guid distrubutrId, Guid? routeId)
        {
            IQueryable<tblCostCentre> data = _ctx.tblCostCentre.Where(s => s.CostCentreType == 5 && s.ParentCostCentreId == distrubutrId);
            if(routeId.HasValue)
            {
                data = data.Where(s => s.RouteId == routeId.Value);
            }
            var mapdata= new List<MapCoordinate>();
            foreach (var outlet in data)
            {
                float longi = 0, lati = 0;
                float.TryParse(outlet.StandardWH_Latitude,out lati);
                float.TryParse(outlet.StandardWH_Longtitude, out longi);
                var item = new MapCoordinate();
                item.Name = outlet.Name;
                item.Description = outlet.Name;
                item.Latitude = lati;
                item.Longitude = longi;
                mapdata.Add(item);

            }
            return mapdata;
        }
    }
}
