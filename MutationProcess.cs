using System.Collections.Generic;
using System;
using System.Linq;

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

                // TODO: Check if connected beam are still in WF
                var node = ChooseMutatedNode(instance);
                if (node != null)
                {
                    var amplitude = EvolutionParameters.NodeMovementPerMutation;
                    double xMove;
                    double yMove;
                    do
                    {
                        xMove = (amplitude - (RandomGenerator.NextDouble() * amplitude)) * 2;
                        yMove = (amplitude - (RandomGenerator.NextDouble() * amplitude)) * 2;
                    }
                    while (!GeometryMethods.NodeInsideWF(node.X + xMove, node.Y + yMove));

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

                var node = new Node(x, y);
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
            }
        }

        public class NewBeamMutation : Mutation
        {
            //TODO: дописать проверку, что балка остается в пределах контура (done)
            //TODO: дописать проверку, что балка не совпадает с существующими (done by previous)
            //TODO: дописать проверку, что балка не накладывается на существующие (should be done automatically)
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

                var beam = new Beam(node1, node2, section);

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

        public static List<Mutation> regularMutations = new List<Mutation>()
        {
            new MoveNodeMutation(){ Probability = EvolutionParameters.MoveNodeProbability},
            new NewNodeMutation(){Probability = EvolutionParameters.NodeAddDeleteProbability},
            new DeleteNodeMutation(){Probability = EvolutionParameters.NodeAddDeleteProbability},
            new NewBeamMutation() { Probability = EvolutionParameters.BeamAddDeleteProbability},
            new DeleteBeamMutation() { Probability = EvolutionParameters.BeamAddDeleteProbability}
        };

        public static void Mutate(Instance instance)
        {
            var mutation = ChooseMutation(regularMutations);
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
            var boundaries = new List<double>() { 0.0 };
            foreach (Mutation mutation in mutations)
            {
                double boundary = mutation.Probability;
                if (randomResult < boundary)
                {
                    return mutation;
                }
                else
                {
                    boundaries.Add((boundaries.Last() + boundary));
                }
            }
            return null;
        }

        public static Node ChooseMutatedNode(Instance instance)
        {
            if (instance.MutatedNodes.Count() > 0)
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
