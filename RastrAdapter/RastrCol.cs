using ASTRALib;
using System;

namespace RastrAdapter
{
    /// <summary>
    /// Класс для получения значения столбца по индексу
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RastrCol<T>
    {
        private readonly ICol _col;

        public RastrCol(ICol col)
        {
            _col = col;
        }
        /// <summary>
        /// Индекс
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return (T)_col.ZN[index];
            }
        }

        public void Set(int index, T value) => _col.set_ZN(index, value);

        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            T k;
            while (true)
            {
                try
                {
                    k = (T)_col.ZN[i];
                }
                catch
                {
                    break;
                }
                yield return k;
                i++;
            }
        }
    }
}
