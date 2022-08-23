using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot_Evolution
{
    public static class Fitness
    {
        public static double WeightCorrelationFactor { get; } = 0.15;
        public static double DeflectionCorrelationFactor { get; } = 0.7;
        public static double AvegareDeflectionCorrelationFactor { get; } = 0.15;
        public static void RegisterResult(Generation generation)
        {
            var nonZeroResults = generation.Instances.
                Where(inst => inst.Result.Deflection * inst.Result.Weight != 0).
                Select(inst => inst.Result);

            var sumPreProbabilitiesForDeflection = nonZeroResults.Sum(res => 1 / Math.Abs(res.Deflection));
            var sumPreProbabilitiesForAverageDeflection = nonZeroResults.Sum(res => 1 / Math.Abs(res.AverageDeflection));
            var sumPreProbabilitiesForWeight = nonZeroResults.Sum(res => 1 / res.Weight);

            var zeroResults = generation.Instances.
                Where(inst => inst.Result.Deflection * inst.Result.Weight == 0).
                Select(inst => inst.Result);

            foreach (var result in nonZeroResults)
            {
                result.Probability =
                    DeflectionCorrelationFactor /
                    ( Math.Abs(result.Deflection) * sumPreProbabilitiesForDeflection ) 
                    +
                    WeightCorrelationFactor / 
                    (result.Weight * sumPreProbabilitiesForWeight)
                    +
                    AvegareDeflectionCorrelationFactor /
                    (Math.Abs(result.AverageDeflection) * sumPreProbabilitiesForAverageDeflection)
                    ;
            }

            foreach (var result in zeroResults)
            {
                result.Probability = 0;
            }
        }
    }
}