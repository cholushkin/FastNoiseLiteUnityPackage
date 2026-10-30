using NUnit.Framework;
using UnityEngine;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class DensityFieldExampleTests
    {
        [Test]
        public void BuildDensityField_ReturnsRequestedDimensions()
        {
            var settings = new NoiseSettings { Frequency = 0.5f };
            var gridSize = new Vector3Int(4, 4, 4);
            var density = DensityFieldExample.BuildDensityField(settings, gridSize, 1f);

            Assert.AreEqual(gridSize.x, density.GetLength(0));
            Assert.AreEqual(gridSize.y, density.GetLength(1));
            Assert.AreEqual(gridSize.z, density.GetLength(2));
        }

        [Test]
        public void BuildDensityField_VariesAcrossGrid()
        {
            var settings = new NoiseSettings { Frequency = 0.5f };
            var gridSize = new Vector3Int(4, 4, 4);
            var density = DensityFieldExample.BuildDensityField(settings, gridSize, 1f);

            float min = float.MaxValue;
            float max = float.MinValue;
            for (int z = 0; z < gridSize.z; z++)
                for (int y = 0; y < gridSize.y; y++)
                    for (int x = 0; x < gridSize.x; x++)
                    {
                        float v = density[x, y, z];
                        if (v < min) min = v;
                        if (v > max) max = v;
                    }

            Assert.Greater(max - min, 1e-4f);
        }
    }
}
