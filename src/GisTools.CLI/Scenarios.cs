using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Entities;
using GisTools.Core.Geometry;
using GisTools.Core.Writers;

namespace GisTools.CLI
{
    public static class Scenarios
    {
        public static void TestPoints()
        {
            Console.WriteLine("\n--- TEST: Generate Point ---");

            var lista = new List<GisFeature>();

            var p1 = new GisFeature(new GeoPoint(0.0, 0.0, 0));
            p1.Attributes.Add("ID", "001");
            p1.Attributes.Add("Nota", "Origen");
            lista.Add(p1);

            var p2 = new GisFeature(new GeoPoint(50.0, 50.0, 10));
            p2.Attributes.Add("ID", "002");
            p2.Attributes.Add("Nota", "Poste");
            lista.Add(p2);

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-points.shp");

            string result = ShapefileWriter.WritePoints(file, lista);

            if (result == "Success")
                Console.WriteLine($"[OK] File created on: {file}");
            else
                Console.WriteLine($"[ERROR] {result}");
        }
        public static void TestLines()
        {
            Console.WriteLine("\n--- TEST: Generate Lines ---");

            var zPoints = new List<GeoPoint>
            {
                new GeoPoint(0, 10),
                new GeoPoint(10, 10),
                new GeoPoint(0, 0),
                new GeoPoint(10, 0)
            };

            var lineFeature = new GisFeature(new GeoLine(zPoints));
            lineFeature.Attributes.Add("Name", "Z Mark");
            lineFeature.Attributes.Add("Type", "Path");

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-lines.shp");

            string result = ShapefileWriter.WriteLines(file, new List<GisFeature> { lineFeature });

            if (result == "Success")
                Console.WriteLine($"[OK] File created on: {file}");
            else
                Console.WriteLine($"[ERROR] {result}");
        }
        public static void TestPolygons()
        {
            Console.WriteLine("\n--- TEST: Generate polygons ---");

            var trianglePoints = new List<GeoPoint>
            {
                new GeoPoint(0, 0),
                new GeoPoint(10, 0),
                new GeoPoint(5, 10),
                new GeoPoint(0, 0)
            };

            var polyFeature = new GisFeature(new GeoPolygon(trianglePoints));
            polyFeature.Attributes.Add("Name", "Triangle");
            polyFeature.Attributes.Add("Area", "Approx 50");

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-polygons.shp");

            string result = ShapefileWriter.WritePolygons(file, new List<GisFeature> { polyFeature });

            if (result == "Success")
                Console.WriteLine($"[OK] File created on: {file}");
            else
                Console.WriteLine($"[ERROR] {result}");
        }
    }
}