using System.Collections.Generic;
using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Demonstrates generating a grid of terrain chunks that share exact edge values by sampling
    /// noise in world space rather than per-chunk local space.
    /// </summary>
    public class ChunkContinuityExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings();
        [Min(2)] public int ChunkResolution = 17;
        public float ChunkWorldSize = 16f;
        public Vector2Int ChunkGridCount = new Vector2Int(2, 2);
        public float HeightScale = 10f;

        readonly List<GameObject> _chunkObjects = new List<GameObject>();

        // Sampling in world space (chunkCoord * chunkWorldSize + local offset) means the shared
        // edge between two adjacent chunks is sampled at identical world coordinates on both sides.
        public static float[,] BuildChunkHeights(NoiseSettings settings, Vector2Int chunkCoord, int resolution, float chunkWorldSize)
        {
            float cellSize = chunkWorldSize / (resolution - 1);
            var noise = settings.CreateNoise();
            var result = new float[resolution, resolution];
            for (int localY = 0; localY < resolution; localY++)
            {
                for (int localX = 0; localX < resolution; localX++)
                {
                    float worldX = chunkCoord.x * chunkWorldSize + localX * cellSize;
                    float worldY = chunkCoord.y * chunkWorldSize + localY * cellSize;
                    result[localX, localY] = noise.GetNoise(worldX, worldY);
                }
            }
            return result;
        }

        public static float MaxEastWestBoundaryError(float[,] westChunk, float[,] eastChunk)
        {
            int westEdgeX = westChunk.GetLength(0) - 1;
            int resolution = westChunk.GetLength(1);
            float maxError = 0f;
            for (int y = 0; y < resolution; y++)
            {
                float error = Mathf.Abs(westChunk[westEdgeX, y] - eastChunk[0, y]);
                if (error > maxError) maxError = error;
            }
            return maxError;
        }

        public static float MaxNorthSouthBoundaryError(float[,] southChunk, float[,] northChunk)
        {
            int southEdgeY = southChunk.GetLength(1) - 1;
            int resolution = southChunk.GetLength(0);
            float maxError = 0f;
            for (int x = 0; x < resolution; x++)
            {
                float error = Mathf.Abs(southChunk[x, southEdgeY] - northChunk[x, 0]);
                if (error > maxError) maxError = error;
            }
            return maxError;
        }

        protected override void Generate()
        {
            foreach (var obj in _chunkObjects)
            {
                if (obj == null) continue;
                if (Application.isPlaying) Destroy(obj);
                else DestroyImmediate(obj);
            }
            _chunkObjects.Clear();

            int countX = Mathf.Max(1, ChunkGridCount.x);
            int countY = Mathf.Max(1, ChunkGridCount.y);
            float cellSize = ChunkWorldSize / (ChunkResolution - 1);

            var chunkHeights = new Dictionary<Vector2Int, float[,]>();

            for (int cy = 0; cy < countY; cy++)
            {
                for (int cx = 0; cx < countX; cx++)
                {
                    var chunkCoord = new Vector2Int(cx, cy);
                    var heights = BuildChunkHeights(Settings, chunkCoord, ChunkResolution, ChunkWorldSize);
                    chunkHeights[chunkCoord] = heights;

                    var child = new GameObject($"Chunk_{cx}_{cy}");
                    child.transform.SetParent(transform, false);
                    child.transform.localPosition = new Vector3(cx * ChunkWorldSize, 0f, cy * ChunkWorldSize);
                    child.AddComponent<MeshFilter>();
                    child.AddComponent<MeshRenderer>();
                    var visualizer = child.AddComponent<NoiseMeshVisualizer>();
                    visualizer.Apply(NoiseMeshVisualizer.BuildGridMesh(heights, cellSize, HeightScale));

                    _chunkObjects.Add(child);
                }
            }

            float maxError = 0f;
            int edgeCount = 0;
            for (int cy = 0; cy < countY; cy++)
            {
                for (int cx = 0; cx < countX; cx++)
                {
                    var coord = new Vector2Int(cx, cy);
                    if (cx + 1 < countX)
                    {
                        float error = MaxEastWestBoundaryError(chunkHeights[coord], chunkHeights[new Vector2Int(cx + 1, cy)]);
                        if (error > maxError) maxError = error;
                        edgeCount++;
                    }
                    if (cy + 1 < countY)
                    {
                        float error = MaxNorthSouthBoundaryError(chunkHeights[coord], chunkHeights[new Vector2Int(cx, cy + 1)]);
                        if (error > maxError) maxError = error;
                        edgeCount++;
                    }
                }
            }

            Debug.Log($"[ChunkContinuityExample] Max boundary error across {edgeCount} chunk edges: {maxError}");
        }
    }
}
