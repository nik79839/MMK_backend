using ASTRALib;
using Domain.Rastrwin3.RastrModel;

namespace RastrAdapter.Tables
{
    public class RastrTableArea: RastrTableBase<District>
    {
        public RastrCol<int> Num { get; set; }
        public RastrCol<string> Name { get; set; }


        public RastrTableArea(ITable table): base(table)
        {
            Num = new((ICol)_table.Cols.Item("na"));
            Name = new((ICol)_table.Cols.Item("name"));
        }

        public override List<District> ToList()
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
