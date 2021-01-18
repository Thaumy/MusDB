using System;
using System.IO;
using System.Collections;

using WaterLibrary.MySQL;
using System.Collections.Generic;
using System.Linq;

using MySql.Data.MySqlClient;
using MySql;
using MusDB;
using WaterLibrary.Util;
using System.Security.Cryptography;

namespace MusDB
{
    class Program
    {
        static void Main(string[] args)
        {
            CLI.Line("初始化MusDB数据库服务..................[ ]");
            Database DB = new("root", "65a1561425f744e2b541303f628963f8", "musdb");
            CLI.InPosition(40, Console.CursorTop - 1,
                        () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

            CLI.Line("统计信息...............................[ ]");
            var result = DB.GetCount();
            CLI.InPosition(40, Console.CursorTop - 1,
                       () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

            CLI.Line($"当前数据库记录存留：{result}");
            CLI.Pause("\a按任意键收集数据");

            List<FileInfo> ETC = new();
            List<(string name, string md5)> Music = new();

            Checker.CheckFiles(@"D:\Thaumy的乐库\Playlists\.喵喵喵", out (int flac, int mp3, int etc, int total) Count);


            CLI.Line($"\nflac:{Count.flac}  mp3:{Count.mp3}  其他:{Count.etc}\n");


            foreach (var el in ETC)
            {
                CLI.Line(el.FullName);
            }

            Checker.CheckConflict(Music);

            CLI.InColor(ConsoleColor.Green, () => CLI.Pause("\n\a检查完成。"));
        }
    }
}
