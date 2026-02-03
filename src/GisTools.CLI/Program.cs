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
            Console.WriteLine("======== GISTOOLS CLI ========");
            Console.WriteLine("1. Generate Point Test");
            Console.Write("Choose: ");

            var tecla = Console.ReadKey();

            if (tecla.KeyChar == '1')
            {
                Scenarios.TestPoints();
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
