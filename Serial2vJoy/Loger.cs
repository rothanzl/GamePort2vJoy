using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeederDemoCS
{
    static class Loger
    {

        public static void Info(string message)
        {
            Console.WriteLine(message);
        }

        public static void InfoContinue(string message)
        {
            
            Console.CursorLeft = 0;
            Console.CursorTop = Console.CursorTop - 1;

            while (message.Length < 50) message += "     ";

            Info(message);
        }

        public static void Warn(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            Console.WriteLine(message);
        }

    }
}
