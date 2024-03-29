﻿namespace Domain.Rastrwin3.RastrModel
{
    public class Node
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public double Pn { get; set; }
        public double Voltage { get; set; }
        public District? District { get; set; }

        public Node(string name, int number, District? district, double pn, double voltage)
        {
            Name = name;
            Number = number;
            District = district;
            Pn = pn;
            Voltage = voltage;
        }
    }
}
