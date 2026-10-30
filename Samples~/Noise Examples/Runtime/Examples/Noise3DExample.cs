using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Demonstrates 3D noise by slicing it along a plane at a chosen position on the remaining axis.
    /// </summary>
    [RequireComponent(typeof(NoiseTextureVisualizer))]
    public class Noise3DExample : NoiseExampleBase
    {
        public enum SlicePlane { XY, XZ, YZ }

        public NoiseSettings Settings = new NoiseSettings();
        public SlicePlane Plane = SlicePlane.XY;
        public float SlicePosition = 0f;

        public static float[,] BuildSliceTexture(NoiseSettings settings, SlicePlane plane, float slicePosition, int resolution)
        {
            var noise = settings.CreateNoise();
            var result = new float[resolution, resolution];
            for (int b = 0; b < resolution; b++)
            {
                for (int a = 0; a < resolution; a++)
                {
                    float x, y, z;
                    switch (plane)
                    {
                        case SlicePlane.XZ:
                            x = a; y = slicePosition; z = b;
                            break;
                        case SlicePlane.YZ:
                            x = slicePosition; y = a; z = b;
                            break;
                        default: // XY
                            x = a; y = b; z = slicePosition;
                            break;
                    }
                    result[a, b] = noise.GetNoise(x, y, z);
                }
            }
            return result;
        }

        protected override void Generate()
        {
            var visualizer = GetComponent<NoiseTextureVisualizer>();
            int resolution = Mathf.Max(2, visualizer.Resolution);
            visualizer.Apply(BuildSliceTexture(Settings, Plane, SlicePosition, resolution));
        }
    }
}
