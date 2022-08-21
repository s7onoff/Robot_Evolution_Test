namespace Robot_Evolution
{
    public static class EvolutionParameters
    {
        public static int NewNodesInsideInitialGeneration { get; } = 6;
        public static int NewNodesOnContourGeneration { get; } = 6;
        public static int NewBeamsInInitialGeneration { get; } = 8;

        public static int NumberOfGenerations { get; } = 180; //TODO: change to results of fitting function
        public static int InstancesPerGeneration { get; } = 25;
        public static int SaveEveryNGeneration { get; } = 10;


        // Mutations probabilities
        public static double MoveNodeProbability { get; } = 0.06;
        public static double NodeAddInsideProbability { get; } = 0.03;
        public static double NodeAddOnContourProbability { get; } = 0.03;
        public static double NodeDeleteProbability { get; } = 0.01;
        public static double BeamAddProbability { get; } = 0.03;
        public static double BeamDeleteProbability { get; } = 0.03;
        public static double BeamChangeSectionProbability { get; } = 0.05;


        public static double NodeMovementPerMutation { get; set; } = 2.0; // meters

        public static int GenerationNonCrossedOver { get; } = 5;
        public static int GenerationNonMutated { get; } = 2;
    }
}