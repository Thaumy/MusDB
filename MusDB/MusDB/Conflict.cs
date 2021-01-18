using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusDB
{
    class Conflict
    {
        private Conflict() { }

        public static void Check(List<(string Name, string MD5)> Music)
        {
            var conflicts = from el in (from el in Music group el by el.MD5) where el.Count() > 1 select el;
            if (conflicts.Count() > 0)
            {
                CLI.Line("冲突项目：");
                foreach (var el in conflicts)
                {
                    foreach (var it in el)
                    {
                        CLI.Line(it.Name);
                    }
                    CLI.Line();
                }
            }
        }
    }
}
