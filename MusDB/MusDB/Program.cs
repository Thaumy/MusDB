using System;
using System.IO;
using System.Collections;

using System.Collections.Generic;
using System.Linq;

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
            CLI.Pause("按任意键收集数据\n");

            (int flac, int mp3, int etc, int total) Count = (0, 0, 0, 0);

            List<(string Name, string MD5, string path, string file_type)> AllFiles = Checker.CheckFiles(@"D:\Thaumy的乐库\Playlists\.喵喵喵", ref Count);

            CLI.Line();
            CLI.Line($"flac:{Count.flac}  mp3:{Count.mp3}\n");
            CLI.Line($"共计:{Count.total}\n");

            CLI.Line("其他项目：");
            foreach (var el in Checker.CheckETC(AllFiles))
            {
                CLI.Line(el);
            }

            CLI.Line("\n冲突项目：\n");
            foreach (var el in Checker.CheckConflict(AllFiles))
            {
                foreach (var it in el)
                {
                    CLI.Line(it);
                }
                CLI.Line();
            }

            Console.Beep();
            CLI.InColor(ConsoleColor.Green, () => CLI.Pause("\n检查完成，按任意键同步到数据库。"));


            List<(string Name, string MD5, string path, string file_type)> MusicFiles = (from el in AllFiles where el.file_type != "" select el).ToList();
            var MusicInDB = DB.GetAll();

            //剔除数据库中已有记录
            foreach (var el in MusicInDB)
            {
                MusicFiles.Remove(el);
            }
            foreach (var el in MusicFiles)
            {
                CLI.Line(el.Name);
            }


            foreach (var el in AllFiles)
            {
                DB.Update(el.Name, el.MD5, el.file_type);
                CLI.Line(el.Name);
            }

        }
    }
}
