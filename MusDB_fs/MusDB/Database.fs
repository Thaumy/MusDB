module App.Database

open System
open System.Collections.Generic
open WaterLibrary.MySql
open MySql.Data.MySqlClient
open App
open Checker

open type File


type Database(user, pwd, database) =
    let mySqlManager =
        new MySqlManager(new MySqlConnMsg("localhost", 3306, user, pwd), database)

    member this.GetCount =
        mySqlManager.GetKey "SELECT COUNT) FROM statistics"
        |> Convert.ToInt32

    member this.GetAll =
        let result =
            mySqlManager.GetTable "SELECT * FROM statistics"
            |> fun it -> it.Rows

        let files = new List<File>()


        for row in result do
            { Name = row.["Name"].ToString()
              Path = ""
              Type = row.["file_type"].ToString()
              Sha256 = row.["MD5"].ToString() }
            |> files.Add

        files

    member this.Add file =
        mySqlManager.DoInConnection
            (fun conn ->
                let mySqlCommand =
                    new MySqlCommand(
                        CommandText =
                            "INSERT INTO statistics (name,md5,file_type) VALUES "
                            + $"(\"{file.Name}\",\"{file.Sha256}\",\"{file.Type}\");",
                        Connection = conn,
                        Transaction = conn.BeginTransaction()
                    )

                if mySqlCommand.ExecuteNonQuery() = 1 then
                    mySqlCommand.Transaction.Commit |> ignore
                    true
                else
                    mySqlCommand.Transaction.Rollback |> ignore
                    false)
