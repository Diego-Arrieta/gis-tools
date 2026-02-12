using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using CoreGeometry = GisTools.Core.Geometry;

namespace Dynamo
{
    internal static class GeometryConverter
    {
        public static CoreGeometry.IGeometry ToInternal(Geometry dynamoGeometry)
        {
            if (dynamoGeometry == null) return null;

            if (dynamoGeometry is Point p)
                return new CoreGeometry.GeoPoint(p.X, p.Y, p.Z);

            if (dynamoGeometry is Polygon poly)
            {
                var geoPoints = poly.Points.Select(pt => new CoreGeometry.GeoPoint(pt.X, pt.Y, pt.Z)).ToList();
                return new CoreGeometry.GeoPolygon(geoPoints);
            }

            if (dynamoGeometry is Curve curve)
            {
                var geoPoints = ExtractVerticesFromCurve(curve);
                return new CoreGeometry.GeoLine(geoPoints);
            }

            throw new NotSupportedException($"Geometry type not supported: {dynamoGeometry.GetType().Name}");
        }

        private static List<CoreGeometry.GeoPoint> ExtractVerticesFromCurve(Curve curve)
        {
            var points = new List<CoreGeometry.GeoPoint>();

            if (curve is PolyCurve polyCurve)
            {
                foreach (var subCurve in polyCurve.Curves())
                {
                    var start = subCurve.StartPoint;
                    points.Add(new CoreGeometry.GeoPoint(start.X, start.Y, start.Z));
                }
                var end = polyCurve.EndPoint;
                points.Add(new CoreGeometry.GeoPoint(end.X, end.Y, end.Z));
            }
            else if (curve is Line line)
            {
                var start = line.StartPoint;
                var end = line.EndPoint;
                points.Add(new CoreGeometry.GeoPoint(start.X, start.Y, start.Z));
                points.Add(new CoreGeometry.GeoPoint(end.X, end.Y, end.Z));
            }
            else
            {
                using (var tessellatedCurve = PolyCurve.ByJoinedCurves(new[] { curve }, 0.001))
                {
                    foreach (var sub in tessellatedCurve.Curves())
                    {
                        var pt = sub.StartPoint;
                        points.Add(new CoreGeometry.GeoPoint(pt.X, pt.Y, pt.Z));
                    }
                    var last = tessellatedCurve.EndPoint;
                    points.Add(new CoreGeometry.GeoPoint(last.X, last.Y, last.Z));
                }
            }

            return points;
        }

        public static Geometry ToDynamo(CoreGeometry.IGeometry coreGeometry)
        {
            if (coreGeometry == null) return null;

            if (coreGeometry is CoreGeometry.GeoPoint gp)
            {
                return Point.ByCoordinates(gp.X, gp.Y, gp.Z);
            }

            if (coreGeometry is CoreGeometry.GeoLine gl)
            {
                var dynamoPoints = gl.Points.Select(pt => Point.ByCoordinates(pt.X, pt.Y, pt.Z)).ToList();

                if (dynamoPoints.Count < 2) return null;

                return PolyCurve.ByPoints(dynamoPoints);
            }

            if (coreGeometry is CoreGeometry.GeoPolygon gPoly)
            {
                var dynamoPoints = gPoly.ExteriorRing.Select(pt => Point.ByCoordinates(pt.X, pt.Y, pt.Z)).ToList();

                if (dynamoPoints.Count > 1 && dynamoPoints.First().IsAlmostEqualTo(dynamoPoints.Last()))
                {
                    dynamoPoints.RemoveAt(dynamoPoints.Count - 1);
                }

                return Polygon.ByPoints(dynamoPoints);
            }

            return null;
        }
    }
}