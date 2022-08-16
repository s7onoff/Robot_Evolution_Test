using System;
using System.Linq;
using NetTopologySuite.Geometries;

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
            var point = new Point(x, y);
            return InitialData.WorkingField.Contains(point);
            //return InitialData.WorkingField.EnclosesPoint(new Point2D(x, y));
        }

        public static bool BeamInsideWF(Node node1, Node node2)
        {
            var point1 = new Coordinate(node1.X, node1.Y);
            var point2 = new Coordinate(node2.X, node2.Y);
            var lineSegment = new LineString(new Coordinate[] {point1, point2} );
            return InitialData.WorkingField.Contains(lineSegment);
            //return InitialData.WorkingField.EnclosesPoint(new Point2D(x, y));
        }

        public static (double maxX, double maxY, double minX, double minY) WorkingFieldboundaries(Polygon polygon)
        {
            var vertices = polygon.Coordinates;
            var maxX = vertices.Max(vertex => vertex.X);
            var minX = vertices.Min(vertex => vertex.X);
            var maxY = vertices.Max(vertex => vertex.Y);
            var minY = vertices.Min(vertex => vertex.Y);
            return ((maxX, maxY, minX, minY));
        }        
    }
}
