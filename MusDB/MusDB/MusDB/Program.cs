using System;
using System.Threading;
using WaterLibrary.MySQL;

namespace MusDB
{
    class Program
    {
        static void ShowGreen(Action todo)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            todo();
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("初始化MusDB数据库服务..................[ ]");
            Console.SetCursorPosition(40, Console.CursorTop - 1);
            var MySqlManager = new MySqlManager(new("localhost", 3306, "root", "65a1561425f744e2b541303f628963f8"), "musdb");
            ShowGreen(() => Console.WriteLine("O"));

            Console.WriteLine("统计信息...............................[ ]");
            Console.SetCursorPosition(40, Console.CursorTop - 1);
            var count = MySqlManager.GetKey("SELECT COUNT(*) FROM statistics");
            ShowGreen(() => Console.WriteLine("O"));

            Console.WriteLine($"当前数据库记录存留：{count}");

            Console.ReadKey();
        }
    }
}
