using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastrAdapter.Tables
{
    public class RastrTableVetv
    {
        private readonly ITable _table;
        public int Count { get; }

        public RastrCol<int> StartNode { get; set; }
        public RastrCol<int> EndNode { get; set; }
        public RastrCol<int> Parallel { get; set; }
        public RastrCol<string> VetvName { get; set; }
        public RastrCol<int> VetvType { get; set; }
        public RastrCol<double> CurrentMax { get; set; }

        public RastrTableVetv(ITable table)
        {
            _table = table;
            Count = _table.Count;
            StartNode = new((ICol)_table.Cols.Item("ip"));
            EndNode = new((ICol)_table.Cols.Item("iq"));
            Parallel = new((ICol)_table.Cols.Item("np"));
            VetvName = new((ICol)_table.Cols.Item("name"));
            VetvType = new((ICol)_table.Cols.Item("tip"));
            CurrentMax = new((ICol)_table.Cols.Item("i_max"));
        }

        public List<Brunch> ToList()
        {
            List<Brunch> brunches = new();
            for (int i = 0; i < Count; i++)
            {
                if (VetvType[i] == 0)
                {
                    brunches.Add(new Brunch(StartNode[i], EndNode[i], Parallel[i], VetvName[i], VetvType[i]));
                }
            }
            return brunches;
        }

        public int FindIndexByName(string name) => ToList().FindIndex(x => x.Name == name);
    }
}
