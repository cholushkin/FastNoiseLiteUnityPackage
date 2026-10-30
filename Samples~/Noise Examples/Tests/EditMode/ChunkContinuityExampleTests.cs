using NUnit.Framework;
using UnityEngine;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class ChunkContinuityExampleTests
    {
        [Test]
        public void AdjacentChunks_EastWestBoundary_IsContinuous()
        {
            var settings = new NoiseSettings();
            int resolution = 9;
            float chunkWorldSize = 16f;

            var west = ChunkContinuityExample.BuildChunkHeights(settings, new Vector2Int(0, 0), resolution, chunkWorldSize);
            var east = ChunkContinuityExample.BuildChunkHeights(settings, new Vector2Int(1, 0), resolution, chunkWorldSize);

            float error = ChunkContinuityExample.MaxEastWestBoundaryError(west, east);
            Assert.Less(error, 1e-4f);
        }

        [Test]
        public void AdjacentChunks_NorthSouthBoundary_IsContinuous()
        {
            var settings = new NoiseSettings();
            int resolution = 9;
            float chunkWorldSize = 16f;

            var south = ChunkContinuityExample.BuildChunkHeights(settings, new Vector2Int(0, 0), resolution, chunkWorldSize);
            var north = ChunkContinuityExample.BuildChunkHeights(settings, new Vector2Int(0, 1), resolution, chunkWorldSize);

            float error = ChunkContinuityExample.MaxNorthSouthBoundaryError(south, north);
            Assert.Less(error, 1e-4f);
        }
    }
}
