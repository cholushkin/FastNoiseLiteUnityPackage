using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Demonstrates using 2D noise as a heightmap to build and visualize a terrain mesh.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(NoiseMeshVisualizer))]
    public class HeightmapTerrainExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings();
        [Min(2)] public int WidthVertices = 64;
        [Min(2)] public int DepthVertices = 64;
        public float CellSize = 1f;
        public float HeightScale = 10f;

        public static float[,] BuildHeights(NoiseSettings settings, int widthVertices, int depthVertices)
        {
            var noise = settings.CreateNoise();
            var result = new float[widthVertices, depthVertices];
            for (int y = 0; y < depthVertices; y++)
            {
                for (int x = 0; x < widthVertices; x++)
                {
                    result[x, y] = noise.GetNoise(x, y);
                }
            }
            return result;
        }

        protected override void Generate()
        {
            var heights = BuildHeights(Settings, WidthVertices, DepthVertices);
            var mesh = NoiseMeshVisualizer.BuildGridMesh(heights, CellSize, HeightScale);
            GetComponent<NoiseMeshVisualizer>().Apply(mesh);
        }
    }
}
