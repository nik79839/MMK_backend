using Domain.Rastrwin3.RastrModel;

namespace Domain.Rastrwin3
{
    public class RastrSchemeInfo
    {
        public List<Node> LoadNodes { get; set; }
        public List<Sech> Seches { get; set; }
        public List<District> Districts { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Brunch> Brunches { get; set; }

        public RastrSchemeInfo(List<Node> loadNodes, List<Sech> seches, List<District> districts, List<Node> nodes, List<Brunch> brunches)
        {
            LoadNodes = loadNodes;
            Seches = seches;
            Districts = districts;
            Nodes = nodes;
            Brunches = brunches;
        }
    }
}
