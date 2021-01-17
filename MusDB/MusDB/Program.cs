using System;
using System.IO;
using System.Collections;

using WaterLibrary.MySQL;
using System.Collections.Generic;
using System.Linq;

using MusDB;
using WaterLibrary.Util;
using System.Security.Cryptography;

namespace MusDB
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Put("初始化MusDB数据库服务..................[ ]");
            Console.SetCursorPosition(40, Console.CursorTop - 1);
            var MySqlManager = new MySqlManager(new("localhost", 3306, "root", "65a1561425f744e2b541303f628963f8"), "musdb");
            ShowGreen(() => Console.WriteLine("O"));

            Put("统计信息...............................[ ]");
            Console.SetCursorPosition(40, Console.CursorTop - 1);
            var result = MySqlManager.GetKey("SELECT COUNT(*) FROM statistics");
            ShowGreen(() => Console.WriteLine("O"));

            Put($"当前数据库记录存留：{result}");
            Pause("按任意键收集数据");
            */
            (int flac, int mp3, int etc) count = (0, 0, 0);

            List<FileInfo> ectList = new();
            List<(string name, string md5)> md5List = new();

            (string, string) ToSHA256(string path)
            {
                using SHA256 SHA256 = SHA256.Create();
                using FileStream file1 = new FileStream(path, FileMode.Open, FileAccess.Read);

                byte[] hashByte1 = SHA256.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组
                string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串

                return new(path, str1);
            }

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
                            CLI.Put(temp.Name);
                            count.flac++;

                            md5List.Add(ToSHA256(temp.FullName));
                        }
                        else if (temp.Name.Contains(".mp3"))
                        {
                            CLI.Put(temp.Name);
                            count.mp3++;

                            md5List.Add(ToSHA256(temp.FullName));
                        }
                        else
                        {
                            CLI.Put(temp.Name);
                            ectList.Add(temp);
                            count.etc++;
                        }
                    }
                    Console.SetCursorPosition(110, Console.CursorTop - 1);
                    CLI.Put((count.flac + count.mp3 + count.etc).ToString());
                }
            }

            fun(@"D:\Thaumy的乐库\Playlists\.喵喵喵");


            CLI.Line();
            CLI.Put($"flac:{count.flac}  mp3:{count.mp3}  其他:{count.etc}");
            CLI.Line();

            foreach (var el in ectList)
            {
                CLI.Put(el.FullName);
            }

            CLI.Put("冲突项目：");
            var a = md5List
                    .GroupBy(x => x.md5)
                    .Where(g => g.Count() > 1)
                    .Select(y => y.Key);

            var b = from c in md5List group c by c.md5;

            foreach (var v in b)
            {
                var s = from c in md5List
                        where c.md5 == v.Key
                        select c.name;
                if (s.ToList().Count > 1)
                {
                    foreach (var k in s)
                    {
                        CLI.Put(k);
                    }
                    CLI.Line();
                }
            }

            CLI.Pause("检查完成。");
        }
    }
}
