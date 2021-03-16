open System
open Config

[<EntryPoint>]
let main _ =

    let (path, database) = Config.Config().GetConfig
    let (usr, pwd, db) = database

    0
