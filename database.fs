module database

open System
open MySqlManaged
open MySql.Data.MySqlClient
open types
open fsharper.op
open fsharper.fn
open fsharper.moreType.Ord
open fsharper.ethType.ethResult
open checker


type Database(msg, schema, table) =
    let managed = MySqlManaged(msg, schema)

    member this.GetCount =
        managed.getFstVal $"SELECT COUNT(*) FROM {table}"
        |> Convert.ToInt32

    member this.GetAll =
        let result =
            [ for el in
                  (managed.getTable "SELECT * FROM statistics")
                      .unwarp()
                      .Rows do
                  el ]

        let f (row: Data.DataRow) =
            { Name = row.["name"].ToString()
              Type = row.["type"].ToString()
              Sha256 = row.["sha256"].ToString()
              Path = "" }

        map f result

    member this.Add(file: File) =
        managed.execute
            $"INSERT INTO statistics (name,type,sha256) VALUES \
                  (\"{file.Name}\",\"{file.Type}\",\"{file.Sha256}\");"
        >>= fun f ->
                match f <| eq 1 with
                | 1 -> Ok true
                | _ -> Ok false
        |> unwarp
