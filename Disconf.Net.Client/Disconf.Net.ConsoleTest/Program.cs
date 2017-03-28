using Disconf.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.ConsoleTest
{
    class Program
    {
        public static string Person { get; set; }
        static void Main(string[] args)
        {
            DisConfigRules.Register();
            Console.WriteLine("press any key to exist...");
            Console.ReadKey();
        }
    }
}
