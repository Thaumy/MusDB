module App.Checker

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Security.Cryptography

type Files =
    struct
        val Name: string
        val Md5: string
        val Path: string
        val Type: string
    end

(*type Count =
    struct
        val flac: int
        val mp3: int
        val etc: int
        val total: int
    end*)

type Checker =
    static member ToSHA256 path =

        let file =
            new FileStream(path, FileMode.Open, FileAccess.Read)

        SHA256.Create()
        |> fun it -> it.ComputeHash file
        |> BitConverter.ToString

    static member CheckAll(config: Config) = 0

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
