using NUnit.Framework;
using QCat.FastNoiseLite.Samples;
using UnityEngine;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class ResourceDistributionExampleTests
    {
        static int CountNonBackground(Color[,] map, Color backgroundColor)
        {
            int count = 0;
            for (int y = 0; y < map.GetLength(1); y++)
                for (int x = 0; x < map.GetLength(0); x++)
                    if (map[x, y] != backgroundColor) count++;
            return count;
        }

        [Test]
        public void LowerThreshold_ProducesMoreNonBackgroundPixels_ThanHigherThreshold()
        {
            var background = new Color(0.15f, 0.15f, 0.15f);
            var layerColor = new Color(0.8f, 0.4f, 0.1f);
            int resolution = 32;

            var lowThresholdLayers = new ResourceLayer[]
            {
                new ResourceLayer { Name = "Ore", Settings = new NoiseSettings { Seed = 1, Frequency = 0.08f }, Threshold = -0.9f, Color = layerColor }
            };
            var highThresholdLayers = new ResourceLayer[]
            {
                new ResourceLayer { Name = "Ore", Settings = new NoiseSettings { Seed = 1, Frequency = 0.08f }, Threshold = 0.99f, Color = layerColor }
            };

            var lowThresholdMap = ResourceDistributionExample.BuildResourceMap(lowThresholdLayers, background, resolution);
            var highThresholdMap = ResourceDistributionExample.BuildResourceMap(highThresholdLayers, background, resolution);

            int lowThresholdCount = CountNonBackground(lowThresholdMap, background);
            int highThresholdCount = CountNonBackground(highThresholdMap, background);

            Assert.Greater(lowThresholdCount, highThresholdCount);
        }
    }
}
