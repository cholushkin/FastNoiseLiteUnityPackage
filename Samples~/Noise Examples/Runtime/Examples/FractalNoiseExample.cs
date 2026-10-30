using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Demonstrates fractal noise (None/FBm/Ridged/PingPong) and the Octaves/Lacunarity/Gain/
    /// WeightedStrength/PingPongStrength knobs already exposed on NoiseSettings.
    /// </summary>
    [RequireComponent(typeof(NoiseTextureVisualizer))]
    public class FractalNoiseExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings { FractalType = global::FastNoiseLite.FractalType.FBm };

        public static float[,] BuildTexture(NoiseSettings settings, int resolution)
        {
            var noise = settings.CreateNoise();
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
