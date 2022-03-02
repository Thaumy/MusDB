module schema

open System
open MySqlManaged
open MySql.Data.MySqlClient
open types
open fsharper.op
open fsharper.fn
open fsharper.moreType.Ord
open fsharper.ethType.ethResult
open checker


type Schema(msg, schema, table) =
    let managed = MySqlManaged(msg, schema)

    member this.getCount =
        managed.getFstVal $"SELECT COUNT(*) FROM {table}"
        |> unwarp
        |> unwarp
        |> Convert.ToInt32

    member this.getAll =
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

    member this.add(file: File) =
        managed.execute
            $"INSERT INTO statistics (name,type,sha256) VALUES \
                  (\"{file.Name}\",\"{file.Type}\",\"{file.Sha256}\");"
        >>= fun f ->
                match f <| eq 1 with
                | 1 -> Ok true
                | _ -> Ok false
        |> unwarp
