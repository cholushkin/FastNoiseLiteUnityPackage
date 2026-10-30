using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Shows original vs domain-warped noise side by side. Settings.FractalType does double duty:
    /// DomainWarp(...) switches on it to pick normal (None/FBm/Ridged/PingPong) vs
    /// DomainWarpProgressive vs DomainWarpIndependent warping, while GetNoise(...) uses the same
    /// field for its own fractal blending.
    /// </summary>
    [RequireComponent(typeof(NoiseTextureVisualizer))]
    public class DomainWarpExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings();

        /// <summary>Builds a (tileResolution*2, tileResolution) texture: left = original noise, right = noise sampled at the warped coordinate.</summary>
        public static float[,] BuildComparisonTexture(NoiseSettings settings, int tileResolution)
        {
            var plainNoise = settings.CreateNoise();
            var warpNoise = settings.CreateDomainWarpNoise();
            var result = new float[tileResolution * 2, tileResolution];

            for (int y = 0; y < tileResolution; y++)
            {
                for (int x = 0; x < tileResolution; x++)
                {
                    result[x, y] = plainNoise.GetNoise(x, y);

                    float wx = x, wy = y;
                    warpNoise.DomainWarp(ref wx, ref wy);
                    result[tileResolution + x, y] = warpNoise.GetNoise(wx, wy);
                }
            }

            return result;
        }

        protected override void Generate()
        {
            var visualizer = GetComponent<NoiseTextureVisualizer>();
            int tileResolution = Mathf.Max(2, visualizer.Resolution);
            visualizer.Apply(BuildComparisonTexture(Settings, tileResolution));
        }
    }
}
