using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using GisTools.Core.Geometry;
using GisTools.Core.Managers;

namespace GisTools.Core.Converters
{
    public static class NtsGeometryConverter
    {
        public static NtsGeometry ToNtsGeometry(IGeometry geometry)
        {
            if (geometry == null) return null;

            var factory = GisEngine.Factory;

            if (geometry is GeoPoint p)
            {
                return factory.CreatePoint(new CoordinateZ(p.X, p.Y, p.Z));
            }

            if (geometry is GeoLine l)
            {
                var coordinates = l.Points.Select(pt => new CoordinateZ(pt.X, pt.Y, pt.Z)).ToArray();
                return factory.CreateLineString(coordinates);
            }

            if (geometry is GeoPolygon poly)
            {
                var coordinates = poly.ExteriorRing.Select(pt => new CoordinateZ(pt.X, pt.Y, pt.Z)).ToList();

                if (coordinates.Count > 0 && !coordinates[0].Equals2D(coordinates.Last()))
                {
                    coordinates.Add(coordinates[0]);
                }

                var linearRing = factory.CreateLinearRing(coordinates.ToArray());
                return factory.CreatePolygon(linearRing);
            }

            return null;
        }

        public static IGeometry ToInternalGeometry(NtsGeometry ntsGeometry)
        {
            if (ntsGeometry == null) return null;

            if (ntsGeometry is Point p)
            {
                return new GeoPoint(p.X, p.Y, double.IsNaN(p.Z) ? 0 : p.Z);
            }

            if (ntsGeometry is LineString l)
            {
                var points = l.Coordinates.Select(c => new GeoPoint(c.X, c.Y, double.IsNaN(c.Z) ? 0 : c.Z));
                return new GeoLine(points);
            }

            if (ntsGeometry is Polygon poly)
            {
                var points = poly.ExteriorRing.Coordinates.Select(c => new GeoPoint(c.X, c.Y, double.IsNaN(c.Z) ? 0 : c.Z));
                return new GeoPolygon(points);
            }

            return null;
        }
    }
}