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

            if (myGeometry is GeoPoint p)
            {
                var geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint);
                geom.AddPoint(p.X, p.Y, p.Z);
                return geom;
            }

            if (myGeometry is GeoLine l)
            {
                var geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLineString);
                foreach (var pt in l.Points)
                {
                    geom.AddPoint(pt.X, pt.Y, pt.Z);
                }
                return geom;
            }

            if (myGeometry is GeoPolygon poly)
            {
                var geom = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPolygon);
                using (var ring = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLinearRing))
                {
                    foreach (var pt in poly.ExteriorRing)
                    {
                        ring.AddPoint(pt.X, pt.Y, pt.Z);
                    }
                    if (poly.ExteriorRing.Count > 0)
                    {
                        var first = poly.ExteriorRing[0];
                        var last = poly.ExteriorRing[poly.ExteriorRing.Count - 1];
                        if (first.X != last.X || first.Y != last.Y)
                        {
                            ring.AddPoint(first.X, first.Y, first.Z);
                        }
                    }
                    geom.AddGeometry(ring);
                }
                return geom;
            }

            return null;
        }

        public static IGeometry ToInternalGeometry(OSGeo.OGR.Geometry gdalGeom)
        {
            if (gdalGeom == null) return null;

            var type = gdalGeom.GetGeometryType();

            if (type == wkbGeometryType.wkbPoint || type == wkbGeometryType.wkbPoint25D)
            {
                return new GeoPoint(gdalGeom.GetX(0), gdalGeom.GetY(0), gdalGeom.GetZ(0));
            }

            if (type == wkbGeometryType.wkbLineString || type == wkbGeometryType.wkbLineString25D)
            {
                var points = new List<GeoPoint>();
                int count = gdalGeom.GetPointCount();
                for (int i = 0; i < count; i++)
                {
                    points.Add(new GeoPoint(gdalGeom.GetX(i), gdalGeom.GetY(i), gdalGeom.GetZ(i)));
                }
                return new GeoLine(points);
            }

            if (type == wkbGeometryType.wkbPolygon || type == wkbGeometryType.wkbPolygon25D)
            {
                var ring = gdalGeom.GetGeometryRef(0);
                var points = new List<GeoPoint>();
                int count = ring.GetPointCount();
                for (int i = 0; i < count; i++)
                {
                    points.Add(new GeoPoint(ring.GetX(i), ring.GetY(i), ring.GetZ(i)));
                }
                return new GeoPolygon(points);
            }

            return null;
        }
    }
}
