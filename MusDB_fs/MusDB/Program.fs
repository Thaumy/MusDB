open System
open App
open Checker
open Config
open CLI
open Database


CLI.Line "初始化MusDB数据库服务..................[ ]"
let (path, database) = Config("./config.json").GetConfig

CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "O"))

CLI.Line "统计信息...............................[ ]"

let result = Database(database).GetCount

CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "O"))

CLI.Line $"当前数据库记录存留：{result}"

CLI.Pause "按任意键收集数据\n"
