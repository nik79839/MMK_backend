namespace Domain.Rastrwin3
{
    /// <summary>
    /// Файл формата rg2
    /// </summary>
    public class RastrFile
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата последнего изменения файла
        /// </summary>
        public DateTime LastModified { get; set; }

        public RastrFile(string name, DateTime lastModified)
        {
            Name = name;
            LastModified = lastModified;
        }
    }
}
