using System.Collections.Generic;
using System.IO;
using MathNet.Spatial.Euclidean;

namespace Robot_Evolution
{
    public static class InitialData
    {
        #region SystemData
        public static string WorkingDirectory { get; set; } = @"\\Fs-project\PS\01_PROJECTS_WD\2515_Lakhta_2_3\07_KM\02_CALC\03_FEA\00_Preliminary_calcs\01_Slab_Beams\Evolution_playing";
        public static string OriginalFile = Path.Combine(WorkingDirectory, "Slab_Slice_00.rtd");
        #endregion SystemData

        #region Sections
        enum Sections
        {
            I40B,
            I30B,
            I50B,
            I70B
        }

        #endregion


        public static List<Node> InitialNodes { get; set; } = new List<Node>();
        public static Dictionary<BoundaryArc, (double R, int Node1, int Node2)> Arcs { get; set; } = 
            new Dictionary<BoundaryArc, (double R, int Node1, int Node2)> 
            { 
                { BoundaryArc.Inner, (16.23, 7, 4) },
                { BoundaryArc.Outer, (30.0, 14, 18) } 
            };
        public static Dictionary<BoundaryArc, (double YMin, double YMax)> ArcsBoundaries { get; set; } = new Dictionary<BoundaryArc, (double YMin, double YMax)>();
        public static Dictionary<BoundaryLine, (int Node1, int Node2)> Lines { get; set; } =
            new Dictionary<BoundaryLine, (int Node1, int Node2)>
            {
                {BoundaryLine.Free1L, (7, 20) },
                {BoundaryLine.Fixed1L, (20, 12) },
                {BoundaryLine.Fixed2L, (12, 13) },
                {BoundaryLine.Fixed3L, (13, 19) },
                {BoundaryLine.Free2L, (19, 14) },
                {BoundaryLine.Free1R, (4, 22) },
                {BoundaryLine.Fixed1R, (22, 9) },
                {BoundaryLine.Fixed2R, (9, 1) },
                {BoundaryLine.Fixed3R, (1, 21) },
                {BoundaryLine.Free2R, (21, 18) }
            };
        public static Dictionary<BoundaryLine, (double YMin, double YMax)> LinesBoundaries { get; set; } = new Dictionary<BoundaryLine, (double YMin, double YMax)>();
        public static Dictionary<BoundaryLine, (double k, double b)> LinesParameters { get; set; } = new Dictionary<BoundaryLine, (double k, double b)>();
        public static List<int> NonMovableNodes { get; set; } = new List<int> { 1, 4, 7, 9, 12, 13, 14, 18, 19, 20, 21, 22 };

        public static List<Point2D> WorkingFieldBoundaryPoints { get; set; } = 
            new List<Point2D> {
                new Point2D(-24.46922, -10.13547),
                new Point2D(-27.71642, -11.48050),
                new Point2D(-29.42360, -5.85271),
                new Point2D(-30.00004, -0.00000),
                new Point2D(-29.42360, 5.85271),
                new Point2D(-27.71642, 11.48050),
                new Point2D(-24.48285, 10.14111),
                new Point2D(-24.96120, 8.98626),
                new Point2D(-20.34180, 7.07284),
                new Point2D(-19.86345, 8.22769),
                new Point2D(-14.98998, 6.20904),
                new Point2D(-15.91328, 3.16534),
                new Point2D(-16.22504, -0.00000),
                new Point2D(-15.91328, -3.16534),
                new Point2D(-14.98998, -6.20904),
                new Point2D(-19.86345, -8.22770),
                new Point2D(-20.34180, -7.07285),
                new Point2D(-24.93395, -8.97497)
            };
        public static Polygon2D WorkingField { get; set; } = new Polygon2D(WorkingFieldBoundaryPoints);
    }
}
