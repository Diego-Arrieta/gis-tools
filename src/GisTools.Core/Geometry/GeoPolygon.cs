using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.Core.Geometry
{
    public class GeoPolygon : IGeometry
    {
        public IReadOnlyList<GeoPoint> ExteriorRing { get; }
        public string GeometryType => "Polygon";

        public GeoPolygon(IEnumerable<GeoPoint> exteriorRing)
        {
            var points = exteriorRing?.ToList() ?? new List<GeoPoint>();

            if (points.Count < 3)
                throw new ArgumentException("Polygon must have at least 3 points.");

            EnsureClosedRing(points);

            ExteriorRing = points;
        }

        private void EnsureClosedRing(List<GeoPoint> points)
        {
            var first = points.First();
            var last = points.Last();

            if (!ArePointsEqual(first, last))
            {
                points.Add(first);
            }
        }

        private bool ArePointsEqual(GeoPoint a, GeoPoint b)
        {
            const double tolerance = 1e-9;
            return Math.Abs(a.X - b.X) < tolerance &&
                   Math.Abs(a.Y - b.Y) < tolerance &&
                   Math.Abs(a.Z - b.Z) < tolerance;
        }
    }
}