namespace Domain.Rastrwin3.RastrModel
{
    /// <summary>
    /// Сечение
    /// </summary>
    public class Sech
    {
        /// <summary>
        /// Номер сечения
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// Имя сечения
        /// </summary>
        public string SechName { get; set; }
        public double PowerFlow { get; set; }

        /// <summary>
        /// Ветви сечения
        /// </summary>
        public List<Brunch>? Brunches { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="num">Номер сечения</param>
        /// <param name="sechName">Имя сечения</param>
        public Sech(int num, string sechName, double powerFlow)
        {
            Num = num;
            SechName = sechName;
            PowerFlow = powerFlow;
        }
    }
}
