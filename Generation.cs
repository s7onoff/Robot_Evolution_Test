﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Robot_Evolution
{
    public class Generation
    {
        public List<Instance> Instances { get; set; }
        public int id { get; set; }
        public int GenerationQuantity { get; set; } = EvolutionParameters.InstancesPerGeneration;
        private readonly Random randomGenerator = new Random();
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
            var nonCrossedOverChildren = ChooseChildsNonCrossedOver(previousGeneration.Instances.Count());
            foreach (var i in nonCrossedOverChildren)
            {
                var instance = Clone(previousGeneration.Instances[i]);
                instance.generationId = this.id;
                instance.Mutate();
                instance.Execute();
                this.Instances.Add(instance);
                Logging.Logger.Info("New cloned instance: {0}", instance.id);
            }

            // Making crossover for other instances using results of fitting function
            for (int i = 0; i < GenerationQuantity; i++)
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
                    Logging.Logger.Info("New crossed over instance: {0}", instance.id);
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

        private List<int> ChooseChildsNonCrossedOver(int generationQuantity)
        {
            var list = new List<int>();
            for (int i = 0; i < EvolutionParameters.GenerationNonCrossedOver; i++)
            {
                int randomNumber;
                do
                {
                    randomNumber = randomGenerator.Next(0, generationQuantity);
                }
                while (list.Contains(randomNumber));

                list.Add(randomNumber);
            }

            Logging.Logger.Info("For non-crossover chosen: {}", String.Join(", ", list.ToArray()));
            return list;
        }

        private (Instance parent1, Instance parent2) ChooseParentsForCrossOver(List<Instance> predecessors)
        {
            Logging.Logger.Info("Crossing over");


            double[] probabilities = new double[(predecessors.Where(inst => inst.Result.RobotCalculationStatus == RobotOM.IRobotCalculationStatus.I_CS_COMPLETED).ToList()).Count];       

            var currentProbabilityNumber = 0.0;
            for (int i = 0; i < probabilities.Count(); i++)
            {
                probabilities[i] = predecessors[i].Result.Probability + currentProbabilityNumber;
                currentProbabilityNumber = probabilities[i];
            }


            var parent1 = predecessors.Where(inst => randomGenerator.NextDouble() >= inst.Result.Probability).First();
            var parent2 = predecessors.Where(inst => randomGenerator.NextDouble() >= inst.Result.Probability).First();

            Logging.Logger.Info("Parents for crossover selected: {0}, {1}", parent1.id.ToString(), parent2.id.ToString());

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
            LogGeneration(generation);
        }

        public static void LogGeneration(Generation generation)
        {
            Logging.Logger.Info(" ");
            Logging.Logger.Info("Generation {0}", generation.id.ToString());
            Logging.Logger.Info("Number of instances: {0}", generation.Instances.Count());
            Logging.Logger.Info("Deflections: {0}", String.Join(", ", generation.Instances.Select(ins => ins.Result.Deflection).ToArray()));
            Logging.Logger.Info("Calculated: {0}", String.Join(", ", generation.Instances.Select(ins => ins.Result.RobotCalculationStatus).ToArray()));
            Logging.Logger.Info("Probabilities for next generation: {0}", String.Join(", ", generation.Instances.Select(ins => ins.Result.Probability).ToArray()));
        }
    }
}
