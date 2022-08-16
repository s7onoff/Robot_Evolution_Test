namespace Robot_Evolution
{
    public static class EvolutionParameters
    {
        public static int NewNodesInInitialGeneration { get; } = 5;
        public static int NewBeamsInInitialGeneration { get; } = 6;

        public static int NumberOfGenerations { get; } = 100; //TODO: change to results of fitting function
        public static int InstancesPerGeneration { get; } = 25;
        public static int SaveEveryNGeneration { get; } = 10;

        public static double MoveNodeProbability { get; } = 0.08;
        public static double NodeAddDeleteProbability { get; } = 0.03;
        public static double BeamAddDeleteProbability { get; } = 0.02;

        public static double NodeMovementPerMutation { get; set; } = 2.0; // meters

        public static int GenerationNonCrossedOver { get; } = 5;
        public static int GenerationNonMutated { get; } = 2;
    }
}
