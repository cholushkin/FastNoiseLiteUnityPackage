using System;
using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class CellularNoiseExampleTests
    {
        static NoiseSettings MakeSettings(global::FastNoiseLite.CellularReturnType returnType)
        {
            return new NoiseSettings
            {
                Seed = 1337,
                Frequency = 0.05f,
                NoiseType = global::FastNoiseLite.NoiseType.Cellular,
                CellularReturnType = returnType
            };
        }

        [Test]
        public void CellValueAndDistance_ProduceDifferentOutput()
        {
            var cellValueSettings = MakeSettings(global::FastNoiseLite.CellularReturnType.CellValue);
            var distanceSettings = MakeSettings(global::FastNoiseLite.CellularReturnType.Distance);

            var cellValueTexture = CellularNoiseExample.BuildTexture(cellValueSettings, 16);
            var distanceTexture = CellularNoiseExample.BuildTexture(distanceSettings, 16);

            bool foundDifference = false;
            for (int x = 0; x < 16 && !foundDifference; x++)
            {
                for (int y = 0; y < 16 && !foundDifference; y++)
                {
                    if (!UnityEngine.Mathf.Approximately(cellValueTexture[x, y], distanceTexture[x, y]))
                    {
                        foundDifference = true;
                    }
                }
            }

            Assert.IsTrue(foundDifference, "Expected CellValue and Distance return types to produce different output.");
        }

        [Test]
        public void BuildTexture_DoesNotThrow_AcrossAllDistanceFunctions()
        {
            var distanceFunctions = (global::FastNoiseLite.CellularDistanceFunction[])Enum.GetValues(typeof(global::FastNoiseLite.CellularDistanceFunction));

            foreach (var distanceFunction in distanceFunctions)
            {
                var settings = MakeSettings(global::FastNoiseLite.CellularReturnType.Distance);
                settings.CellularDistanceFunction = distanceFunction;

                Assert.DoesNotThrow(() => CellularNoiseExample.BuildTexture(settings, 8));
            }
        }
    }
}
