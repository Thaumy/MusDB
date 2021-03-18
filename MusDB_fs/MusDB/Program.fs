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
    CLI.Line el.Name

CLI.Line "\n冲突项目：\n"

for name in conflictNames do
    CLI.Line name


CLI.InColor
    ConsoleColor.Green
    (fun _ ->
        CLI.Pause "\n\a检查完成，按任意键匹配数据"
        CLI.Line "")

let musicInDb = database.GetAll

CLI.Line "以下项目在本地文件中不存在："


CLI.Line "以下项目在本地文件中不存在："

let musicOnlyInDb = List<File>()

for el in musicInDb do
    let isExist =
        let mutable exist = false

        for it in musicFiles do
            if el.Name = it.Name
               && el.Type = it.Type
               && el.Sha256 = it.Sha256 then
                exist <- true
            else
                ()

        exist

    if isExist = false then
        musicOnlyInDb.Add el

for el in musicOnlyInDb do
    CLI.Line el.Name

CLI.Line "以下项目在本地文件中不存在："

let musicOnlyInLocal = List<File>()

for el in musicFiles do
    let isExist =
        let mutable exist = false

        for it in musicInDb do
            if el.Name = it.Name
               && el.Type = it.Type
               && el.Sha256 = it.Sha256 then
                exist <- true
            else
                ()

        exist

    if isExist = false then
        musicOnlyInLocal.Add el

for el in musicOnlyInLocal do
    CLI.Line el.Name


CLI.InColor
    ConsoleColor.Green
    (fun _ ->
        CLI.Pause "\n按任意键将本地新增数据添加到数据库。"
        CLI.Line "\n")


for el in musicOnlyInLocal do
    database.Add el |> ignore


CLI.InColor ConsoleColor.Green (fun _ -> CLI.Pause("\n\a任务完成，任意键退出。"))
