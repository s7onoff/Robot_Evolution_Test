using System.Collections.Generic;
using RobotOM;

namespace Robot_Evolution
{
    public class Instance
    {

        public List<Node> Nodes { get; set; }
        public List<Node> NonMovableNodes { get; set; }
        public List<Node> MovableNodes { get; set; }
        public List<IRobotBar> Bars { get; set; }

        public class Node
        {
            public IRobotNode robotNode { get; set; }
            public Node(double x, double y)
            {

            }
        }

        public class Beam
        {
            public IRobotBar robotBeam { get; set; }
            public Beam(Node node1, Node node2)
            {

            }
        }

        public class Genotype
        {

        }

        public void ReadFromRobot()
        {
            foreach (int nodeNumber in InitialData.NonMovableNodes)
            {
                this.NonMovableNodes.Add(new Node())
            }
            
        }

    }
}
