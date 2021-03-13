using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;


namespace MusDB
{
    public static class Config
    {
        public static void GetConfig(out string path, out (string usr, string pwd, string db) database)
        {
            string configPath = "./config.json";//配置文件搜索路径
            string jsonString;
            try//尝试获取配置文件
            {
                jsonString = File.ReadAllText(configPath, Encoding.UTF8);
            }
            catch//找不到配置文件则创建
            {
                FileStream fileStream = new(configPath, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new(fileStream);
                streamWriter.WriteLine("{}");
                streamWriter.Close();
                fileStream.Close();

                jsonString = File.ReadAllText(configPath, Encoding.UTF8);
            }

            var jObject = JObject.Parse(jsonString);

            path = jObject["path"].ToString();
            var databaseNode = jObject["database"];//database节点
            database = (databaseNode["usr"].ToString(), databaseNode["pwd"].ToString(), databaseNode["db"].ToString());
        }
    }
}
