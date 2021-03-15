﻿module Database

open System
open System.Collections.Generic
open System.Linq
open System.Text
open System.Threading.Tasks
open WaterLibrary.MySql
open MySql.Data.MySqlClient
open System.Data

type Database(user, pwd, database) =
    let mySqlManager =
        new MySqlManager(new MySqlConnMsg("localhost", 3306, user, pwd), database)

    let getKey =
        mySqlManager.GetKey("SELECT COUNT(*) FROM statistics")

    let getCount = Convert.ToInt32(getKey)

    let getAll =
        let result =
            mySqlManager
                .GetTable(
                    "SELECT * FROM statistics"
                )
                .Rows

        let list = new List<(string * string * string)>()

        for row in result do
            list.Add((row.["Name"].ToString(), row.["MD5"].ToString(), row.["file_type"].ToString()))

        list
