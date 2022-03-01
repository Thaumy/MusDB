open System
open types
open fn
open fsharper.fn
open ui
open config
open checker
open database

while true do

    //Eq predicate
    let p a b =
        if a.Name = b.Name
           && a.Sha256 = b.Sha256
           && a.Type = b.Type then
            true
        else
            false

    let hashp a b =
        if a.Sha256 = b.Sha256 then
            true
        else
            false

    CLI.InColor ConsoleColor.Yellow (fun _ -> CLI.Line "正在运行MUSDB音乐统计工作流\n")
    CLI.Line "查找配置文件信息 ......................[    ]"

    let currPath =
        $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}config.json"

    let (musicPath, msg, schema, table) = getConfig currPath

    CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "DONE"))

    CLI.Line "联络到数据库 ..........................[    ]"
    let database = Database(msg, schema, table)
    CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "DONE"))

    CLI.Line $"\n当前数据库曲目登记数量 : {database.GetCount}"


    let _ =
        CLI.Put "\npress "
        CLI.InColor ConsoleColor.Green (fun _ -> CLI.Put "ENTER")
        CLI.Pause " to collect data\n"

    CLI.Line "数据聚合 ..............................[    ]"

    let allInfoList = Checker.GetFileSystemInfosList musicPath
    let allFiles = Checker.GetAllFiles allInfoList

    let musicFiles = filter (fun x -> x.Type <> "") allFiles
    let otherFiles = filter (fun x -> x.Type = "") allFiles

    let flacFiles =
        filter (fun x -> x.Type = "flac") musicFiles

    let mp3Files =
        filter (fun x -> x.Type = "mp3") musicFiles

    let hashConflictFiles = concat <| sames hashp musicFiles

    CLI.InPosition 40 (Console.CursorTop - 1) (fun _ -> CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "DONE"))

    CLI.Line $"\n共计 : {musicFiles.Length}    FLAC : {flacFiles.Length}    MP3 : {mp3Files.Length}\n"

    let _ =
        CLI.InColor ConsoleColor.Yellow (fun _ -> CLI.Line "存在其他项目 :")

        map
            (fun x ->
                CLI.Put x.Name
                CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.InRight x.Path)
                CLI.newLine)
            otherFiles

    let _ =
        if hashConflictFiles.Length <> 0 then
            CLI.InColor ConsoleColor.Red (fun _ -> CLI.Line "\n存在冲突曲目 :")

            map
                (fun x ->
                    CLI.Put x.Name
                    CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.InRight x.Path)
                    CLI.newLine)
                hashConflictFiles
            |> ignore
        else
            ()

    let musicInDb = database.GetAll

    let musicLocalOnly = (leftOnly p musicFiles musicInDb)
    let musicDbOnly = (leftOnly p musicInDb musicFiles)

    let _ =
        if musicDbOnly.Length <> 0 then
            CLI.InColor ConsoleColor.Red (fun _ -> CLI.Line "\n发现已登记曲目缺失 :")

            map (fun x -> CLI.Line x.Name) musicDbOnly
            |> ignore
        else
            CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "\n已登记的曲目全部存在。")

    let _ =
        if musicLocalOnly.Length <> 0 then
            CLI.InColor ConsoleColor.Green (fun _ -> CLI.Line "\n发现新增曲目 :")

            map
                (fun x ->
                    CLI.Put x.Name
                    CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.InRight x.Path)
                    CLI.newLine)
                musicLocalOnly
            |> ignore

            CLI.InColor ConsoleColor.Green (fun _ -> CLI.Pause "\n按任意键将新增数据录入数据库\n" |> ignore)
        else
            CLI.InColor ConsoleColor.Gray (fun _ -> CLI.Line "\n无新增曲目。")


    let _ =
        map
            (fun x ->
                let isSuccess = database.Add x

                CLI.InColor
                    (if isSuccess then
                         ConsoleColor.Green
                     else
                         ConsoleColor.Red)
                    (fun _ ->
                        CLI.Put(
                            if isSuccess then
                                "Added : "
                            else
                                "Failed: "
                        ))

                CLI.Line $"{x.Name}")
            musicLocalOnly

    CLI.InColor ConsoleColor.White (fun _ -> CLI.Pause "\n按任意键重新开始检查\n" |> ignore)
