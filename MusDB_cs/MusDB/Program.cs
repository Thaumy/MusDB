using System;
using System.Linq;
using System.Collections.Generic;

using MusDB;

Config.GetConfig(out string path, out (string usr, string pwd, string db) database);

CLI.Line = "初始化MusDB数据库服务..................[ ]";
Database DB = new(database.usr, database.pwd, database.db);
CLI.InPosition(40, Console.CursorTop - 1,
            () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

CLI.Line = "统计信息...............................[ ]";
var result = DB.GetCount();
CLI.InPosition(40, Console.CursorTop - 1,
           () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

CLI.Line = $"当前数据库记录存留：{result}";
CLI.Pause("按任意键收集数据");
CLI.Line = "\n";

(int flac, int mp3, int etc, int total) Count = (0, 0, 0, 0);

List<(string Name, string MD5, string path, string file_type)> AllFiles = Checker.CheckFiles(path, ref Count);

CLI.Line = "\n";
CLI.Line = $"flac:{Count.flac}  mp3:{Count.mp3}\n";
CLI.Line = $"共计:{Count.total}\n";

CLI.Line = "其他项目：";
Checker.CheckETC(AllFiles).ForEach((el) => { CLI.Line = el; });

CLI.Line = "\n冲突项目：\n";
Checker.CheckConflict(AllFiles).ForEach((el)
    =>
{
    el.ForEach((it)
   =>
    { CLI.Line = it; });
    CLI.Line = "\n";
});

CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n\a检查完成，按任意键匹配数据"); CLI.Line = "\n"; });

List<(string Name, string MD5, string path, string file_type)> MusicFiles = (from el in AllFiles where el.file_type != "" select (el.Name, el.MD5, el.path, el.file_type)).ToList();
var MusicInDB = DB.GetAll();

CLI.Line = "以下项目在本地文件中不存在：";
foreach (var el in MusicInDB)
{
    int i = MusicFiles.FindIndex(x => x.Name == el.Name && x.MD5 == el.MD5 && x.file_type == el.file_type);//剔除数据库中已有记录
    if (i != -1)
    {
        MusicFiles.RemoveAt(i);
    }
    else
    {
        CLI.Line = el.Name;
    }
}
CLI.Line = "\n以下项目在数据库中不存在（本地新增）：";
MusicFiles.ForEach((el) =>
{
    CLI.Line = el.Name;
    CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
               () => { CLI.Line = el.path; });
});

CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n按任意键将本地新增数据同步到数据库。"); CLI.Line = "\n"; });
MusicFiles.ForEach((el) =>
{
    DB.Update(el.Name, el.MD5, el.file_type);
    CLI.Line = "已添加：" + el.Name;
});

CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n\a任务完成，任意键退出。"); });
