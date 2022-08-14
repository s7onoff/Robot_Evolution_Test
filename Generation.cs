using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robot_Evolution
{
    public class Generation
    {
        //TODO: чекнуть, что все методы, использующие рандомайзер, не вызываются по несколько раз
        public List<Instance> Instances { get; set; }
        public int id { get; set; }
        public int GenerationQuantity { get; set; } = EvolutionParameters.InstancesPerGeneration;
        private Random RandomGenerator = new Random();
        public Generation()
        {
            this.id = GenerationsRegistrator.Counter;
            this.Instances = new List<Instance>();
        }

        public void generateOriginalGeneration()
        {
            id = 0;
            var originalInstance = new Instance()
            { id = 0, generationId = 0 };
            originalInstance.ReadOriginalFromRobot();

            Instances.Add(originalInstance);

            GenerationsRegistrator.Register(this);
        }

        
        public void generateInitialGeneration()
        {
            id = 1;
            for (int i = 0; i < GenerationQuantity; i++)
            {
                var instance = new Instance
                {
                    id = i,
                    generationId = this.id
                };
                instance.MutateInitial();
                instance.Execute();
                this.Instances.Add(instance);
            }

            GenerationsRegistrator.Register(this);
        }


        public void generateRegularGeneration()
        {
            var previousGeneration = GenerationsRegistrator.Generations.Last();
            id = GenerationsRegistrator.Counter;
            // Non-crossed-over instances copied
            var nonCrossedOverChildren = ChooseChildsNonCrossedOver();
            foreach (var i in nonCrossedOverChildren)
            {
                var instance = Clone(previousGeneration.Instances[i]);
                instance.generationId = this.id;
                instance.Mutate();
                instance.Execute();
                this.Instances.Add(instance);
            }

            // Making crossover for other instances using results of fitting function
            for (int i = 0; i < GenerationQuantity- nonCrossedOverChildren.Count(); i++)
            {
                if (this.Instances.Exists(_ => _.id == i))
                {
                    continue;
                }
                else
                {
                    var parentsForCrossOver = ChooseParentsForCrossOver(previousGeneration.Instances);
                    var instance = CrossOver.Crosover(parentsForCrossOver.parent1, parentsForCrossOver.parent2);
                    instance.id = i;
                    instance.generationId = this.id;
                    instance.Mutate();
                    instance.Execute();
                    this.Instances.Add(instance);
                }
            }

            GenerationsRegistrator.Register(this);
        }

        private Node Clone(Node oldNode)
        {
            var newNode = new Node(oldNode.X, oldNode.Y);

            return newNode;
        }

        private Beam Clone(Beam oldBeam, Instance oldInstance, Instance newInstance)
        {
            var node1OfDominantParent = oldInstance.Nodes().IndexOf(oldBeam.Node1);
            var node2OfDominantParent = oldInstance.Nodes().IndexOf(oldBeam.Node2);

            var node1 = newInstance.Nodes()[node1OfDominantParent];
            var node2 = newInstance.Nodes()[node2OfDominantParent];

            var newBeam = new Beam(node1, node2, oldBeam.Section);
            return newBeam;
        }
        private Instance Clone(Instance oldInstance)
        {
            var newInstance = new Instance()
            {
                id = oldInstance.id,
                generationId = this.id
            };

            foreach (var node in oldInstance.MutatedNodes)
            {
                newInstance.MutatedNodes.Add(Clone(node));
            }

            foreach (var beam in oldInstance.MutatedBeams)
            {
                newInstance.MutatedBeams.Add(Clone(beam, oldInstance, newInstance));
            }

            return newInstance;
        }

        private List<int> ChooseChildsNonCrossedOver()
        {
            var list = new List<int>();
            for (int i = 0; i < EvolutionParameters.GenerationNonCrossedOver; i++)
            {
                list.Add(RandomGenerator.Next(EvolutionParameters.InstancesPerGeneration));
            }
            return list;
        }

        private (Instance parent1, Instance parent2) ChooseParentsForCrossOver(List<Instance> predecessors)
        {
            var sumProbabilities = predecessors.Sum(_ => _.Result.ProbabilityForNext);
            double[] probabilities = new double[predecessors.Count];

            var currentProbabilityNumber = 0.0;
            for (int i = 0; i < predecessors.Count; i++)
            {
                probabilities[i] = predecessors[i].Result.ProbabilityForNext;
                currentProbabilityNumber += probabilities[i];
            }

            var parent1prob = RandomGenerator.NextDouble() * sumProbabilities;
            var parent2prob = RandomGenerator.NextDouble() * sumProbabilities;

            var parent1 = new Instance();
            var parent2 = new Instance();

            for (int i = 0; i < probabilities.Count(); i++)
            {
                if(probabilities[i] >= parent1prob)
                {
                    parent1 = predecessors[i];
                    break;
                }
            }

            for (int i = 0; i < probabilities.Count(); i++)
            {
                if (probabilities[i] >= parent2prob)
                {
                    parent2 = predecessors[i];
                    break;
                }
            }

            return (parent1, parent2);
        }
    }
    public static class GenerationsRegistrator
    {
        public static int Counter { get; set; } = 0;
        public static List<Generation> Generations { get; set; } = new List<Generation>();

        public static void Register(Generation generation)
        {
            Generations.Add(generation);
            Fitness.RegisterResult(generation);
            Counter++;
        }
    }
}
