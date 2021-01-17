﻿using System;
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
            var MySqlManager = new MySqlManager(new("localhost", 3306, "root", "65a1561425f744e2b541303f628963f8"), "musdb");
            CLI.InPosition(40, Console.CursorTop - 1,
                        () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

            CLI.Line("统计信息...............................[ ]");
            var result = MySqlManager.GetKey("SELECT COUNT(*) FROM statistics");
            CLI.InPosition(40, Console.CursorTop - 1,
                       () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

            CLI.Line($"当前数据库记录存留：{result}");
            CLI.Pause("按任意键收集数据");

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

            void db(string name, string md5, string file_type)
            {
                MySqlManager.DoInConnection(conn =>
                {
                    using MySqlCommand MySqlCommand = new MySqlCommand
                    {
                        CommandText = string.Format("INSERT INTO statistics (name,md5,file_type) VALUES (\"{0}\",\"{1}\",\"{2}\");",
                        name, md5, file_type
                        ),

                        Connection = conn,

                        /* 开始事务 */
                        Transaction = conn.BeginTransaction()
                    };

                    if (MySqlCommand.ExecuteNonQuery() == 1)
                    {
                        /* 指向表修改1行数据，拷贝表删除1行数据 */
                        MySqlCommand.Transaction.Commit();
                        return true;
                    }
                    else
                    {
                        MySqlCommand.Transaction.Rollback();
                        return false;
                    }
                });
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
                            db(temp.Name, md5.Item2, "flac");

                            CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                               () => { CLI.Line(temp.DirectoryName); });
                        }
                        else if (temp.Name.Contains(".mp3"))
                        {
                            CLI.Line(" |  mp3  " + temp.Name);
                            mp3++;

                            var md5 = ToSHA256(temp.FullName);
                            Music.Add(md5);
                            db(temp.Name, md5.Item2, "mp3");

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


            CLI.Line();
            CLI.Line($"flac:{flac}  mp3:{mp3}  其他:{etc}");
            CLI.Line();

            foreach (var el in ETC)
            {
                CLI.Line(el.FullName);
            }

            CLI.Line("冲突项目：");

            var conflicts = from el in (from el in Music group el by el.md5) where el.Count() > 1 select el;

            foreach (var el in conflicts)
            {
                foreach (var it in el)
                {
                    CLI.Line(it.name);
                }
                CLI.Line();

            }

            CLI.Pause("检查完成。");
        }
    }
}
