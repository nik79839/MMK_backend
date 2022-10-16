using Data.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Rastrwin3
{
    public class RastrSchemeInfo
    {
        public List<Node> LoadNodes { get; set; }
        public List<Sech> Seches { get; set; }
        public List<District> Districts { get; set; }

        public RastrSchemeInfo(List<Node> loadNodes, List<Sech> seches, List<District> districts)
        {
            LoadNodes = loadNodes;
            Seches = seches;
            Districts = districts;
        }
    }
}
