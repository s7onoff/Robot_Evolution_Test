using System;
using System.Collections.Generic;
using RobotOM;

namespace Robot_Evolution
{
    public static class RobotInitialMethods
    {
        public static RobotOM.RobotApplication RobotApplication = new RobotOM.RobotApplication();
        public static RobotProject Project = RobotApplication.Project;
        public static RobotStructure RobotStructure = Project.Structure;


        public static void Start()
        {
            Project.Open(InitialData.OriginalFile);

            CalcLines();
            ReadNodes();

        }

        public static void Finish()
        {
            Project.Close();
        }
        
        static void ReadNodes()
        {
            var allNodes = RobotStructure.Nodes.GetAll();

            var listOfNodes = new List<IRobotNode>();
            var listofNodeNumbers = new List<int>();

            for (int i = 1; i < (allNodes.Count) + 1; i++)
            {
                var node = (IRobotNode)allNodes.Get(i);
                if (!node.IsCalc)
                {
                    listOfNodes.Add(node);
                    listofNodeNumbers.Add(node.Number);
                }
            }
        }

        static (double k, double b, double y1, double y2) CalcLineParameters(BoundaryLine line)
        {
            var node1 = InitialData.Lines[line].Node1;
            var node2 = InitialData.Lines[line].Node2;
            var x1 = ((IRobotNode)RobotStructure.Nodes.Get(node1)).X;
            var x2 = ((IRobotNode)RobotStructure.Nodes.Get(node2)).X;
            var y1 = ((IRobotNode)RobotStructure.Nodes.Get(node1)).Y;
            var y2 = ((IRobotNode)RobotStructure.Nodes.Get(node2)).Y;

            var k = (x2 - x1) / (y2 - y1);
            var b = x2 - (k * y2);

            return (k, b, y1, y2);
        }

        static (double node1Y, double node2Y) CalcRadiusBoundaries(BoundaryArc arc)
        {
            var node1 = InitialData.Arcs[arc].Node1;
            var node2 = InitialData.Arcs[arc].Node2;
            var y1 = ((IRobotNode)RobotStructure.Nodes.Get(node1)).Y;
            var y2 = ((IRobotNode)RobotStructure.Nodes.Get(node2)).Y;

            double[] ys = { y1, y2 };

            var yMin = y1 < y2 ? y1 : y2;
            var yMax = y1 > y2 ? y1 : y2;

            return (yMin, yMax);
        }

        static void CalcLines()
        {
            foreach (var line in InitialData.Lines.Keys)
            {
                var result = CalcLineParameters(line);
                InitialData.LinesParameters.Add(line, (result.k, result.b));

                var y1 = result.y1;
                var y2 = result.y1;

                var yMin = y1 < y2 ? y1 : y2;
                var yMax = y1 > y2 ? y1 : y2;

                InitialData.LinesBoundaries.Add(line, (yMin, yMax));
            }

            foreach (var arc in InitialData.Arcs.Keys)
            {
                InitialData.ArcsBoundaries.Add(arc, CalcRadiusBoundaries(arc));
            }
        }

        static Dictionary<int, (double x, double y)> CalcNodesCoords(List<int> nodes)
        {
            var nodesCoordinates = new Dictionary<int, (double x, double y)>();
            foreach (var node in nodes)
            {
                var x = ((IRobotNode)RobotStructure.Nodes.Get(node)).X;
                var y = ((IRobotNode)RobotStructure.Nodes.Get(node)).Y;
                nodesCoordinates.Add(node, (x, y));
            }
            return nodesCoordinates;
        }
    }
}
