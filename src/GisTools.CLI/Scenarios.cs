using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GisTools.Core.Entities;
using GisTools.Core.Geometry;
using GisTools.Core.IO;
using GisTools.Core.Readers;

namespace GisTools.CLI
{
    public static class Scenarios
    {
        private static string GetDesktopPath(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        }
        private static Dictionary<string, object> CreateSampleAttributes(int id, string description)
        {
            return new Dictionary<string, object>
            {
                { "ID", id },
                { "Description", description },
                { "Date", DateTime.Now.ToShortDateString() }
            };
        }
        public static void TestPointWriting()
        {
            Console.WriteLine("\n--- TEST: Generate Point ---");

            var point = new GeoPoint(540000, 9420000, 10);
            var attributes = CreateSampleAttributes(1, "Test Point");
            var feature = new Feature(point, attributes);

            string file = GetDesktopPath("my-points.shp");

            try
            {
                ShapefileWriter.Write(file, new List<Feature> { feature });
                Console.WriteLine($"[OK] File created: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
        public static void TestLineWriting()
        {
            Console.WriteLine("\n--- TEST: Generate Lines ---");

            var points = new List<GeoPoint>
            {
                new GeoPoint(540000, 9420000),
                new GeoPoint(540000, 9420100),
                new GeoPoint(540100, 9420050),
                new GeoPoint(540000, 9420150)
            };

            var line = new GeoLine(points);
            var attributes = CreateSampleAttributes(10, "Test Line");
            var feature = new Feature(line, attributes);

            string file = GetDesktopPath("my-lines.shp");

            try
            {
                ShapefileWriter.Write(file, new List<Feature> { feature });
                Console.WriteLine($"[OK] File created: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
        public static void TestPolygonWriting()
        {
            Console.WriteLine("\n--- TEST: Generate Polygons ---");

            var trianglePoints = new List<GeoPoint>
            {
                new GeoPoint(540000, 9420000),
                new GeoPoint(540000, 9420100),
                new GeoPoint(540100, 9420050),
                // Note: The Core will automatically close the ring if we omit the last point
            };

            var polygon = new GeoPolygon(trianglePoints);
            var attributes = CreateSampleAttributes(99, "Test Polygon");
            var feature = new Feature(polygon, attributes);

            string file = GetDesktopPath("my-polygons.shp");

            try
            {
                ShapefileWriter.Write(file, new List<Feature> { feature });
                Console.WriteLine($"[OK] File created: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
        public static void TestPointReading()
        {
            Console.WriteLine("\n--- TEST: Reading Shapefile (Features) ---");

            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-points.shp");

            if (!File.Exists(file))
            {
                Console.WriteLine("File not found. Run Test 1 first.");
                return;
            }

            var features = ShapefileReader.Read(file);

            Console.WriteLine($"Features Read: {features.Count}");

            foreach (var feature in features)
            {
                var geom = feature.Geometry;

                if (geom is GeoPoint p)
                {
                    Console.WriteLine($" [POINT] ({p.X}, {p.Y}, {p.Z})");
                }

                if (feature.Attributes.Count > 0)
                {
                    Console.Write("   [DATA]: ");
                    foreach (var attr in feature.Attributes)
                    {
                        Console.Write($"{attr.Key}={attr.Value} | ");
                    }
                    Console.WriteLine(); // Salto de línea
                }
            }
        }
        public static void TestLineReading()
        {
            Console.WriteLine("\n--- TEST: Reading Lines ---");
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-lines.shp");

            if (!File.Exists(file)) return;

            var features = ShapefileReader.Read(file);

            foreach (var feature in features)
            {
                if (feature.Geometry is GeoLine l)
                {
                    Console.WriteLine($" [LINE] Vertices count: {l.Points.Count}");
                }
            }
        }
        public static void TestPolygonReading()
        {
            Console.WriteLine("\n--- TEST: Reading Polygons ---");
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my-polygons.shp");

            if (!File.Exists(file)) return;

            var features = ShapefileReader.Read(file);

            foreach (var feature in features)
            {
                if (feature.Geometry is GeoPolygon poly)
                {
                    Console.WriteLine($" [POLYGON] Exterior ring vertices: {poly.ExteriorRing.Count}");
                }
            }
        }
        public static void TestAttributeReading()
        {
            string file = GetDesktopPath("my-points.shp");

            Console.WriteLine($"\n--- TEST: Inspecting Attributes from {Path.GetFileName(file)} ---");

            if (!File.Exists(file))
            {
                Console.WriteLine($"[ERROR] File not found: {file}");
                return;
            }

            try
            {
                var features = GisTools.Core.Readers.ShapefileReader.Read(file);

                if (features.Count == 0)
                {
                    Console.WriteLine("[WARN] The shapefile is empty.");
                    return;
                }

                var firstFeature = features[0];
                var columnNames = firstFeature.Attributes.Keys.ToList();

                Console.WriteLine($"[INFO] Total Features: {features.Count}");
                Console.WriteLine($"[INFO] Columns Detected ({columnNames.Count}):");

                foreach (var col in columnNames)
                {
                    Console.WriteLine($"   - {col}");
                }

                bool hasWkt = columnNames.Any(c => c.ToUpper() == "WKT" || c.ToUpper() == "THE_GEOM");

                Console.WriteLine("\n----------------RESULT----------------");
                if (hasWkt)
                {
                    Console.WriteLine("[FAIL] ❌ The 'WKT' column is still present.");
                }
                else
                {
                    Console.WriteLine("[SUCCESS] ✅ Clean schema! No geometry columns found in attributes.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
        public static void InspectAttributes()
        {
            string file = GetDesktopPath("my-points2.shp");

            Console.WriteLine($"\n--- 🔍 INSPECCIÓN DE DATOS: {Path.GetFileName(file)} ---");

            if (!File.Exists(file))
            {
                Console.WriteLine($"[ERROR] Archivo no encontrado: {file}");
                return;
            }

            try
            {
                var features = GisTools.Core.Readers.ShapefileReader.Read(file);

                if (features.Count == 0)
                {
                    Console.WriteLine("[WARN] El Shapefile está vacío.");
                    return;
                }

                var headers = features[0].Attributes.Keys.ToList();

                Console.WriteLine($"[INFO] Registros Totales: {features.Count}");
                Console.WriteLine($"[INFO] Columnas Encontradas: {string.Join(" | ", headers)}");
                Console.WriteLine(new string('-', 50));

                int count = 0;
                foreach (var feature in features)
                {
                    if (count >= 5) break;

                    var values = new List<string>();
                    foreach (var header in headers)
                    {
                        object val = feature.Attributes.ContainsKey(header) ? feature.Attributes[header] : "null";
                        values.Add(val?.ToString() ?? "null");
                    }

                    Console.WriteLine($"Fila {count + 1}: {string.Join(" | ", values)}");
                    count++;
                }
                Console.WriteLine(new string('-', 50));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Falló la lectura: {ex.Message}");
            }
        }
    }
}