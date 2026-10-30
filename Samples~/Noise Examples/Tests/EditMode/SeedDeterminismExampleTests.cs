using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class SeedDeterminismExampleTests
    {
        [TestCase(0, 16)]
        [TestCase(1, 64)]
        [TestCase(1337, 32)]
        [TestCase(-7, 8)]
        public void CheckSeedDeterminism_ReturnsTrue_ForSameSeedAndSettings(int seed, int sampleCount)
        {
            var settings = new NoiseSettings { Frequency = 0.05f };

            bool pass = SeedDeterminismExample.CheckSeedDeterminism(settings, seed, sampleCount);

            Assert.IsTrue(pass);
        }

        [Test]
        public void IndependentInstances_WithSameSeed_ProduceIdenticalSamples()
        {
            var settingsA = new NoiseSettings { Seed = 1337, Frequency = 0.05f };
            var settingsB = new NoiseSettings { Seed = 1337, Frequency = 0.05f };

            var noiseA = settingsA.CreateNoise();
            var noiseB = settingsB.CreateNoise();

            float[,] coords = { { 0, 0 }, { 5, 5 }, { -3.5f, 12.25f }, { 100, -100 }, { 0.001f, 0.002f } };

            for (int i = 0; i < coords.GetLength(0); i++)
            {
                float x = coords[i, 0];
                float y = coords[i, 1];
                Assert.AreEqual(noiseA.GetNoise(x, y), noiseB.GetNoise(x, y));
            }
        }
    }
}
