module config

open System.IO
open System.Text
open Newtonsoft.Json.Linq
open MySqlManaged
open fsharper.enhType

let useConfig configPath =
    let config =
        try //尝试获取配置文件
            File.ReadAllText(configPath, Encoding.UTF8)
        with
        | _ -> //找不到配置文件则创建
            use fileStream =
                new FileStream(configPath, FileMode.Create, FileAccess.Write)

            use streamWriter = new StreamWriter(fileStream)

            streamWriter.WriteLine "{}"
            |> streamWriter.Close
            |> fileStream.Close

            File.ReadAllText(configPath, Encoding.UTF8)

    let root = JObject.Parse config
    let database = root.["database"]

    schema.schemaName <- Some <| database.Value<string> "schema"

    schema.msg <-
        Some
        <| { DataSource = database.Value<string> "datasource"
             Port = database.Value<uint16> "port"
             User = database.Value<string> "usr"
             Password = database.Value<string> "pwd" }

    schema.table <- Some <| database.Value<string> "table"

    root.Value<string> "path"