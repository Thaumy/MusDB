module App.Checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Security.Cryptography
open App
open CLI
open Config


type File =
    { Name: string
      Path: string
      Type: string
      Sha256: string }

type Count =
    { mutable Flac: int
      mutable Mp3: int
      mutable Others: int
      mutable SumAll: int }

type Checker =
    static member ToSha256 path =

        let file =
            new FileStream(path, FileMode.Open, FileAccess.Read)

        SHA256.Create()
        |> fun it -> it.ComputeHash file
        |> BitConverter.ToString

    static member CheckAll musicPath =

        let rec checker path =
            let files = new List<File>()

            let count =
                { Mp3 = 0
                  Flac = 0
                  Others = 0
                  SumAll = 0 }

            for el in (new DirectoryInfo(path)).GetFileSystemInfos() do
                match el with
                | :? DirectoryInfo as di ->
                    let (childFile, childCount) = checker di.FullName

                    files.AddRange(childFile)
                    count.Mp3 <- count.Mp3 + childCount.Mp3
                    count.Flac <- count.Flac + childCount.Flac
                    count.Others <- count.Others + childCount.Others
                    count.SumAll <- count.SumAll + childCount.SumAll

                    CLI.Line ""
                | :? FileInfo as fi ->
                    CLI.Put(count.SumAll.ToString().PadLeft(4, '0'))

                    if fi.Name.Contains(".flac") then
                        CLI.Line(" | flac  " + fi.Name)

                        { Name = fi.Name
                          Sha256 = Checker.ToSha256(fi.FullName)
                          Path = fi.DirectoryName
                          Type = "flac" }
                        |> files.Add

                        count.Flac <- count.Flac + 1

                        CLI.InPosition
                            (Console.WindowWidth / 5 * 3)
                            (Console.CursorTop - 1)
                            (fun _ -> CLI.Line fi.DirectoryName)

                    elif fi.Name.Contains(".mp3") then
                        CLI.Line(" |  mp3  " + fi.Name)

                        { Name = fi.Name
                          Sha256 = Checker.ToSha256(fi.FullName)
                          Path = fi.DirectoryName
                          Type = "mp3" }
                        |> files.Add

                        count.Mp3 <- count.Mp3 + 1

                        CLI.InPosition
                            (Console.WindowWidth / 5 * 3)
                            (Console.CursorTop - 1)
                            (fun _ -> CLI.Line fi.DirectoryName)

                    else
                        CLI.Line fi.Name

                        { Name = fi.Name
                          Sha256 = ""
                          Path = fi.DirectoryName
                          Type = "" }
                        |> files.Add

                        count.Others <- count.Others + 1

                    count.SumAll <- count.SumAll + 1
                | _ -> ()

            (files, count)

        checker musicPath

    static member CheckMusic(files: List<File>) =
        let musicFile =
            [ for el in files do
                  if el.Type <> "" then el ]

        (musicFile, musicFile.Length)

    static member CheckOthers(files: List<File>) =
        let otherFile =
            [ for el in files do
                  if el.Type = "" then el ]

        (otherFile, otherFile.Length)

    static member ConflictNames(files: List<File>) =
        let conflicts =
            [ for el in files.GroupBy(fun el -> el.Sha256) do
                  if el.Count() > 1 then el ]

        [ for el in conflicts do
              for it in el -> it.Name ]
