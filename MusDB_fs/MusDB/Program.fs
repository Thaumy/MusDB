open System
open System.Linq
open System.Collections.Generic
open App
open CLI
open Config
open Checker
open Database


CLI.Line "初始化MusDB数据库服务..................[ ]"
let (musicPath, databaseConfig) = Config("./config.json").GetConfig

CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "O"))

CLI.Line "统计信息...............................[ ]"

let database = Database(databaseConfig)

CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "O"))

CLI.Line $"当前数据库记录存留：{database.GetCount}"

CLI.Pause "按任意键收集数据\n"

let (allFiles, allCount) = Checker.CheckAll musicPath
let (musicFiles, musicCount) = Checker.CheckMusic allFiles
let (otherFiles, otherCount) = Checker.CheckOthers allFiles
let conflictNames = Checker.ConflictNames allFiles

CLI.Line "\n"
CLI.Line $"flac:{allCount.Flac}  mp3:{allCount.Mp3}\n"
CLI.Line $"共计:{musicCount}\n"

CLI.Line "其他项目："

for el in musicFiles do
    CLI.Line el

CLI.Line "\n冲突项目：\n"

for el in conflictNames do
    CLI.Line el


CLI.InColor
    ConsoleColor.Green
    (fun _ ->
        CLI.Pause "\n\a检查完成，按任意键匹配数据"
        CLI.Line "")

let musicInDb = database.GetAll

CLI.Line "以下项目在本地文件中不存在："
