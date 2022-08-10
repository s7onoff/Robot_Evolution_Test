namespace Robot_Evolution
{
    public static class EvolutionParameters
    {
        public static double CrossoverFactor { get; } = 0.1;
        public static double MutationFactor { get; } = 0.1;
        public static int InstancesPerGeneration { get; } = 20;
        public static int GenerationNonCrossedOver { get; } = 4;
        public static int GenerationNonMutated { get; } = 2;
        public static double GenerationVariationFactor { get; } = 0.1;
    }
}
