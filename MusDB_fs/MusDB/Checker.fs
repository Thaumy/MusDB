module Checker

open System
open System.Collections.Generic
open System.Linq

open System.IO
open System.Security.Cryptography

type Checker() =
    let toSHA256 path =
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

type Count=
    struct
        val flac:int
        val mp3:int
        val etc:int 
        val total:int
     end

let CheckFiles path (count:Count)=
    let files = new List<TempStruct>()
    
    for el in DirectoryInfo(Path).GetFileSystemInfos() do
        if el is DirectoryInfo then
            files.AddRange(CheckFiles(el.FullName, ref Count));
            //CLI.Line = "\n";
        else
            //CLI.Put(Count.total.ToString().PadLeft(4, '0'));
            let temp = (FileInfo)el
            if temp.Name.Contains(".flac") then
                //CLI.Line = " | flac  " + temp.Name;
                let result = ToSHA256(temp.FullName)
                files.Add(temp.Name, result, temp.DirectoryName, "flac")
    
                //CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                //() => { CLI.Line = temp.DirectoryName; });
    
                count.flac++
                elif temp.Name.Contains(".mp3") then
                //CLI.Line = " |  mp3  " + temp.Name;
    
                let result = ToSHA256(temp.FullName);
                files.Add(temp.Name, result, temp.DirectoryName, "mp3")
    
                //CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                //() => { CLI.Line = temp.DirectoryName; });
    
                count.mp3++
                else
                //CLI.Line = temp.Name
                files.Add(temp.Name, "", temp.DirectoryName, "")
                count.etc++
                        
        count.total++
   
    files

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
