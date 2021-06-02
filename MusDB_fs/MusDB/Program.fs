open System
open App
open CLI
open Config
open Checker
open Database


CLI.Line "查找配置文件信息 ......................[    ]"

let currPath =
    $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}config.json"

let (musicPath, databaseConfig) = Config(currPath).GetConfig
CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "DONE"))

CLI.Line "联络到数据库 ..........................[    ]"
let database = Database(databaseConfig)
CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "DONE"))

CLI.Line ""
CLI.Line $"当前数据库记录存留 : {database.GetCount}"

CLI.Put "Press "
CLI.InColor ConsoleColor.Green (fun _ -> CLI.Put "ENTER")
CLI.Pause " to collect data\n" |> ignore

let mutable index = 1

let (allFiles, allCount) =
    Checker.CheckAll
        musicPath
        (fun _ ->
            CLI.Put(index.ToString().PadLeft(4, ' '))
            index <- index + 1)

CLI.Line "数据聚合 ..............................[    ]"
let (musicFiles, musicCount) = Checker.CheckMusic allFiles
let (otherFiles, otherCount) = Checker.CheckOthers allFiles
let conflictFiles = Checker.CheckConflicts allFiles
CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "DONE"))

CLI.Line ""
CLI.Line $"共计 : {musicCount}    FLAC : {allCount.Flac}    MP3 : {allCount.Mp3}\n"
CLI.Line "其他项目 :"

for el in otherFiles do
    CLI.Line el.Name

CLI.Line ""
CLI.Line "冲突项目 :"

for el in conflictFiles do
    CLI.Line el.Name

CLI.InColor
    ConsoleColor.Green
    (fun _ ->
        CLI.Line ""
        CLI.Pause "检查完成，按任意键匹配数据" |> ignore
        CLI.Line "")

let musicInDb : list<File> = database.GetAll

CLI.Line "以下项目仅在数据库存在 :"

for el in Checker.LeftOnly musicInDb musicFiles do
    CLI.Line el.Name

CLI.Line "以下项目仅在本地存在 :"

for el in Checker.LeftOnly musicFiles musicInDb do
    CLI.Line el.Name


CLI.Line ""
CLI.InColor ConsoleColor.Green (fun _ -> CLI.Pause "按任意键将新增数据录入数据库" |> ignore)
CLI.Line ""

for el in Checker.LeftOnly musicFiles musicInDb do
    let success = database.Add el

    let (color, text) =
        if success then
            (ConsoleColor.Green, "Added : ")
        else
            (ConsoleColor.Red, "Failed: ")

    CLI.InColor color (fun _ -> CLI.Put text)
    CLI.Line $"{el.Name}"

CLI.Line ""
CLI.InColor ConsoleColor.Green (fun _ -> CLI.Pause "任务完成，任意键退出。" |> ignore)
