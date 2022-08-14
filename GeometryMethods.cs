using System;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
using System.Linq;

namespace Robot_Evolution
{
    public static class GeometryMethods
    {
        public static double? XofNodeOnArc(double y, BoundaryArc arc)
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

        public static double? XofNodeOnLine(double y, BoundaryLine line)
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
        
        public static bool NodeInsideWF(double x, double y)
        {
            return InitialData.WorkingField.EnclosesPoint(new Point2D(x, y));
        }

        public static (double maxX, double maxY, double minX, double minY) WorkingFieldboundaries(Polygon2D polygon)
        {
            var vertices = polygon.Vertices;
            var maxX = vertices.Max(vertex => vertex.X);
            var minX = vertices.Min(vertex => vertex.X);
            var maxY = vertices.Max(vertex => vertex.Y);
            var minY = vertices.Min(vertex => vertex.Y);
            return ((maxX, maxY, minX, minY));
        }
    }
}
