using ASTRALib;

namespace RastrAdapter.Tables
{
    public abstract class RastrTableBase<T>
    {
        protected readonly ITable _table;
        public int Count { get; }

        protected RastrTableBase(ITable table)
        {
            _table = table;
            Count = _table.Count;
        }

        public abstract List<T> ToList();
        public virtual void Set(T value)
        { 

        }
    }
}
