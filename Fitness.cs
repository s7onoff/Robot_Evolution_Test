using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_Evolution
{
    public static class Fitness
    {
        public static Result BestResultForDeflection { get; set; } = new Result();
        public static Result BestResultForWeight { get; set; } = new Result();
        public static double WeightCorrelationFactor { get; set; } = 0.3;
        public static void RegisterResult(Generation generation)
        {
            var minimumDeflection = generation.Instances.Min(_ => _.Result.Deflection);
            var minimumWeight = generation.Instances.Min(_ => _.Result.Weight);
            foreach (var instance in generation.Instances)
            {
                var deflectionProbability = minimumDeflection / instance.Result.Deflection;
                var weightProbability = minimumWeight / instance.Result.Weight + WeightCorrelationFactor;
                instance.Result.ProbabilityForNext = weightProbability * deflectionProbability;
            }
        }
    }
}
