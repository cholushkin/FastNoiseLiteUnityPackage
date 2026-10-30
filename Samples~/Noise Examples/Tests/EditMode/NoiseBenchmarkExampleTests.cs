using System;
using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class NoiseBenchmarkExampleTests
    {
        static readonly global::FastNoiseLite.NoiseType[] AllNoiseTypes =
            (global::FastNoiseLite.NoiseType[])Enum.GetValues(typeof(global::FastNoiseLite.NoiseType));

        [Test]
        public void BenchmarkRaw2D_RunsForEveryNoiseType_AndReturnsFiniteNonNegativeMilliseconds()
        {
            var settings = new NoiseSettings { Seed = 1337, Frequency = 0.05f };

            foreach (var type in AllNoiseTypes)
            {
                double ms = NoiseBenchmarkExample.BenchmarkRaw2D(settings, type, sideLength: 8, iterations: 1, warmupIterations: 0);

                Assert.IsFalse(double.IsNaN(ms));
                Assert.IsFalse(double.IsInfinity(ms));
                Assert.GreaterOrEqual(ms, 0.0);
            }
        }

        [Test]
        public void BenchmarkRaw3D_RunsForEveryNoiseType_AndReturnsFiniteNonNegativeMilliseconds()
        {
            var settings = new NoiseSettings { Seed = 1337, Frequency = 0.05f };

            foreach (var type in AllNoiseTypes)
            {
                double ms = NoiseBenchmarkExample.BenchmarkRaw3D(settings, type, sideLength: 8, iterations: 1, warmupIterations: 0);

                Assert.IsFalse(double.IsNaN(ms));
                Assert.IsFalse(double.IsInfinity(ms));
                Assert.GreaterOrEqual(ms, 0.0);
            }
        }
    }
}
