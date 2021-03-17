module App.Checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Security.Cryptography
open App
open Config
open CLI


type Files =
    { Name: string
      Md5: string
      Path: string
      Type: string }

type Count =
    { mutable Flac: int
      mutable Mp3: int
      mutable Etc: int
      mutable SumAll: int }

type Checker =
    static member ToSHA256 path =

        let file =
            new FileStream(path, FileMode.Open, FileAccess.Read)

        SHA256.Create()
        |> fun it -> it.ComputeHash file
        |> BitConverter.ToString




    static member CheckAll(config: Config) : List<Files> =
        let (path, database) = config.GetConfig
        let files = new List<Files>()

        let info =
            (new DirectoryInfo(path)).GetFileSystemInfos()

        let rec checker path count : List<Files> =
            for el in info do
                match el with
                | :? DirectoryInfo as _di ->
                    files.AddRange(checker path count)
                    CLI.Line ""
                | :? FileInfo as fi ->
                    CLI.Put(count.SumAll.ToString().PadLeft(4, '0'))

                    if fi.Name.Contains(".flac") then
                        CLI.Line(" | flac  " + fi.Name)

                        let it =
                            { Name = fi.Name
                              Md5 = Checker.ToSHA256(fi.FullName)
                              Path = fi.DirectoryName
                              Type = "flac" }

                        files.Add it

                        CLI.InPosition
                            (Console.WindowWidth / 5 * 3)
                            (Console.CursorTop - 1)
                            (fun _ -> CLI.Line fi.DirectoryName)

                        count.Flac <- count.Flac + 1
                    elif fi.Name.Contains(".mp3") then
                        CLI.Line(" |  mp3  " + fi.Name)

                        let it =
                            { Name = fi.Name
                              Md5 = Checker.ToSHA256(fi.FullName)
                              Path = fi.DirectoryName
                              Type = "mp3" }

                        files.Add it

                        CLI.InPosition
                            (Console.WindowWidth / 5 * 3)
                            (Console.CursorTop - 1)
                            (fun _ -> CLI.Line fi.DirectoryName)

                        count.Mp3 <- count.Mp3 + 1

                    else

                        CLI.Line fi.Name

                        let it =
                            { Name = fi.Name
                              Md5 = ""
                              Path = fi.DirectoryName
                              Type = "" }

                        files.Add it

                        count.Etc <- count.Etc + 1

            count.SumAll <- count.SumAll + 1

            files

        files

    //tring Path, ref (int flac, int mp3, int etc, int total) Count

    static member CheckOthers(files: List<Files>) =
        [ for el in files do
              if el.Type = "" then el.Name ]

    static member CheckConflict(files: List<Files>) =
        let conflicts =
            [ for el in files.GroupBy(fun el -> el.Md5) do
                  if el.Count() > 1 then el ]

        let result = new List<List<string>>()

        if conflicts.Any() then
            for el in conflicts do
                let it = new List<string>()

                for foo in el do
                    it.Add foo.Name

                result.Add it

        result
