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
        /// <summary>
        /// 私有构造
        /// </summary>
        private Checker() { }

        /// <summary>
        /// 文件转SHA256签名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToSHA256(string path)
        {
            using SHA256 SHA256 = SHA256.Create();
            using FileStream File = new FileStream(path, FileMode.Open, FileAccess.Read);

            string result = BitConverter.ToString(SHA256.ComputeHash(File));

            return result;
        }

        /// <summary>
        /// 检查某路径下的所有文件并计数
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Count"></param>
        /// <returns>所有文件的数据列表</returns>
        public static List<(string Name, string MD5, string path, string file_type)> CheckFiles(string Path, ref (int flac, int mp3, int etc, int total) Count)
        {
            List<(string Name, string MD5, string path, string file_type)> Files = new();

            foreach (var el in new DirectoryInfo(Path).GetFileSystemInfos())
            {
                if (el is DirectoryInfo)
                {
                    Files.AddRange(CheckFiles(el.FullName, ref Count));
                    CLI.Line();
                }
                else
                {
                    CLI.Put(Count.total.ToString().PadLeft(4, '0'));
                    var temp = (FileInfo)el;
                    if (temp.Name.Contains(".flac"))
                    {
                        CLI.Line(" | flac  " + temp.Name);


                        var result = ToSHA256(temp.FullName);
                        Files.Add(new(temp.Name, result, temp.DirectoryName, "flac"));

                        CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                           () => { CLI.Line(temp.DirectoryName); });

                        Count.flac++;
                    }
                    else if (temp.Name.Contains(".mp3"))
                    {
                        CLI.Line(" |  mp3  " + temp.Name);


                        var result = ToSHA256(temp.FullName);
                        Files.Add(new(temp.Name, result, temp.DirectoryName, "mp3"));

                        CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                           () => { CLI.Line(temp.DirectoryName); });

                        Count.mp3++;
                    }
                    else
                    {
                        CLI.Line(temp.Name);
                        Files.Add(new(temp.Name, "", temp.DirectoryName, ""));

                        Count.etc++;
                    }
                    Count.total++;
                }

            }
            return Files;
        }

        public static IEnumerable<string> CheckETC(List<(string Name, string MD5, string path, string file_type)> Files)
        {
            return from el in Files where el.file_type == "" select el.Name;
        }

        public static List<List<string>> CheckConflict(List<(string Name, string MD5, string path, string file_type)> Files)
        {
            var conflicts = from el in (from el in Files group el by el.MD5) where el.Count() > 1 select el;
            List<List<string>> result = new();
            if (conflicts.Count() > 0)
            {
                foreach (var el in conflicts)
                {
                    List<string> it = new();
                    foreach (var inner_el in el)
                    {
                        it.Add(inner_el.Name);
                    }
                    result.Add(it);
                }
            }
            return result;
        }
    }
}
