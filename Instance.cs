using System.Collections.Generic;
using System;
using System.Linq;
using RobotOM;
using System.Xml.Linq;

namespace Robot_Evolution
{
    public static class OriginalPart
    {
        public static List<Node> OriginalNodes { get; set; } = new List<Node>();
        public static List<Beam> OriginalBeams { get; set; } = new List<Beam>();
    }
    public class Instance
    {
        //TODO: Parents
        public int id { get; set; }
        public int generationId { get; set; }

        public Instance()
        {
            MutatedNodes = new List<Node>();
            MutatedBeams = new List<Beam>();
            OriginalNodes = OriginalPart.OriginalNodes;
            OriginalBeams = OriginalPart.OriginalBeams;
            Result = new Result();
        }
        public List<Node> Nodes()
        {
            return OriginalNodes.Concat(MutatedNodes).ToList();
        }
        public List<Node> MutatedNodes { get; set; }
        public List<Node> OriginalNodes { get; set; }
        public List<Beam> MutatedBeams { get; set; }
        public List<Beam> OriginalBeams { get; set; }
        public Result Result { get; set; }

        public void ReadOriginalFromRobot()
        {
            OriginalNodes = new List<Node>();
            OriginalBeams = new List<Beam>();
            var nodesFromRobot = RobotInteraction.ReadNodes();
            var nodeId = 1;
            foreach (var nodeRobot in nodesFromRobot)
            {
                var node = new Node(nodeRobot.X, nodeRobot.Y);
                node.Movable = false;
                node.ID = nodeId;
                node.RobotID = nodeRobot.Number;
                OriginalNodes.Add(node);
                OriginalPart.OriginalNodes.Add(node);
            }

            var beamsFromRobot = RobotInteraction.ReadBeams();
            var beamId = 1;
            foreach (var beamRobot in beamsFromRobot)
            {
                var node1RobotNumber = beamRobot.Node1;
                var node2RobotNumber = beamRobot.Node2;
                var node1 = OriginalNodes.Find(_ => _.RobotID.Equals(node1RobotNumber));
                var node2 = OriginalNodes.Find(_ => _.RobotID.Equals(node2RobotNumber));
                Section section;
                var sectionLabel = beamRobot.SectionLabel;
                try
                {
                    section = Sections.SectionsToUse.Find(_ => _.NameInRobotDB.Equals(sectionLabel));
                }
                catch (ArgumentNullException)
                {
                    section = Sections.CreateSection(sectionLabel);
                }
                var beam = new Beam(node1, node2, section);
                beam.ID = beamId;
                beam.RobotID = beamRobot.Number;
                OriginalBeams.Add(beam);
                OriginalPart.OriginalBeams.Add(beam);
            }
        }

        public void Execute()
        {
            RobotInteraction.AddMutations(this);
            this.Result = RobotInteraction.CalcResult();
            RobotInteraction.SaveAs(this);
            RobotInteraction.DeleteMutations(this);
            // TODO: serialize this
        }

        public void Mutate()
        {
            MutationProcess.Mutate(this);
        }

        public void MutateInitial()
        {
            var newNodeMutation = new MutationProcess.NewNodeMutation();
            for (int _ = 0; _ < EvolutionParameters.NewNodesInInitialGeneration; _++)
            {
                newNodeMutation.Action(this);
            }

            var newBeamMutation = new MutationProcess.NewBeamMutation();
            for (int _ = 0; _ < EvolutionParameters.NewBeamsInInitialGeneration; _++)
            {
                newBeamMutation.Action(this);
            }
        }
    }



    public class Node
    {
        public int ID { get; set; }
        public int RobotID { get; set; }
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
        public int RobotID { get; set; }
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

    public class Result
    {
        public double Deflection { get; set; }
        public double Weight { get; set; }
        public double ProbabilityForNext { get; set; }
    }
}
