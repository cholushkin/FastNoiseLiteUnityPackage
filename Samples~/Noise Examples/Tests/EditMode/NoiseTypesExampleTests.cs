using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class NoiseTypesExampleTests
    {
        [Test]
        public void BuildTypesGrid_ReturnsExpectedDimensions()
        {
            var settings = new NoiseSettings();
            var grid = NoiseTypesExample.BuildTypesGrid(settings, 8);

            Assert.AreEqual(8 * 3, grid.GetLength(0));
            Assert.AreEqual(8 * 2, grid.GetLength(1));
        }

        [Test]
        public void BuildTypesGrid_DifferentNoiseTypesProduceDifferentValues()
        {
            var settings = new NoiseSettings();
            int tileResolution = 8;
            var grid = NoiseTypesExample.BuildTypesGrid(settings, tileResolution);

            bool foundDifference = false;
            for (int tile = 1; tile < 6 && !foundDifference; tile++)
            {
                int originX = (tile % 3) * tileResolution;
                int originY = (tile / 3) * tileResolution;

                for (int y = 0; y < tileResolution && !foundDifference; y++)
                {
                    for (int x = 0; x < tileResolution && !foundDifference; x++)
                    {
                        float first = grid[x, y];
                        float other = grid[originX + x, originY + y];
                        if (!UnityEngine.Mathf.Approximately(first, other))
                        {
                            foundDifference = true;
                        }
                    }
                }
            }

            Assert.IsTrue(foundDifference, "Expected at least two of the six noise-type tiles to differ somewhere.");
        }
    }
}
