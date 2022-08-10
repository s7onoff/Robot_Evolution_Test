using RobotOM;

namespace Robot_Evolution
{
    public static class RobotInitialMethods
    {
        public static RobotOM.RobotApplication RobotApplication;
        public static RobotProject Project;
        public static RobotStructure RobotStructure;

        static RobotInitialMethods()
        {
            RobotApplication = new RobotOM.RobotApplication();
            Project = RobotApplication.Project;
            RobotStructure = Project.Structure;
            Project.Open(InitialData.OriginalFile);

            CalcLines();

            Project.Close();
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
    }
}
