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

            (int flac, int mp3, int etc) = (0, 0, 0);
            int total = 0;

            List<FileInfo> ETC = new();
            List<(string name, string md5)> Music = new();

            (string, string) ToSHA256(string path)
            {
                using SHA256 SHA256 = SHA256.Create();
                using FileStream File = new FileStream(path, FileMode.Open, FileAccess.Read);

                string result = BitConverter.ToString(SHA256.ComputeHash(File));

                return new(path, result);
            }


            void fun(string path)
            {
                foreach (var el in new DirectoryInfo(path).GetFileSystemInfos())
                {
                    if (el is DirectoryInfo)
                    {
                        fun(el.FullName);
                    }
                    else
                    {
                        CLI.Put(total.ToString().PadLeft(4, '0'));
                        var temp = (FileInfo)el;
                        if (temp.Name.Contains(".flac"))
                        {
                            CLI.Line(" | flac  " + temp.Name);
                            flac++;

                            var md5 = ToSHA256(temp.FullName);
                            Music.Add(md5);
                            DB.Update(temp.Name, md5.Item2, "flac");

                            CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                               () => { CLI.Line(temp.DirectoryName); });
                        }
                        else if (temp.Name.Contains(".mp3"))
                        {
                            CLI.Line(" |  mp3  " + temp.Name);
                            mp3++;

                            var md5 = ToSHA256(temp.FullName);
                            Music.Add(md5);
                            DB.Update(temp.Name, md5.Item2, "mp3");

                            CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                               () => { CLI.Line(temp.DirectoryName); });
                        }
                        else
                        {
                            CLI.Line(temp.Name);
                            ETC.Add(temp);
                            etc++;
                        }
                        total++;
                    }

                }
            }

            fun(@"D:\Thaumy的乐库\Playlists\.喵喵喵");


            CLI.Line($"\nflac:{flac}  mp3:{mp3}  其他:{etc}\n");


            foreach (var el in ETC)
            {
                CLI.Line(el.FullName);
            }

            Conflict.Check(Music);

            CLI.InColor(ConsoleColor.Green, () => CLI.Pause("\n\a检查完成。"));
        }
    }
}
