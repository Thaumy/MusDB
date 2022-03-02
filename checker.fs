module checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open fsharper.ethType.ethOption
open System.Security.Cryptography
open fsharper.fn
open types
open ui

let showMusic musicName isFlac musicPath =
    let text, color =
        if isFlac then
            ("FLAC", ConsoleColor.Cyan)
        else
            ("MP3 ", ConsoleColor.White)

    inColor ConsoleColor.DarkGray (fun _ -> put "|")
    inColor color (fun _ -> put $" {text} ")
    inColor ConsoleColor.DarkGray (fun _ -> put "|")
    put $"{musicName}"
    inColor ConsoleColor.DarkGray (fun _ -> inRight musicPath)
    newLine ()

let toSha256 path =
    let file =
        new FileStream(path, FileMode.Open, FileAccess.Read)

    SHA256.Create()
    |> fun it -> it.ComputeHash file |> BitConverter.ToString

let getFileSystemInfosList path =
    map id [ for x in DirectoryInfo(path).GetFileSystemInfos() -> x ]

let rec getAllFiles (list: FileSystemInfo list) =
    match list with
    | x :: xs ->
        match x with
        | :? DirectoryInfo as di ->
            getAllFiles (getFileSystemInfosList di.FullName)
            @ getAllFiles xs
        | :? FileInfo as fi ->
            match true with
            | _ when fi.Name.Contains ".flac" ->
                showMusic fi.Name true fi.DirectoryName

                { Name = fi.Name
                  Sha256 = toSha256 fi.FullName
                  Path = fi.DirectoryName
                  Type = "flac" }
                :: getAllFiles xs
            | _ when fi.Name.Contains ".mp3" ->
                showMusic fi.Name false fi.DirectoryName

                { Name = fi.Name
                  Sha256 = toSha256 fi.FullName
                  Path = fi.DirectoryName
                  Type = "mp3" }
                :: getAllFiles xs
            | _ ->
                { Name = fi.Name
                  Sha256 = toSha256 fi.FullName
                  Path = fi.DirectoryName
                  Type = "" }
                :: getAllFiles xs
        | _ -> []
    | [] -> []

let checkConflicts (files: MusicFile list) =
    [ for el in files.GroupBy(fun el -> el.Sha256) do
          if el.Count() > 1 then
              for it in el -> it ]
