using System;
using System.IO;
using System.Collections;

using WaterLibrary.MySQL;
using System.Collections.Generic;
using System.Linq;

using WaterLibrary.Util;
using System.Security.Cryptography;

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
            List<string> md5List = new();

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

                            using (MD5 MD5 = MD5.Create())
                            {
                                using (FileStream file1 = new FileStream(temp.FullName, FileMode.Open))
                                {
                                    byte[] hashByte1 = MD5.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组 
                                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串 
                                    md5List.Add(str1);
                                }
                            }

                        }
                        else if (temp.Name.Contains(".mp3"))
                        {
                            Put(temp.Name);
                            count.mp3++;

                            using (MD5 MD5 = MD5.Create())
                            {
                                using (FileStream file1 = new FileStream(temp.FullName, FileMode.Open))
                                {
                                    byte[] hashByte1 = MD5.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组 
                                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串 
                                    md5List.Add(str1);
                                }
                            }
                        }
                        else
                        {
                            Put(temp.Name);
                            ectList.Add(temp);
                            count.etc++;
                        }
                    }
                    Console.SetCursorPosition(110, Console.CursorTop - 1);
                    Put((count.flac + count.mp3 + count.etc).ToString());
                }
            }

            fun(@"D:\Thaumy的乐库\Playlists\.喵喵喵");


            Put("");
            Put($"flac:{count.flac}  mp3:{count.mp3}  其他:{count.etc}");

            foreach (var el in ectList)
            {
                Put(el.FullName);
            }

            Put("冲突项目：");
            var a = md5List
                    .GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(y => y.Key)
                    .ToList();

            foreach (var v in a)
            {
                Put(v);
            }
            Put("检查完成。");

            Pause();
        }
    }
}
