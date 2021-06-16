module App.Checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Security.Cryptography
open App
open Util
open Mod
open CLI


type Checker =
    static member ToSha256 path =

        let file = new FileStream(path, FileMode.Open, FileAccess.Read)

        SHA256.Create() |> fun it -> it.ComputeHash file |> BitConverter.ToString

    static member CheckAll musicPath todo =

        let rec checker path todo =

            let files = new List<File>()

            let count = { Mp3 = 0; Flac = 0; Others = 0; SumAll = 0 }

            for el in (new DirectoryInfo(path)).GetFileSystemInfos() do
                match el with
                | :? DirectoryInfo as di ->

                    let (childFile, childCount) = checker di.FullName todo

                    files.AddRange(childFile)
                    count.Mp3 <- count.Mp3 + childCount.Mp3
                    count.Flac <- count.Flac + childCount.Flac
                    count.Others <- count.Others + childCount.Others
                    count.SumAll <- count.SumAll + childCount.SumAll

                | :? FileInfo as fi ->

                    if fi.Name.Contains ".flac" then
                        todo ()
                        CLI.Put " |"
                        CLI.InColor ConsoleColor.Cyan (fun _ -> CLI.Put " FLAC  ")
                        CLI.Put fi.Name

                        { Name = fi.Name
                          Sha256 = Checker.ToSha256(fi.FullName)
                          Path = fi.DirectoryName
                          Type = "flac" }
                        |> files.Add

                        count.Flac <- count.Flac + 1

                        CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.InRight fi.DirectoryName)
                        CLI.Line ""
                    elif fi.Name.Contains ".mp3" then
                        todo ()
                        CLI.Put(" |  MP3  " + fi.Name)

                        { Name = fi.Name
                          Sha256 = Checker.ToSha256(fi.FullName)
                          Path = fi.DirectoryName
                          Type = "mp3" }
                        |> files.Add

                        count.Mp3 <- count.Mp3 + 1

                        CLI.InColor ConsoleColor.DarkGray (fun _ -> CLI.InRight fi.DirectoryName)
                        CLI.Line ""
                    else
                        { Name = fi.Name; Sha256 = ""; Path = fi.DirectoryName; Type = "" } |> files.Add

                        count.Others <- count.Others + 1

                    count.SumAll <- count.SumAll + 1
                | _ -> ()

            CLI.Line ""
            (files, count)

        checker musicPath todo

    static member GetFileSystemInfosList path = map id [ for x in (new DirectoryInfo(path)).GetFileSystemInfos() do x ]

    static member GetAllFiles (list:FileSystemInfo list) =
        match list with
        | x :: xs -> 
            match x with
            | :? DirectoryInfo as di ->
                Checker.GetAllFiles(Checker.GetFileSystemInfosList di.FullName)
            | :? FileInfo      as fi -> 
                match true with
                | _ when fi.Name.Contains ".flac" ->
                    { Name = fi.Name
                      Sha256 = Checker.ToSha256(fi.FullName)
                      Path = fi.DirectoryName
                      Type = "flac" } :: Checker.GetAllFiles xs
                | _ when fi.Name.Contains ".mp3"  ->
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

    static member GetMusicFiles files = filter (fun x -> x.Type <> "") files
    static member GetOtherFiles files = filter (fun x -> x.Type = "") files

    static member CheckConflicts(files: File list) =
        [ for el in files.GroupBy(fun el -> el.Sha256) do
              if el.Count() > 1 then for it in el -> it ]
