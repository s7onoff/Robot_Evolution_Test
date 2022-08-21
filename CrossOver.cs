using System.Collections.Generic;
using System;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Robot_Evolution
{
    public static class CrossOver
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Crossover");
        private static readonly Random randomGenerator = new Random();

        //public static Instance CrosoverOld(Instance parent1, Instance parent2)
        //{
        //    Logger.Debug("Crossing over. Generation {0}. Parents: {1}, {2}", parent1.GenerationID, parent1.ID, parent2.ID);
        //    var child = new Instance();
        //    var parents = new List<Instance>() { parent1, parent2 };

        //    // all the mutated beams
        //    var allTheBeams = parent1.MutatedBeams.Concat(parent2.MutatedBeams).ToList();
        //    Logger.Debug("BeamsNumbers: {0}", string.Join(", ", allTheBeams.Select(b => b.ID)));

        //    // child instance will get average amount of beams of two parents
        //    var averageNumberOfBeams = parents.Average(x => x.MutatedBeams.Count);
        //    var beamsAlreadyTaken = new List<int>();

        //    for (int i = 0; i < averageNumberOfBeams; i++)
        //    {
        //        int beamNumber;
        //        do
        //        {
        //            beamNumber = randomGenerator.Next(0, allTheBeams.Count());
        //        } while (beamsAlreadyTaken.Contains(beamNumber));

        //        beamsAlreadyTaken.Add(beamNumber);
        //        // any beam from any parents
        //        var inheritedBeam = allTheBeams[beamNumber];

        //        Logger.Debug("Trying to add beam: {0} (beam id: {1})", beamNumber, inheritedBeam.ID);

        //        // check if beam lays along any of existing child beams
        //        if (!GeometryMethods.BeamLaysAlongAnyOfBeams(inheritedBeam.Node1.X, inheritedBeam.Node1.Y, inheritedBeam.Node2.X, inheritedBeam.Node2.Y, child.Beams()))
        //        {
        //            Logger.Debug("Adding beam: {0}", inheritedBeam.ID);
        //            // if child already has nodes from old beam, they are chosen, else -- they are created
        //            var node1 = child.ChooseNodeByCoordinates(inheritedBeam.Node1.X, inheritedBeam.Node1.Y);
        //            var node2 = child.ChooseNodeByCoordinates(inheritedBeam.Node2.X, inheritedBeam.Node2.Y);

        //            if (node1 == null)
        //            {
        //                node1 = new Node(inheritedBeam.Node1.X, inheritedBeam.Node1.Y, child.FreeNodeID);
        //                child.MutatedNodes.Add(node1);
        //            }
        //            if (node2 == null)
        //            {
        //                node2 = new Node(inheritedBeam.Node2.X, inheritedBeam.Node2.Y, child.FreeNodeID);
        //                child.MutatedNodes.Add(node2);
        //            }

        //            // nodes from above, section is the same 
        //            var beam = new Beam(node1, node2, inheritedBeam.Section, child.FreeBeamID);

        //            child.MutatedBeams.Add(beam);
        //            Logger.Debug("Child got beam: {0}", beam.ID);
        //        }
        //    }

        //    return child;
        //}

        public static Instance Crosover(Instance parent1, Instance parent2)
        {
            Logger.Debug("Crossing over. Generation {0}. Parents: {1}, {2}", parent1.GenerationID, parent1.ID, parent2.ID);
            var child = new Instance();
            var parents = new List<Instance>() { parent1, parent2 };

            var crossoverBorder = randomGenerator.NextDouble();

            Logger.Debug("Crossover border = {0}", crossoverBorder);

            for (int i = 0; i < parent1.MutatedBeams.Count * crossoverBorder; i++)
            {
                Logger.Debug("Trying to add beam {0} from parent {1}", i, parent1.ID);
                AddBeam(parent1.MutatedBeams[i], child);
            }

            for (int i = (int)Math.Round(parent2.MutatedBeams.Count * crossoverBorder, 0); i < parent2.MutatedBeams.Count; i++)
            {
                Logger.Debug("Trying to add beam {0} from parent {1}", i, parent2.ID);
                AddBeam(parent2.MutatedBeams[i], child);
            }

            for (int i = 0; i < parent1.MutatedNodes.Count * crossoverBorder; i++)
            {
                Logger.Debug("Trying to add node {0} from parent {1}", i, parent1.ID);
                AddNode(parent1.MutatedNodes[i], child);
            }

            for (int i = (int)Math.Round(parent2.MutatedNodes.Count * crossoverBorder, 0); i < parent2.MutatedNodes.Count; i++)
            {
                Logger.Debug("Trying to add node {0} from parent {1}", i, parent2.ID);
                AddNode(parent2.MutatedNodes[i], child);
            }

            return child;
        }

        private static void AddBeam(Beam inheritedBeam, Instance child)
        {
            // check if beam lays along any of existing child beams
            if (!GeometryMethods.BeamLaysAlongAnyOfBeams(inheritedBeam.Node1.X, inheritedBeam.Node1.Y, inheritedBeam.Node2.X, inheritedBeam.Node2.Y, child.Beams()))
            {
                Logger.Debug("Adding beam: {0}", inheritedBeam.ID);
                // if child already has nodes from old beam, they are chosen, else -- they are created
                var node1 = child.ChooseNodeByCoordinates(inheritedBeam.Node1.X, inheritedBeam.Node1.Y);
                var node2 = child.ChooseNodeByCoordinates(inheritedBeam.Node2.X, inheritedBeam.Node2.Y);

                if (node1 == null)
                {
                    node1 = new Node(inheritedBeam.Node1.X, inheritedBeam.Node1.Y, child.FreeNodeID);
                    child.MutatedNodes.Add(node1);
                }
                if (node2 == null)
                {
                    node2 = new Node(inheritedBeam.Node2.X, inheritedBeam.Node2.Y, child.FreeNodeID);
                    child.MutatedNodes.Add(node2);
                }

                // nodes from above, section is the same 
                var beam = new Beam(node1, node2, inheritedBeam.Section, child.FreeBeamID);

                child.MutatedBeams.Add(beam);
                Logger.Debug("Child got beam: {0}", beam.ID);
            }
        }

        private static void AddNode(Node inheritedNode, Instance child)
        {
            if (!child.Nodes().Where(n => n.X == inheritedNode.X && n.Y == inheritedNode.Y).Any())
            {
                var x = inheritedNode.X;
                var y = inheritedNode.Y;
                var id = child.FreeNodeID;
                child.MutatedNodes.Add(new Node(x, y, id));
                Logger.Debug("Child got node: {0}", id);
            }
            else
            {
                Logger.Debug("Child has already got this node");
            }
        }
    }
}
