using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GisTools.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("======= GIS TOOLS CLI =======");
                Console.ResetColor();
                Console.WriteLine("1. Test Points");
                Console.WriteLine("2. Test Lines");
                Console.WriteLine("3. Test Polygons");
                Console.WriteLine("0. Exit");
                Console.Write("\nSelect option: ");

                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.KeyChar)
                {
                    case '1':
                        Scenarios.TestPoints();
                        break;
                    case '2':
                        Scenarios.TestLines();
                        break;
                    case '3':
                        Scenarios.TestPolygons();
                        break;
                    case '0':
                        keepRunning = false;
                        continue;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
            }
        }
    }
}
