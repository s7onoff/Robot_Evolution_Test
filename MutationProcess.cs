using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;


namespace Robot_Evolution
{
    public class MutationProcess
    {
        public abstract class Mutation
        {
            public double Probability { get; set; }

            public abstract void Action(Instance instance);

        }

        public class MoveNodeMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                var node = ChooseMutatedNode(instance);
                if (node != null)
                {
                    var amplitude = EvolutionParameters.NodeMovementPerMutation;
                    double xMove;
                    double yMove;
                    bool check;
                    do
                    {
                        xMove = (amplitude - (RandomGenerator.NextDouble() * amplitude));
                        yMove = (amplitude - (RandomGenerator.NextDouble() * amplitude));

                        check = GeometryMethods.NodeInsideWF(node.X + xMove, node.Y + yMove);
                        foreach (var beam in node.ConnectedBeams(instance))
                        {
                            if (beam.node == 1)
                            {
                                check = check || GeometryMethods.BeamInsideWF(node.X + xMove, node.Y + yMove, beam.beam.Node2.X, beam.beam.Node2.Y);
                            }

                            else
                            {
                                check = check || GeometryMethods.BeamInsideWF(beam.beam.Node1.X, beam.beam.Node1.Y, node.X + xMove, node.Y + yMove);
                            }
                        }
                    }
                    while (!check);

                    node.X += xMove;
                    node.Y += yMove;
                }
            }
        }

        public class NewNodeMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                double x;
                double y;
                do
                {
                    var amplitudeX = InitialData.WFBoundariesX.maxX - InitialData.WFBoundariesX.minX;
                    var amplitudeY = InitialData.WFBoundariesY.maxY - InitialData.WFBoundariesY.minY;
                    x = (RandomGenerator.NextDouble() * amplitudeX) + InitialData.WFBoundariesX.minX;
                    y = (RandomGenerator.NextDouble() * amplitudeY) + InitialData.WFBoundariesY.minY;
                }
                while (!GeometryMethods.NodeInsideWF(x, y));

                var node = new Node(x, y, instance.FreeNodeID);
                instance.MutatedNodes.Add(node);
            }
        }

        public class NewNodeOnContourMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                // Choosing random line on a ring
                var chosenNumber = RandomGenerator.Next(InitialData.WorkingField.ExteriorRing.NumPoints);

                var firstPoint = InitialData.WorkingField.ExteriorRing.GetPointN(chosenNumber).Coordinate;
                var secondPoint = InitialData.WorkingField.ExteriorRing.GetPointN(chosenNumber + 1).Coordinate;

                // Segment of the ring
                var line = new NetTopologySuite.Geometries.LineSegment(firstPoint, secondPoint);

                // A point on segment
                var randomPointOnTheLine = line.PointAlong(RandomGenerator.NextDouble());
                var node = new Node(randomPointOnTheLine.X, randomPointOnTheLine.Y, instance.FreeNodeID);

                instance.MutatedNodes.Add(node);
            }
        }

        public class DeleteNodeMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                var node = ChooseMutatedNode(instance);
                if (node != null)
                {
                    instance.MutatedNodes.Remove(node);
                }

                var beams = node.ConnectedBeams(instance);
                foreach (var beam in beams)
                {
                    instance.MutatedBeams.Remove(beam.beam);
                }
            }
        }

        public class NewBeamMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                Node node1;
                Node node2;

                do
                {
                    node1 = ChooseAnyNode(instance);
                    node2 = ChooseAnyNode(instance);
                } while (!GeometryMethods.BeamInsideWF(node1, node2));

                var section = ChooseSection();

                var beam = new Beam(node1, node2, section, instance.FreeBeamID);

                instance.MutatedBeams.Add(beam);
            }
        }

        public class DeleteBeamMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                var beam = ChooseBeam(instance);
                instance.MutatedBeams.Remove(beam);
            }
        }

        public class ChangeBeamSectionMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                var beam = ChooseBeam(instance);
                var section = ChooseSection();
                beam.Section = section;
            }
        }


        public static List<Mutation> regularMutations = new List<Mutation>()
        {
            new MoveNodeMutation(){ Probability = EvolutionParameters.MoveNodeProbability},
            new NewNodeMutation(){Probability = EvolutionParameters.NewNodeInsideProbability},
            new NewNodeOnContourMutation() {Probability = EvolutionParameters.NewNodeOnContourProbability},
            new DeleteNodeMutation(){Probability = EvolutionParameters.NodeDeleteProbability},
            new NewBeamMutation() { Probability = EvolutionParameters.BeamAddDeleteProbability},
            new DeleteBeamMutation() { Probability = EvolutionParameters.BeamAddDeleteProbability},
            new ChangeBeamSectionMutation() {Probability = EvolutionParameters.BeamChangeSectionProbability}
        };

        public static void Mutate(Instance instance)
        {
            var mutation = ChooseMutation(regularMutations);
            Logging.Logger.Debug("Mutation chosen: {0} for instance {1}", mutation == null? "Nothing" : mutation.ToString(), instance.ID.ToString());

            if (mutation != null)
            {
                mutation.Action(instance);
            }
        }


        private static readonly Random RandomGenerator = new Random();

        private static Mutation ChooseMutation(List<Mutation> mutations)
        {
            // Random gives us double from 0 to 1
            var randomResult = RandomGenerator.NextDouble();

            // Boundaries are to choose mutation with preset probabilities 
            // whole probability to mutate is equal to sum of all mutations probabilities
            var currentProbability = 0.0;
            foreach (Mutation mutation in mutations)
            {
                currentProbability += mutation.Probability;
                if (randomResult <= currentProbability) return mutation;
            }
            return null;
        }

        public static Node ChooseMutatedNode(Instance instance)
        {
            if (instance.MutatedNodes?.Count() > 0)
            {
                var nodeNumber = RandomGenerator.Next(0, instance.MutatedNodes.Count()-1);
                return instance.MutatedNodes[nodeNumber];
            }
            else
            {
                return null;
            }
        }
        public static Node ChooseAnyNode(Instance instance)
        {
            var nodeNumber = RandomGenerator.Next(1, instance.Nodes().Count);
            return instance.Nodes()[nodeNumber];
        }


        public static Beam ChooseBeam(Instance instance)
        {
            var beamNumber = RandomGenerator.Next(1, instance.MutatedBeams.Count());
            return instance.MutatedBeams[beamNumber];
        }

        public static Section ChooseSection()
        {
            var sectionNumber = RandomGenerator.Next(1, Sections.SectionsToUse.Count());
            return Sections.SectionsToUse[sectionNumber];
        }
    }
}
