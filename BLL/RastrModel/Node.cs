using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.RastrModel
{
    public class Node
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public District? District { get; set; }
    }
}
