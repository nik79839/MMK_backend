using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repairs
{
    public class Repair
    {
        public string EquipmentName { get; set; }

        public DateTime StartTimeRepair { get; set; }

        public DateTime EndTimeRepair { get; set; }
        public string Status { get; set; }
        public string Object { get; set; }
    }
}
