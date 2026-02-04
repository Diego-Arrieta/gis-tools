using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Entities;
using GisTools.Core.Geometry;
using GisTools.Core.Readers;
using GisTools.Core.Writers;

namespace GisTools.CLI
{
    public static class Scenarios
    {
        public static void TestPointWriting()
        {
            Console.WriteLine("\n--- TEST: Generate Point ---");

            var features = new List<GisFeature>();

            var p1 = new GisFeature(new GeoPoint(540000, 9420000));
            p1.Attributes.Add("ID", "001");
            p1.Attributes.Add("City", "Piura");
            features.Add(p1);

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-points.shp");

            string result = ShapefileWriter.WritePoints(file, features, 32717);

            if (result == "Success")
                Console.WriteLine($"[OK] File created on: {file}");
            else
                Console.WriteLine($"[ERROR] {result}");
        }
        public static void TestLineWriting()
        {
            Console.WriteLine("\n--- TEST: Generate Lines ---");

            var zPoints = new List<GeoPoint>
            {
                new GeoPoint(540000, 9420000),
                new GeoPoint(540000, 9420100),
                new GeoPoint(540100, 9420050),
                new GeoPoint(540000, 9420150)
            };

            var lineFeature = new GisFeature(new GeoLine(zPoints));
            lineFeature.Attributes.Add("Name", "Z Mark");
            lineFeature.Attributes.Add("Type", "Path");

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-lines.shp");

            string result = ShapefileWriter.WriteLines(file, new List<GisFeature> { lineFeature }, 32717);

            if (result == "Success")
                Console.WriteLine($"[OK] File created on: {file}");
            else
                Console.WriteLine($"[ERROR] {result}");
        }
        public static void TestPolygonWriting()
        {
            Console.WriteLine("\n--- TEST: Generate polygons ---");

            var trianglePoints = new List<GeoPoint>
            {
                new GeoPoint(540000, 9420000),
                new GeoPoint(540000, 9420100),
                new GeoPoint(540100, 9420050),
                new GeoPoint(540000, 9420000)
            };

            var polyFeature = new GisFeature(new GeoPolygon(trianglePoints));
            polyFeature.Attributes.Add("Name", "Triangle");
            polyFeature.Attributes.Add("Area", "Approx 50");

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-polygons.shp");

            string result = ShapefileWriter.WritePolygons(file, new List<GisFeature> { polyFeature }, 32717);

            if (result == "Success")
                Console.WriteLine($"[OK] File created on: {file}");
            else
                Console.WriteLine($"[ERROR] {result}");
        }
        public static void TestPointReading()
        {
            Console.WriteLine("\n--- TEST: Reading Shapefile ---");

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-points.shp");

            if (!File.Exists(file))
            {
                Console.WriteLine("File not found. Run Test 1 first.");
                return;
            }

            var features = ShapefileReader.ReadAll(file);

            Console.WriteLine($"Files Read: {features.Count}");

            foreach (var f in features)
            {
                Console.WriteLine($"Type: {f.Geometry.GeometryType} | Attrs: {f.Attributes.Count}");
                if (f.Attributes.ContainsKey("ID"))
                {
                    Console.WriteLine($" - ID: {f.Attributes["ID"]}");
                }
            }
        }
        public static void TestLineReading()
        {
            Console.WriteLine("\n--- TEST: Reading Lines (Z Shape) ---");
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-lines.shp");

            if (!File.Exists(file))
            {
                Console.WriteLine("File not found. Run Test 2 first.");
                return;
            }

            var features = ShapefileReader.ReadAll(file);
            Console.WriteLine($"Features found: {features.Count}");

            foreach (var f in features)
            {
                if (f.Geometry is GeoLine l)
                {
                    Console.WriteLine($"[LINE] Vertices count: {l.Points.Count}");
                    // Print first and last point to verify
                    if (l.Points.Count > 0)
                    {
                        var start = l.Points[0];
                        var end = l.Points[l.Points.Count - 1];
                        Console.WriteLine($"  Start: ({start.X},{start.Y}) -> End: ({end.X},{end.Y})");
                    }
                }
            }
        }

        public static void TestPolygonReading()
        {
            Console.WriteLine("\n--- TEST: Reading Polygons (Triangle) ---");
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-polygons.shp");

            if (!File.Exists(file))
            {
                Console.WriteLine("File not found. Run Test 3 first.");
                return;
            }

            var features = ShapefileReader.ReadAll(file);
            Console.WriteLine($"Features found: {features.Count}");

            foreach (var f in features)
            {
                if (f.Geometry is GeoPolygon poly)
                {
                    Console.WriteLine($"[POLYGON] Exterior ring vertices: {poly.ExteriorRing.Count}");
                    foreach (var attr in f.Attributes)
                        Console.WriteLine($"  - {attr.Key}: {attr.Value}");
                }
            }
        }
    }
}