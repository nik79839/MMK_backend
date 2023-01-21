using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastrAdapter.Tables
{
    public class RastrTableVetv: RastrTableBase<Brunch>
    {
        public RastrCol<int> StartNode { get; set; }
        public RastrCol<int> EndNode { get; set; }
        public RastrCol<int> Parallel { get; set; }
        public RastrCol<string> VetvName { get; set; }
        public RastrCol<int> VetvType { get; set; }
        public RastrCol<double> Current { get; set; }

        public RastrTableVetv(ITable table): base(table)
        {
            StartNode = new((ICol)_table.Cols.Item("ip"));
            EndNode = new((ICol)_table.Cols.Item("iq"));
            Parallel = new((ICol)_table.Cols.Item("np"));
            VetvName = new((ICol)_table.Cols.Item("name"));
            VetvType = new((ICol)_table.Cols.Item("tip"));
            Current = new((ICol)_table.Cols.Item("i_max"));
        }

        public override List<Brunch> ToList()
        {
            List<Brunch> brunches = new();
            for (int i = 0; i < Count; i++)
            {
                if (VetvType[i] == 0)
                {
                    brunches.Add(new Brunch(StartNode[i], EndNode[i], Parallel[i], VetvName[i], VetvType[i], Math.Round(Current[i])));
                }
            }
            return brunches;
        }
    }
}
