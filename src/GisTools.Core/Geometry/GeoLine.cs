using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.Core.Geometry
{
    public class GeoLine : IGeometry
    {
        public List<GeoPoint> Points { get; set; }
        public string GeometryType => "LineString";

        public GeoLine(List<GeoPoint> points)
        {
            Points = points ?? new List<GeoPoint>();
        }
    }
}
