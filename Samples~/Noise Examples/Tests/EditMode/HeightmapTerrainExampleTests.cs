using NUnit.Framework;
using UnityEngine;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class HeightmapTerrainExampleTests
    {
        [Test]
        public void BuildHeights_ProducesGridMesh_WithExpectedTopology()
        {
            var settings = new NoiseSettings();
            var heights = HeightmapTerrainExample.BuildHeights(settings, 8, 8);
            var mesh = NoiseMeshVisualizer.BuildGridMesh(heights, 1f, 5f);

            Assert.AreEqual(64, mesh.vertexCount);
            Assert.AreEqual((8 - 1) * (8 - 1) * 6, mesh.triangles.Length);
        }

        [Test]
        public void BuildHeights_ProducesVaryingHeight()
        {
            var settings = new NoiseSettings();
            var heights = HeightmapTerrainExample.BuildHeights(settings, 8, 8);
            var mesh = NoiseMeshVisualizer.BuildGridMesh(heights, 1f, 5f);

            float minY = float.MaxValue;
            float maxY = float.MinValue;
            foreach (var vertex in mesh.vertices)
            {
                if (vertex.y < minY) minY = vertex.y;
                if (vertex.y > maxY) maxY = vertex.y;
            }

            Assert.Greater(maxY - minY, 1e-4f);
        }
    }
}
