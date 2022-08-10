using System.Collections.Generic;

namespace Robot_Evolution
{
    public class Generation
    {
        public List<Instance> Instances { get; set; }
        public int GenerationNumber { get; set; }
        public int GenerationQuantity { get; set; }

        public Generation()
        {
            this.GenerationNumber = GenerationCounter.Counter;
            this.GenerationQuantity = EvolutionParameters.InstancesPerGeneration;
        }
        
        
        public Generation createInitialGeneration()
        {
            var generation = new Generation();

            generation.Instances.Add(new Instance());

            for (int instanceNum = 1; instanceNum < generation.GenerationQuantity; instanceNum++)
            {

            }

            GenerationCounter.Counter++;
            return generation;
        }

    }
    public static class GenerationCounter
    {
        public static int Counter { get; set; } = 0;
    }
}
