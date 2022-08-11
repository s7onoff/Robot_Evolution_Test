using System.Collections.Generic;
using RobotOM;

namespace Robot_Evolution
{
    public class Instance
    {

        public int id { get; set; }
        public int generationId { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Node> NonMovableNodes { get; set; }
        public List<Node> MovableNodes { get; set; }
        public List<IRobotBar> Bars { get; set; }

        public class Genotype
        {

        }

        public class StablePart
        {
            // Считать сюда из InitialData все координаты узлов
        }

        public class MutatedPart
        {

        }

        public void ReadFromRobot()
        {
            foreach (int nodeNumber in InitialData.NonMovableNodes)
            {
                // this.NonMovableNodes.Add(new Node());
            }
            
        }

    }



    public class Node
    {
        public int ID { get; set; }
        public bool Movable { get; set; }
        public bool OnContourArc { get; set; }
        public bool OnContourLine { get; set; }
        public bool HasSupport { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Node(double x, double y)
        {

        }
    }

    public class Beam
    {
        public int ID { get; set; }
        public bool Movable { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        //public Section Section { get; set; }

        public Beam(Node node1, Node node2)
        {

        }
    }
}
