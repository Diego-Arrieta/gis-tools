using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Geometry;
using OSGeo.OGR;

namespace GisTools.Core.Helpers
{
    public static class GdalGeometryConverter
    {
        public static OSGeo.OGR.Geometry ToGdalGeometry(IGeometry myGeometry)
        {
            if (myGeometry == null) return null;

            if (myGeometry is GeoPoint point)
            {
                var geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint);
                geom.AddPoint(point.X, point.Y, point.Z);
                return geom;
            }

            if (myGeometry is GeoLine line)
            {
                var geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLineString);
                foreach (var pt in line.Points)
                {
                    geom.AddPoint(pt.X, pt.Y, pt.Z);
                }
                return geom;
            }

            if (myGeometry is GeoPolygon polygon)
            {
                var geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPolygon);
                using (var ring = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLinearRing))
                {
                    foreach (var pt in polygon.ExteriorRing)
                    {
                        ring.AddPoint(pt.X, pt.Y, pt.Z);
                    }

                    if (polygon.ExteriorRing.Count > 0)
                    {
                        var first = polygon.ExteriorRing[0];
                        var last = polygon.ExteriorRing[polygon.ExteriorRing.Count - 1];
                        if (first.X != last.X || first.Y != last.Y)
                        {
                            ring.AddPoint(first.X, first.Y, first.Z);
                        }
                    }
                    geom.AddGeometry(ring);
                }
                return geom;
            }

            return null; // Unknown geometry type
        }

    }
}
