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
            CLI.Pause("按任意键收集数据");
            CLI.Line();

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

            CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n\a检查完成，按任意键匹配数据"); CLI.Line(); });

            List<(string Name, string MD5, string path, string file_type)> MusicFiles = (from el in AllFiles where el.file_type != "" select (el.Name, el.MD5, el.path, el.file_type)).ToList();
            var MusicInDB = DB.GetAll();

            CLI.Line("以下项目在本地文件中不存在：");
            foreach (var el in MusicInDB)
            {
                int i = MusicFiles.FindIndex(x => x.Name == el.Name && x.MD5 == el.MD5 && x.file_type == el.file_type);//剔除数据库中已有记录
                if (i != -1)
                {
                    MusicFiles.RemoveAt(i);
                }
                else
                {
                    CLI.Line(el.Name);
                }
            }
            CLI.Line("以下项目在数据库中不存在（本地新增）：");
            foreach (var el in MusicFiles)
            {
                CLI.Line(el.Name);
                CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                           () => { CLI.Line(el.path); });
            }
            CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n按任意键将本地新增数据同步到数据库。"); CLI.Line(); });

            foreach (var el in MusicFiles)
            {
                DB.Update(el.Name, el.MD5, el.file_type);
                CLI.Line("已添加：" + el.Name);
            }

            CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n\a任务完成，任意键退出。"); });
        }
    }
}
