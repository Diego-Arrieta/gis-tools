using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.Core.Geometry
{
    public class GeoLine : IGeometry
    {
        public IReadOnlyList<GeoPoint> Points { get; set; }
        public string GeometryType => "LineString";

        public GeoLine(IEnumerable<GeoPoint> points)
        {
            var pointList = points?.ToList() ?? new List<GeoPoint>();

            if (pointList.Count < 2) 
                throw new ArgumentException("LineString must have at least 2 points.");

            Points = pointList;
        }
    }
}