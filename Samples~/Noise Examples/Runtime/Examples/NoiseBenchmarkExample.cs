using System;
using System.Text;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Measures raw FastNoiseLite sampling throughput (2D and 3D) for every NoiseType, to help
    /// compare the relative cost of each algorithm on the current machine.
    /// </summary>
    public class NoiseBenchmarkExample : NoiseExampleBase
    {
        [Serializable]
        public struct BenchmarkResult
        {
            public global::FastNoiseLite.NoiseType Type;
            public double Milliseconds2D;
            public double SamplesPerSecond2D;
            public double Milliseconds3D;
            public double SamplesPerSecond3D;
        }

        public NoiseSettings Settings = new NoiseSettings();
        [Min(0)] public int WarmupIterations = 3;
        [Min(1)] public int Iterations = 5;
        [Min(1)] public int SideLength2D = 256;
        [Min(1)] public int SideLength3D = 32;
        [TextArea(5, 30)] public string Report;

        // Accumulated into so the JIT/compiler cannot prove the sampled values are unused and
        // eliminate the sampling loop; its value is otherwise irrelevant to the benchmark.
        static double _sink;

        /// <summary>Times <paramref name="iterations"/> full passes over a sideLength x sideLength grid.</summary>
        public static double BenchmarkRaw2D(NoiseSettings settings, global::FastNoiseLite.NoiseType type, int sideLength, int iterations, int warmupIterations)
        {
            var noise = settings.CreateNoise();
            noise.SetNoiseType(type);
            double sink = 0;

            for (int w = 0; w < warmupIterations; w++)
                for (int y = 0; y < sideLength; y++)
                    for (int x = 0; x < sideLength; x++)
                        sink += noise.GetNoise(x, y);

            var stopwatch = Stopwatch.StartNew();
            for (int it = 0; it < iterations; it++)
                for (int y = 0; y < sideLength; y++)
                    for (int x = 0; x < sideLength; x++)
                        sink += noise.GetNoise(x, y);
            stopwatch.Stop();

            _sink += sink;
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        /// <summary>Times <paramref name="iterations"/> full passes over a sideLength^3 grid.</summary>
        public static double BenchmarkRaw3D(NoiseSettings settings, global::FastNoiseLite.NoiseType type, int sideLength, int iterations, int warmupIterations)
        {
            var noise = settings.CreateNoise();
            noise.SetNoiseType(type);
            double sink = 0;

            for (int w = 0; w < warmupIterations; w++)
                for (int z = 0; z < sideLength; z++)
                    for (int y = 0; y < sideLength; y++)
                        for (int x = 0; x < sideLength; x++)
                            sink += noise.GetNoise(x, y, z);

            var stopwatch = Stopwatch.StartNew();
            for (int it = 0; it < iterations; it++)
                for (int z = 0; z < sideLength; z++)
                    for (int y = 0; y < sideLength; y++)
                        for (int x = 0; x < sideLength; x++)
                            sink += noise.GetNoise(x, y, z);
            stopwatch.Stop();

            _sink += sink;
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        protected override void Generate()
        {
            var sb = new StringBuilder();
            var types = (global::FastNoiseLite.NoiseType[])Enum.GetValues(typeof(global::FastNoiseLite.NoiseType));
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                double ms2D = BenchmarkRaw2D(Settings, type, SideLength2D, Iterations, WarmupIterations);
                double ms3D = BenchmarkRaw3D(Settings, type, SideLength3D, Iterations, WarmupIterations);

                double samples2D = (double)SideLength2D * SideLength2D * Iterations;
                double samples3D = (double)SideLength3D * SideLength3D * SideLength3D * Iterations;

                var result = new BenchmarkResult
                {
                    Type = type,
                    Milliseconds2D = ms2D,
                    SamplesPerSecond2D = samples2D / (ms2D / 1000.0),
                    Milliseconds3D = ms3D,
                    SamplesPerSecond3D = samples3D / (ms3D / 1000.0)
                };

                if (i > 0) sb.Append('\n');
                sb.Append($"{result.Type}: 2D {result.Milliseconds2D:F2}ms ({result.SamplesPerSecond2D:F0} samples/s), " +
                          $"3D {result.Milliseconds3D:F2}ms ({result.SamplesPerSecond3D:F0} samples/s)");
            }
            Report = sb.ToString();
            Debug.Log(Report);
        }
    }
}
