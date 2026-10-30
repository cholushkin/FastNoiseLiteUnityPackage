using NUnit.Framework;
using QCat.FastNoiseLite.Samples;

namespace QCat.FastNoiseLite.Samples.Tests
{
    public class FractalNoiseExampleTests
    {
        static NoiseSettings MakeSettings(global::FastNoiseLite.FractalType fractalType, int octaves)
        {
            return new NoiseSettings
            {
                Seed = 1337,
                Frequency = 0.05f,
                FractalType = fractalType,
                Octaves = octaves
            };
        }

        [Test]
        public void IncreasingOctaves_ChangesOutput_WhenFractalTypeIsFBm()
        {
            var one = MakeSettings(global::FastNoiseLite.FractalType.FBm, 1);
            var four = MakeSettings(global::FastNoiseLite.FractalType.FBm, 4);

            float valueOneOctave = FractalNoiseExample.BuildTexture(one, 4)[2, 3];
            float valueFourOctaves = FractalNoiseExample.BuildTexture(four, 4)[2, 3];

            Assert.AreNotEqual(valueOneOctave, valueFourOctaves);
        }

        [Test]
        public void FractalTypeNone_MatchesPlainNoise_RegardlessOfOctaves()
        {
            var settingsNoneLowOctaves = MakeSettings(global::FastNoiseLite.FractalType.None, 1);
            var settingsNoneHighOctaves = MakeSettings(global::FastNoiseLite.FractalType.None, 6);
            var plainSettings = MakeSettings(global::FastNoiseLite.FractalType.None, 3);

            var plainNoise = plainSettings.CreateNoise();

            float[,] coords = { { 0, 0 }, { 10, 5 }, { -7, 13 }, { 42.5f, -3.5f } };

            for (int i = 0; i < coords.GetLength(0); i++)
            {
                float x = coords[i, 0];
                float y = coords[i, 1];

                float expected = plainNoise.GetNoise(x, y);
                float lowOctaves = settingsNoneLowOctaves.CreateNoise().GetNoise(x, y);
                float highOctaves = settingsNoneHighOctaves.CreateNoise().GetNoise(x, y);

                Assert.AreEqual(expected, lowOctaves);
                Assert.AreEqual(expected, highOctaves);
            }
        }

        [Test]
        public void RidgedAndPingPong_StayWithinSaneBounds()
        {
            var ridged = MakeSettings(global::FastNoiseLite.FractalType.Ridged, 4);
            var pingPong = MakeSettings(global::FastNoiseLite.FractalType.PingPong, 4);

            var ridgedNoise = ridged.CreateNoise();
            var pingPongNoise = pingPong.CreateNoise();

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    float ridgedValue = ridgedNoise.GetNoise(x * 3.1f, y * 2.7f);
                    float pingPongValue = pingPongNoise.GetNoise(x * 3.1f, y * 2.7f);

                    Assert.That(ridgedValue, Is.InRange(-2f, 2f));
                    Assert.That(pingPongValue, Is.InRange(-2f, 2f));
                }
            }
        }
    }
}
