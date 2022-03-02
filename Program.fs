open System

open fsharper.fn
open fn
open types
open ui
open config
open checker

while true do

    //Eq predicate
    let p a b =
        a.Name = b.Name
        && a.Sha256 = b.Sha256
        && a.Type = b.Type

    let hashp a b = a.Sha256 = b.Sha256

    inColor ConsoleColor.Yellow (fun _ -> line "正在运行MUSDB音乐统计工作流\n")
    line "查找配置文件信息 ......................[    ]"



    let musicPath =
        useConfig $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}config.json"

    inCoordinate 40 (Console.CursorTop - 1) (fun _ -> inColor ConsoleColor.Green (fun _ -> line "DONE"))

    line "联络到数据库 ..........................[    ]"

    inCoordinate 40 (Console.CursorTop - 1) (fun _ -> inColor ConsoleColor.Green (fun _ -> line "DONE"))

    line $"\n当前数据库曲目登记数量 : {schema.count ()}"

    put "\npress "
    inColor ConsoleColor.Green (fun _ -> put "ENTER")
    pause " to collect data\n" |> ignore

    line "已开始数据聚合 ........................[    ]"
    inCoordinate 40 (Console.CursorTop - 1) (fun _ -> inColor ConsoleColor.Green (fun _ -> line "DONE"))
    
    let allInfoList = getFileSystemInfosList musicPath
    let allFiles = getAllFiles allInfoList

    let musicFiles = filter (fun x -> x.Type <> "") allFiles
    let otherFiles = filter (fun x -> x.Type = "") allFiles

    let flacFiles =
        filter (fun x -> x.Type = "flac") musicFiles

    let mp3Files =
        filter (fun x -> x.Type = "mp3") musicFiles

    let hashConflictFiles = concat <| sames hashp musicFiles

    inCoordinate 40 (Console.CursorTop - 1) (fun _ -> inColor ConsoleColor.Green (fun _ -> line "DONE"))

    line $"\n共计 : {musicFiles.Length}    FLAC : {flacFiles.Length}    MP3 : {mp3Files.Length}\n"

    inColor ConsoleColor.Yellow (fun _ -> line "存在其他项目 :")

    ignore
    <| map
        (fun x ->
            put x.Name
            inColor ConsoleColor.DarkGray (fun _ -> inRight x.Path)
            newLine ())
        otherFiles


    ignore
    <| if hashConflictFiles.Length <> 0 then
           inColor ConsoleColor.Red (fun _ -> line "\n存在冲突曲目 :")

           map
               (fun x ->
                   put x.Name
                   inColor ConsoleColor.DarkGray (fun _ -> inRight x.Path)
                   newLine ())
               hashConflictFiles
           |> ignore
       else
           ()


    let musicInSchema = schema.files ()

    let musicLocalOnly = (leftOnly p musicFiles musicInSchema)
    let musicDbOnly = (leftOnly p musicInSchema musicFiles)

    ignore
    <| if musicDbOnly.Length <> 0 then
           inColor ConsoleColor.Red (fun _ -> line "\n发现已登记曲目缺失 :")

           map (fun x -> line x.Name) musicDbOnly |> ignore
       else
           inColor ConsoleColor.Green (fun _ -> line "\n已登记的曲目全部存在。")

    ignore
    <| if musicLocalOnly.Length <> 0 then
           inColor ConsoleColor.Green (fun _ -> line "\n发现新增曲目 :")

           ignore
           <| map
               (fun x ->
                   put x.Name
                   inColor ConsoleColor.DarkGray (fun _ -> inRight x.Path)
                   newLine ())
               musicLocalOnly

           inColor ConsoleColor.Green (fun _ -> pause "\n按任意键将新增数据录入数据库\n" |> ignore)
       else
           inColor ConsoleColor.Gray (fun _ -> line "\n无新增曲目。")

    ignore
    <| map
        (fun x ->
            let isSuccess = schema.add x

            inColor
                (if isSuccess then
                     ConsoleColor.Green
                 else
                     ConsoleColor.Red)
                (fun _ ->
                    put (
                        if isSuccess then
                            "Added : "
                        else
                            "Failed: "
                    ))

            line $"{x.Name}")
        musicLocalOnly

    inColor ConsoleColor.White (fun _ -> pause "\n按任意键重新开始检查\n" |> ignore)
