module App.Config

open System.IO
open System.Text
open Newtonsoft.Json.Linq


type Config(configPath) =
    member _this.GetConfig =
        let jsonString =
            try //尝试获取配置文件
                File.ReadAllText(configPath, Encoding.UTF8)
            with _ -> //找不到配置文件则创建
                use fileStream =
                    new FileStream(configPath, FileMode.Create, FileAccess.Write)

                use streamWriter = new StreamWriter(fileStream)

                streamWriter.WriteLine "{}"
                |> streamWriter.Close
                |> fileStream.Close

                File.ReadAllText(configPath, Encoding.UTF8)

        let jObject = JObject.Parse jsonString

        (jObject.["path"], jObject.["database"])
        ||> fun path database ->
                (path, (database.["usr"].ToString(), database.["pwd"].ToString(), database.["db"].ToString()))
        ||> fun path database -> (path, database)
