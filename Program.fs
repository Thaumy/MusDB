open System

open fsharper.fn
open types
open ui
open config
open checker

while true do

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

    let localFiles =
        getFileSystemInfosList musicPath |> getLocalFiles

    let localMusicFiles =
        filter (fun x -> x.Type <> "") localFiles
        |> map (fun x -> { x with Path = "" })

    let localOtherFiles =
        filter (fun x -> x.Type = "") localFiles
        |> map (fun x -> { x with Path = "" })

    let localFlacFiles =
        filter (fun x -> x.Type = "flac") localMusicFiles

    let localMp3Files =
        filter (fun x -> x.Type = "mp3") localMusicFiles

    let hashConflictFiles =
        duplicateWhen (fun x y -> x.Sha256 = y.Sha256) localMusicFiles

    inCoordinate 40 (Console.CursorTop - 1) (fun _ -> inColor ConsoleColor.Green (fun _ -> line "DONE"))

    line $"\n共计 : {localMusicFiles.Length}    FLAC : {localFlacFiles.Length}    MP3 : {localMp3Files.Length}\n"

    inColor ConsoleColor.Yellow (fun _ -> line "存在其他项目 :")

    ignore
    <| map
        (fun x ->
            put x.Name
            inColor ConsoleColor.DarkGray (fun _ -> inRight x.Path)
            newLine ())
        localOtherFiles


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


    let schemaMusicFiles = schema.files ()

    let localOnlyMusicFiles =
        (leftJoinNoInner localMusicFiles schemaMusicFiles)

    let schemaOnlyMusicFiles =
        (leftJoinNoInner schemaMusicFiles localMusicFiles)

    ignore
    <| if schemaOnlyMusicFiles.Length <> 0 then
           inColor ConsoleColor.Red (fun _ -> line "\n发现已登记曲目缺失 :")

           map (fun x -> line x.Name) schemaOnlyMusicFiles
           |> ignore
       else
           inColor ConsoleColor.Green (fun _ -> line "\n已登记的曲目全部存在。")

    ignore
    <| if localOnlyMusicFiles.Length <> 0 then
           inColor ConsoleColor.Green (fun _ -> line "\n发现新增曲目 :")

           ignore
           <| map
               (fun x ->
                   put x.Name
                   inColor ConsoleColor.DarkGray (fun _ -> inRight x.Path)
                   newLine ())
               localOnlyMusicFiles

           inColor
               ConsoleColor.Green
               (fun _ ->
                   match pause "\ny：将新增数据录入数据库，其他键：重新开始检查\n" with
                   | "y" ->
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
                           localOnlyMusicFiles

                       inColor ConsoleColor.White (fun _ -> pause "\n按任意键重新开始检查\n" |> ignore)
                   | _ -> ())
       else
           inColor ConsoleColor.Gray (fun _ -> line "\n无新增曲目。")
