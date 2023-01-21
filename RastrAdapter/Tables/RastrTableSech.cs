using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastrAdapter.Tables
{
    public class RastrTableSech: RastrTableBase<Sech>
    {
        public RastrCol<int> Num { get; set; }
        public RastrCol<string> Name { get; set; }
        public RastrCol<double> PowerFlow { get; set; }

        public RastrTableSech(ITable table): base(table)
        {
            Num = new((ICol)_table.Cols.Item("ns"));
            Name = new((ICol)_table.Cols.Item("name"));
            PowerFlow = new((ICol)_table.Cols.Item("psech"));
        }

        public override List<Sech> ToList()
        {
            List<Sech> districts = new();
            for (int i = 0; i < Count; i++)
            {
                districts.Add(new Sech(Num[i], Name[i], Math.Round(PowerFlow[i],2)));
            }
            return districts;
        }
    }
}
