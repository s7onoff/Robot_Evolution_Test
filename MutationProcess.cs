using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;


namespace Robot_Evolution
{
    public class MutationProcess
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Creation");
        
        private static readonly Random RandomGenerator = new Random();
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
                    double xMove;
                    double yMove;

                    bool check;
                    var checksTries = 0;

                    var amplitude = EvolutionParameters.NodeMovementPerMutation;
                    do
                    {
                        // If node is on contour - move along relevant line
                        if (GeometryMethods.PointOnContour(node.X, node.Y))
                        {
                            var line = GeometryMethods.GetContourLineOfContourPoint(node.X, node.Y);
                            var randomPointOnTheLine = line.PointAlong(RandomGenerator.NextDouble());
                            xMove = node.X - randomPointOnTheLine.X;
                            yMove = node.Y - randomPointOnTheLine.Y;
                        }
                        // If node is somewhere inside - move randomly
                        else
                        {
                            xMove = (amplitude - (RandomGenerator.NextDouble() * amplitude) * 2);
                            yMove = (amplitude - (RandomGenerator.NextDouble() * amplitude) * 2);
                        }

                        // If point went outside of border - check failed
                        check = GeometryMethods.PointInsideWF(node.X + xMove, node.Y + yMove);

                        // If any connected beam moved outside of border - check failed
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

                        // We don't want to to this checks a lot of time
                        if (checksTries++ == 40)
                        {
                            xMove = 0;
                            yMove = 0;
                            Logger.Warn("Moving node failed after {0} attempts", checksTries);
                            break;
                        }
                    }
                    while (!check);

                    node.X += xMove;
                    node.Y += yMove;

                    Logger.Debug("Node {0} moved by {1}, {2}", node.ID, xMove, yMove);
                }
            }
        }

        public class NodeAddMutation : Mutation
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
                while (!GeometryMethods.PointInsideWF(x, y));

                var node = new Node(x, y, instance.FreeNodeID);
                instance.MutatedNodes.Add(node);

                Logger.Debug("Node {0} added", node.ID);

                var addBeamMutation = new BeamAddMutation();
                addBeamMutation.Action(instance, node);
            }
        }

        public class NodeAddOnContourMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                // Choosing random line on a ring
                var pointsOfContour = InitialData.WorkingField.ExteriorRing.NumPoints;
                var chosenNumber = RandomGenerator.Next(pointsOfContour);

                var firstPoint = InitialData.WorkingField.ExteriorRing.GetPointN(chosenNumber).Coordinate;
                var secondPoint = 
                    chosenNumber == InitialData.WorkingField.ExteriorRing.NumPoints - 1 ? 
                    InitialData.WorkingField.ExteriorRing.GetPointN(1).Coordinate : 
                    InitialData.WorkingField.ExteriorRing.GetPointN(chosenNumber + 1).Coordinate;

                // Segment of the ring
                var line = new NetTopologySuite.Geometries.LineSegment(firstPoint, secondPoint);

                // A point on segment
                var randomPointOnTheLine = line.PointAlong(RandomGenerator.NextDouble());
                var node = new Node(randomPointOnTheLine.X, randomPointOnTheLine.Y, instance.FreeNodeID);

                instance.MutatedNodes.Add(node);
                Logger.Debug("Node {0} added", node.ID);
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
                    var beams = node.ConnectedBeams(instance);

                    foreach (var beam in beams)
                    {
                        instance.MutatedBeams.Remove(beam.beam);
                    }
                }
            }
        }

        public class BeamAddMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                Node node1;
                Node node2;

                var beams = instance.Beams();
                do
                {
                    node1 = ChooseAnyNode(instance);
                    node2 = ChooseAnyNode(instance);
                    // do this until chosen nodes wouldn't form a beam, that:
                    //      1) lays inside Working Field
                    //      2) doesn't lays along any other existing beam
                } while (!GeometryMethods.BeamInsideWF(node1, node2) || GeometryMethods.BeamLaysAlongAnyOfBeams(node1.X, node1.Y, node2.X, node2.Y, beams));

                var section = ChooseSection();

                var beam = new Beam(node1, node2, section, instance.FreeBeamID);

                instance.MutatedBeams.Add(beam);
                Logger.Debug("Beam {0} added", beam.ID);
            }

            public void Action(Instance instance, Node node1)
            {
                Node node2;

                var beams = instance.Beams();
                do
                {
                    node2 = ChooseAnyNode(instance);
                    // do this until chosen nodes wouldn't form a beam, that:
                    //      1) lays inside Working Field
                    //      2) doesn't lays along any other existing beam
                    var check1 = !GeometryMethods.BeamInsideWF(node1, node2);
                    var check2 = GeometryMethods.BeamLaysAlongAnyOfBeams(node1.X, node1.Y, node2.X, node2.Y, beams);
                } while (!GeometryMethods.BeamInsideWF(node1, node2) || GeometryMethods.BeamLaysAlongAnyOfBeams(node1.X, node1.Y, node2.X, node2.Y, beams));

                var section = ChooseSection();

                var beam = new Beam(node1, node2, section, instance.FreeBeamID);

                instance.MutatedBeams.Add(beam);
                Logger.Debug("Beam {0} added", beam.ID);
            }
        }

        public class DeleteBeamMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                var beam = ChooseBeam(instance);
                if (beam != null) 
                {
                    instance.MutatedBeams.Remove(beam);
                    Logger.Debug("Beam {0} removed", beam.ID);
                }
            }
        }

        public class BeamChangeSectionMutation : Mutation
        {
            public override void Action(Instance instance)
            {
                var beam = ChooseBeam(instance);
                if (beam != null)
                {
                    var section = ChooseSection();
                    beam.Section = section;
                    Logger.Debug("Beam {0} section changed", beam.ID);
                }
            }
        }


        public static List<Mutation> regularMutations = new List<Mutation>()
        {
            new MoveNodeMutation(){ Probability = EvolutionParameters.MoveNodeProbability },
            new NodeAddMutation(){ Probability = EvolutionParameters.NodeAddInsideProbability },
            new NodeAddOnContourMutation() { Probability = EvolutionParameters.NodeAddOnContourProbability },
            new DeleteNodeMutation(){ Probability = EvolutionParameters.NodeDeleteProbability },
            new BeamAddMutation() { Probability = EvolutionParameters.BeamAddProbability },
            new DeleteBeamMutation() { Probability = EvolutionParameters.BeamDeleteProbability },
            new BeamChangeSectionMutation() { Probability = EvolutionParameters.BeamChangeSectionProbability }
        };

        public static void Mutate(Instance instance)
        {
            var mutation = ChooseMutation(regularMutations);
            Logger.Debug("Mutation chosen: {0} for instance {1}_{2}", mutation == null? "Nothing" : mutation.ToString(), instance.GenerationID, instance.ID);

            if (mutation != null)
            {
                mutation.Action(instance);
            }
        }


        private static Mutation ChooseMutation(List<Mutation> mutations)
        {
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
            if (instance.MutatedBeams.Count() > 0)
            {
                var beamNumber = RandomGenerator.Next(0, instance.MutatedBeams.Count());
                return instance.MutatedBeams[beamNumber];
            }
            else return null;
            
        }

        public static Section ChooseSection()
        {
            var sectionNumber = RandomGenerator.Next(1, Sections.SectionsToUse.Count());
            return Sections.SectionsToUse[sectionNumber];
        }
    }
}
