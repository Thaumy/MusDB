module Config

open MySql.Data
open Newtonsoft.Json.Linq
open System.IO
open System.Text

type Config(x: int, y: int) =
    let getConfig =
        let configPath = "./config.json" //配置文件搜索路径

        let jsonString =
            try //尝试获取配置文件
                File.ReadAllText(configPath, Encoding.UTF8)
            with _ -> //找不到配置文件则创建
                let fileStream =
                    new FileStream(configPath, FileMode.Create, FileAccess.Write)

                let streamWriter = new StreamWriter(fileStream)
                streamWriter.WriteLine("{}")
                streamWriter.Close()
                fileStream.Close()

                File.ReadAllText(configPath, Encoding.UTF8)

        let jObject = JObject.Parse(jsonString)

        let path = jObject.["path"].ToString()
        let databaseNode = jObject.["database"] //database节点

        let database =
            (databaseNode.["usr"].ToString(), databaseNode.["pwd"].ToString(), databaseNode.["db"].ToString())

        (path, database)
