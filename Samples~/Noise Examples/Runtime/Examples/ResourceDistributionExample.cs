using System;
using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>One independently-configured noise layer used to scatter a resource type.</summary>
    [Serializable]
    public struct ResourceLayer
    {
        public string Name;
        public NoiseSettings Settings;
        [Range(-1f, 1f)] public float Threshold;
        public Color Color;
    }

    /// <summary>
    /// Stacks several noise layers (e.g. ore, crystal, vegetation) and colors each pixel by the
    /// first layer whose noise value exceeds its threshold, demonstrating a practical resource
    /// distribution pattern built entirely from FastNoiseLite samples.
    /// </summary>
    [RequireComponent(typeof(NoiseTextureVisualizer))]
    public class ResourceDistributionExample : NoiseExampleBase
    {
        public ResourceLayer[] Layers = new ResourceLayer[]
        {
            new ResourceLayer { Name = "Ore", Settings = new NoiseSettings { Seed = 1, Frequency = 0.08f }, Threshold = 0.4f, Color = new Color(0.8f, 0.4f, 0.1f) },
            new ResourceLayer { Name = "Crystal", Settings = new NoiseSettings { Seed = 2, Frequency = 0.05f }, Threshold = 0.5f, Color = new Color(0.4f, 0.8f, 1f) },
            new ResourceLayer { Name = "Vegetation", Settings = new NoiseSettings { Seed = 3, Frequency = 0.03f }, Threshold = 0.2f, Color = new Color(0.2f, 0.7f, 0.2f) }
        };
        public Color BackgroundColor = new Color(0.15f, 0.15f, 0.15f);

        /// <summary>
        /// For each pixel, uses the color of the first layer (in array order) whose noise value
        /// exceeds its threshold, or <paramref name="backgroundColor"/> if none match.
        /// </summary>
        public static Color[,] BuildResourceMap(ResourceLayer[] layers, Color backgroundColor, int resolution)
        {
            var noises = new global::FastNoiseLite[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                noises[i] = layers[i].Settings.CreateNoise();

            var result = new Color[resolution, resolution];
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    Color pixel = backgroundColor;
                    for (int i = 0; i < layers.Length; i++)
                    {
                        if (noises[i].GetNoise(x, y) > layers[i].Threshold)
                        {
                            pixel = layers[i].Color;
                            break;
                        }
                    }
                    result[x, y] = pixel;
                }
            }
            return result;
        }

        protected override void Generate()
        {
            var visualizer = GetComponent<NoiseTextureVisualizer>();
            var colors = BuildResourceMap(Layers, BackgroundColor, visualizer.Resolution);
            visualizer.Apply(colors);
        }
    }
}
