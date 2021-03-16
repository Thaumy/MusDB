module Checker

open System
open System.Collections.Generic
open System.Linq

open System.IO
open System.Security.Cryptography

type Checker =
    member this.ToSHA256 path =
        let sha256 = SHA256.Create()

        let file =
            new FileStream(path, FileMode.Open, FileAccess.Read)

        BitConverter.ToString(sha256.ComputeHash(file))

type TempStruct =
    struct
        val Name: string
        val MD5: string
        val path: string
        val fileType: string
    end

type Count =
    struct
        val flac: int
        val mp3: int
        val etc: int
        val total: int
    end


let CheckETC (files: List<TempStruct>) =
    [ for el in files do
          if el.fileType = "" then el.Name ]

let CheckConflict (files: List<TempStruct>) =
    let conflicts =
        [ for el in files.GroupBy(fun el -> el.MD5) do
              if el.Count() > 1 then el ]

    let result = new List<List<string>>()

    if conflicts.Any() then

        for el in conflicts do
            let it = new List<string>()

            for foo in el do
                it.Add(foo.Name)

            result.Add(it)

    result
