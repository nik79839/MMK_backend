using ASTRALib;
using Domain.Rastrwin3.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastrAdapter.Tables
{
    public class RastrTableArea
    {
        private readonly ITable _table;
        public int Count { get; }

        public RastrCol<int> Num { get; set; }
        public RastrCol<string> Name { get; set; }


        public RastrTableArea(ITable table)
        {
            _table = table;
            Count = _table.Count;
            Num = new((ICol)_table.Cols.Item("na"));
            Name = new((ICol)_table.Cols.Item("name"));
        }

        public List<District> ToList()
        {
            List<District> districts = new();
            for (int i = 0; i < Count; i++)
            {
                districts.Add(new District(Name[i], Num[i]));
            }
            return districts;
        }
    }
}
