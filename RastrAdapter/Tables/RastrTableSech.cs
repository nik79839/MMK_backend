using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastrAdapter.Tables
{
    public class RastrTableSech
    {
        private readonly ITable _table;
        public int Count { get; }

        public RastrCol<int> Num { get; set; }
        public RastrCol<string> Name { get; set; }
        public RastrCol<double> PowerFlow { get; set; }


        public RastrTableSech(ITable table)
        {
            _table = table;
            Count = _table.Count;
            Num = new((ICol)_table.Cols.Item("ns"));
            Name = new((ICol)_table.Cols.Item("name"));
            PowerFlow = new((ICol)_table.Cols.Item("psech"));
        }

        public List<Sech> ToList()
        {
            List<Sech> districts = new();
            for (int i = 0; i < Count; i++)
            {
                districts.Add(new Sech(Num[i], Name[i], PowerFlow[i]));
            }
            return districts;
        }
    }
}
