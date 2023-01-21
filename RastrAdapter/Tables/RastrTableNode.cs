using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;

namespace RastrAdapter.Tables
{
    public class RastrTableNode: RastrTableBase<Node>
    {
        public RastrCol<int> Num { get; set; }
        public RastrCol<string> Name { get; set; }
        public RastrCol<double> Pn { get; set; }
        public RastrCol<double> Qn { get; set; }
        public RastrCol<double> Voltage { get; set; }
        public RastrCol<int> AreaNum { get; set; }
        public RastrCol<string> AreaName { get; set; }

        public RastrTableNode(ITable table): base(table)
        {
            Num = new((ICol)_table.Cols.Item("ny"));
            Name = new((ICol)_table.Cols.Item("name"));
            Pn = new((ICol)_table.Cols.Item("pn"));
            Qn = new((ICol)_table.Cols.Item("qn"));
            Voltage = new((ICol)_table.Cols.Item("vras"));
            AreaNum = new((ICol)_table.Cols.Item("na"));
            AreaName = new((ICol)_table.Cols.Item("na_name"));
        }

        public override List<Node> ToList()
        {
            List<Node> nodes = new();
            for (int i = 0; i < Count; i++)
            {
                nodes.Add(new Node(Name[i], Num[i], new District(AreaName[i], AreaNum[i]), Math.Round(Pn[i],2), Math.Round(Voltage[i],2)));
            }
            return nodes;
        }

        public int FindIndexByNum(int ny) => ToList().FindIndex(x => x.Number == ny);

        public override void Set(Node node)
        {
            Pn.Set(index, Pn[index] * 0.98);
            Qn.Set(index, Pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
        }
    }
}
