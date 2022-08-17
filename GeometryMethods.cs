using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Robot_Evolution
{
    public static class GeometryMethods
    {
        public static bool NodeInsideWF(double x, double y)
        {
            var point = new Point(x, y);
            return InitialData.WorkingField.Contains(point);
        }

        public static bool BeamInsideWF(Node node1, Node node2)
        {
            var point1 = new Coordinate(node1.X, node1.Y);
            var point2 = new Coordinate(node2.X, node2.Y);
            var lineSegment = new LineString(new Coordinate[] {point1, point2} );
            return InitialData.WorkingField.Contains(lineSegment);
        }
        public static bool BeamInsideWF(double x1, double y1, double x2, double y2)
        {
            var point1 = new Coordinate(x1, y1);
            var point2 = new Coordinate(x2, y2);
            var lineSegment = new LineString(new Coordinate[] { point1, point2 });
            return InitialData.WorkingField.Contains(lineSegment);
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
