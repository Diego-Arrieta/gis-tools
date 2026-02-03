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
            Console.WriteLine("\n--- Generate Point Test ---");

            var lista = new List<GisFeature>();

            var p1 = new GisFeature(new GeoPoint(0, 0, 0));
            p1.Attributes.Add("ID", "001");
            p1.Attributes.Add("Nota", "Origen");
            lista.Add(p1);

            var p2 = new GisFeature(new GeoPoint(50, 50, 10));
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
    }
}
