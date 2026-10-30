using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Demonstrates cellular (Voronoi-style) noise. A full 4x7 grid of every
    /// CellularDistanceFunction/CellularReturnType combination would be unreadable at tile size,
    /// so this example shows one full-resolution texture for whichever combination is picked in
    /// the Inspector.
    /// </summary>
    [RequireComponent(typeof(NoiseTextureVisualizer))]
    public class CellularNoiseExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings { NoiseType = global::FastNoiseLite.NoiseType.Cellular };

        public static float[,] BuildTexture(NoiseSettings settings, int resolution)
        {
            var noise = settings.CreateNoise();
            noise.SetNoiseType(global::FastNoiseLite.NoiseType.Cellular); // guarantee Cellular regardless of Settings.NoiseType
            var result = new float[resolution, resolution];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    result[x, y] = noise.GetNoise(x, y);
                }
            }
            return result;
        }

        protected override void Generate()
        {
            var visualizer = GetComponent<NoiseTextureVisualizer>();
            int resolution = Mathf.Max(2, visualizer.Resolution);
            visualizer.Apply(BuildTexture(Settings, resolution));
        }
    }
}
