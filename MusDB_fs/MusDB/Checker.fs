module App.Checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Security.Cryptography
open App
open CLI


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

    static member CheckAll musicPath todo =

        let rec checker path todo =

            let files = new List<File>()

            let count =
                { Mp3 = 0
                  Flac = 0
                  Others = 0
                  SumAll = 0 }

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
                        { Name = fi.Name
                          Sha256 = ""
                          Path = fi.DirectoryName
                          Type = "" }
                        |> files.Add

                        count.Others <- count.Others + 1

                    count.SumAll <- count.SumAll + 1
                | _ -> ()

            CLI.Line ""
            (files, count)

        checker musicPath todo

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

    static member CheckConflicts(files: List<File>) =
        [ for el in files.GroupBy(fun el -> el.Sha256) do
              if el.Count() > 1 then
                  for it in el -> it ]

    static member LeftOnly L R =
        let rec exist it list =
            match list with
            | head :: tail ->
                if it.Name = head.Name
                   && it.Type = head.Type
                   && it.Sha256 = head.Sha256 then
                    true
                else
                    exist it tail
            | [] -> false

        let rec loop list R=
            [ match list with
              | head :: tail ->
                  if not (exist head R) then
                      yield head
                  else
                      yield! loop tail R
              | [] -> yield! [] ]

        loop L R