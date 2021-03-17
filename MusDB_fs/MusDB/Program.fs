open System
open App
open CLI
open Config
open Checker
open Database


CLI.Line "初始化MusDB数据库服务..................[ ]"
let (musicPath, database) = Config("./config.json").GetConfig

CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "O"))

CLI.Line "统计信息...............................[ ]"

let result = Database(database).GetCount

CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "O"))

CLI.Line $"当前数据库记录存留：{result}"

CLI.Pause "按任意键收集数据\n"

let (allFiles, allCount) = Checker.CheckAll musicPath
let (musicFiles, musicCount) = Checker.CheckMusic allFiles
let (otherFiles, otherCount) = Checker.CheckOthers allFiles
let conflictNames = Checker.ConflictNames allFiles

CLI.Line "\n"
CLI.Line $"flac:{}  mp3:{}\n"
CLI.Line $"共计:{musicCount}\n"

CLI.Line "其他项目："

for el in musicFiles do CLI.Line el

CLI.Line "\n冲突项目：\n"

for el in conflictNames do CLI.Line el

(*
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
});*)

