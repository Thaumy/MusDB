using System;
using System.IO;
using System.Collections;

using WaterLibrary.MySQL;
using System.Collections.Generic;

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

            (int flac, int mp3, int etc) count = (0, 0, 0);

            List<FileInfo> ectList = new();

            void fun(string path)
            {
                DirectoryInfo dic = new DirectoryInfo(path);
                FileSystemInfo[] allin = dic.GetFileSystemInfos();

                foreach (var el in allin)
                {
                    if (el is DirectoryInfo)
                    {
                        fun(el.FullName);
                    }
                    else
                    {
                        var temp = (FileInfo)el;
                        if (temp.Name.Contains(".flac"))
                        {
                            Put(temp.Name);
                            count.flac++;
                        }
                        else if (temp.Name.Contains(".mp3"))
                        {
                            Put(temp.Name);
                            count.mp3++;
                        }
                        else
                        {
                            Put(temp.Name);
                            ectList.Add(temp);
                            count.etc++;
                        }
                    }
                }
            }

            fun(@"D:\Thaumy的乐库\Playlists\.喵喵喵");

            Put("");
            Put($"flac:{count.flac}  mp3:{count.mp3}  其他:{count.etc}");
            foreach (var el in ectList)
            {
                Put(el.FullName);
            }
            Pause();
        }
    }
}
