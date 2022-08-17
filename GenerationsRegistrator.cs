using System.Collections.Generic;
using System.Linq;

namespace Robot_Evolution
{
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

        private static void LogGeneration(Generation generation)
        {
            Logging.Logger.Info("-=-=-=-=-=-=-=-=-=-=-=-=-=- Generation {0} -=-=-=-=-=-=-=-=-=-=-=-=-=", generation.ID.ToString());
            Logging.Logger.Info("Number of instances: {0}", generation.Instances.Count());
            Logging.Logger.Info("Deflections:  \r\n {0}", string.Join(" \r\n ", generation.Instances.Select(ins => ins.Result.Deflection).ToArray()));
            Logging.Logger.Info("Weights:  \r\n {0}", string.Join(" \r\n ", generation.Instances.Select(ins => ins.Result.Weight).ToArray()));
            //Logging.Logger.Info("Calculated: {0}", string.Join("\r ", generation.Instances.Select(ins => ins.Result.RobotCalculationStatus).ToArray()));
            Logging.Logger.Info("Probabilities for next generation: \r\n {0}", string.Join(" \r\n ", generation.Instances.Select(ins => ins.Result.Probability).ToArray()));
            foreach (var instance in generation.Instances)
            {
                LogInstance(instance);
            }
        }

        private static void LogInstance(Instance instance)
        {
            Logging.Logger.Info("Generation {0} | Instance {1}",
                                instance.GenerationID,
                                instance.ID);
            Logging.Logger.Info("Instance Nodes: \r\n {0}", string.Join(" \r\n ", instance.Nodes().Select(node => string.Join(" | ", node.ID, node.X, node.Y))));
            Logging.Logger.Info("Instance Beams: \r\n {0}", string.Join(" \r\n ", instance.Beams().Select(beam => string.Join(" | ", beam.ID, beam.Node1.ID, beam.Node2.ID))));
            var nodesOfBeams = instance.Beams().Select(b => b.Node1.ID).Concat(instance.Beams().Select(b => b.Node2.ID));
            var nodes = instance.Nodes().Select(n => n.ID);
            var difOfNodes = nodesOfBeams.Where(nb => nodes.All(n => n != nb));
            if (difOfNodes.Count() != 0)
            {
                Logging.Logger.Error("Beams have more nodes than nodes exist. Problem nodes: {0}", string.Join(", ", difOfNodes));
            }
        }
    }
}
