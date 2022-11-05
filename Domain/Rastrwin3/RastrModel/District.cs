namespace Domain.Rastrwin3.RastrModel
{
    public class District
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public District(string name, int number)
        {
            Name = name;
            Number = number;
        }
    }
}
