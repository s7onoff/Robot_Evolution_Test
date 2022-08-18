using System.Collections.Generic;
using System;
using System.Linq;
using RobotOM;
using System.Xml.Linq;

namespace Robot_Evolution
{
    public static class OriginalPart
    {
        // public static List<Node> OriginalNodes { get; set; } = new List<Node>();
        // public static List<Beam> OriginalBeams { get; set; } = new List<Beam>();
    }
    public class Instance
    {
        //TODO: Parents
        public int ID { get; set; }
        public int GenerationID { get; set; }
        
        private int freeNodeID = 1;

        public int FreeNodeID
        {
            get { return freeNodeID++; }
            set { freeNodeID = value; }
        }

        private int freeBeamID = 1;

        public int FreeBeamID
        {
            get { return freeBeamID++; }
            set { freeBeamID = value; }
        }

        // public int FreeNodeID { get; set; } = 1;
        // public int FreeBeamID { get; set; } = 1;
        public List<Node> MutatedNodes { get; set; }
        public static List<Node> OriginalNodes { get; set; } = new List<Node>();
        public List<Beam> MutatedBeams { get; set; }
        public static List<Beam> OriginalBeams { get; set; } = new List<Beam>();
        public Result Result { get; set; }



        public Instance()
        {
            MutatedNodes = new List<Node>();
            MutatedBeams = new List<Beam>();
            // OriginalNodes = OriginalPart.OriginalNodes;
            // OriginalBeams = OriginalPart.OriginalBeams;
            Result = new Result();
            //FreeNodeID = this.Nodes().Any() ? this.Nodes().Max(n => n.ID) + 1 : 1;
            //FreeBeamID = this.Beams().Any() ? this.Beams().Max(n => n.ID) + 1 : 1;

            FreeNodeID = OriginalNodes.Any() ? OriginalNodes.Max(n => n.ID) + 1 : 1;
            FreeBeamID = OriginalBeams.Any() ? OriginalBeams.Max(n => n.ID) + 1 : 1;
        }
        public List<Node> Nodes()
        {
            return OriginalNodes.Concat(MutatedNodes).ToList();
        }
        public List<Beam> Beams()
        {
            return OriginalBeams.Concat(MutatedBeams).ToList();
        }
        public void ReadOriginalFromRobot()
        {
            OriginalNodes = new List<Node>();
            OriginalBeams = new List<Beam>();

            var nodesFromRobot = RobotInteraction.ReadNodes();
            foreach (var nodeRobot in nodesFromRobot)
            {
                var node = new Node(nodeRobot.X, nodeRobot.Y, FreeNodeID)
                {
                    RobotID = nodeRobot.Number
                };
                OriginalNodes.Add(node);
                //OriginalPart.OriginalNodes.Add(node);
            }

            var beamsFromRobot = RobotInteraction.ReadBeams();

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
                var beam = new Beam(node1, node2, section, FreeBeamID);
                beam.RobotID = beamRobot.Number;
                OriginalBeams.Add(beam);
                // OriginalPart.OriginalBeams.Add(beam);
            }
        }

        public void Execute()
        {
            RobotInteraction.AddMutations(this);
            RobotInteraction.CheckIntegrity(this, "after adding mutations");
            this.Result = RobotInteraction.CalcResult();
            RobotInteraction.CheckIntegrity(this, "after calculations");
            if (this.GenerationID % EvolutionParameters.SaveEveryNGeneration == 0 || GenerationID <= 5)
            {
                RobotInteraction.SaveAs(this);
            }
            RobotInteraction.CheckIntegrity(this, "after save");
            RobotInteraction.DeleteMutations(this);
            RobotInteraction.CheckIntegrity(this, "after deleting mutations");
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
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Main");
        public int ID { get; set; }
        public int RobotID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Node(double x, double y, int id)
        {
            X = x; Y = y;
            ID = id;
        }

        public List<(Beam beam, int node)> ConnectedBeams(Instance instance)
        {
            var list = instance.Beams().Where(b => b.Node1 == this || b.Node2 == this).Select(b => b.Node1.ID == ID ? (b, 1) : (b, 2)).ToList();
            Logger.Debug("For node {0} of instance {1}, list of connected beams: {2}", ID, instance.ID, string.Join(" | ", list.Select(beam => string.Join("/", beam.b.ID, beam.b.RobotID))));
            return list;
            
        }
    }

    public class Beam
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Main");
        public int ID { get; set; }
        public int RobotID { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public Section Section { get; set; }

        public Beam(Node node1, Node node2, Section section, int id)
        {
            Node1 = node1;
            Node2 = node2;
            Section = section;
            ID = id;
        }
    }

    public class Result
    {
        public double Deflection { get; set; }
        public double Weight { get; set; }
        public double Probability { get; set; }
        public IRobotCalculationStatus RobotCalculationStatus { get; set; }
    }
}
