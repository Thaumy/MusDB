using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaterLibrary.MySQL;

using MySql.Data.MySqlClient;
using MySql.Data;
using MusDB;
using WaterLibrary.Utils;
using System.Security.Cryptography;

using System.Data;

namespace MusDB
{
    class Database
    {
        private readonly MySqlManager MySqlManager;

        /// <summary>
        /// 公有构造
        /// </summary>
        public Database(string User, string PWD, string Database)
        {
            MySqlManager = new MySqlManager(new("localhost", 3306, User, PWD), Database);
        }

        /// <summary>
        /// 取得计数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return Convert.ToInt32(MySqlManager.GetKey("SELECT COUNT(*) FROM statistics"));
        }

        /// <summary>
        /// 取得所有数据
        /// </summary>
        /// <returns></returns>
        public List<(string Name, string MD5, string file_type)> GetAll()
        {
            var result = MySqlManager.GetTable("SELECT * FROM statistics").Rows;

            List<(string Name, string MD5, string file_type)> List = new();

            foreach (DataRow Row in result)
            {
                (string Name, string MD5, string file_type) it;

                it.Name = Convert.ToString(Row["Name"]);
                it.MD5 = Convert.ToString(Row["MD5"]);
                it.file_type = Convert.ToString(Row["file_type"]);

                List.Add(it);
            }
            return List;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="MD5"></param>
        /// <param name="file_type"></param>
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
