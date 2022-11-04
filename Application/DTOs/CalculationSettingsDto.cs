using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CalculationSettingsDto
    {
        public List<int> NodesForWorsening { get; set; }
        public string? PathToRegim { get; set; }
        public int PercentLoad { get; set; }
        public int PercentForWorsening { get; set; }
        public int CountOfImplementations { get; set; }
        public string? Description { get; set; }
    }
}
