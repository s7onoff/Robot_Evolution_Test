using System;
using System.Collections.Generic;
using RobotOM;

namespace Robot_Evolution
{
    public static class RobotInteraction
    {
        public static RobotOM.RobotApplication RobotApplication = new RobotOM.RobotApplication();
        public static RobotProject Project = RobotApplication.Project;
        public static RobotStructure RobotStructure = Project.Structure;

        public static void Start()
        {
            Project.Open(InitialData.OriginalFile);

            // CalcLines();
            ReadNodes();
            ReadBeams();
        }

        public static void Finish()
        {
            Project.Close();
        }

        public static void CalcResult()
        {
            //TODO: Implement
        }

        #region MethodsToRead

        public static List<(int Number, double X, double Y)> ReadNodes()
        {
            var allNodes = RobotStructure.Nodes.GetAll();
            var nodesCollection = new List<(int Number, double X, double Y)>();

            for (int i = 1; i < (allNodes.Count) + 1; i++)
            {
                var node = (IRobotNode)allNodes.Get(i);
                if (!node.IsCalc)
                {
                    nodesCollection.Add((node.Number, node.X, node.Y));
                }
            }
            return nodesCollection;
        }

        public static List<(int Number, int Node1, int Node2, string SectionLabel)> ReadBeams()
        {
            var allBeams = RobotStructure.Bars.GetAll();

            var listOfBeams = new List<IRobotNode>();
            var listOfBeamNumbers = new List<int>();
            var beamsCollection = new List<(int Number, int Node1, int Node2, string SectionLabel)>();

            for (int i = 1; i < (allBeams.Count) + 1; i++)
            {
                var beam = (IRobotBar)allBeams.Get(i);
                beamsCollection.Add((beam.Number, beam.StartNode, beam.EndNode, (beam.GetLabel(IRobotLabelType.I_LT_BAR_SECTION)).Name));
            }
            return beamsCollection;
        }

        public static void ReadContour()
        {
            //TODO: Maybe realize
            //var contour = RobotStructure.
        }

        #endregion MethodsToRead

        #region MethodsToWrite

        public static void DeleteMutations()
        {
            var nodesSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_NODE);
            nodesSelector.AddText(""); //TODO:
            RobotStructure.Nodes.DeleteMany(nodesSelector);

            var barssSelector = RobotStructure.Selections.Create(IRobotObjectType.I_OT_BAR);
            barssSelector.AddText(""); //TODO:           
            RobotStructure.Bars.DeleteMany(nodesSelector);

        }

        public static void AddMutations(Instance instance)
        {
            foreach (var node in instance.MutatedNodes)
            {
                var n = RobotStructure.Nodes.FreeNumber;
                var x = node.X;
                var y = node.Y;
                RobotStructure.Nodes.Create(n, x, y, 5000);
            }

            foreach (var beam in instance.MutatedBeams)
            {
                var n = RobotStructure.Nodes.FreeNumber;
                var x = node.X;
                var y = node.Y;
                RobotStructure.Nodes.Create(n, x, y, 5000);
            }
        }

        #endregion MethodsToWrite

        //TODO: Убрать всё
        #region OldMethods

        // TODO: Переписать, мы все равно считываем все стержни методом ReadBeams
        // Или вообще убрать отсюда, потому что потом внутренними структурами быстрее по каждой балке найти и посчитать всё
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

        // TODO: Переместить отсюда?
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

        // TODO: Переместить отсюда?
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

        // TODO: убрать, есть считывание всех узлов
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
        #endregion OldMethods
    }
}