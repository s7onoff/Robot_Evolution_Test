﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Robot_Evolution
{
    public class Generation
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Creation");
        public List<Instance> Instances { get; set; }
        public int ID { get; set; }
        public int GenerationQuantity { get; set; } = EvolutionParameters.InstancesPerGeneration;

        private readonly Random randomGenerator = new Random();
        public Generation()
        {
            this.ID = GenerationsRegistrator.Counter;
            this.Instances = new List<Instance>();
        }

        public void GenerateOriginalGeneration()
        {
            ID = 0;
            var originalInstance = new Instance()
            { ID = 0, GenerationID = 0 };
            originalInstance.ReadOriginalFromRobot();

            Instances.Add(originalInstance);

            GenerationsRegistrator.Register(this);
        }

        
        public void GenerateInitialGeneration()
        {
            ID = 1;
            for (int i = 0; i < GenerationQuantity; i++)
            {
                var instance = new Instance
                {
                    ID = i,
                    GenerationID = this.ID
                };
                instance.MutateInitial();
                instance.Execute();
                this.Instances.Add(instance);
            }

            GenerationsRegistrator.Register(this);
        }


        public void GenerateRegularGeneration()
        {
            var previousGeneration = GenerationsRegistrator.Generations.Last();
            ID = GenerationsRegistrator.Counter;

            // Non-crossed-over instances copied
            var nonCrossedOverChildren = ChooseChildsNonCrossedOver(previousGeneration.Instances.Count());

            Logger.Debug("Generation: {0}. For non-crossover chosen: {1}", previousGeneration.ID, String.Join(", ", nonCrossedOverChildren));

            foreach (var i in nonCrossedOverChildren)
            {
                var instance = Clone(previousGeneration.Instances[i]);
                instance.GenerationID = this.ID;
                Logger.Debug("New cloned instance: {0}", instance.ID);
                instance.Mutate();
                instance.Execute();
                this.Instances.Add(instance);
            }

            // Making crossover for other instances using results of fitting function
            for (int i = 0; i < GenerationQuantity; i++)
            {
                if (this.Instances.Exists(_ => _.ID == i))
                {
                    continue;
                }
                else
                {
                    var parentsForCrossOver = ChooseParentsForCrossOver(previousGeneration.Instances);
                    var instance = CrossOver.Crosover(parentsForCrossOver.parent1, parentsForCrossOver.parent2);
                    instance.ID = i;
                    instance.GenerationID = this.ID;
                    instance.Mutate();
                    instance.Execute();
                    this.Instances.Add(instance);
                    Logger.Debug("New crossed over instance: {0}", instance.ID);
                }
            }

            GenerationsRegistrator.Register(this);
        }


        //TODO: Cloning -> another class
        private Node Clone(Node oldNode, int id)
        {
            var newNode = new Node(oldNode.X, oldNode.Y, id);

            return newNode;
        }


        private Beam Clone(Beam oldBeam, Instance oldInstance, Instance newInstance)
        {
            // works only if previously all the nodes was cloned

            // var node1OfOld = oldInstance.Nodes().Where(n => n.ID == oldBeam.Node1.ID).Select(n => n.ID).First();
            // var node2OfOld = oldInstance.Nodes().Where(n => n.ID == oldBeam.Node2.ID).Select(n => n.ID).First();

            Logger.Debug("Nodes of old instance: {0}, {1}", oldBeam.Node1.ID, oldBeam.Node2.ID);

            var node1 = newInstance.Nodes().Where(n => n.ID == oldBeam.Node1.ID).First();
            var node2 = newInstance.Nodes().Where(n => n.ID == oldBeam.Node2.ID).First();

            Logger.Debug("Nodes of new instance: {0}, {1}", node1.ID, node2.ID);

            var newBeam = new Beam(node1, node2, oldBeam.Section, newInstance.FreeBeamID);
            return newBeam;
        }


        private Instance Clone(Instance oldInstance)
        {
            var newInstance = new Instance()
            {
                ID = oldInstance.ID,
                GenerationID = this.ID
            };

            foreach (var node in oldInstance.MutatedNodes)
            {
                newInstance.MutatedNodes.Add(Clone(node, node.ID));
                Logger.Debug("Cloning node {0} from instance {1}_{2} to instance {3}_{4}", node.ID, oldInstance.GenerationID, oldInstance.ID, newInstance.GenerationID, newInstance.ID);
            }

            foreach (var beam in oldInstance.MutatedBeams)
            {
                newInstance.MutatedBeams.Add(Clone(beam, oldInstance, newInstance));
                Logger.Debug("Cloning beam {0} from instance {1}_{2} to instance {3}_{4}", beam.ID, oldInstance.GenerationID, oldInstance.ID, newInstance.GenerationID, newInstance.ID);
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

            return list;
        }


        private (Instance parent1, Instance parent2) ChooseParentsForCrossOver(List<Instance> predecessors)
        {
            var possiblePredecessors = predecessors.Where(inst => inst.Result.Deflection * inst.Result.Weight != 0).ToList();

            if(possiblePredecessors.Count == 0)
            {
                return (null, null);
            }

            double[] probabilities = new double[possiblePredecessors.Count];       

            var currentProbabilityNumber = 0.0;
            for (int i = 0; i < probabilities.Count(); i++)
            {
                probabilities[i] = possiblePredecessors[i].Result.Probability + currentProbabilityNumber;
                currentProbabilityNumber = probabilities[i];
            }

            var random1 = randomGenerator.NextDouble();
            var random2 = randomGenerator.NextDouble();

            var parent1 = possiblePredecessors[Array.IndexOf(probabilities, probabilities.Where(number => random1 <= number).First())];
            var parent2 = possiblePredecessors[Array.IndexOf(probabilities, probabilities.Where(number => random2 <= number).First())];

            // Logger.Debug("Random: {0}. Chosen: {1}", random1, parent1.ID);
            // Logger.Debug("Random: {0}. Chosen: {1}", random2, parent2.ID);

            return (parent1, parent2);
        }
    }
}