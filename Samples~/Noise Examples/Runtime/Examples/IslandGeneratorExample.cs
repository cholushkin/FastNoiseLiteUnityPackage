using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Shapes 2D noise into an island by multiplying it with a radial falloff mask, then
    /// visualizes the result as a colored terrain mesh (sea/land/peak gradient).
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(NoiseMeshVisualizer))]
    public class IslandGeneratorExample : NoiseExampleBase
    {
        public NoiseSettings Settings = new NoiseSettings();
        [Min(2)] public int Resolution = 128;
        public float CellSize = 1f;
        [Range(0.01f, 1f)] public float IslandRadius = 0.8f;
        [Min(0.1f)] public float FalloffPower = 2f;
        [Range(-1f, 1f)] public float SeaLevel = 0f;
        public float ElevationScale = 10f;
        public Color SeaColor = new Color(0.1f, 0.35f, 0.7f);
        public Color LandColor = new Color(0.25f, 0.6f, 0.2f);
        public Color PeakColor = Color.white;

        /// <summary>
        /// Builds a heightfield equal to raw noise multiplied by a radial falloff mask so the
        /// grid's center stays near full noise strength while its edges taper toward zero.
        /// </summary>
        public static float[,] BuildIslandHeights(NoiseSettings settings, int resolution, float islandRadius, float falloffPower)
        {
            var noise = settings.CreateNoise();
            var heights = new float[resolution, resolution];
            float center = (resolution - 1) / 2f;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float nx = (x - center) / center;
                    float ny = (y - center) / center;
                    float dist = Mathf.Sqrt(nx * nx + ny * ny);
                    float falloff = Mathf.Clamp01(1f - Mathf.Pow(Mathf.Clamp01(dist / islandRadius), falloffPower));
                    heights[x, y] = noise.GetNoise(x, y) * falloff;
                }
            }

            return heights;
        }

        protected override void Generate()
        {
            var heights = BuildIslandHeights(Settings, Resolution, IslandRadius, FalloffPower);

            // Mirror NoiseMeshVisualizer.BuildGridMesh's own min/max normalization so the sea-level
            // gradient key lands on the same position the mesh will actually evaluate.
            float minH = float.MaxValue, maxH = float.MinValue;
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                for (int x = 0; x < heights.GetLength(0); x++)
                {
                    float h = heights[x, y];
                    if (h < minH) minH = h;
                    if (h > maxH) maxH = h;
                }
            }
            if (maxH - minH < 1e-6f) maxH = minH + 1e-6f;

            float seaLevelT = Mathf.Clamp01(Mathf.InverseLerp(minH, maxH, SeaLevel));

            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(SeaColor, 0f),
                    new GradientColorKey(LandColor, seaLevelT),
                    new GradientColorKey(PeakColor, 1f)
                },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });

            var mesh = NoiseMeshVisualizer.BuildGridMesh(heights, CellSize, ElevationScale, colorByHeight: true, gradient: gradient);
            GetComponent<NoiseMeshVisualizer>().Apply(mesh);
        }
    }
}
