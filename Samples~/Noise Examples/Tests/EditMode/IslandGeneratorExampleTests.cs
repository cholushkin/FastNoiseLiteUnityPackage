using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class IslandGeneratorExampleTests
    {
        [Test]
        public void Falloff_AttenuatesCorners_MoreThanCenter()
        {
            var settings = new NoiseSettings { Seed = 1337, Frequency = 0.05f };
            int resolution = 64;
            float islandRadius = 0.8f;
            float falloffPower = 2f;

            var heights = IslandGeneratorExample.BuildIslandHeights(settings, resolution, islandRadius, falloffPower);

            int center = (resolution - 1) / 2;
            float centerMagnitude = System.Math.Abs(heights[center, center]);
            float cornerMagnitude = System.Math.Abs(heights[0, 0]);

            Assert.Greater(centerMagnitude, cornerMagnitude);
        }

        [Test]
        public void Falloff_ClampsCornersToExactlyZero()
        {
            var settings = new NoiseSettings { Seed = 1337, Frequency = 0.05f };
            int resolution = 64;
            float islandRadius = 0.8f;
            float falloffPower = 2f;

            var heights = IslandGeneratorExample.BuildIslandHeights(settings, resolution, islandRadius, falloffPower);

            Assert.AreEqual(0f, heights[0, 0]);
            Assert.AreEqual(0f, heights[resolution - 1, 0]);
            Assert.AreEqual(0f, heights[0, resolution - 1]);
            Assert.AreEqual(0f, heights[resolution - 1, resolution - 1]);
        }
    }
}
