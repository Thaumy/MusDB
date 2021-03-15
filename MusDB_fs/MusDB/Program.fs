open System
open Config

[<EntryPoint>]
let main _ =
    let config = Config.Config()
    let (path, database) = config.GetConfig()
    let (usr, pwd, db) = database

    0
