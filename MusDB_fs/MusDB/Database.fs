module App.Database

open System
open System.Collections.Generic
open WaterLibrary.MySql
open MySql.Data.MySqlClient


type Database(user, pwd, database) =
    let mySqlManager =
        new MySqlManager(new MySqlConnMsg("localhost", 3306, user, pwd), database)

    member this.GetCount =
        mySqlManager.GetKey "SELECT COUNT(*) FROM statistics"
        |> Convert.ToInt32

    member this.GetAll =
        let result =
            mySqlManager.GetTable "SELECT * FROM statistics"
            |> fun it -> it.Rows

        let list = new List<(string * string * string)>()

        for row in result do
            (row.["Name"].ToString(), row.["MD5"].ToString(), row.["file_type"].ToString())
            |> list.Add

        list

    member this.Update name md5 fileType =
        mySqlManager.DoInConnection
            (fun conn ->
                let mySqlCommand =
                    new MySqlCommand(
                        CommandText =
                            $"INSERT INTO statistics (name,md5,file_type) VALUES (\"{name}\",\"{md5}\",\"{fileType}\");",
                        Connection = conn,
                        Transaction = conn.BeginTransaction()
                    )

                if mySqlCommand.ExecuteNonQuery() = 1 then
                    mySqlCommand.Transaction.Commit |> ignore
                    true
                else
                    mySqlCommand.Transaction.Rollback |> ignore
                    false

                )
