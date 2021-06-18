module App.Database

open System
open WaterLibrary.MySql
open MySql.Data.MySqlClient
open App
open Mod
open Util
open Checker


type Database(user, pwd, database) =
    let mySqlManager =
        new MySqlManager(new MySqlConnMsg("localhost", 3306, user, pwd), database)

    member this.GetCount =
        mySqlManager.GetKey "SELECT COUNT(*) FROM statistics"
        |> Convert.ToInt32

    member this.GetAll =
        let result =
            [ for el in (mySqlManager.GetTable "SELECT * FROM statistics")
                  .Rows do
                  el ]

        let f (row: Data.DataRow) =
            { Name = row.["name"].ToString()
              Type = row.["type"].ToString()
              Sha256 = row.["sha256"].ToString() 
              Path = "" }

        map f result

    member this.Add file =
        mySqlManager.DoInConnection
            (fun conn ->
                let mySqlCommand =
                    new MySqlCommand(
                        CommandText =
                            "INSERT INTO statistics (name,type,sha256) VALUES "
                            + $"""("{file.Name}","{file.Type}","{file.Sha256}");""",
                        Connection = conn,
                        Transaction = conn.BeginTransaction()
                    )

                if mySqlCommand.ExecuteNonQuery() = 1 then
                    mySqlCommand.Transaction.Commit()
                    true
                else
                    mySqlCommand.Transaction.Rollback()
                    false)
