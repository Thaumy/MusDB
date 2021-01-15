using System;
using System.IO;

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
        static void Put(string str) => Console.WriteLine(str);
        static void Pause() => Console.ReadKey();

        static void Main(string[] args)
        {
            Put("初始化MusDB数据库服务..................[ ]");
            Console.SetCursorPosition(40, Console.CursorTop - 1);
            var MySqlManager = new MySqlManager(new("localhost", 3306, "root", "65a1561425f744e2b541303f628963f8"), "musdb");
            ShowGreen(() => Console.WriteLine("O"));

            Put("统计信息...............................[ ]");
            Console.SetCursorPosition(40, Console.CursorTop - 1);
            var result = MySqlManager.GetKey("SELECT COUNT(*) FROM statistics");
            ShowGreen(() => Console.WriteLine("O"));

            Put($"当前数据库记录存留：{result}");
            Put("按任意键收集数据");

            int count = 0;

            void fun(string path)
            {
                DirectoryInfo dic = new DirectoryInfo(path);
                FileSystemInfo[] allin = dic.GetFileSystemInfos();

                var f = dic.GetFiles("*flac*");
                foreach (var el in allin)
                {
                    if (el is DirectoryInfo)
                    {
                        fun(el.FullName);
                    }
                    else
                    {
                        Put(el.FullName);
                        count++;
                    }
                }
            }

            fun(@"D:\Thaumy的乐库\Playlists\.喵喵喵");

            Put($"一共找到flac文件：{count}");

            Pause();
        }
    }
}
