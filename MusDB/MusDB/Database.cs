using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaterLibrary.MySQL;

using MySql.Data.MySqlClient;
using MySql;
using MusDB;
using WaterLibrary.Util;
using System.Security.Cryptography;

namespace MusDB
{
    class Database
    {
        private readonly MySqlManager MySqlManager;

        public Database(string User, string PWD, string Database)
        {
            MySqlManager = new MySqlManager(new("localhost", 3306, User, PWD), Database);
        }

        public int GetCount()
        {
            return Convert.ToInt32(MySqlManager.GetKey("SELECT COUNT(*) FROM statistics"));
        }

        public void Update(string Name, string MD5, string file_type)
        {
            MySqlManager.DoInConnection(conn =>
            {
                using MySqlCommand MySqlCommand = new MySqlCommand
                {
                    CommandText = string.Format("INSERT INTO statistics (name,md5,file_type) VALUES (\"{0}\",\"{1}\",\"{2}\");",
                    Name, MD5, file_type
                    ),

                    Connection = conn,

                    /* 开始事务 */
                    Transaction = conn.BeginTransaction()
                };

                if (MySqlCommand.ExecuteNonQuery() == 1)
                {
                    /* 指向表修改1行数据，拷贝表删除1行数据 */
                    MySqlCommand.Transaction.Commit();
                    return true;
                }
                else
                {
                    MySqlCommand.Transaction.Rollback();
                    return false;
                }
            });
        }
    }
}
