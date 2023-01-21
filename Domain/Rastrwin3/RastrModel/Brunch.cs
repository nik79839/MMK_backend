namespace Domain.Rastrwin3.RastrModel
{
    public class Brunch
    {
        public int StartNode { get; set; }
        public int EndNode { get; set; }
        public int ParallelNumber { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public Brunch(int startNode, int endNode, int parallelNumber, string name, int type)
        {
            StartNode = startNode;
            EndNode = endNode;
            ParallelNumber = parallelNumber;
            Name = name;
            Type = type;
        }
    }
}
