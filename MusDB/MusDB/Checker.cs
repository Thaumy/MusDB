using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Collections;

using WaterLibrary.MySQL;

using MySql.Data.MySqlClient;
using MySql;
using MusDB;
using WaterLibrary.Util;
using System.Security.Cryptography;

namespace MusDB
{
    class Checker
    {
        private Checker() { }

        public static string ToSHA256(string path)
        {
            using SHA256 SHA256 = SHA256.Create();
            using FileStream File = new FileStream(path, FileMode.Open, FileAccess.Read);

            string result = BitConverter.ToString(SHA256.ComputeHash(File));

            return result;
        }

        public static List<(string Name, string MD5, string file_type)> CheckFiles(string Path, out (int flac, int mp3, int etc, int total) Count)
        {
            Count = (0, 0, 0, 0);
            List<(string Name, string MD5, string file_type)> Files = new();

            foreach (var el in new DirectoryInfo(Path).GetFileSystemInfos())
            {
                if (el is DirectoryInfo)
                {
                    Files.AddRange(CheckFiles(el.FullName, out Count));
                }
                else
                {
                    CLI.Put(Count.total.ToString().PadLeft(4, '0'));
                    var temp = (FileInfo)el;
                    if (temp.Name.Contains(".flac"))
                    {
                        CLI.Line(" | flac  " + temp.Name);


                        var result = ToSHA256(temp.FullName);
                        Files.Add(new(temp.FullName, result, "flac"));

                        CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                           () => { CLI.Line(temp.DirectoryName); });

                        Count.flac++;
                    }
                    else if (temp.Name.Contains(".mp3"))
                    {
                        CLI.Line(" |  mp3  " + temp.Name);


                        var result = ToSHA256(temp.FullName);
                        Files.Add(new(temp.FullName, result, "mp3"));

                        CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                           () => { CLI.Line(temp.DirectoryName); });

                        Count.mp3++;
                    }
                    else
                    {
                        CLI.Line(temp.Name);
                        Files.Add(new(temp.FullName, "null", "null"));

                        Count.etc++;
                    }
                    Count.total++;
                }

            }
            return Files;
        }

        public static void CheckConflict(List<(string Name, string MD5)> Music)
        {
            var conflicts = from el in (from el in Music group el by el.MD5) where el.Count() > 1 select el;
            if (conflicts.Count() > 0)
            {
                CLI.Line("冲突项目：");
                foreach (var el in conflicts)
                {
                    foreach (var it in el)
                    {
                        CLI.Line(it.Name);
                    }
                    CLI.Line();
                }
            }
        }
    }
}
