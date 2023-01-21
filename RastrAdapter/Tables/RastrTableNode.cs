using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastrAdapter.Tables
{
    public class RastrTableNode
    {
        private readonly ITable _table;
        public int Count { get; }

        public RastrCol<int> Num { get; set; }
        public RastrCol<string> Name { get; set; }
        public RastrCol<double> Pn { get; set; }
        public RastrCol<double> Qn { get; set; }
        public RastrCol<double> Voltage { get; set; }
        public RastrCol<int> AreaNum { get; set; }
        public RastrCol<string> AreaName { get; set; }


        public RastrTableNode(ITable table)
        {
            _table = table;
            Count = _table.Count;
            Num = new((ICol)_table.Cols.Item("ny"));
            Name = new((ICol)_table.Cols.Item("name"));
            Pn = new((ICol)_table.Cols.Item("pn"));
            Qn = new((ICol)_table.Cols.Item("qn"));
            Voltage = new((ICol)_table.Cols.Item("vras"));
            AreaNum = new((ICol)_table.Cols.Item("na"));
            AreaName = new((ICol)_table.Cols.Item("na_name"));
        }

        public List<Node> ToList()
        {
            List<Node> nodes = new();
            for (int i = 0; i < Count; i++)
            {
                nodes.Add(new Node(Name[i], Num[i], new District(AreaName[i], AreaNum[i]), Pn[i]));
            }
            return nodes;
        }

        public int FindIndexByNum(int ny) => ToList().FindIndex(x => x.Number == ny);
    }
}
