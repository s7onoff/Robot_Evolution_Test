using System.Collections.Generic;
using System.IO;
using NetTopologySuite.Geometries;

namespace Robot_Evolution
{
    public static class InitialData
    {
        #region SystemData
        public static string WorkingDirectory { get; set; } = @"C:\Users\Sesemenov\Documents\!tmp\2022-08-15";
        // public static string WorkingDirectory { get; set; } = @"\\Fs-project\PS\01_PROJECTS_WD\2515_Lakhta_2_3\07_KM\02_CALC\03_FEA\00_Preliminary_calcs\01_Slab_Beams\Evolution_playing";
        //public static string WorkingDirectory { get; set; } = @"C:\Users\flood\Documents\Evolution_playing";
        public static string OriginalFile = Path.Combine(WorkingDirectory, "Slab_Slice_00.rtd");
        #endregion SystemData

        #region old

        // public static List<Node> InitialNodes { get; set; } = new List<Node>();
        // public static List<int> NonMovableNodes { get; set; } = new List<int> { 1, 4, 7, 9, 12, 13, 14, 18, 19, 20, 21, 22 };
        //public static Dictionary<BoundaryArc, (double R, int Node1, int Node2)> Arcs { get; set; } = 
        //    new Dictionary<BoundaryArc, (double R, int Node1, int Node2)> 
        //    { 
        //        { BoundaryArc.Inner, (16.23, 7, 4) },
        //        { BoundaryArc.Outer, (30.0, 14, 18) } 
        //    };
        //public static Dictionary<BoundaryArc, (double YMin, double YMax)> ArcsBoundaries { get; set; } = new Dictionary<BoundaryArc, (double YMin, double YMax)>();
        //public static Dictionary<BoundaryLine, (int Node1, int Node2)> Lines { get; set; } =
        //    new Dictionary<BoundaryLine, (int Node1, int Node2)>
        //    {
        //        {BoundaryLine.Free1L, (7, 20) },
        //        {BoundaryLine.Fixed1L, (20, 12) },
        //        {BoundaryLine.Fixed2L, (12, 13) },
        //        {BoundaryLine.Fixed3L, (13, 19) },
        //        {BoundaryLine.Free2L, (19, 14) },
        //        {BoundaryLine.Free1R, (4, 22) },
        //        {BoundaryLine.Fixed1R, (22, 9) },
        //        {BoundaryLine.Fixed2R, (9, 1) },
        //        {BoundaryLine.Fixed3R, (1, 21) },
        //        {BoundaryLine.Free2R, (21, 18) }
        //    };
        //public static Dictionary<BoundaryLine, (double YMin, double YMax)> LinesBoundaries { get; set; } = new Dictionary<BoundaryLine, (double YMin, double YMax)>();
        //public static Dictionary<BoundaryLine, (double k, double b)> LinesParameters { get; set; } = new Dictionary<BoundaryLine, (double k, double b)>();

        //public static Coordinate[] WorkingFieldBoundaryPoints { get; set; } = new Coordinate[] {
        //        new Coordinate(-24.46922, -10.13547),
        //        new Coordinate(-27.71642, -11.48050),
        //        new Coordinate(-29.42360, -5.85271),
        //        new Coordinate(-30.00004, -0.00000),
        //        new Coordinate(-29.42360, 5.85271),
        //        new Coordinate(-27.71642, 11.48050),
        //        new Coordinate(-24.48285, 10.14111),
        //        new Coordinate(-24.96120, 8.98626),
        //        new Coordinate(-20.34180, 7.07284),
        //        new Coordinate(-19.86345, 8.22769),
        //        new Coordinate(-14.98998, 6.20904),
        //        new Coordinate(-15.91328, 3.16534),
        //        new Coordinate(-16.22504, -0.00000),
        //        new Coordinate(-15.91328, -3.16534),
        //        new Coordinate(-14.98998, -6.20904),
        //        new Coordinate(-19.86345, -8.22770),
        //        new Coordinate(-20.34180, -7.07285),
        //        new Coordinate(-24.93395, -8.97497),
        //        new Coordinate(-24.46922, -10.13547),
        //};
        //public static Polygon WorkingField { get; set; } = new Polygon(new LinearRing(WorkingFieldBoundaryPoints));

        #endregion old

        public static Polygon WorkingField { get; set; }
        public static (double minX, double maxX) WFBoundariesX { get; set; }
        public static (double minY, double maxY) WFBoundariesY { get; set; }
        public static int DeflectionMonitoringRobotId { get; set; } = 16;
    }
}
