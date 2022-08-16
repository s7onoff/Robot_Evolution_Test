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
            var sumPreProbabilities = generation.Instances.Where(inst => inst.Result.Deflection*inst.Result.Weight != 0).Sum(inst => 1 / (Math.Abs(inst.Result.Deflection) * inst.Result.Weight));


            foreach (var instance in generation.Instances)
            {
                if (instance.Result.Deflection != 0 && instance.Result.Weight != 0)
                {
                    instance.Result.Probability = 1 / (Math.Abs(instance.Result.Deflection) * instance.Result.Weight * sumPreProbabilities);
                }
                else
                {
                    instance.Result.Probability = 0;
                }
            }
        }
    }
}
