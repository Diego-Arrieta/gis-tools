using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.Core.Geometry
{
    public class GeoPolygon : IGeometry
    {
        public List<GeoPoint> ExteriorRing { get; set; }
        public string GeometryType => "Polygon";

        public GeoPolygon(List<GeoPoint> exteriorRing)
        {
            ExteriorRing = exteriorRing ?? new List<GeoPoint>();
        }
    }
}
