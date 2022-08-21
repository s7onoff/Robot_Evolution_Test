using System;
using System.Collections.Generic;
using System.Linq;

namespace Robot_Evolution
{
    public static class GenerationsRegistrator
    {
        public static readonly NLog.Logger CreationLogger = NLog.LogManager.GetLogger("Creation");
        public static readonly NLog.Logger GenerationLogger = NLog.LogManager.GetLogger("Generation");
        public static int Counter { get; set; } = 0;
        public static List<Generation> Generations { get; set; } = new List<Generation>();

        public static void Register(Generation generation)
        {
            Generations.Add(generation);
            Fitness.RegisterResult(generation);
            Counter++;
            LogGeneration(generation);

            if(generation.Instances.Where(ins => ins.Result.Probability == 0).Count() > 0.4 * generation.Instances.Count() && generation.ID > 1)
            {
                GenerationLogger.Error("Generation failed");
            }
        }

        private static void LogGeneration(Generation generation)
        {
            GenerationLogger.Info("-=-=-=-=-=-=-=-=-=-=-=-=-=- Generation {0} -=-=-=-=-=-=-=-=-=-=-=-=-=-", 
                generation.ID.ToString());
            GenerationLogger.Info("Number of instances: {0}", 
                generation.Instances.Count());
            GenerationLogger.Info("Average deflection:  {0}", 
                string.Join(" \r\n ", 
                generation.Instances.Select(ins => Math.Abs(ins.Result.Deflection)).Average()));
            GenerationLogger.Info("Average average deflection:  {0}", 
                string.Join(" \r\n ", 
                generation.Instances.Select(ins => Math.Abs(ins.Result.AverageDeflection)).Average()));
            GenerationLogger.Info("Average weight:  {0}",
                string.Join(" \r\n ",
                generation.Instances.Select(ins => ins.Result.Weight).Average()));
            GenerationLogger.Info("Average number of beams:  {0}",
                string.Join(" \r\n ",
                generation.Instances.Select(ins => ins.Beams().Count()).Average()));

            CreationLogger.Info("Deflections:  \r\n {0}", 
                string.Join(" \r\n ", 
                generation.Instances.Select(ins => ins.GenerationID + " | " + ins.ID + " | " + ins.Result.Deflection.ToString()).ToArray()));
            CreationLogger.Info("Average Deflections:  \r\n {0}", 
                string.Join(" \r\n ", 
                generation.Instances.Select(ins => ins.GenerationID + " | " + ins.ID + " | " + ins.Result.AverageDeflection.ToString()).ToArray()));
            CreationLogger.Info("Weights:  \r\n {0}", 
                string.Join(" \r\n ", 
                generation.Instances.Select(ins => ins.GenerationID + " | " + ins.ID + " | " + ins.Result.Weight.ToString()).ToArray()));
            //Logger.Info("Calculated: {0}", string.Join("\r ", generation.Instances.Select(ins => ins.Result.RobotCalculationStatus).ToArray()));
            CreationLogger.Info("Probabilities for next generation: \r\n {0}", 
                string.Join(" \r\n ", 
                generation.Instances.Select(ins => ins.GenerationID + " | " + ins.ID + " | " + ins.Result.Probability.ToString()).ToArray()));
            foreach (var instance in generation.Instances)
            {
                LogInstance(instance);
            }
        }

        private static void LogInstance(Instance instance)
        {
            CreationLogger.Info("Generation {0} | Instance {1}",
                                instance.GenerationID,
                                instance.ID);
            CreationLogger.Info("Instance Nodes: \r\n {0}", 
                string.Join(" \r\n ", 
                instance.Nodes().Select(node => string.Join(" | ", node.ID, node.X, node.Y))));
            CreationLogger.Info("Instance Beams: \r\n {0}", 
                string.Join(" \r\n ", 
                instance.Beams().Select(beam => string.Join(" | ", beam.ID, beam.Node1.ID, beam.Node2.ID))));
            //var nodesOfBeams = instance.Beams().Select(b => b.Node1.ID).Concat(instance.Beams().Select(b => b.Node2.ID));
            //var nodes = instance.Nodes().Select(n => n.ID);
            //var difOfNodes = nodesOfBeams.Where(nb => nodes.All(n => n != nb));
            //if (difOfNodes.Count() != 0)
            //{
            //    InstanceLogger.Error("Beams have more nodes than nodes exist. Problem nodes: {0}", string.Join(", ", difOfNodes));
            //}
        }

    }
}
