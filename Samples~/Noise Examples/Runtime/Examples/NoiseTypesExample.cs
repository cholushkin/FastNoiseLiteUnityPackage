using UnityEngine;
using System;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Samples all six FastNoiseLite.NoiseType values side by side in a single 3x2 tile grid,
    /// using Settings for every other parameter.
    /// </summary>
    [RequireComponent(typeof(NoiseTextureVisualizer))]
    public class NoiseTypesExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings();

        /// <summary>Builds a (tileResolution*3, tileResolution*2) grid, one tile per NoiseType, in enum declaration order.</summary>
        public static float[,] BuildTypesGrid(NoiseSettings settings, int tileResolution)
        {
            var types = (global::FastNoiseLite.NoiseType[])Enum.GetValues(typeof(global::FastNoiseLite.NoiseType));
            int width = tileResolution * 3;
            int height = tileResolution * 2;
            var result = new float[width, height];

            for (int i = 0; i < types.Length; i++)
            {
                int col = i % 3;
                int row = i / 3;
                var noise = settings.CreateNoise();
                noise.SetNoiseType(types[i]);

                int originX = col * tileResolution;
                int originY = row * tileResolution;
                for (int y = 0; y < tileResolution; y++)
                {
                    for (int x = 0; x < tileResolution; x++)
                    {
                        result[originX + x, originY + y] = noise.GetNoise(x, y);
                    }
                }
            }

            return result;
        }

        protected override void Generate()
        {
            var visualizer = GetComponent<NoiseTextureVisualizer>();
            int tileResolution = Mathf.Max(2, visualizer.Resolution);
            visualizer.Apply(BuildTypesGrid(Settings, tileResolution));
        }
    }
}
