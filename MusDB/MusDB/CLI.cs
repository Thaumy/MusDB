using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusDB
{
    class CLI
    {
        private CLI() { }
        public static void InColor(ConsoleColor Color, Action todo)
        {
            Console.ForegroundColor = Color;
            todo();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Put(string text) => Console.WriteLine(text);

        public static void Pause() => Console.ReadKey();
        public static void Pause(string text)
        {
            Console.WriteLine(text); Console.ReadKey();
        }

        public static void Line() => Console.WriteLine();

        public static void InPosition(int left, int right, Action todo)
        {
            Console.SetCursorPosition(left, right);
            todo();
        }
    }
}
