using System.Text;
using UnityEngine;

namespace QCat.FastNoiseLite.Samples
{
    /// <summary>
    /// Proves that two independently-constructed FastNoiseLite instances built from the same
    /// seed and settings produce bit-for-bit identical output, across a list of seeds to test.
    /// </summary>
    public class SeedDeterminismExample : NoiseExampleBase
    {
        public NoiseSettings BaseSettings = new NoiseSettings();
        public int[] SeedsToTest = new int[] { 0, 1, 1337, 42, -7 };
        [Min(1)] public int SampleCount = 64;
        [TextArea(5, 20)] public string Report;

        /// <summary>
        /// Builds two separate noise instances from <paramref name="baseSettings"/> pinned to
        /// <paramref name="seed"/> and checks they agree exactly on every sampled coordinate.
        /// </summary>
        public static bool CheckSeedDeterminism(NoiseSettings baseSettings, int seed, int sampleCount)
        {
            // Mutates baseSettings.Seed in place to pin the seed for this check; callers that
            // need their own settings preserved should pass a dedicated instance.
            baseSettings.Seed = seed;
            var instanceA = baseSettings.CreateNoise();
            var instanceB = baseSettings.CreateNoise();

            for (int i = 0; i < sampleCount; i++)
            {
                float x = i * 0.37f;
                float y = i * 0.91f;
                if (instanceA.GetNoise(x, y) != instanceB.GetNoise(x, y)) return false;
            }
            return true;
        }

        protected override void Generate()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < SeedsToTest.Length; i++)
            {
                int seed = SeedsToTest[i];
                bool pass = CheckSeedDeterminism(BaseSettings, seed, SampleCount);
                if (i > 0) sb.Append('\n');
                sb.Append($"Seed {seed}: {(pass ? "PASS" : "FAIL")}");
            }
            Report = sb.ToString();
            Debug.Log(Report);
        }
    }
}
