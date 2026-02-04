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
                Console.WriteLine("1. Write Points (UTM)");
                Console.WriteLine("2. Write Lines (Z)");
                Console.WriteLine("3. Write Polygons (Triangle)");
                Console.WriteLine("-----------------------------");
                Console.WriteLine("4. Read Points");
                Console.WriteLine("5. Read Lines");
                Console.WriteLine("6. Read Polygons");
                Console.WriteLine("-----------------------------");
                Console.WriteLine("0. Exit");
                Console.Write("\nSelect option: ");

                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.KeyChar)
                {
                    case '1': Scenarios.TestPointWriting(); break;
                    case '2': Scenarios.TestLineWriting(); break;
                    case '3': Scenarios.TestPolygonWriting(); break;

                    case '4': Scenarios.TestPointReading(); break;
                    case '5': Scenarios.TestLineReading(); break;
                    case '6': Scenarios.TestPolygonReading(); break;

                    case '0': keepRunning = false; continue;
                    default: Console.WriteLine("Invalid option."); break;
                }

                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
            }
        }
    }
}
