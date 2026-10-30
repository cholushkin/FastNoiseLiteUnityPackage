using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class DomainWarpExampleTests
    {
        static NoiseSettings MakeSettings()
        {
            return new NoiseSettings
            {
                Seed = 1337,
                Frequency = 0.05f,
                DomainWarpAmplitude = 30f
            };
        }

        [Test]
        public void BuildComparisonTexture_IsReproducible_ForIdenticalSettings()
        {
            var settingsA = MakeSettings();
            var settingsB = MakeSettings();

            var textureA = DomainWarpExample.BuildComparisonTexture(settingsA, 8);
            var textureB = DomainWarpExample.BuildComparisonTexture(settingsB, 8);

            Assert.AreEqual(textureA.GetLength(0), textureB.GetLength(0));
            Assert.AreEqual(textureA.GetLength(1), textureB.GetLength(1));

            for (int x = 0; x < textureA.GetLength(0); x++)
            {
                for (int y = 0; y < textureA.GetLength(1); y++)
                {
                    Assert.AreEqual(textureA[x, y], textureB[x, y]);
                }
            }
        }

        [Test]
        public void WarpedTile_DiffersFromOriginalTile_WhenAmplitudeIsNonZero()
        {
            var settings = MakeSettings();
            int tileResolution = 8;
            var texture = DomainWarpExample.BuildComparisonTexture(settings, tileResolution);

            bool foundDifference = false;
            for (int x = 0; x < tileResolution && !foundDifference; x++)
            {
                for (int y = 0; y < tileResolution && !foundDifference; y++)
                {
                    float original = texture[x, y];
                    float warped = texture[tileResolution + x, y];
                    if (!UnityEngine.Mathf.Approximately(original, warped))
                    {
                        foundDifference = true;
                    }
                }
            }

            Assert.IsTrue(foundDifference, "Expected domain warping to change the sampled value somewhere.");
        }
    }
}
