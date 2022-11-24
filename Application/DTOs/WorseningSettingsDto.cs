using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class WorseningSettingsDto
    {
        public int NodeNumber { get; set; }
        public int? MaxValue { get; set; }

        public WorseningSettingsDto(int nodeNumber, int? maxValue)
        {
            NodeNumber = nodeNumber;
            MaxValue = maxValue;
        }
    }
}
