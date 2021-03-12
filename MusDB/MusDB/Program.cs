using System;
using System.IO;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MusDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string configPath = "./config.json";
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(configPath, System.Text.Encoding.UTF8);
            }
            catch//找不到配置文件则创建
            {
                FileStream fileStream = new(configPath, FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new(fileStream);
                streamWriter.WriteLine("{}");
                streamWriter.Close();
                fileStream.Close();

                jsonString = File.ReadAllText(configPath, System.Text.Encoding.UTF8);
            }
            var jObject = JObject.Parse(jsonString);
            var path = jObject["path"].ToString();
            var databaseNode = jObject["database"];
            (string db, string usr, string pwd) = (databaseNode["db"].ToString(), databaseNode["usr"].ToString(), databaseNode["pwd"].ToString());

            CLI.Line = "初始化MusDB数据库服务..................[ ]";
            Database DB = new(usr, pwd, db);
            CLI.InPosition(40, Console.CursorTop - 1,
                        () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

            CLI.Line = "统计信息...............................[ ]";
            var result = DB.GetCount();
            CLI.InPosition(40, Console.CursorTop - 1,
                       () => { CLI.InColor(ConsoleColor.Green, () => Console.WriteLine("O")); });

            CLI.Line = $"当前数据库记录存留：{result}";
            CLI.Pause("按任意键收集数据");
            CLI.Line = "\n";

            (int flac, int mp3, int etc, int total) Count = (0, 0, 0, 0);

            List<(string Name, string MD5, string path, string file_type)> AllFiles = Checker.CheckFiles(path, ref Count);

            CLI.Line = "\n";
            CLI.Line = $"flac:{Count.flac}  mp3:{Count.mp3}\n";
            CLI.Line = $"共计:{Count.total}\n";

            CLI.Line = "其他项目：";
            foreach (var el in Checker.CheckETC(AllFiles))
            {
                CLI.Line = el;
            }

            CLI.Line = "\n冲突项目：\n";
            foreach (var el in Checker.CheckConflict(AllFiles))
            {
                foreach (var it in el)
                {
                    CLI.Line = it;
                }
                CLI.Line = "\n";
            }

            CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n\a检查完成，按任意键匹配数据"); CLI.Line = "\n"; });

            List<(string Name, string MD5, string path, string file_type)> MusicFiles = (from el in AllFiles where el.file_type != "" select (el.Name, el.MD5, el.path, el.file_type)).ToList();
            var MusicInDB = DB.GetAll();

            CLI.Line = "以下项目在本地文件中不存在：";
            foreach (var el in MusicInDB)
            {
                int i = MusicFiles.FindIndex(x => x.Name == el.Name && x.MD5 == el.MD5 && x.file_type == el.file_type);//剔除数据库中已有记录
                if (i != -1)
                {
                    MusicFiles.RemoveAt(i);
                }
                else
                {
                    CLI.Line = el.Name;
                }
            }
            CLI.Line = "以下项目在数据库中不存在（本地新增）：";
            foreach (var el in MusicFiles)
            {
                CLI.Line = el.Name;
                CLI.InPosition(Console.WindowWidth / 5 * 3, Console.CursorTop - 1,
                           () => { CLI.Line = el.path; });
            }
            CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n按任意键将本地新增数据同步到数据库。"); CLI.Line = "\n"; });

            foreach (var el in MusicFiles)
            {
                DB.Update(el.Name, el.MD5, el.file_type);
                CLI.Line = "已添加：" + el.Name;
            }

            CLI.InColor(ConsoleColor.Green, () => { CLI.Pause("\n\a任务完成，任意键退出。"); });
        }
    }
}
