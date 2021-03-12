using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusDB
{
    class CLI
    {
        /// <summary>
        /// 私有构造
        /// </summary>
        private CLI() { }

        /// <summary>
        /// 使用指定颜色进行操作
        /// </summary>
        /// <param name="Color"></param>
        /// <param name="todo"></param>
        public static void InColor(ConsoleColor Color, Action todo)
        {
            Console.ForegroundColor = Color;
            todo();
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 不换行打印
        /// </summary>
        /// <param name="text"></param>
        public static void Put(string text) => Console.Write(text);

        /// <summary>
        /// 暂停，任意键继续
        /// </summary>
        public static void Pause() => Console.ReadKey();
        /// <summary>
        /// 暂停，任意键继续。在这之前将打印一行文本，换行。
        /// </summary>
        /// <param name="text"></param>
        public static void Pause(string text)
        {
            Console.WriteLine(text); Console.ReadKey();
        }

        /// <summary>
        /// 换行打印
        /// </summary>
        /// <param name="text"></param>
        public static string Line
        {
            set
            {
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// 在指定位置进行操作
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="todo"></param>
        public static void InPosition(int left, int right, Action todo)
        {
            Console.SetCursorPosition(left, right);
            todo();
        }
    }
}
