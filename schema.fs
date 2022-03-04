module schema

open System
open System.Data
open MySqlManaged
open fsharper.moreType.GenericPipable
open types
open fsharper.op
open fsharper.fn
open fsharper.moreType.Ord
open fsharper.enhType


let mutable msg: Option'<MySqlConnMsg> = None
let mutable schemaName: Option'<string> = None
let mutable table: Option'<string> = None

let mutable managed: Option'<MySqlManaged> = None

let private FetchManagedPipeline =
    let activate () =
        let m =
            MySqlManaged(msg.unwarp (), schemaName.unwarp ())

        managed <- Some m
        m

    let activated () = managed.unwarp ()

    GenericStatePipe(activate = activate, activated = activated)
        .build ()

let private Managed () = FetchManagedPipeline.invoke ()

let count () =
    Managed()
        .getFstVal $"SELECT COUNT(*) FROM {table.unwarp ()}"
    |> unwarp
    |> unwarp
    |> Convert.ToInt32

let files () =
    Managed()
        .getTable $"SELECT * FROM {table.unwarp ()}"
    >>= fun t ->
            [ for r in t.Rows -> r ]
            |> map
                (fun (r: DataRow) ->
                    { Name = r.["name"].ToString()
                      Type = r.["type"].ToString()
                      Sha256 = r.["sha256"].ToString()
                      Path = "" })
            |> Ok
    |> unwarp

let add (file: MusicFile) =
    Managed()
        .execute $"INSERT INTO statistics (name,type,sha256) VALUES \
                  (\"{file.Name}\",\"{file.Type}\",\"{file.Sha256}\");"
    >>= fun f ->
            match f <| eq 1 with
            | 1 -> Ok true
            | _ -> Ok false
    |> unwarp
