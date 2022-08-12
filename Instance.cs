using System.Collections.Generic;
using RobotOM;

namespace Robot_Evolution
{
    public class Instance
    {

        public int id { get; set; }
        public int generationId { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Node> MutatedNodes { get; set; }
        public List<Node> OriginalNodes { get; set; }
        public List<Beam> Beams { get; set; }
        public List<Beam> MutatedBeams { get; set; }
        public List<Beam> OriginalBeams { get; set; }

        public class Genotype
        {

        }

        public class OriginalPart
        {
            // Считать сюда из InitialData все координаты узлов
        }

        public class MutatedPart
        {

        }

        public void ReadOriginalFromRobot()
        {
            var nodesFromRobot = RobotInteraction.ReadNodes();
            var nodeId = 1;
            foreach (var nodeRobot in nodesFromRobot)
            {
                var node = new Node(nodeRobot.X, nodeRobot.Y);
                node.Movable = false;
                node.ID = nodeId;
                OriginalNodes.Add(node);
            }
        }

        public void WriteToRobot()
        {

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
            X = x; Y = y;
        }
    }

    public class Beam
    {
        public int ID { get; set; }
        public bool Movable { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public Section Section { get; set; }

        public Beam(Node node1, Node node2, Section section)
        {
            Node1 = node1;
            Node2 = node2;
            Section = section;
        }
    }
}
