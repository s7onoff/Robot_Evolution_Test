using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Robot_Evolution
{
    public static class GeometryMethods
    {
        public static bool PointInsideWF(double x, double y)
        {
            var point = new Point(x, y);
            return InitialData.WorkingField.Contains(point);
        }

        public static bool PointOnContour(double x, double y)
        {
            var point = new Point(x, y);
            var contourLineSegments = InitialData.WorkingField.ExteriorRing;
            return contourLineSegments.Contains(point);
        }

        public static LineSegment GetContourLineOfContourPoint(double x, double y)
        {
            var point = new Point(x, y);
            var contourLineCoordinates = InitialData.WorkingField.ExteriorRing.Coordinates;

            for (int i = 0; i < contourLineCoordinates.Count()-1; i++)
            {
                var line = new LineSegment(contourLineCoordinates[i], contourLineCoordinates[i + 1]);
                if (PointOnLineSegment(point, line))
                {
                    return line;
                }
            }
            return null;
        }

        public static bool PointOnLineSegment(Point point, LineSegment lineSegment)
        {
            var segmentPoints = new Coordinate[] { lineSegment.P0, lineSegment.P1 };
            var line = new LineString(segmentPoints);
            return line.Contains(point);
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

        public static bool BeamLaysAlongAnyOfBeams(double x1, double y1, double x2, double y2, List<Beam> beams)
        {
            var point1 = new Coordinate(x1, y1);
            var point2 = new Coordinate(x2, y2);

            // If any beam of "beams" lays along segment of (x1,y1):(x2,y2) - returns true
            return beams.
                Select(b => LineFromBeam(b).DistancePerpendicular(point1) == 0 && LineFromBeam(b).DistancePerpendicular(point2) == 0).
                Where(res => res == true).
                Any();
        }

        public static Polygon WorkingFieldFromPoints(List<(double x, double y)> points)
        {
            var coordinatesList = new List<Coordinate>();
            foreach (var point in points)
            {
                coordinatesList.Add(new Coordinate(point.x, point.y));
            }

            var polygon = new Polygon(new LinearRing(coordinatesList.ToArray()));
            return polygon;
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


        public static LineSegment LineFromBeam(Beam beam)
        {
            var pointStart = new Coordinate(beam.Node1.X, beam.Node1.Y);
            var pointEnd = new Coordinate(beam.Node2.X, beam.Node2.Y);
            return new LineSegment(pointStart, pointEnd);
        }
    }
}
