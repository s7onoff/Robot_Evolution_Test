using System;
using MathNet.Spatial.Euclidean;

namespace Robot_Evolution
{
    public static class GeometryMethods
    {
        static double? XofNodeOnArc(double y, BoundaryArc arc)
        {
            var r = InitialData.Arcs[arc].R;
            var yMin = InitialData.ArcsBoundaries[arc].YMin;
            var yMax = InitialData.ArcsBoundaries[arc].YMax;
            var node1 = InitialData.Arcs[arc].Node1;
            var node2 = InitialData.Arcs[arc].Node2;

            if (y > yMin && y < yMax)
            {
                return Math.Sqrt(r * r - y * y);
            }
            else
            {
                return null;
            }
        }

        static double? XofNodeOnLine(double y, BoundaryLine line)
        {
            var k = InitialData.LinesParameters[line].k;
            var b = InitialData.LinesParameters[line].b;
            var yMin = InitialData.LinesBoundaries[line].YMin;
            var yMax = InitialData.LinesBoundaries[line].YMax;

            if (y > yMin && y < yMax)
            {
                return k * y + b;
            }
            else
            {
                return null;
            }
        }
        
        static bool NodeInside(double x, double y)
        {
            return InitialData.WorkingField.EnclosesPoint(new Point2D(x, y));
        }
    }
}
