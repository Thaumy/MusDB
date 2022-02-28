module checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Security.Cryptography
open fsharper.fn
open types
open ui

 let showMusic musicName isFlac musicPath =
    let (text,color)=if isFlac then ("FLAC",ConsoleColor.Cyan) else ("MP3 ",ConsoleColor.White)
    CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.Put "|")
    CLI.InColor color (fun _ -> CLI.Put $" {text} ")
    CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.Put "|")
    CLI.Put $"{musicName}"
    CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.InRight musicPath)
    CLI.newLine

type Checker =
    static member ToSha256 path =
        let file = new FileStream(path, FileMode.Open, FileAccess.Read)
        SHA256.Create() |> fun it -> it.ComputeHash file |> BitConverter.ToString

    static member GetFileSystemInfosList path = map id [ for x in (new DirectoryInfo(path)).GetFileSystemInfos() do x ]

    static member GetAllFiles (list:FileSystemInfo list) =
        match list with
        | x :: xs -> 
            match x with
            | :? DirectoryInfo as di ->
                Checker.GetAllFiles(Checker.GetFileSystemInfosList di.FullName) @ Checker.GetAllFiles xs
            | :? FileInfo      as fi -> 
                match true with
                | _ when fi.Name.Contains ".flac" ->
                    showMusic fi.Name true fi.DirectoryName
                    { Name = fi.Name
                      Sha256 = Checker.ToSha256(fi.FullName)
                      Path = fi.DirectoryName
                      Type = "flac" } :: Checker.GetAllFiles xs
                | _ when fi.Name.Contains ".mp3"  ->
                    showMusic fi.Name false fi.DirectoryName
                    { Name = fi.Name
                      Sha256 = Checker.ToSha256(fi.FullName)
                      Path = fi.DirectoryName
                      Type = "mp3" } :: Checker.GetAllFiles xs
                | _  ->
                    { Name = fi.Name
                      Sha256 = Checker.ToSha256(fi.FullName)
                      Path = fi.DirectoryName
                      Type = "" } :: Checker.GetAllFiles xs
            | _  ->[]
        | [] -> []

    static member CheckConflicts(files: File list) =
        [ for el in files.GroupBy(fun el -> el.Sha256) do
              if el.Count() > 1 then for it in el -> it ]
