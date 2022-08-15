namespace Robot_Evolution
{
    public static class EvolutionParameters
    {
        public static int NewNodesInInitialGeneration { get; } = 5;
        public static int NewBeamsInInitialGeneration { get; } = 6;
        public static int NumberOfGenerations { get; } = 200; //TODO: change to results of fitting function
        public static double MoveNodeProbability { get; } = 0.07;
        public static double NodeAddDeleteProbability { get; } = 0.03;
        public static double BeamAddDeleteProbability { get; } = 0.02;
        public static int InstancesPerGeneration { get; } = 30;
        public static double NodeMovementPerMutation { get; set; } = 3.0; // meters
        public static int GenerationNonCrossedOver { get; } = 4;
        public static int GenerationNonMutated { get; } = 2;
        public static double GenerationVariationFactor { get; } = 0.1;
    }
}
