using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.RastrModel
{
    public class Brunch
    {
        public int StartNode { get; set; }
        public int EndNode { get; set; }
        public int ParallelNumber { get; set; }

        public Brunch(int startNode, int endNode, int parallelNumber)
        {
            StartNode = startNode;
            EndNode = endNode;
            ParallelNumber = parallelNumber;
        }
    }
}
