using System.Collections.Generic;
using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Robot_Evolution
{
    public static class CrossOver
    {
        private static readonly Random randomGenerator = new Random();

        public static Instance Crosover(Instance parent1, Instance parent2)
        {
            Logging.Logger.Debug("Crossing over parents: {0}, {1}", parent1.ID, parent2.ID);
            var child = new Instance();

            var parents = new List<Instance>() { parent1, parent2 };

            var allTheBeams = parent1.MutatedBeams.Concat(parent2.MutatedBeams).ToList();

            var averageBeams = parents.Average(x => x.MutatedBeams.Count);
            var averageNodes = parents.Average(x => x.MutatedNodes.Count);

            for (int i = 0; i < averageBeams; i++)
            {
                var newBeamFrom = allTheBeams[randomGenerator.Next(0, allTheBeams.Count())];

                var beamsAsSegments = child.MutatedBeams.Select(b => LineFromBeam(b));
                var newBeamAsSegment = new LineSegment(newBeamFrom.Node1.X, newBeamFrom.Node1.Y, newBeamFrom.Node2.X, newBeamFrom.Node2.Y);

                if (!beamsAsSegments.Where(b => b.Equals(newBeamAsSegment)).Any())
                {
                    var node1 = new Node(newBeamFrom.Node1.X, newBeamFrom.Node1.Y, child.FreeNodeID);
                    var node2 = new Node(newBeamFrom.Node2.X, newBeamFrom.Node2.Y, child.FreeNodeID);
                    var beam = new Beam(node1, node2, newBeamFrom.Section, child.FreeBeamID);


                    child.MutatedNodes.Add(node1);
                    child.MutatedNodes.Add(node2);
                    child.MutatedBeams.Add(beam);
                }
            }

            return child;
        }

        private static LineSegment LineFromBeam(Beam beam)
        {            
            return new LineSegment(beam.Node1.X, beam.Node1.Y, beam.Node2.X, beam.Node2.Y);
        }
    }
}
