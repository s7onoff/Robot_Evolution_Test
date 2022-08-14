using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace Robot_Evolution
{
    public static class CrossOver
    {
        private static Random RandomGenerator = new Random();

        public static Instance Crosover(Instance parent1, Instance parent2)
        {
            var child = new Instance();

            var parents = new List<Instance>() { parent1, parent2};

            var parentWithMoreNodes = parents.OrderByDescending(x => x.MutatedNodes.Count).FirstOrDefault();
            var minimumNodes = parents.Min(x => x.MutatedNodes.Count);
            var maximumNodes = parents.Max(x => x.MutatedNodes.Count);

            var nodesQuantity = RandomGenerator.Next(minimumNodes, maximumNodes);

            for (int i = 0; i < nodesQuantity; i++)
            {
                double x;
                double y;
                var randomParent = RandomGenerator.Next(0, 1) == 0 ? parent1 : parent2;

                if (randomParent.MutatedNodes.Count > i)
                {
                    x = randomParent.MutatedNodes[i].X;
                    y = randomParent.MutatedNodes[i].Y;
                }
                else
                {
                    x = parentWithMoreNodes.MutatedNodes[i].X;
                    y = parentWithMoreNodes.MutatedNodes[i].Y;
                }

                child.MutatedNodes.Add(new Node(x, y));
            }

            var parentWithMoreBeams = parents.OrderByDescending(x => x.MutatedBeams.Count).FirstOrDefault();
            var minimumBeams = parents.Min(x => x.MutatedBeams.Count);
            var maximumBeams = parents.Max(x => x.MutatedBeams.Count);

            var beamsQuantity = RandomGenerator.Next(minimumBeams, maximumBeams);

            for (int i = 0; i < beamsQuantity; i++)
            {
                var randomParent = RandomGenerator.Next(0, 1) == 0 ? parent1 : parent2;
                Instance dominantParent;

                if (randomParent.MutatedBeams.Count > i)
                {
                    dominantParent = randomParent;
                }
                else
                {
                    dominantParent = parentWithMoreBeams;
                }

                var beamForChild = dominantParent.MutatedBeams[i];
                var node1OfDominantParent = dominantParent.Nodes().IndexOf(beamForChild.Node1);
                var node2OfDominantParent = dominantParent.Nodes().IndexOf(beamForChild.Node2);

                var node1 = child.Nodes()[node1OfDominantParent];
                var node2 = child.Nodes()[node2OfDominantParent];


                child.MutatedBeams.Add(new Beam(node1, node2, beamForChild.Section));
            }

            return child;
        }
    }
}
