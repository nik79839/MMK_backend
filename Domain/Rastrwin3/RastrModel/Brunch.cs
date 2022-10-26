﻿namespace Domain.Rastrwin3.RastrModel
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