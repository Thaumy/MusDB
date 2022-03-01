module config

open System.IO
open System.Text
open Newtonsoft.Json.Linq
open MySqlManaged


let getConfig configPath =
    let jsonString =
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

    let root = JObject.Parse jsonString
    let path = root.Value<string> "path"
    let database = root.["database"]
    let schema = database.Value<string> "schema"

    let msg =
        { DataSource = database.Value<string> "datasource"
          Port = database.Value<uint16> "port"
          User = database.Value<string> "usr"
          Password = database.Value<string> "pwd" }

    let table = database.Value<string> "table"

    (path, msg, schema, table)
