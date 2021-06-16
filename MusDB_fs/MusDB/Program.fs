open System
open App
open Mod
open Util
open CLI
open Config
open Checker
open Database


CLI.Line "查找配置文件信息 ......................[    ]"

let currPath =
    $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}config.json"

let (musicPath, databaseConfig) = getConfig currPath

CLI.InPosition 40 (Console.CursorTop - 1) (CLI.InColor ConsoleColor.Green (CLI.Line "DONE"))

CLI.Line "联络到数据库 ..........................[    ]"
let database = Database(databaseConfig)
CLI.InPosition 40 (Console.CursorTop - 1) (CLI.InColor ConsoleColor.Green (CLI.Line "DONE"))

CLI.Line $"\n当前数据库记录存留 : {database.GetCount}"


let _ =
    CLI.Put "Press "
    CLI.InColor ConsoleColor.Green (CLI.Put "ENTER")
    CLI.Pause " to collect data\n"

let allInfoList = Checker.GetFileSystemInfosList musicPath
let allFiles = Checker.GetAllFiles allInfoList

CLI.Line "数据聚合 ..............................[    ]"

let musicFiles = Checker.GetMusicFiles allFiles
let otherFiles = Checker.GetOtherFiles allFiles

let musicCount = musicFiles.Length
let flacFiles = filter (fun x -> x.Type = "flac") musicFiles
let mp3Files = filter (fun x -> x.Type = "mp3") musicFiles

let conflictFiles = Checker.CheckConflicts allFiles
CLI.InPosition 40 (Console.CursorTop - 1) (CLI.InColor ConsoleColor.Green (CLI.Line "DONE"))

CLI.Line $"\n共计 : {musicCount}    FLAC : {flacFiles.Length}    MP3 : {mp3Files.Length}\n"

let _ = 
    CLI.Line "其他项目 :"
    map (fun x->CLI.Line x.Name) otherFiles

CLI.Line ""
CLI.Line "冲突项目 :"

for el in conflictFiles do
    CLI.Line el.Name

CLI.InColor ConsoleColor.Green (CLI.Pause "检查完成，按任意键匹配数据\n" |> ignore)

let musicInDb = database.GetAll

let _ =
    CLI.Line "以下项目仅在数据库存在 :"
    map (fun x -> CLI.Line x.Name) (leftOnly musicInDb musicFiles)

let _ =
    CLI.Line "以下项目仅在本地存在 :"
    map (fun x -> CLI.Line x.Name) (leftOnly musicFiles musicInDb)


CLI.InColor ConsoleColor.Green (CLI.Pause "\n按任意键将新增数据录入数据库\n" |> ignore)

let _ = map (fun x ->
    let isSuccess = database.Add x
    CLI.InColor 
        (if isSuccess then ConsoleColor.Green else ConsoleColor.Red) 
        (CLI.Put (if isSuccess then "Added : " else "Failed: "))

    CLI.Line $"{x.Name}") (leftOnly musicFiles musicInDb)

CLI.InColor ConsoleColor.Green (CLI.Pause "任务完成，任意键退出。" |> ignore)
